using System;
using System.IO;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Assertions;
using UnityEngine.Networking;
using UnityEngine.XR.ARSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    internal sealed class MagicLeapImageDatabase : MutableRuntimeReferenceImageLibrary
    {
        // Serialized database header bytes
        public const ulong kMagicBytes = 0x4e8d5ce7045df074;

        // Job handle is received from the subsystem that created this database.  Used to gate
        // add image jobs until the native tracker handle resource has completed (~6000ms).
        JobHandle m_CreateAssociatedNativeTrackerJobHandle;

        public IntPtr nativeProviderPtr => m_NativeProviderPtr;
        IntPtr m_NativeProviderPtr;

        // Native Database pointer
        public IntPtr nativePtr => m_NativePtr;
        IntPtr m_NativePtr;

        struct AddImageJob : IJob
        {
            [DeallocateOnJobCompletion]
            [NativeDisableUnsafePtrRestriction]
            public NativeArray<byte> grayscaleImage;

            [DeallocateOnJobCompletion]
            [NativeDisableUnsafePtrRestriction]
            public NativeArray<byte> name;

            [NativeDisableUnsafePtrRestriction]
            public IntPtr nativeProvider;

            [NativeDisableUnsafePtrRestriction]
            public IntPtr database;

            public int width;

            public int height;

            [NativeDisableUnsafePtrRestriction]
            public ManagedReferenceImage managedReferenceImage;

            public unsafe void Execute()
            {
                var success = AddImage(
                    database, nativeProvider,
                    ref managedReferenceImage,
                    grayscaleImage.GetUnsafePtr(),
                    width, height,
                    name.GetUnsafePtr());

                if (!success)
                {
                    managedReferenceImage.Dispose();
                }

                RcoApi.Release(database);
                RcoApi.Release(nativeProvider);
            }

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageDatabase_AddImage")]
            static extern unsafe bool AddImage(
                IntPtr database, IntPtr nativeProvider,
                ref ManagedReferenceImage managedReferenceImage,
                void* image, int width, int height, void* name);
        }

        public MagicLeapImageDatabase(XRReferenceImageLibrary serializedLibrary, IntPtr nativeProviderHandle, JobHandle imageTrackerCreationJobHandle)
        {
            m_NativeProviderPtr = nativeProviderHandle;
            m_CreateAssociatedNativeTrackerJobHandle = imageTrackerCreationJobHandle;
            m_NativePtr = Construct();

            if (m_NativePtr == IntPtr.Zero)
                throw new InvalidOperationException($"Native Library for {serializedLibrary.name} failed to construct.");

            if (serializedLibrary != null && serializedLibrary.count > 0)
            {
                DeserializeImageDatabaseFile(serializedLibrary);
            }
        }

        ~MagicLeapImageDatabase()
        {
            if (m_NativePtr != IntPtr.Zero)
            {
                int n = count;
                for (int i = 0; i < n; ++i)
                {
                    GetManagedReferenceImage(m_NativePtr, i).Dispose();
                }
                RcoApi.Release(m_NativePtr);
            }
        }

        static readonly TextureFormat[] k_SupportedFormats =
        {
            TextureFormat.Alpha8,
            TextureFormat.R8,
            TextureFormat.RFloat,
            TextureFormat.RGB24,
            TextureFormat.RGBA32,
            TextureFormat.ARGB32,
            TextureFormat.BGRA32,
        };

        public override int supportedTextureFormatCount => k_SupportedFormats.Length;

        protected override TextureFormat GetSupportedTextureFormatAtImpl(int index) => k_SupportedFormats[index];

        protected override JobHandle ScheduleAddImageJobImpl(
            NativeSlice<byte> imageBytes,
            Vector2Int sizeInPixels,
            TextureFormat format,
            XRReferenceImage referenceImage,
            JobHandle inputDeps)
        {
            if (m_NativeProviderPtr != MagicLeapImageTrackingSubsystem.nativeProviderPtr)
                throw new InvalidOperationException($"Attempted to modify dead image library.  The native provider that created this library doesn't exist and the native resource for this library has been released.  Dispose this object.");

            var grayscaleImage = new NativeArray<byte>(
                sizeInPixels.x * sizeInPixels.y,
                Allocator.Persistent,
                NativeArrayOptions.UninitializedMemory);

            var jobDependencies = JobHandle.CombineDependencies(m_CreateAssociatedNativeTrackerJobHandle, inputDeps);

            var conversionHandle = ConversionJob.Schedule(imageBytes, sizeInPixels, format, grayscaleImage, jobDependencies);

            // Retain in case we destroy before the images have been processed
            RcoApi.Retain(m_NativePtr);
            RcoApi.Retain(m_NativeProviderPtr);

            return new AddImageJob
            {
                nativeProvider = m_NativeProviderPtr,
                database = m_NativePtr,
                managedReferenceImage = new ManagedReferenceImage(referenceImage),
                grayscaleImage = grayscaleImage,
                width = sizeInPixels.x,
                height = sizeInPixels.y,
                name = new NativeArray<byte>(Encoding.UTF8.GetBytes(referenceImage.name + "\0"), Allocator.Persistent),
            }.Schedule(conversionHandle);
        }

        protected override XRReferenceImage GetReferenceImageAt(int index) => GetManagedReferenceImage(m_NativePtr, index).ToReferenceImage();

        public override int count => GetReferenceImageCount(m_NativePtr);

        unsafe void DeserializeImageDatabaseFile(XRReferenceImageLibrary serializedLibrary)
        {
            var libraryPath = MagicLeapImageTrackingSubsystem.GetDatabaseFilePathFromLibrary(serializedLibrary);

            if (!File.Exists(libraryPath))
                throw new ArgumentException($"Serialized Image Tracking Library at '{libraryPath}' doesn't exist.");

            var bytes = File.ReadAllBytes(libraryPath);

            if (bytes.Length < sizeof(ulong))
                throw new ArgumentException($"Serialized Image Tracking Library at '{libraryPath}' has no image data within it.");

            var magicBytes = BitConverter.ToUInt64(bytes, 0);
            var byteOffset = sizeof(ulong);

            if (magicBytes != kMagicBytes)
                throw new ArgumentException($"Serialized Image Tracking Library at '{libraryPath}' is not a valid image database file.");

            for (int i = 0; i < serializedLibrary.count; ++i)
            {
                var referenceImage = serializedLibrary[i];
                var imageHandle = MagicLeapImageTrackingSubsystem.Native.InvalidHandle;

                var widthInPixels = BitConverter.ToInt32(bytes, byteOffset);
                byteOffset += sizeof(int);
                var heightInPixels = BitConverter.ToInt32(bytes, byteOffset);
                byteOffset += sizeof(int);

                // Retain in case we destroy before the images have been processed
                RcoApi.Retain(m_NativePtr);
                RcoApi.Retain(m_NativeProviderPtr);
                fixed (byte* texturePtr = &bytes[byteOffset])
                {
                    var grayscaleImage = new NativeArray<byte>(widthInPixels * heightInPixels, Allocator.Persistent);
                    NativeArray<byte>.Copy(bytes, byteOffset, grayscaleImage, 0, widthInPixels * heightInPixels);
                    new AddImageJob
                    {
                        nativeProvider = m_NativeProviderPtr,
                        database = m_NativePtr,
                        managedReferenceImage = new ManagedReferenceImage(referenceImage),
                        grayscaleImage = grayscaleImage,
                        width = widthInPixels,
                        height = heightInPixels,
                        name = new NativeArray<byte>(Encoding.UTF8.GetBytes(referenceImage.name + "\0"), Allocator.Persistent)
                    }.Schedule(m_CreateAssociatedNativeTrackerJobHandle);
                }
                byteOffset += widthInPixels * heightInPixels;
            }
        }

        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageDatabase_Construct")]
        static extern IntPtr Construct();

        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageDatabase_GetReferenceImageCount")]
        static extern int GetReferenceImageCount(IntPtr imageDatabase);

        [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageDatabase_GetReferenceImageAt")]
        static extern ManagedReferenceImage GetManagedReferenceImage(IntPtr imageDatabase, int index);
    }
}
