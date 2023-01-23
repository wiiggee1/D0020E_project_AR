using System.Collections;
using System.Linq;
using NUnit.Framework;
using UnityEngine.TestTools;

using UnityEditor.XR.MagicLeap;
using MLRemote;
using UnityEngine;

public class XrSdkPlayModeSetupTests
{
    const string kImportTestPath = "Plugins/Lumin/Editor/x64";

    [SetUp]
    public void Setup()
    {
        var tp = new TemporaryAssetPath(kImportTestPath);
        MagicLeapRemoteImportSupport.ImportSupportLibrares(tp.path);
    }

    [Ignore("This is a fake test to setup playmode tests with the ml library")]
    [UnityTest]
    public IEnumerator ImportMagicLeapLibrary()
    {
        yield return new WaitForSeconds(5f);
    }
}

