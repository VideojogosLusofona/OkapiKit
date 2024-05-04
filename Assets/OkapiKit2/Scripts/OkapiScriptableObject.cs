using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKitV2
{
    public abstract class OkapiScriptableObject : ScriptableObject
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

        public abstract string GetRawDescription(string ident, ScriptableObject refObject);

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
    }
}