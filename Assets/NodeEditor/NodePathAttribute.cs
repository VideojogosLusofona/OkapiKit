using System;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NodePathAttribute : Attribute
    {
        public string Path { get; }

        public NodePathAttribute(string path)
        {
            Path = path;
        }
    }
}
