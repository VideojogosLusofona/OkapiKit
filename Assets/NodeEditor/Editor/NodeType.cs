using System;

namespace NodeEditor
{
    public class NodeType
    {
        public NodeType() { }
        public NodeType(string name, Type type) { this.name = name; this.type = type; }

        public string   name;
        public Type     type;
    }
}
