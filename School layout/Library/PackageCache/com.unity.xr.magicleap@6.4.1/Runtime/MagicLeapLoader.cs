using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;

using UnityEngine.XR;
using UnityEngine.XR.ARSubsystems;
using UnityEngine.XR.InteractionSubsystems;
using UnityEngine.XR.MagicLeap.Meshing;
using UnityEngine.XR.Management;

#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.XR.MagicLeap.Remote;
using UnityEditor.XR.Management;
using UnityEngine.Rendering;
#endif //UNITY_EDITOR

#if UNITY_INPUT_SYSTEM
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.InputSystem.XR;
#endif //UNITY_INPUT_SYSTEM

#if UNITY_2020_1_OR_NEWER
using XRTextureLayout = UnityEngine.XR.XRDisplaySubsystem.TextureLayout;
#endif // UNITY_2020_1_OR_NEWER

namespace UnityEngine.XR.MagicLeap
{
#if UNITY_INPUT_SYSTEM
#if UNITY_EDITOR
    [InitializeOnLoad]
#endif //UNITY_EDITOR
    static class InputLayoutLoader
    {
#if UNITY_EDITOR
        static InputLayoutLoader()
        {
            RegisterInputLayouts();
        }
#endif //UNITY_EDITOR

        public static void RegisterInputLayouts()
        {
            UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapLightwear>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("MagicLeap Lightwear")
            );
            UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapController>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("MagicLeap Controller")
            );
            UnityEngine.InputSystem.InputSystem.RegisterLayout<MagicLeapHandDevice>(
                matches: new InputDeviceMatcher()
                    .WithInterface(XRUtilities.InterfaceMatchAnyVersion)
                    .WithProduct("^(MagicLeapHand - )")
            );
        }
    }
#endif //UNITY_INPUT_SYSTEM

#if UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    [XRSupportedBuildTarget(BuildTargetGroup.Lumin)]
    [XRSupportedBuildTarget(BuildTargetGroup.Standalone, new BuildTarget[]{BuildTarget.StandaloneOSX, BuildTarget.StandaloneWindows, BuildTarget.StandaloneWindows64})]
#endif // UNITY_EDITOR && XR_MANAGEMENT_3_2_0_OR_NEWER
    public sealed class MagicLeapLoader : XRLoaderHelper
    {
        static class Graphics
        {
#if PLATFORM_LUMIN
            const string k_Library = "ml_graphics";
            [DllImport(k_Library, CallingConvention = CallingConvention.Cdecl, EntryPoint = "MLGraphicsEnableBlobCacheGL")]
            static extern uint EnableBlobCacheGL(string path, uint max_entry_bytes, uint max_total_bytes);
#else
            static uint EnableBlobCacheGL(string path, uint max_entry_bytes, uint max_total_bytes) => 0;
#endif // PLATFORM_LUMIN

            internal static void SetupGLCache(MagicLeapSettings.GLCache cacheSettings)
            {
                var result = EnableBlobCacheGL(cacheSettings.cachePath, cacheSettings.maxBlobSizeInBytes, cacheSettings.maxFileSizeInBytes);
                MagicLeapLogger.AssertError(result == 0, kLogTag, $"Failed to initialize blob cache ({cacheSettings.cachePath} / {cacheSettings.maxBlobSizeInBytes} / {cacheSettings.maxFileSizeInBytes})");
            }
        }
        enum Privileges : uint
        {
            ControllerPose = 263,
            GesturesConfig = 269,
            GesturesSubscribe = 268,
            LowLatencyLightwear = 59,
            WorldReconstruction = 33
        }
        const string kLogTag = "MagicLeapLoader";
        // Integrated Subsystems
        static List<XRDisplaySubsystemDescriptor> s_DisplaySubsystemDescriptors = new List<XRDisplaySubsystemDescriptor>();
        static List<XRInputSubsystemDescriptor> s_InputSubsystemDescriptors = new List<XRInputSubsystemDescriptor>();
        static List<XRMeshSubsystemDescriptor> s_MeshSubsystemDescriptor = new List<XRMeshSubsystemDescriptor>();
        static List<XRGestureSubsystemDescriptor> s_GestureSubsystemDescriptors = new List<XRGestureSubsystemDescriptor>();

        public XRDisplaySubsystem displaySubsystem => GetLoadedSubsystem<XRDisplaySubsystem>();
        public XRInputSubsystem inputSubsystem => GetLoadedSubsystem<XRInputSubsystem>();
        public XRMeshSubsystem meshSubsystem => GetLoadedSubsystem<XRMeshSubsystem>();
        public XRGestureSubsystem gestureSubsystem => GetLoadedSubsystem<XRGestureSubsystem>();

        // ARSubsystems
        static List<XRSessionSubsystemDescriptor> s_SessionSubsystemDescriptors = new List<XRSessionSubsystemDescriptor>();
        static List<XRPlaneSubsystemDescriptor> s_PlaneSubsystemDescriptors = new List<XRPlaneSubsystemDescriptor>();
        static List<XRAnchorSubsystemDescriptor> s_AnchorSubsystemDescriptors = new List<XRAnchorSubsystemDescriptor>();
        static List<XRRaycastSubsystemDescriptor> s_RaycastSubsystemDescriptors = new List<XRRaycastSubsystemDescriptor>();
        static List<XRImageTrackingSubsystemDescriptor> s_ImageTrackingSubsystemDescriptors = new List<XRImageTrackingSubsystemDescriptor>();

        public XRSessionSubsystem sessionSubsystem => GetLoadedSubsystem<XRSessionSubsystem>();
        public XRPlaneSubsystem planeSubsystem => GetLoadedSubsystem<XRPlaneSubsystem>();
        public XRAnchorSubsystem anchorSubsystem => GetLoadedSubsystem<XRAnchorSubsystem>();
        public XRRaycastSubsystem raycastSubsystem => GetLoadedSubsystem<XRRaycastSubsystem>();
        public XRImageTrackingSubsystem imageTrackingSubsystem => GetLoadedSubsystem<XRImageTrackingSubsystem>();

#if UNITY_EDITOR
        public static MagicLeapLoader assetInstance => (MagicLeapLoader)AssetDatabase.LoadAssetAtPath("Packages/com.unity.xr.magicleap/XR/Loaders/Magic Leap Loader.asset", typeof(MagicLeapLoader));
#endif // UNITY_EDITOR

        private bool m_DisplaySubsystemRunning = false;
        private int m_MeshSubsystemRefcount = 0;

        public override bool Initialize()
        {
#if UNITY_EDITOR
            if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.OpenGLCore)
            {
                Debug.LogWarning(
                    "To use Magic Leap Zero Iteration mode, the editor must be running under OpenGL.\n" +
                    " 1) Go to Edit -> Project Settings -> Player, and select the first tab there (Standalone Settings)...\n" +
                    " 2) Open the 'Other Settings' section, and uncheck 'Auto Graphics API for Windows' An API list will appear.\n" +
                    " 3) Use the + button to add 'OpenGLCore' to the API list.\n" +
                    " 4) Drag it to the top of the list. The editor will now switch to using OpenGL.\n");
            }

            if (!MagicLeapRemoteManager.Initialize())
                return false;
#endif // UNITY_EDITOR

#if UNITY_INPUT_SYSTEM
            InputLayoutLoader.RegisterInputLayouts();
#endif
            MagicLeapPrivileges.Initialize();

            ApplySettings();

            // Display Subsystem depends on Input Subsystem, so initialize that first.
            CheckForInputRelatedPermissions();
            CreateSubsystem<XRInputSubsystemDescriptor, XRInputSubsystem>(s_InputSubsystemDescriptors, "MagicLeap-Input");
            CreateSubsystem<XRDisplaySubsystemDescriptor, XRDisplaySubsystem>(s_DisplaySubsystemDescriptors, "MagicLeap-Display");
            CreateSubsystem<XRGestureSubsystemDescriptor, XRGestureSubsystem>(s_GestureSubsystemDescriptors, "MagicLeap-Gesture");

            if (CanCreateMeshSubsystem())
            {
                CreateSubsystem<XRMeshSubsystemDescriptor, XRMeshSubsystem>(s_MeshSubsystemDescriptor, "MagicLeap-Mesh");
                if (meshSubsystem != null)
                {
                    // Register native callbacks for feature API
                    meshSubsystem.RegisterNativeSubsystemCallbacks();

                    MeshingSettings.meshingSettings = MLSpatialMapper.GetDefaultMeshingSettings();
                    MeshingSettings.batchSize = MLSpatialMapper.Defaults.batchSize;
                    MeshingSettings.density = MLSpatialMapper.Defaults.density;
                    MeshingSettings.SetBounds(Vector3.zero, Quaternion.identity, MLSpatialMapper.Defaults.boundsExtents);
                }
            }

            // Now that subsystem creation is strictly handled by the loaders we must create the following subsystems
            // that live in ARSubsystems
            CreateSubsystem<XRSessionSubsystemDescriptor, XRSessionSubsystem>(s_SessionSubsystemDescriptors, "MagicLeap-Session");
            CreateSubsystem<XRPlaneSubsystemDescriptor, XRPlaneSubsystem>(s_PlaneSubsystemDescriptors, "MagicLeap-Planes");
            CreateSubsystem<XRAnchorSubsystemDescriptor, XRAnchorSubsystem>(s_AnchorSubsystemDescriptors, "MagicLeap-Anchor");
            CreateSubsystem<XRRaycastSubsystemDescriptor, XRRaycastSubsystem>(s_RaycastSubsystemDescriptors, "MagicLeap-Raycast");
            CreateSubsystem<XRImageTrackingSubsystemDescriptor, XRImageTrackingSubsystem>(s_ImageTrackingSubsystemDescriptors, "MagicLeap-ImageTracking");

            return true;
        }

        public override bool Start()
        {
            StartSubsystem<XRInputSubsystem>();
            StartSubsystem<XRGestureSubsystem>();

            // If we have the ControllerPose privilege, let's also ensure that the Controller Api is enabled
            var hasControllerPose = MagicLeapPrivileges.IsPrivilegeApproved((uint) Privileges.ControllerPose);
            inputSubsystem?.SetControllerApiEnabled(hasControllerPose);

            if (!isLegacyDeviceActive)
            {
                StartSubsystem<XRDisplaySubsystem>();

                // we need to set the layout after the display subsystem has started, as these values are not used during
                // the XRDisplayProvider's startup (Starting the Subsystem).
                var settings = MagicLeapSettings.currentSettings;
#if UNITY_2020_1_OR_NEWER
                if (settings != null && settings.forceMultipass)
                    displaySubsystem.textureLayout = XRTextureLayout.SeparateTexture2Ds;
                else
                    displaySubsystem.textureLayout = XRTextureLayout.Texture2DArray;
#else
                if (settings != null && settings.forceMultipass)
                    displaySubsystem.singlePassRenderingDisabled = true;
                else
                    displaySubsystem.singlePassRenderingDisabled = false;
#endif // UNITY_2020_1_OR_NEWER
                m_DisplaySubsystemRunning = true;
            }

            return true;
        }

        public override bool Stop()
        {
            if (m_DisplaySubsystemRunning)
            {
                StopSubsystem<XRDisplaySubsystem>();
                m_DisplaySubsystemRunning = false;
            }
            if (CanCreateMeshSubsystem() && m_MeshSubsystemRefcount > 0)
            {
                m_MeshSubsystemRefcount = 0;
                StopSubsystem<XRMeshSubsystem>();
            }
            StopSubsystem<XRInputSubsystem>();
            StopSubsystem<XRGestureSubsystem>();
            return true;
        }

        public override bool Deinitialize()
        {
            if (CanCreateMeshSubsystem())
                DestroySubsystem<XRMeshSubsystem>();
            DestroySubsystem<XRDisplaySubsystem>();
            DestroySubsystem<XRGestureSubsystem>();
            DestroySubsystem<XRInputSubsystem>();
            DestroySubsystem<XRImageTrackingSubsystem>();
            DestroySubsystem<XRRaycastSubsystem>();
            DestroySubsystem<XRAnchorSubsystem>();
            DestroySubsystem<XRPlaneSubsystem>();
            DestroySubsystem<XRSessionSubsystem>();
            MagicLeapPrivileges.Shutdown();
            return true;
        }

        internal static bool isLegacyDeviceActive
        {
            get { return XRSettings.enabled && (XRSettings.loadedDeviceName == "Lumin"); }
        }

        internal void StartMeshSubsystem()
        {
            if (!CanCreateMeshSubsystem())
                return;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            m_MeshSubsystemRefcount += 1;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 1)
            {
                MagicLeapLogger.Debug(kLogTag, "Starting Mesh Subsystem");
                StartSubsystem<XRMeshSubsystem>();
            }
        }

        internal void StopMeshSubsystem()
        {
            if (!CanCreateMeshSubsystem())
                return;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 0)
                return;

            m_MeshSubsystemRefcount -= 1;
            //MagicLeapLogger.Debug(kLogTag, "m_MeshSubsystemRefcount: {0}", m_MeshSubsystemRefcount);
            if (m_MeshSubsystemRefcount == 0)
            {
                MagicLeapLogger.Debug(kLogTag, "Stopping Mesh Subsystem");
                StopSubsystem<XRMeshSubsystem>();
            }
        }

        private void ApplySettings()
        {
            var settings = MagicLeapSettings.currentSettings;
            if (settings != null)
            {
                // set depth buffer precision
                MagicLeapLogger.Debug(kLogTag, $"Setting Depth Precision: {settings.depthPrecision}");
                Rendering.RenderingSettings.depthPrecision = settings.depthPrecision;
                // set frame timing hint
                MagicLeapLogger.Debug(kLogTag, $"Setting Frame Timing Hint: {settings.frameTimingHint}");
                Rendering.RenderingSettings.frameTimingHint = settings.frameTimingHint;

#if PLATFORM_LUMIN && !UNITY_EDITOR
                if (settings.glCacheSettings.enabled)
                {
                    Graphics.SetupGLCache(settings.glCacheSettings);
                }
#endif // PLATFORM_LUMIN && !UNITY_EDITOR
            }
        }

        [Conditional("DEVELOPMENT_BUILD")]
        private void CheckForInputRelatedPermissions()
        {
            if (!MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.ControllerPose))
                Debug.LogWarning("No controller privileges specified; Controller data will not be available via XRInput Subsystem!");
            if (!(MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.GesturesConfig) && MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.GesturesSubscribe)))
                Debug.LogWarning("No gestures privileges specified; Gesture and Hand data will not be available via XRInput Subsystem!");
        }

        private bool CanCreateMeshSubsystem()
        {
            if (MagicLeapPrivileges.IsPrivilegeApproved((uint)Privileges.WorldReconstruction))
                return true;
#if DEVELOPMENT_BUILD
            Debug.LogError("Unable to create Mesh Subsystem due to missing 'WorldReconstruction' privilege. Please add to manifest");
#endif // DEVELOPMENT_BUILD
            return false;
        }
    }
#if UNITY_EDITOR
    internal static class XRMangementEditorExtensions
    {
        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTargetGroup group)
        {
            var settings = XRGeneralSettingsPerBuildTarget.XRGeneralSettingsForBuildTarget(group);
            return settings?.Manager?.activeLoaders?.Contains(loader) ?? false;
        }

        internal static bool IsEnabledForPlatform(this XRLoader loader, BuildTarget target)
        {
            return loader.IsEnabledForPlatform(BuildPipeline.GetBuildTargetGroup(target));
        }
    }
#endif // UNITY_EDITOR
}
