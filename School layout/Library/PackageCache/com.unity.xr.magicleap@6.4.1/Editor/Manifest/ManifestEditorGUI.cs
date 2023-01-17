using System;
using System.Collections.Generic;
using System.IO;

using UnityEditor;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    internal static class ManifestEditorGUI
    {
        internal static class Messages
        {
            internal const string kCannotLocateSDK = "Cannot find the Lumin SDK. Please specify the path to the Lumin SDK in the editor preferences";
            internal const string kUnusableSDK = "Your Lumin SDK appears to have an API level below the minimum API level. Please upgrade your Lumin SDK";
            internal const string kShouldSynchronize = "Click the 'Synchronize' button below to update the privileges list against the Lumin SDK specified in the editor preferences. If you've recently upgraded the Lumin SDK, you'll need to do this to get access to newer privileges";
            internal const string kLuminPlatformDeprecation = "Unity 2020 LTS will be the last version of the editor which supports Magic Leap 1. Developers can continue to build for Magic Leap 1 using Unity 2020 LTS or 2019 LTS.";
        }
        class RenderState
        {
            private Lazy<Dictionary<string, bool>> _GroupFoldoutExpanded = new Lazy<Dictionary<string, bool>>(() => new Dictionary<string, bool>());
            public Dictionary<string, bool> GroupFoldoutExpanded => _GroupFoldoutExpanded.Value;
            public bool IsPerformingSDKUpdate { get; set; }
        }
        public static void RenderManifest(MagicLeapManifestSettings settings)
        {
            var serializedObject = settings.ToSerializedObject();
            serializedObject.UpdateIfRequiredOrScript();
            var id = GUIUtility.GetControlID(FocusType.Passive);
            var state = (RenderState)GUIUtility.GetStateObject(typeof(RenderState), id);
            var missingSdk = !SDKUtility.sdkAvailable;
            if (missingSdk)
                EditorGUILayout.HelpBox(Messages.kCannotLocateSDK, MessageType.Error, true);
            var invalidSdk = !SDKUtility.isCompatibleSDK;
            if (invalidSdk)
                EditorGUILayout.HelpBox(Messages.kUnusableSDK, MessageType.Error, true);
            using (new EditorGUI.DisabledScope(missingSdk || invalidSdk))
            {
#if UNITY_2020_1_OR_NEWER
                EditorGUILayout.HelpBox(Messages.kLuminPlatformDeprecation, MessageType.Warning, true);
#endif
                var apiLevel = serializedObject.FindProperty("m_MinimumAPILevel");
                apiLevel.intValue = PlatformLevelSelector.SelectorGUI(apiLevel.intValue);

                var allowBMS = serializedObject.FindProperty("m_AllowBackgroundMusicService");
                allowBMS.boolValue = EditorGUILayout.ToggleLeft("Allow Background Music Service", allowBMS.boolValue);

                EditorGUILayout.LabelField("Privileges", EditorStyles.boldLabel);
                var priv_groups = serializedObject.FindProperty("m_PrivilegeGroups");
                for (int i = 0; i < priv_groups.arraySize; i++)
                {
                    var group = priv_groups.GetArrayElementAtIndex(i);
                    var group_name = group.FindPropertyRelative("m_Name").stringValue;
                    if (!state.GroupFoldoutExpanded.TryGetValue(group_name, out var _))
                        state.GroupFoldoutExpanded[group_name] = false;
                    state.GroupFoldoutExpanded[group_name] = EditorGUILayout.BeginFoldoutHeaderGroup(state.GroupFoldoutExpanded[group_name], group_name);
                    if (state.GroupFoldoutExpanded[group_name])
                    {
                        var privs = group.FindPropertyRelative("m_Privileges");
                        for (int j = 0; j < privs.arraySize; j++)
                        {
                            var priv = privs.GetArrayElementAtIndex(j);
                            var enabled = priv.FindPropertyRelative("m_Enabled");
                            var name = priv.FindPropertyRelative("m_Name").stringValue;

                            var disabled = ShouldDisable(apiLevel.intValue, priv);
                            var value = UpdateValue(name, enabled.boolValue, disabled);
                            using (new EditorGUI.DisabledScope(disabled))
                            using (new EditorGUI.IndentLevelScope())
                                enabled.boolValue = EditorGUILayout.ToggleLeft(name, value);
                        }
                    }
                    EditorGUILayout.EndFoldoutHeaderGroup();
                }

                serializedObject.ApplyModifiedProperties();
                GUILayout.FlexibleSpace();
                EditorGUILayout.HelpBox(Messages.kShouldSynchronize, MessageType.Info, true);
                var sdkUpdateRequested = GUILayout.Button("Synchronize");
                if (sdkUpdateRequested)
                {
                    state.IsPerformingSDKUpdate = true;
                    EditorApplication.delayCall += () => {
                        settings.RebuildPrivilegeGroups(PrivilegeParser.ParsePrivilegesFromHeader(Path.Combine(SDKUtility.sdkPath, PrivilegeParser.kPrivilegeHeaderPath)));
                        state.IsPerformingSDKUpdate = false;
                    };
                }
            }
        }

        private static bool ShouldDisable(int apiLevel, SerializedProperty priv)
        {
            var privApiLevel = priv.FindPropertyRelative("m_ApiLevel");
            if (privApiLevel.intValue > apiLevel)
                return true;
            var privName = priv.FindPropertyRelative("m_Name");
            // some privileges require special handling
            if (privName.stringValue == "LowLatencyLightwear")
                return true; // always enable LowLatencyLightwear"
            if (privName.stringValue == "Occlusion" && apiLevel > 4)
                return true;
            return false;
        }

        private static bool UpdateValue(string name, bool value, bool disabled)
        {
            if (name == "LowLatencyLightwear")
                return true;
            else if (disabled)
                return false;
            return value;
        }
    }
}