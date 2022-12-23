using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using UnityEngine;
using UnityEditor;

namespace WSoft.Tools.AssetReferenceViewer
{
    public class AssetReferenceDatabase
    {
        public class PrefabInfo
        {
            public List<string> GUIDsReferenced = new List<string>();
        }

        public class SOInfo
        {
            public string baseScriptGUID = "";
        }

        public Dictionary<string, string> AssetPathToGUID = new Dictionary<string, string>();
        public Dictionary<string, string> GUIDToAssetPath = new Dictionary<string, string>();

        // The following dicts all use GUIDS where there are strings
        public Dictionary<string, PrefabInfo> GUIDToPrefabInfo = new Dictionary<string, PrefabInfo>();
        public Dictionary<string, SOInfo> GUIDToSOInfo = new Dictionary<string, SOInfo>();
        public Dictionary<string, List<string>> GUIDToAssetsReferencedBy = new Dictionary<string, List<string>>();

        public bool initialized { get; private set; }

        static readonly string[] IGNORED_FILEPATHS = { "Assets/Wwise", "Assets/StreamingAssets", "Assets/TextMesh Pro" };

        public AssetReferenceDatabase()
        {
            initialized = false;
        }

        public void Initialize()
        {
            initialized = false;

            InitializeGUIDS();
            InitializePrefabInfos();
            InitializeSOInfos();

            initialized = true;
        }

        public void Clear()
        {
            AssetPathToGUID.Clear();
            GUIDToAssetPath.Clear();
            GUIDToPrefabInfo.Clear();
            GUIDToAssetsReferencedBy.Clear();
        }

        /// <summary>
        /// Gets the filepath relative to the Assets folder
        /// </summary>
        string GetRelativeFilepath(string filepath)
        {
            string relativeFilepath = filepath.Substring(Application.dataPath.Length - 6);
            relativeFilepath = relativeFilepath.Replace('\\', '/');
            return relativeFilepath;
        }

        bool IsViewableFile(string filepath)
        {
            foreach (string ignoredFilepath in IGNORED_FILEPATHS)
            {
                if (filepath.Contains(ignoredFilepath))
                    return false;
            }

            return !filepath.EndsWith(".meta") && !filepath.EndsWith(".gitkeep");
        }

        void InitializeGUIDS()
        {
            foreach (string filepath in Directory.EnumerateFiles(Application.dataPath, "*.*", SearchOption.AllDirectories))
            {
                if (!IsViewableFile(filepath))
                    continue;

                string relativeFilepath = GetRelativeFilepath(filepath);
                string GUID = AssetDatabase.AssetPathToGUID(relativeFilepath);
                if (GUIDToAssetPath.ContainsKey(GUID))
                    continue;

                AssetPathToGUID.Add(relativeFilepath, GUID);
                GUIDToAssetPath.Add(GUID, relativeFilepath);

                GUIDToAssetsReferencedBy.Add(GUID, new List<string>());
            }
        }

        void InitializePrefabInfos()
        {
            foreach (string filepath in Directory.EnumerateFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories))
            {
                PrefabInfo prefabInfo = GetPrefabInfo(filepath);
                string relativeFilepath = GetRelativeFilepath(filepath);
                GUIDToPrefabInfo.Add(AssetPathToGUID[relativeFilepath], prefabInfo);
            }
        }

        void InitializeSOInfos()
        {
            foreach (string filepath in Directory.EnumerateFiles(Application.dataPath, "*.asset", SearchOption.AllDirectories))
            {
                SOInfo soInfo = GetSOInfo(filepath);
                string relativeFilepath = GetRelativeFilepath(filepath);
                GUIDToSOInfo.Add(AssetPathToGUID[relativeFilepath], soInfo);
            }
        }

        PrefabInfo GetPrefabInfo(string filepath)
        {
            PrefabInfo info = new PrefabInfo();

            string relativeFilepath = GetRelativeFilepath(filepath);
            string prefabGUID = AssetPathToGUID[relativeFilepath];

            foreach (string line in File.ReadLines(filepath))
            {
                if (line.Contains("{fileID: "))
                {
                    string guid = ParseGUID(line);

                    if (!string.IsNullOrEmpty(guid))
                    {
                        if (!GUIDToAssetsReferencedBy.ContainsKey(guid))
                            GUIDToAssetsReferencedBy.Add(guid, new List<string>());

                        info.GUIDsReferenced.Add(guid);
                        GUIDToAssetsReferencedBy[guid].Add(prefabGUID);
                    }
                }
            }

            return info;
        }

        SOInfo GetSOInfo(string filepath)
        {
            SOInfo info = new SOInfo();

            string relativeFilepath = GetRelativeFilepath(filepath);
            string soGUID = AssetPathToGUID[relativeFilepath];

            bool foundGUID = false;

            foreach (string line in File.ReadLines(filepath))
            {
                if (line.Contains("{fileID: ") && !foundGUID)
                {
                    string guid = ParseGUID(line);

                    if (!string.IsNullOrEmpty(guid))
                    {
                        if (!GUIDToAssetsReferencedBy.ContainsKey(guid))
                            GUIDToAssetsReferencedBy.Add(guid, new List<string>());

                        info.baseScriptGUID = guid;
                        GUIDToAssetsReferencedBy[guid].Add(soGUID);
                        foundGUID = true;
                    }
                }
            }

            return info;
        }

        string ParseGUID(string line)
        {
            int fileIDIndex = line.IndexOf("{fileID: ");
            if (line.Length < fileIDIndex + 16)
                return "";

            string fileIDStr = line.Substring(fileIDIndex + 8, 8);
            int fileID;
            if (int.TryParse(fileIDStr, out fileID))
            {
                int guidIndex = line.IndexOf("guid: ");
                if (guidIndex != -1 && line.Length >= guidIndex + 38)
                {
                    string guid = line.Substring(guidIndex + 6, 32);
                    return guid.Contains(" ") ? " " : guid;
                }
            }

            return "";
        }
    }

}