using System;

namespace NodeEditor
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class NodePathAttribute : Attribute
    {
        public string fullPath { get; }
        public string path { get; }
        public string name { get; }

        public NodePathAttribute(string fullPath)
        {
            this.fullPath = fullPath;

            path = fullPath;

            int idx = path.LastIndexOf('/');
            if (idx != -1)
            {
                path = fullPath.Substring(0, idx);
                name = fullPath.Substring(idx + 1);
            }
            else
            {
                path = "";
                name = fullPath;
            }
        }
    }
}
