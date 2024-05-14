using System;
using UnityEngine;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeDefaultPropertyVisibilityAttribute : Attribute
    {
        public bool defaultVisibility { get; }

        public NodeDefaultPropertyVisibilityAttribute(bool visibility)
        {
            defaultVisibility = visibility;
        }
    }
}
