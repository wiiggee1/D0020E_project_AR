using System;
using System.Runtime.InteropServices;

using UnityEngine;
using UnityEngine.XR;

namespace UnityEngine.XR.MagicLeap.Meshing
{
    public static class MeshingSettings
    {

        public static IntPtr AcquireConfidence(MeshId meshId, out int count) => UnityMagicLeap_MeshingAcquireConfidence(meshId, out count);
        public static void ReleaseConfidence(MeshId meshId) => UnityMagicLeap_MeshingReleaseConfidence(meshId);
        public static void SetBounds(Transform transform, Vector3 extents)
        {
            SetBounds(transform.localPosition, transform.localRotation, extents);
        }

        public static void SetBounds(Vector3 position, Quaternion rotation, Vector3 extents)
        {
            UnityMagicLeap_MeshingSetBounds(position, rotation, extents);
        }

        public static int batchSize
        {
            set { UnityMagicLeap_MeshingSetBatchSize(value); }
        }

        public static float density
        {
            set { UnityMagicLeap_MeshingSetDensity(value); }
        }

        public static MLMeshingSettings meshingSettings
        {
            set { UnityMagicLeap_MeshingUpdateSettings(ref value); }
        }

#if PLATFORM_LUMIN
        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingUpdateSettings(ref MLMeshingSettings newSettings);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetDensity(float density);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingSetBatchSize(int batchSize);

        [DllImport("UnityMagicLeap")]
        internal static extern IntPtr UnityMagicLeap_MeshingAcquireConfidence(MeshId meshId, out int count);

        [DllImport("UnityMagicLeap")]
        internal static extern void UnityMagicLeap_MeshingReleaseConfidence(MeshId meshId);
#else
        internal static void UnityMagicLeap_MeshingUpdateSettings(ref MLMeshingSettings newSettings) { }

        internal static void UnityMagicLeap_MeshingSetDensity(float density) { }

        internal static void UnityMagicLeap_MeshingSetBounds(Vector3 center, Quaternion rotation, Vector3 extents) { }

        internal static void UnityMagicLeap_MeshingSetBatchSize(int batchSize) {}

        internal static IntPtr UnityMagicLeap_MeshingAcquireConfidence(MeshId meshId, out int count) { count = 0; return IntPtr.Zero; }

        internal static void UnityMagicLeap_MeshingReleaseConfidence(MeshId meshId) { }
#endif
    }

    [Flags]
    public enum MLMeshingFlags
    {
        None = 0,
        PointCloud = 1 << 0,
        ComputeNormals = 1 << 1,
        ComputeConfidence = 1 << 2,
        Planarize = 1 << 3,
        RemoveMeshSkirt = 1 << 4,
        IndexOrderCCW = 1 << 5
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct MLMeshingSettings
    {
        public MLMeshingFlags flags;
        public float fillHoleLength;
        public float disconnectedComponentArea;
    }
}