using System;
using System.Linq;
using Unity.Collections;
using Unity.Collections.LowLevel.Unsafe;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.Scripting;
using UnityEngine.XR.InteractionSubsystems;

namespace UnityEngine.XR.MagicLeap
{
    /// <summary>
    /// MagicLeap implementation of the <c>XRGestureSubsystem</c>. Do not create this directly. Use the <c>SubsystemManager</c> instead.
    /// </summary>
    [Preserve]
    public sealed class MagicLeapGestureSubsystem : XRGestureSubsystem
    {
        /// <summary>
        /// A collection of all MagicLeapKeyPoseGestureEvents managed by this subsystem.
        /// This is cleared every frame and refreshed with new gesture events.
        /// </summary>
        public NativeArray<MagicLeapKeyPoseGestureEvent> keyPoseGestureEvents { get { return magicLeapProvider.keyPoseGestureEvents; } }

        /// <summary>
        /// A collection of all MagicLeapTouchpadGestureEvents managed by this subsystem.
        /// This is cleared every frame and refreshed with new gesture events.
        /// </summary>
        public NativeArray<MagicLeapTouchpadGestureEvent> touchpadGestureEvents { get { return magicLeapProvider.touchpadGestureEvents; } }

        MagicLeapGestureProvider magicLeapProvider;

        /// <summary>
        /// Creates the provider interface.
        /// </summary>
        /// <returns>The provider interface for MagicLeap</returns>
        protected override Provider CreateProvider()
        {
            magicLeapProvider = new MagicLeapGestureProvider();
            return magicLeapProvider;
        }

        internal bool ControllerGesturesEnabled
        {
            get
            {
#if PLATFORM_LUMIN
                return NativeApi.IsControllerGesturesEnabled();
#else
                Debug.LogWarning("Attempting to get MagicLeapGestureSubsystem.ControllerGesturesEnabled while not on the Lumin platform.  This will be ignored.");
                return false;
#endif
            }
            set
            {
#if PLATFORM_LUMIN
                NativeApi.SetControllerGesturesEnabled(value);
#else
                Debug.LogWarning("Attempting to set MagicLeapGestureSubsystem.ControllerGesturesEnabled while not on the Lumin platform.  This will be ignored.");
#endif
            }
        }

        internal bool HandGesturesEnabled
        {
            get
            {
#if PLATFORM_LUMIN
                return NativeApi.IsHandGesturesEnabled();
#else
                Debug.LogWarning("Attempting to get MagicLeapGestureSubsystem.HandGesturesEnabled while not on the Lumin platform.  This will be ignored.");
                return false;
#endif
            }
            set
            {
#if PLATFORM_LUMIN
                NativeApi.SetHandGesturesEnabled(value);
#else
                Debug.LogWarning("Attempting to set MagicLeapGestureSubsystem.HandGesturesEnabled while not on the Lumin platform.  This will be ignored.");
#endif
            }
        }

        class MagicLeapGestureProvider : Provider
        {
            public MagicLeapGestureProvider()
            {
#if PLATFORM_LUMIN
                NativeApi.Create();
#endif
            }

            public override void Start()
            {
#if PLATFORM_LUMIN
                NativeApi.Start();
#endif
            }

            public override void Stop()
            {
#if PLATFORM_LUMIN
                NativeApi.Stop();
#endif
            }

            public override void Update()
            {
#if PLATFORM_LUMIN
                NativeApi.Update();

                RetrieveGestureEvents();
#endif
            }

            public unsafe delegate void* GetGesturesDelegate(out int gestureEventsLength, out int elementSize);

            unsafe void GetGestureEvents<T>(ref NativeArray<T> gestureEventsArray, GetGesturesDelegate getGesturesAction) where T : struct
            {
                // gestureEventsPtr is not owned by this code, contents must be copied out in this function.
                int gestureEventsLength, elementSize;
                void* gestureEventsPtr = getGesturesAction(out gestureEventsLength, out elementSize);

                if (!(gestureEventsLength == 0 && gestureEventsArray.Length == 0))
                {
                    if (gestureEventsArray.IsCreated)
                        gestureEventsArray.Dispose();
                    gestureEventsArray = new NativeArray<T>(gestureEventsLength, Allocator.Persistent);

                    var sizeOfGestureEvent = UnsafeUtility.SizeOf<T>();
                    UnsafeUtility.MemCpy(gestureEventsArray.GetUnsafePtr(), gestureEventsPtr, elementSize * gestureEventsLength);
                }
            }

            unsafe void RetrieveGestureEvents()
            {
                if (NativeApi.IsHandGesturesEnabled())
                    GetGestureEvents<MagicLeapKeyPoseGestureEvent>(ref m_KeyPoseGestureEvents, NativeApi.GetKeyPoseGestureEventsPtr);
                if (NativeApi.IsControllerGesturesEnabled())
                    GetGestureEvents<MagicLeapTouchpadGestureEvent>(ref m_TouchpadGestureEvents, NativeApi.GetTouchpadGestureEventsPtr);

                // Count up valid activate gestures (Have to do this as we cannot dynamically grow NativeArray).
                // This should be possible to fix with NativeList (when out of preview package).
                int activateGestureEventCount = 0;
                foreach (var gestureEvent in m_KeyPoseGestureEvents)
                {
                    if (gestureEvent.state == GestureState.Started && gestureEvent.keyPose == MagicLeapKeyPose.Finger)
                        activateGestureEventCount++;
                }

                if (m_ActivateGestureEvents.IsCreated)
                    m_ActivateGestureEvents.Dispose();
                m_ActivateGestureEvents = new NativeArray<ActivateGestureEvent>(activateGestureEventCount, Allocator.Persistent);

                int iActivateGestureEvent = 0;
                foreach (var gestureEvent in m_KeyPoseGestureEvents)
                {
                    if (gestureEvent.state == GestureState.Started && gestureEvent.keyPose == MagicLeapKeyPose.Finger)
                        m_ActivateGestureEvents[iActivateGestureEvent++] =
                            new ActivateGestureEvent(GetNextGUID(), gestureEvent.state);
                }
            }

            public override void Destroy()
            {
#if PLATFORM_LUMIN
                NativeApi.Destroy();

                m_KeyPoseGestureEvents.Dispose();
                m_TouchpadGestureEvents.Dispose();
#endif
                base.Destroy();
            }

            public NativeArray<MagicLeapKeyPoseGestureEvent> keyPoseGestureEvents { get { return m_KeyPoseGestureEvents; } }
            NativeArray<MagicLeapKeyPoseGestureEvent> m_KeyPoseGestureEvents = new NativeArray<MagicLeapKeyPoseGestureEvent>(0, Allocator.Persistent);

            public NativeArray<MagicLeapTouchpadGestureEvent> touchpadGestureEvents { get { return m_TouchpadGestureEvents; } }
            NativeArray<MagicLeapTouchpadGestureEvent> m_TouchpadGestureEvents = new NativeArray<MagicLeapTouchpadGestureEvent>(0, Allocator.Persistent);
        }

#if UNITY_EDITOR || PLATFORM_LUMIN
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
#endif
        static void RegisterDescriptor()
        {
#if PLATFORM_LUMIN
            XRGestureSubsystemDescriptor.RegisterDescriptor(
                new XRGestureSubsystemDescriptor.Cinfo
                {
                    id = "MagicLeap-Gesture",
                    subsystemImplementationType = typeof(MagicLeapGestureSubsystem),
                }
            );
#endif
        }

        static class NativeApi
        {
            const string Library = "UnityMagicLeap";

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesCreate")]
            public static extern void Create();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesUpdate")]
            public static extern void Update();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesStart")]
            public static extern void Start();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesGetKeyPoseGestureEventsPtr")]
            public static extern unsafe void* GetKeyPoseGestureEventsPtr(out int gesturesLength, out int elementSize);

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesGetTouchpadGestureEventsPtr")]
            public static extern unsafe void* GetTouchpadGestureEventsPtr(out int gesturesLength, out int elementSize);

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesDestroy")]
            public static extern void Destroy();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesStop")]
            public static extern void Stop();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesIsControllerGesturesEnabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool IsControllerGesturesEnabled();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesIsHandGesturesEnabled")]
            [return: MarshalAs(UnmanagedType.I1)]
            public static extern bool IsHandGesturesEnabled();

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesSetControllerGesturesEnabled")]
            public static extern void SetControllerGesturesEnabled([MarshalAs(UnmanagedType.I1)] bool value);

            [DllImport(Library, EntryPoint="UnityMagicLeap_GesturesSetHandGesturesEnabled")]
            public static extern void SetHandGesturesEnabled([MarshalAs(UnmanagedType.I1)] bool value);
        }

        // High GUID bits saved for common (Activate) gesture for this subsystem
        static GestureId s_NextGUID = new GestureId(1, 0);
        static GestureId GetNextGUID()
        {
            unchecked
            {
                s_NextGUID.subId1 += 1;
                if (s_NextGUID.subId1 != 0) return s_NextGUID;
                s_NextGUID.subId1 += 1;
            }

            return s_NextGUID;
        }
    }
}
