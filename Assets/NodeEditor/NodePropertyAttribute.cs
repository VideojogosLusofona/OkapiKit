using System;
using UnityEngine;

////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
/// Attributes for properties within BaseNodes and derived
/// 
/// NodePropertyVisibility allows to override the definition of the Node for property visibility
///     [NodePropertyVisibility(true)
/// NodeInput defines this node as a potential input node (for the graph part)
///     [NodeInput]
/// NodeOutput defines this node as a potential output node (for the graph part)
///     [NodeOutput]

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

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class NodeInputAttribute : Attribute
    {
        public NodeInputAttribute()
        {
        }
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public sealed class NodeOutputAttribute : Attribute
    {
        public NodeOutputAttribute()
        {
        }
    }
}
