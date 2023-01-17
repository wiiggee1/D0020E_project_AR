using System;
using System.Collections.Generic;
using UnityEngine;

namespace UnityEditor.XR.MagicLeap
{
    [Serializable]
    public class ImageDatabaseEntry
    {
        public string assetGuid;
        [SerializeField]
        private long _timestamp;

        /// <summary>
        /// Getter/Setter to map the 'DateTime' we store for the entry vs what we can serialize
        ///  - note that the JSONUtility class cannot serialize a DateTime
        /// </summary>
        public DateTime timeStamp
        {
            get
            {
                return DateTime.FromFileTimeUtc(_timestamp);
            }
            set
            {
                _timestamp = value.ToFileTimeUtc();
            }
        }
    }

    /// <summary>
    /// A scriptable object that is used to cache image library binary blob
    /// generation data to prevent rebuilds everytime the user presses play in
    /// the editor.
    /// </summary>
    [Serializable]
    public class MagicLeapImageDatabaseLibraryCache
    {
        public MagicLeapImageDatabaseLibraryCache()
        {
            m_ImageLibraryCache = new List<ImageDatabaseEntry>(25);
        }

        [Obsolete("To be removed when we bump the major version", false)]
        public Dictionary<string, DateTime> m_LibraryCache;

        public List<ImageDatabaseEntry> m_ImageLibraryCache;
    }
}
