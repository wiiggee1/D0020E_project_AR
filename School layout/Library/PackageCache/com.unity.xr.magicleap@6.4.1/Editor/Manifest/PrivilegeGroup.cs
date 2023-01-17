using System;

using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    public class PrivilegeGroup
    {
        [SerializeField]
        private string m_Name;

        [SerializeField]
        internal Privilege[] m_Privileges;

        public string Name
        {
            get => m_Name;
            set => m_Name = value;
        }

        public Privilege[] Privileges
        {
            get => m_Privileges;
            set => m_Privileges = value;
        }

        public override bool Equals(object obj)
        {
            var p2 = obj as PrivilegeGroup;
            if (p2 == null)
                return false;
            return Name == p2.Name
                && System.Object.ReferenceEquals(Privileges, p2.Privileges);
        }

        public override int GetHashCode()
        {
            unchecked // Overflow is fine, just wrap
            {
                int hash = 17;
                // Suitable nullity checks etc, of course :)
                if (!string.IsNullOrEmpty(Name))
                    hash = hash * 486187739 + Name.GetHashCode();
                hash = hash * 486187739 + Privileges.GetHashCode();
                return hash;
            }
        }
    }
}