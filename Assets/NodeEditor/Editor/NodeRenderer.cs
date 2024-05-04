using System;
using UnityEngine;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeRendererAttribute : Attribute
    {
        public Type type { get; }
        
        public NodeRendererAttribute(Type type)
        {
            this.type = type;
        }
    }
}
