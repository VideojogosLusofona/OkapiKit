using System;
using UnityEngine;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class NodePropertyVisibilityAttribute : Attribute
    {
        public bool nodeVisibility { get; }

        public NodePropertyVisibilityAttribute(bool visibility)
        {
            nodeVisibility = visibility;
        }
    }
}
