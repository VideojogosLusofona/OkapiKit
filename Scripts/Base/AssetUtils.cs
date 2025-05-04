﻿using UnityEngine;
using System;
using System.Linq;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OkapiKit
{

    public class AssetUtils
    {
        public static T CreateOrReplaceAsset<T>(T asset, string path) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(asset, path);
                existingAsset = asset;
            }
            else
            {
                EditorUtility.CopySerialized(asset, existingAsset);
            }

            return existingAsset;
#else
        Debug.LogError("CreateOrReplaceAsset not available in runtime!");
        return null;
#endif
        }

        public static T CreateOrReplaceAsset<T>(T oldAsset, T newAsset) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            string path = AssetDatabase.GetAssetPath(oldAsset);
            if (path == "")
            {
                Debug.LogError("Can't find path for asset!");
                return null;
            }

            T existingAsset = AssetDatabase.LoadAssetAtPath<T>(path);

            if (existingAsset == null)
            {
                AssetDatabase.CreateAsset(newAsset, path);
                existingAsset = newAsset;
            }
            else
            {
                EditorUtility.CopySerialized(newAsset, existingAsset);
            }

            return existingAsset;
#else
        Debug.LogError("CreateOrReplaceAsset not available in runtime!");
        return null;
#endif
        }

        public static T Copy<T>(T obj, string path) where T : UnityEngine.Object
        {
            if (obj == null) return null;

            var copyObj = UnityEngine.Object.Instantiate(obj);
            copyObj.name = obj.name;

            var filename = path + "/" + obj.name + ".asset";
            Uri p1 = new Uri(Application.dataPath);
            Uri p2 = new Uri(filename);
            Uri relativePath = p1.MakeRelativeUri(p2);

            return CreateOrReplaceAsset<T>(copyObj, relativePath.ToString());
        }

        public static T[] GetAll<T>() where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            string findKey = $"t:{typeof(T).ToString()}";
            string[] guids = AssetDatabase.FindAssets(findKey);
            var objects = guids
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<T>)
                .Where(obj => obj != null) // Ensure it's a valid Speaker instance
                .ToArray();
#else
        var objects = Resources.FindObjectsOfTypeAll<T>();
#endif

            return objects;
        }

        public static T FindAssetByName<T>(string name) where T : UnityEngine.Object
        {
#if UNITY_EDITOR
            string[] guids = AssetDatabase.FindAssets($"{name} t:{typeof(T).Name}");

            foreach (string guid in guids)
            {
                string path = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(path);
                if (asset != null && asset.name == name)
                    return asset;
            }

            return null;
#else
            Debug.LogError("CreateOrReplaceAsset not available in runtime!");
            return null;
#endif

        }
    }
}