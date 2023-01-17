using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using UnityEditor;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    // Create a new type of Settings Asset.
    public class MagicLeapManifestSettings : ScriptableObject
    {
        public const string kBuildManifestPath = "Temp/StagingArea/Temp/manifest.xml";
        public const string kCustomManifestPath = "Assets/Plugins/Lumin/manifest.xml";
        public const string kDefaultSettingsPath = "Assets/Plugins/Lumin/MagicLeapManifestSettings.asset";

        [SerializeField]
        private int m_MinimumAPILevel;

        [SerializeField]
        private bool m_AllowBackgroundMusicService;

        [SerializeField]
        private PrivilegeGroup[] m_PrivilegeGroups;

        public int minimumAPILevel
        {
            get
            {
                return m_MinimumAPILevel;
            }
            set
            {
                Undo.RecordObject(this, "Changed Minimum API Level");
                m_MinimumAPILevel = value;
            }
        }

        public bool allowBackgroundMusicService
        {
            get
            {
                return m_AllowBackgroundMusicService;
            }
            set
            {
                Undo.RecordObject(this, "Changed Allow Background Music Service");
                m_AllowBackgroundMusicService = value;
            }
        }

        public IEnumerable<string> requiredPermissions
        {
            get
            {
                return privileges.Where(p => p.Enabled).Select(p => p.Name);
            }
        }

        IEnumerable<Privilege> privileges => m_PrivilegeGroups.SelectMany(pg => pg.Privileges);

        public bool TryGetPrivilegeRequested(string name, out bool isRequested)
        {
            isRequested = false;
            foreach (var priv in privileges)
            {
                if (priv.Name == name)
                {
                    isRequested = priv.Enabled;
                    return true;
                }
            }
            return false;
        }

        public static bool customManifestExists
        {
            get { return File.Exists(kCustomManifestPath); }
        }


        public static MagicLeapManifestSettings GetOrCreateSettings(string path = kDefaultSettingsPath)
        {
            if (string.IsNullOrEmpty(path))
                throw new ArgumentException("path");
            var dir = Path.GetDirectoryName(path);
            if (!Directory.Exists(dir))
                Directory.CreateDirectory(dir);
            var settings = AssetDatabase.LoadAssetAtPath<MagicLeapManifestSettings>(path);
            if (settings == null)
            {
                settings = ScriptableObject.CreateInstance<MagicLeapManifestSettings>();
                settings.m_MinimumAPILevel = 4;
                settings.m_AllowBackgroundMusicService = false;
                if (SDKUtility.sdkAvailable)
                {
                    settings.RebuildPrivilegeGroups(PrivilegeParser.ParsePrivilegesFromHeader(Path.Combine(SDKUtility.sdkPath, PrivilegeParser.kPrivilegeHeaderPath)));
                }
                else
                    Debug.LogWarning(ManifestEditorGUI.Messages.kCannotLocateSDK);

                //settings.m_Privileges = new Privilege[1] { new Privilege { Name = "LowLatencyLightwear", Enabled = true } };
                AssetDatabase.CreateAsset(settings, path);
                AssetDatabase.SaveAssets();
            }
            return settings;
        }

        internal void RebuildPrivilegeGroups(IEnumerable<PrivilegeDescriptor> privilegeDescriptors)
        {
            var groups = new Dictionary<string, List<Privilege>>();
            foreach (var pd in privilegeDescriptors)
            {
                if (pd.category == Privilege.Category.Invalid) continue; // ignore invalid privileges..
                if (pd.category == Privilege.Category.Trusted) continue; // ignore trusted privileges..
                var name = pd.category.ToString();
                if (!groups.ContainsKey(name))
                    groups[name] = new List<Privilege>();
                groups[name].Add(new Privilege { ApiLevel = pd.apiLevel.HasValue ? (uint)pd.apiLevel.Value : 0, Name = pd.name, Enabled = false });
            }

            var pg_list = new List<PrivilegeGroup>();
            foreach (var pg in groups)
            {
                pg_list.Add(new PrivilegeGroup { Name = pg.Key, Privileges = pg.Value.ToArray() });
            }
            m_PrivilegeGroups = pg_list.ToArray();
        }

        internal SerializedObject ToSerializedObject()
        {
            return new SerializedObject(this);
        }
    }

    // Register a SettingsProvider using UIElements for the drawing framework:
    static class MagicLeapManifestSettingsRegister
    {
        [SettingsProvider]
        public static SettingsProvider CreateSettingsProvider()
        {
            // First parameter is the path in the Settings window.
            // Second parameter is the scope of this setting: it only appears in the Settings window for the Project scope.
            var provider = new SettingsProvider("MagicLeap/", SettingsScope.Project)
            {
                label = "Manifest Settings",
                guiHandler = (searchContext) =>
                {
                    var settings = MagicLeapManifestSettings.GetOrCreateSettings();
                    ManifestEditorGUI.RenderManifest(settings);
                },
                // Populate the search keywords to enable smart search filtering and label highlighting:
                keywords = new HashSet<string>(new[] { "MagicLeap", "Manifest" })
            };

            return provider;
        }
    }
}