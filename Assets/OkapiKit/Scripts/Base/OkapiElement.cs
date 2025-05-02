using NaughtyAttributes;
using System;
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

        protected virtual string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

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

        protected void CheckButton(string textName, string buttonName)
        {
            if (buttonName == "")
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, $"{textName} is not defined!", "We need to define a button name to be able to use it."));
            }
            else
            {
                try
                {
                    Input.GetButton(buttonName);
                }
                catch (Exception)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, $"{textName} {buttonName} is not defined!", $"We need to define a {buttonName} on Edit/Project Settings/Input to be able to use it."));
                }
            }
        }
    }
}