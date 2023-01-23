using System.Runtime.InteropServices;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.XR.MagicLeap.Internal;

namespace Tests
{
    public class PerceptionTests
    {
        public unsafe struct CoordinateFrameId
        {
            private fixed ulong data[2];

            public static CoordinateFrameId Invalid
                => default(CoordinateFrameId);
        }

        [DllImport("UnityMagicLeap", EntryPoint="UnityMagicLeap_TryGetPose", CallingConvention = CallingConvention.Cdecl)]
        private static extern bool TryGetPose(CoordinateFrameId id, out Pose pose);

        [Test]
        [UnityPlatform(RuntimePlatform.Lumin)]
        public void CanSuccesfullyAcquirePerceptionHandle()
        {
            using (var handle = PerceptionHandle.Acquire())
                Assert.That(() => handle.active, Is.True);
        }

        [Test]
        [UnityPlatform(RuntimePlatform.Lumin)]
        public void PassingAnInvalidHandleLogsAUsefulError()
        {
            using (var handle = PerceptionHandle.Acquire())
            {
                var result = TryGetPose(CoordinateFrameId.Invalid, out var pose);
                Assert.That(() => result, Is.False);
            }
        }
    }
}