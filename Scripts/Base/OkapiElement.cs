using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public abstract class OkapiElement : MonoBehaviour
    {
        [SerializeField, HideInInspector]
        protected bool _showInfo = true;
        [SerializeField, ResizableTextArea, ReadOnly]
        protected string _explanation;
        [SerializeField, ResizableTextArea, ReadOnly]
        protected List<LogEntry> _logs = new List<LogEntry>();
        [SerializeField, ResizableTextArea]
        protected string description;

        public string           explanation => _explanation;
        public List<LogEntry>   logs => _logs;

        public bool showInfo
        {
            get { return _showInfo; }
            set { _showInfo = value; }
        }

        public abstract string GetRawDescription(string ident, GameObject refObject);

        public string UpdateExplanation()
        {
            _logs.Clear();

            CheckErrors();
            return Internal_UpdateExplanation();
        }

        protected abstract string Internal_UpdateExplanation();
        protected virtual void CheckErrors()
        {

        }        

        protected virtual void Awake()
        {
            UpdateExplanation();
        }

        public static string GetLayerString(LayerMask mask)
        {
            string ret = "";
            
            for (int i = 0; i < 32; i++)
            {
                if ((mask & (1 << i)) != 0)
                {
                    if (ret == "") ret += "[";
                    else ret += ",";
                    ret += LayerMask.LayerToName(i);
                }
            }
            if (ret != "") ret += "]";
            else ret = "[UNDEFINED]";

            return ret;
        }
    }
}