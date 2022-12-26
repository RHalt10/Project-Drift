using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

namespace WSoft.Tools.AssetReferenceViewer
{
    public class AssetReferenceViewer : EditorWindow
    {
        [System.Flags]
        public enum Filter
        {
            None = 0,
            Image = 1,
            Prefab = 2,
            Scene = 4,
            Script = 8,
            ScriptableObject = 16,
            All = ~0
        }

        Vector2 scrollbar;

        AssetReferenceDatabase database = new AssetReferenceDatabase();

        Dictionary<string, bool> AssetPathToOpened = new Dictionary<string, bool>();
        bool referencesTabOpened = false;

        Filter currentFilter = Filter.All;

        [MenuItem("WolverineSoft/Asset Reference Viewer")]
        public static void OpenWindow()
        {
            AssetReferenceViewer viewer = GetWindow<AssetReferenceViewer>(false, "Asset Reference Viewer", true);
            viewer.StartReload();
        }

        void OnGUI()
        {
            RenderFilters();

            scrollbar = EditorGUILayout.BeginScrollView(scrollbar);
            foreach (KeyValuePair<string, string> result in database.AssetPathToGUID)
            {
                RenderResult(result.Key, result.Value);
            }
            EditorGUILayout.EndScrollView();
        }

        void StartReload()
        {
            database.Clear();
            database.Initialize();

            AssetPathToOpened.Clear();
            foreach (KeyValuePair<string, string> result in database.AssetPathToGUID)
            {
                AssetPathToOpened.Add(result.Value, false);
            }
        }

        void RenderFilters()
        {
            currentFilter = (Filter)EditorGUILayout.EnumFlagsField(currentFilter, "Filter");
        }

        void RenderResult(string assetPath, string guid)
        {
            if (!AssetPathToOpened.ContainsKey(assetPath))
                AssetPathToOpened.Add(assetPath, false);

            if (currentFilter != GetFilterForPath(assetPath))
                return;

            bool opened = AssetPathToOpened[assetPath];
            opened = EditorGUILayout.BeginFoldoutHeaderGroup(opened, assetPath);
            AssetPathToOpened[assetPath] = opened;

            if (opened)
            {
                int numReferencedBy = database.GUIDToAssetsReferencedBy[guid].Count;

                EditorGUILayout.LabelField("GUID: " + guid);
                EditorGUILayout.LabelField("Referenced By: " + numReferencedBy + " assets");

                if (assetPath.EndsWith(".prefab"))
                    RenderPrefab(assetPath, guid);
                else if (assetPath.EndsWith(".asset"))
                    RenderSO(assetPath, guid);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void RenderPrefab(string assetPath, string guid)
        {
            referencesTabOpened = EditorGUILayout.BeginFoldoutHeaderGroup(referencesTabOpened, "References in Prefab");
            if (referencesTabOpened)
            {
                AssetReferenceDatabase.PrefabInfo info = database.GUIDToPrefabInfo[guid];
                foreach (string guidReferenced in info.GUIDsReferenced)
                {
                    EditorGUILayout.LabelField(database.GUIDToAssetPath[guidReferenced]);
                }
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        void RenderSO(string assetPath, string guid)
        {
            if (!database.GUIDToSOInfo.ContainsKey(guid))
                return;

            string baseScriptGUID = database.GUIDToSOInfo[guid].baseScriptGUID;
            string baseScriptName = GUIDToScriptName(baseScriptGUID);

            EditorGUILayout.LabelField("Base Script: " + baseScriptName);
        }

        Filter GetFilterForPath(string assetPath)
        {
            if (assetPath.EndsWith(".png") || assetPath.EndsWith(".jpg")) return Filter.Image;
            if (assetPath.EndsWith(".prefab")) return Filter.Prefab;
            if (assetPath.EndsWith(".unity")) return Filter.Scene;
            if (assetPath.EndsWith(".cs")) return Filter.Script;
            if (assetPath.EndsWith(".asset")) return Filter.ScriptableObject;

            return Filter.All;
        }

        string GUIDToScriptName(string guid)
        {
            string filepath = database.GUIDToAssetPath[guid];
            int slashIndex = filepath.LastIndexOf("/");
            return filepath.Substring(slashIndex + 1, filepath.Length - slashIndex - 4);
        }
    }

}