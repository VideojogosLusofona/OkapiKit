using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;

namespace OkapiKit
{
    [CreateAssetMenu(menuName = "Okapi Kit/Configuration")]
    public class OkapiConfig : ScriptableObject
    {
        [SerializeField, ResizableTextArea, ReadOnly]
        protected List<LogEntry> _logs = new List<LogEntry>();

        [SerializeField]
        private float       maxPingTime = 4.0f;
        [SerializeField, Header("Scene View")]
        private bool        displayConditions = true;
        [SerializeField]
        private bool        displayHypertags = false;
        [SerializeField, Header("Sorting")]
        private OrderMode   _orderMode = OrderMode.Z;
        [SerializeField]
        private float       _orderScaleY = 0.1f;
        [SerializeField]
        private int         _orderMin = -1000;
        [SerializeField]
        private int         _orderMax = 1000;
        [SerializeField]
        private float       _orderMinZ = -5.0f;
        [SerializeField]
        private float       _orderMaxZ = 5.0f;

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
#if UNITY_EDITOR
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
                
                _logs.Add(new LogEntry(LogEntry.Type.Error, err, "There can only be one Okapi Config object in the whole project.\nThis is where some Okapi Kit configurations are made, and we don't want different objects to conflict with each other."));
            }
#endif
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

        public static bool showTags => (instance) ? (instance.displayHypertags) : (false);
        public static bool showConditions => (instance) ? (instance.displayConditions) : (false);

        public static OrderMode orderMode => instance?._orderMode ?? OrderMode.Z;
        public static float orderScaleY => instance?._orderScaleY ?? 0.0001f;
        public static int orderMin => instance?._orderMin ?? -1000;
        public static int orderMax => instance?._orderMax ?? 1000;
        public static float orderMinZ => instance?._orderMinZ ?? -5.0f;
        public static float orderMaxZ => instance?._orderMaxZ ?? 5.0f;

        static OkapiConfig _instance = null;

        public static OkapiConfig instance { 
            get {
                if (_instance) return _instance;

                var allConfigs = FindAllInstances<OkapiConfig>();
                if (allConfigs.Count == 1)
                {
                    _instance = allConfigs[0];
                }

                return _instance;
            } 
        }

        static List<T> FindAllInstances<T>() where T : ScriptableObject
        {
#if UNITY_EDITOR
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
#else
            return null;
#endif
        }
    }
}
