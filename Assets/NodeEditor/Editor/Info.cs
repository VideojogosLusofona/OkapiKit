using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.Reflection;

namespace NodeEditor
{
    [InitializeOnLoad]
    public class Info
    {
        protected static Dictionary<Type, Type> cachedRendererTypes;
        protected static Dictionary<string, Type> cachedNodeTypes;

        static Info()
        {
            cachedRendererTypes = new();
            cachedNodeTypes = new();

            foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
            {
                foreach (var t in assembly.GetTypes())
                {
                    if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseNodeRenderer)))
                    {
                        var attr = t.GetCustomAttribute<NodeRendererAttribute>();
                        if (attr != null)
                        {
                            cachedRendererTypes.Add(attr.type, t);
                        }
                    }
                    if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseNode)))
                    {
                        cachedNodeTypes.Add(t.ToString(), t);
                    }
                }
            }
        }

        public static Type GetRendererType(Type type)
        {
            if (cachedRendererTypes.TryGetValue(type, out var rendererType))
            {
                return rendererType;
            }

            if (type.BaseType != null)
            {
                if (type.BaseType.IsClass)
                {
                    rendererType = GetRendererType(type.BaseType);
                    if (rendererType != null)
                    {
                        cachedRendererTypes[type] = rendererType;
                        return rendererType;
                    }
                }
            }

            return null;
        }

        public static Type GetNodeType(string nodeTypeName)
        {
            if (cachedNodeTypes.TryGetValue(nodeTypeName, out Type ret))
            {
                return ret;
            }

            return null;
        }
    }
}
