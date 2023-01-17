using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using Unity.Jobs;
using UnityEngine.Lumin;
using UnityEngine.Scripting;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.MagicLeap.Internal;

namespace UnityEngine.XR.MagicLeap
{
    using MLLog = UnityEngine.XR.MagicLeap.MagicLeapLogger;

    [Preserve]
    [UsesLuminPrivilege("CameraCapture")]
    public sealed class MagicLeapImageTrackingSubsystem : XRImageTrackingSubsystem
    {
        const string kLogTag = "Unity-ImageTracking";

        [Conditional("DEVELOPMENT_BUILD")]
        static void DebugError(string msg) => LogError(msg);

        static void LogWarning(string msg) => MLLog.Warning(kLogTag, $"{msg}");
        static void LogError(string msg) => MLLog.Error(kLogTag, $"{msg}");
        static void Log(string msg) => MLLog.Debug(kLogTag, $"{msg}");

        internal static readonly string k_StreamingAssetsPath = Path.Combine(Application.streamingAssetsPath, "MLImageLibraries");
        internal static readonly string k_ImageTrackingDependencyPath = Path.Combine("Library", "MLImageTracking");

        internal static string GetDatabaseFilePathFromLibrary(XRReferenceImageLibrary library) => Path.Combine(k_StreamingAssetsPath, $"{library.name}_{library.guid}.imgpak");

#if !UNITY_2020_2_OR_NEWER
        protected override Provider CreateProvider() => new MagicLeapProvider();
#endif

        /// <summary>
        /// Checks to see whether the native provider is valid and whether permission has been granted.
        /// </summary>
        /// <returns>
        /// <c>true</c> if the native provider has been instantiated and has a valid native resource.
        /// </returns>
        /// <remarks>
        /// There are a number of reasons for false.  Either privileges were denied or the device experienced
        /// and internal error and was not able to create the native tracking resource.  Should the latter be
        /// the case, native error logs will have more information.
        /// </remarks>
        public bool IsValid() => MagicLeapProvider.IsSubsystemStateValid();

        /// <summary>
        /// Increment the internal reference count to the Native Image Tracker.
        /// </summary>
        /// <remarks>
        /// Attention: Do not use unless you plan on managing the lifetime of the Magic Leap Image Tracker.
        ///
        /// By utilizing the <c>IncRefcount</c>, you are indicating that you are 'holding on' to the Magic Leap Image Tracker -
        /// that is, you are managing the clean-up of the Image tracker yourself.
        ///
        /// As long as you are managing the lifetime of the image tracker yourself, you do not need to call <c>DecRefcount</c>
        /// </remarks>
        public static void IncRefcount()
        {
            RcoApi.Retain(nativeProviderPtr);
        }

        /// <summary>
        /// Decrement the internal reference count to the Native Image Tracker.
        /// </summary>
        /// <remarks>
        /// Attention: Do not use unless you plan on managing the lifetime of the Magic Leap Image Tracker.
        ///
        /// By utilizing the <c>DecRefcount</c>, you are indicating that you are releasing the reference to the Magic Leap Image Tracker -
        /// that is, you are relinquishing controlling the lifetime of the Image Tracking subsystem.
        ///
        /// As long as you are managing the lifetime of the image tracker yourself, you do not need to call <c>DecRefcount</c>
        /// </remarks>
        public static void DecRefcount()
        {
            RcoApi.Release(nativeProviderPtr);
        }

        // Reference Counted Native Provider
        internal static IntPtr nativeProviderPtr => MagicLeapProvider.s_NativeProviderPtr;

        /// <summary>
        /// The <see cref="JobHandle"/> that refers to the native tracker handle creation job.
        /// </summary>
        /// <remarks>
        /// The creation of the native image tracker handle that enables image tracking on
        /// Lumin devices has an average startup time of anywhere between ~1500ms - ~6000ms
        /// depending on the state of the device and is blocking.  This subsystem opts to
        /// perform this operation asynchronously because of this.
        /// Please note that if you enable the image tracker in a disabled state, you do not
        /// incur the ~1500 - ~6000 ms startup cost.
        /// As is stands, getting `MagicLeapProvider.s_NativeTrackerCreationJobHandle` will go away, thus making
        /// `nativeTrackerCreationJobHandle` irrelevant.
        /// </remarks>
        [Obsolete("This usage is no longer supported as creation of the native Image Tracker is no longer asynchronous.")]
        public static JobHandle nativeTrackerCreationJobHandle => MagicLeapProvider.s_NativeTrackerCreationJobHandle;

        /// <summary>
        /// Returns whether the subsystem is in control of the stationary settings for images.
        /// </summary>
        /// <returns>
        /// Returns <c>true</c> if the subsystem is determining the stationary setting for images currently being tracked. Returns <c>false</c> if the user is responsible.
        /// </returns>
        public bool GetAutomaticImageStationarySettingsEnforcementPolicy() => Native.GetAutomaticImageStationarySettingsEnforcementPolicy(nativeProviderPtr);

        /// <summary>
        /// Sets whether the subsystem should have ownership of the stationary setting for images based on the <c>"maxNumberOfMovingImages"</c>.
        /// </summary>
        /// <param name="shouldEnforceSubsystemOwnership">
        /// If passed in <c>true</c> the subsystem will automatically set the stationary parameter on images based on the <c>"maxNumberOfMovingImages"</c>.
        /// If passed in <c>false</c> the developer is responsible for settings the stationary parameter using <see cref="TrySetReferenceImageStationary"/>.
        /// </param>
        public void SetAutomaticImageStationarySettingsEnforcementPolicy(bool shouldEnforceSubsystemOwnership) => Native.SetAutomaticImageStationarySettingsEnforcementPolicy(nativeProviderPtr, shouldEnforceSubsystemOwnership);

        /// <summary>
        /// Attempts to set whether the subsystem should treat the given <c>XRReferenceImage</c> as stationary or not.
        /// </summary>
        /// <param name="referenceImage">The <c>XRReferenceImage</c> that should have its <c>isStationary</c> value changed.</param>
        /// <param name="isStationary">Whether or not the target image should be treated as a stationary image in the real world.  The default value is <c>false</c>.</param>
        /// <returns><c>true</c> if the image settings were updated successfully and <c>false</c> otherwise.</returns>
        /// <remarks>
        /// This will implicitly transfer ownership of the stationary setting for images from the subsystem to the user.  In order to give ownership back, use <see cref="SetAutomaticImageStationarySettingsEnforcementPolicy"/>.
        /// </remarks>
        public bool TrySetReferenceImageStationary(XRReferenceImage referenceImage, bool isStationary)
        {
            if (!IsValid())
                return false;

            return Native.TrySetReferenceImageStationary(nativeProviderPtr, referenceImage.textureGuid, isStationary);
        }

        class MagicLeapProvider : Provider
        {
            internal static IntPtr s_NativeProviderPtr = IntPtr.Zero;
            internal static JobHandle s_NativeTrackerCreationJobHandle = default(JobHandle);
            internal static JobHandle s_SetDatabaseJobHandle = default(JobHandle);

            internal static bool IsSubsystemStateValid()
            {
                return s_NativeProviderPtr != IntPtr.Zero
                    && !s_NativeTrackerCreationJobHandle.Equals(default(JobHandle))
                    && s_NativeTrackerCreationJobHandle.IsCompleted
                    && Native.IsNativeTrackerHandleValid(s_NativeProviderPtr);
            }

            PerceptionHandle m_PerceptionHandle;

            /// <summary>
            /// The privilege required to access camera capture.
            /// </summary>
            const uint k_MLPrivilegeID_CameraCapture = 26;


            struct DummyNoopJob : IJob
            {
                public void Execute()
                {
                    Thread.Sleep(10); // Just a delay to ensure we get a small pause.
                }
            }

            /// <summary>
            /// Allows the user to re-request privileges
            /// </summary>
            /// <returns>
            /// <c>true</c> if the Color Camera privileges were granted and <c>false</c> otherwise.
            /// </returns>
            public bool RequestPrivilegesIfNecessary()
            {
                if (MagicLeapPrivileges.IsPrivilegeApproved(k_MLPrivilegeID_CameraCapture))
                {
                    return true;
                }
                else
                {
                    return MagicLeapPrivileges.RequestPrivilege(k_MLPrivilegeID_CameraCapture);
                }
            }

            public MagicLeapProvider()
            {
                m_PerceptionHandle = PerceptionHandle.Acquire();

                if (s_NativeProviderPtr == IntPtr.Zero)
                    s_NativeProviderPtr = Native.Construct();

                if (RequestPrivilegesIfNecessary())
                {
                    // We set the s_NativeTrackerCreationJobHandle to a 'do nothing' job that quits immediately.
                    // This should have the effect of having any dependencies on s_NativeTrackerCreationJobHandle
                    // automatically go.
                    RcoApi.Retain(s_NativeProviderPtr);
                    if (!Native.CreateNativeTracker(s_NativeProviderPtr))
                    {
                        LogError($"Unable to create native tracker due to internal device error.  Subsystem will be set to invalid.  See native output for more details.");
                    }
                    else
                    {
                        s_NativeTrackerCreationJobHandle = new DummyNoopJob().Schedule();
                    }
                    RcoApi.Release(s_NativeProviderPtr);
                }
                else
                {
                    LogWarning($"Could not start the image tracking subsystem because privileges were denied.");
                }
            }

#if UNITY_2020_2_OR_NEWER
            public override void Start() { }
            public override void Stop() { }
#endif

            /// <summary>
            /// Destroy the image tracking subsystem.
            /// </summary>
            public override void Destroy()
            {
                if (s_NativeProviderPtr != IntPtr.Zero)
                {
                    if (RcoApi.RetainCount(s_NativeProviderPtr) == 0)
                    {
                        // We assume at this point in time that someone else will be maintaining the cleanup
                        // of the Image Tracker
                        LogWarning("");
                        Native.Destroy(s_NativeProviderPtr);
                    }
                    s_NativeProviderPtr = IntPtr.Zero;
                    s_NativeTrackerCreationJobHandle = default(JobHandle);
                    s_SetDatabaseJobHandle = default(JobHandle);
                }

                if (m_PerceptionHandle.active)
                {
                    m_PerceptionHandle.Dispose();

                    // Release retained privileges class
                    MagicLeapPrivileges.Shutdown();
                }
            }

            /// <summary>
            /// The <see cref="JobHandle"/> that refers to the Image Database binding job
            /// </summary>
            /// <remarks>
            /// The initial call to setting the Image Database can be expensive; it can be anywhere from 500 - 6000 ms.
            /// Therefore we do this asynchronously to hide that cost. You can check to see if this <see cref="JobHandle"/>
            /// is busy by testing it against a `default(JobHandle)`.
            /// </remarks>
            struct SetDatabaseTrackerJob : IJob
            {
                [NativeDisableUnsafePtrRestriction]
                public IntPtr nativeProvider;
                [NativeDisableUnsafePtrRestriction]
                public IntPtr databasePointer;

                public void Execute()
                {
                    Native.SetDatabase(nativeProvider, databasePointer);
                    RcoApi.Release(s_NativeProviderPtr);
                }
            }

            /// <summary>
            /// The current <c>RuntimeReferenceImageLibrary</c>.  If <c>null</c> then the subsystem will be set to "off".
            /// </summary>
            public override RuntimeReferenceImageLibrary imageLibrary
            {
                set
                {
                    if (RequestPrivilegesIfNecessary())
                    {
                        if (value == null)
                        {
                            Native.SetDatabase(s_NativeProviderPtr, IntPtr.Zero);
                            MagicLeapFeatures.SetFeatureRequested(Feature.ImageTracking, false);

                            s_SetDatabaseJobHandle = default(JobHandle);
                        }
                        else if (value is MagicLeapImageDatabase database)
                        {
                            if (database.nativeProviderPtr != s_NativeProviderPtr)
                            {
                                throw new InvalidOperationException($"Attempted to set an invalid image library.  The native resource for this library has been released making the library invalid.");
                            }

                            // This is the jobified version of setting the database.
                            if (!s_NativeTrackerCreationJobHandle.Equals(default(JobHandle)))
                            {
                                RcoApi.Retain(s_NativeProviderPtr);
                                s_SetDatabaseJobHandle = new SetDatabaseTrackerJob
                                {
                                    nativeProvider = s_NativeProviderPtr,
                                    databasePointer = database.nativePtr
                                }.Schedule(s_NativeTrackerCreationJobHandle);

                                MagicLeapFeatures.SetFeatureRequested(Feature.ImageTracking, true);
                            }
                        }
                        else
                        {
                            throw new ArgumentException($"The {value.GetType().Name} is not a valid Magic Leap image library.");
                        }
                    }
                }
            }

            public unsafe override TrackableChanges<XRTrackedImage> GetChanges(XRTrackedImage defaultTrackedImage, Allocator allocator)
            {
                if (!IsSubsystemStateValid() ||
                    s_SetDatabaseJobHandle.Equals(default(JobHandle)) ||
                    !s_SetDatabaseJobHandle.IsCompleted)
                    return default(TrackableChanges<XRTrackedImage>);

                void* addedPtr, updatedPtr, removedPtr;
                int addedLength, updatedLength, removedLength, stride;

                var context = Native.AcquireChanges(
                    s_NativeProviderPtr,
                    out addedPtr, out addedLength,
                    out updatedPtr, out updatedLength,
                    out removedPtr, out removedLength,
                    out stride);

                try
                {
                    return new TrackableChanges<XRTrackedImage>(
                        addedPtr, addedLength,
                        updatedPtr, updatedLength,
                        removedPtr, removedLength,
                        defaultTrackedImage, stride,
                        allocator);
                }
                finally
                {
                    Native.ReleaseChanges(context);
                }
            }

            /// <summary>
            /// Stores the requested maximum number of concurrently tracked moving images.
            /// </summary>
            /// <remarks>
            /// Magic Leap Image Tracking has the ability to set an enforcement policy on the maximum number of tracked
            /// moving images.  If the policy has been explicitly set to <c>false</c> by using
            /// <see cref="MagicLeapImageTrackingSubsystem.SetAutomaticImageStationarySettingsEnforcementPolicy"/> then
            /// this value will not be honored by the native provider until the policy is set to <c>true</c>.
            /// </remarks>
            public override int requestedMaxNumberOfMovingImages
            {
                get => m_RequestedNumberOfMovingImages;
                set
                {
                    // The TrySetMaximumNumberOfMovingImages function can not actually fail to set
                    // the native value but it will return false if the value won't be honored.
                    // Therefore it is safe to simply call this and cache the value.
                    Native.TrySetMaximumNumberOfMovingImages(s_NativeProviderPtr, value);
                    m_RequestedNumberOfMovingImages = value;
                }
            }
            int m_RequestedNumberOfMovingImages = 25;

            /// <summary>
            /// Stores the current maximum number of moving images that can be tracked by the native platform.
            /// </summary>
            /// <remarks>
            /// Magic Leap Image Tracking has the ability to set an enforcement policy on the maximum number of tracked
            /// moving images.  If the policy has been explicitly set to <c>false</c> by using
            /// <see cref="MagicLeapImageTrackingSubsystem.SetAutomaticImageStationarySettingsEnforcementPolicy"/> then
            /// this value will indicate the current number of explicitly declared moving images in the current library
            /// otherwise it will return the same value as <see cref="requestedMaxNumberOfMovingImages"/>.
            /// </remarks>
            public override int currentMaxNumberOfMovingImages => Native.GetMaxNumberOfMovingImages();

            /// <summary>
            /// Creates a <c>RuntimeReferenceImageLibrary</c> from the passed in <c>XRReferenceImageLibrary</c> passed in.
            /// </summary>
            /// <param name="serializedLibrary">The <c>XRReferenceImageLibrary</c> that is used to create the <c>RuntimeReferenceImageLibrary</c></param>
            /// <returns>A new <c>RuntimeReferenceImageLibrary</c> created from the old  </returns>
            public override RuntimeReferenceImageLibrary CreateRuntimeLibrary(XRReferenceImageLibrary serializedLibrary)
            {
                if (s_NativeProviderPtr == IntPtr.Zero || s_NativeTrackerCreationJobHandle.Equals(default(JobHandle)))
                    return null;

                return new MagicLeapImageDatabase(serializedLibrary, s_NativeProviderPtr, s_NativeTrackerCreationJobHandle);
            }
        }

        static internal class Native
        {
            public static readonly ulong InvalidHandle = ulong.MaxValue;

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_Construct")]
            public static extern IntPtr Construct();

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_Destroy")]
            public static extern void Destroy(IntPtr nativeProviderPtr);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_IsNativeTrackerHandleValid")]
            public static extern bool IsNativeTrackerHandleValid(IntPtr nativeProviderPtr);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_SetDatabase")]
            public static extern void SetDatabase(IntPtr nativeProviderPtr, IntPtr database);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_AcquireChanges")]
            public static extern unsafe void* AcquireChanges(
                IntPtr nativeProviderPtr,
                out void* addedPtr, out int addedLength,
                out void* updatedPtr, out int updatedLength,
                out void* removedPtr, out int removedLength,
                out int stride);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_ReleaseChanges")]
            public static extern unsafe void ReleaseChanges(void* changes);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_TrySetMaximumNumberOfMovingImages")]
            public static extern bool TrySetMaximumNumberOfMovingImages(IntPtr nativeProviderPtr, int count);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_GetMaximumNumberOfMovingImages")]
            public static extern int GetMaxNumberOfMovingImages();

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_GetAutomaticStationaryImageSettingsEnforcementPolicy")]
            public static extern bool GetAutomaticImageStationarySettingsEnforcementPolicy(IntPtr nativeProviderPtr);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_SetAutomaticStationaryImageSettingsEnforcementPolicy")]
            public static extern void SetAutomaticImageStationarySettingsEnforcementPolicy(IntPtr nativeProviderPtr, bool shouldEnforceSubsystemOwnership);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_TrySetReferenceImageStationary")]
            public static extern bool TrySetReferenceImageStationary(IntPtr nativeProviderPtr, Guid guid, bool isStationary);

            [DllImport("UnityMagicLeap", CallingConvention = CallingConvention.Cdecl, EntryPoint = "UnityMagicLeap_ImageTracking_TryCreateNativeTracker")]
            public static extern bool CreateNativeTracker(IntPtr nativeProviderPtr);
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        static void RegisterDescriptor()
        {
#if PLATFORM_LUMIN
            XRImageTrackingSubsystemDescriptor.Create(new XRImageTrackingSubsystemDescriptor.Cinfo
            {
                id = "MagicLeap-ImageTracking",
#if UNITY_2020_2_OR_NEWER
                providerType = typeof(MagicLeapImageTrackingSubsystem.MagicLeapProvider),
                subsystemTypeOverride = typeof(MagicLeapImageTrackingSubsystem),
#else
                subsystemImplementationType = typeof(MagicLeapImageTrackingSubsystem),
#endif
                supportsMovingImages = true,
                supportsMutableLibrary = true
            });
#endif
        }
    }
}
