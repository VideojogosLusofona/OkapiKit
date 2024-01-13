using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace OkapiKit
{
    [CreateAssetMenu(menuName = "Okapi Kit/Configuration")]
    public class OkapiConfig : ScriptableObject
    {
        [SerializeField, ResizableTextArea, ReadOnly]
        protected List<LogEntry> _logs = new List<LogEntry>();

        [SerializeField]
        private float maxPingTime = 4.0f;

        protected OkapiElement  pingComponent;
        protected DateTime      pingTime;

        public List<LogEntry> logs => _logs;

        private void OnEnable()
        {
            pingComponent = null;
            pingTime = DateTime.MinValue;
        }

        public void ForceCheckErrors()
        {
            _logs.Clear();
            CheckErrors();
        }

        void CheckErrors()
        {
            // Find all objects of this type on the project
            var allConfigs = FindAllInstances<OkapiConfig>();
            if (allConfigs.Count > 1)
            {
                var err = "There should only be on Okapi Config object in the project! Objects found:\n";
                for (int i = 0; i < allConfigs.Count; i++)
                {
                    err += "  " + allConfigs[i].name;
                    if (i < allConfigs.Count - 1) err += "\n";
                }
                
                _logs.Add(new LogEntry(LogEntry.Type.Error, err));
            }
        }        
        
        public static void PingComponent(OkapiElement element)
        {
            var config = instance;
            if (config == null) return;

            config.pingComponent = element;
            config.pingTime = DateTime.Now;
        }

        public static bool IsPinged(OkapiElement element)
        {
            var config = instance;
            if (config == null) return false;

            if (config.pingComponent == element)
            {
                var elapsedTime = DateTime.Now - config.pingTime;
                if (elapsedTime.Seconds < config.maxPingTime)
                {
                    return true;
                }
            }

            return false;
        }

        static OkapiConfig instance { 
            get {
                var allConfigs = FindAllInstances<OkapiConfig>();
                if (allConfigs.Count == 1)
                {
                    return allConfigs[0];
                }

                return null;
            } 
        }

        static List<T> FindAllInstances<T>() where T : ScriptableObject
        {
            string[] guids = AssetDatabase.FindAssets("t:" + typeof(T).Name); // Find all assets of type T
            List<T> assets = new List<T>();

            foreach (string guid in guids)
            {
                string assetPath = AssetDatabase.GUIDToAssetPath(guid);
                T asset = AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                {
                    assets.Add(asset);
                }
            }

            return assets;
        }
    }
}
