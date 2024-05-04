using System;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = true, AllowMultiple = false)]
    public sealed class NodeWidthAttribute : Attribute
    {
        public float width { get; }

        public NodeWidthAttribute(float width)
        {
            this.width = width;
        }
    }
}
