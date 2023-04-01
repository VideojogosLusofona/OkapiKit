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
        [SerializeField, ResizableTextArea]
        protected string description;

        public string explanation => _explanation;

        public bool showInfo
        {
            get { return _showInfo; }
            set { _showInfo = value; }
        }

        public abstract string GetRawDescription(string ident, GameObject refObject);

        public abstract string UpdateExplanation();

        protected virtual void Awake()
        {
            UpdateExplanation();
        }
    }
}