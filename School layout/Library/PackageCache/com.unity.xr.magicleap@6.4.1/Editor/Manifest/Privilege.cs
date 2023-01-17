using System;

using UnityEngine;
using UnityEngine.Serialization;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    public class Privilege
    {
        public enum Category
        {
            Invalid,
            Sensitive,
            Reality,
            Autogranted,
            Trusted
        }

        [SerializeField]
        private uint m_ApiLevel;

        [FormerlySerializedAs("enabled")]
        [SerializeField]
        private bool m_Enabled;

        [FormerlySerializedAs("name")]
        [SerializeField]
        private string m_Name;

        public uint ApiLevel
        {
            get => m_ApiLevel;
            set => m_ApiLevel = value;
        }

        [Obsolete("Use Privilege.Enabled instead")]
        public bool enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        public bool Enabled
        {
            get => m_Enabled;
            set => m_Enabled = value;
        }

        [Obsolete("Use Privilege.Name instead")]
        public string name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public override bool Equals(object obj)
        {
            var p2 = obj as Privilege;
            if (p2 == null)
                return false;
            return Name == p2.Name
                && Enabled == p2.Enabled;
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (!string.IsNullOrEmpty(Name))
                    hash = hash * 486187739 + Name.GetHashCode();
                hash = hash * 486187739 + Enabled.GetHashCode();
                return hash;
            }
        }
    }
}