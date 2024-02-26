using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [System.Serializable]
    public struct OkapiValue
    {
        public enum Type { Float = 0, Integer = 1, VariableInstance = 2, Variable = 3 };

        [SerializeField] private bool               init;
        [SerializeField] private Type               type;
        [SerializeField] private VariableInstance   variableInstance;
        [SerializeField] private Variable           variable;
        [SerializeField] private float              floatValue;
        [SerializeField] private int                intValue;

        public object GetRawValue()
        {
            switch (type)
            {
                case Type.Float:
                    return floatValue;
                case Type.Integer:
                    return intValue;
                case Type.VariableInstance:
                    return (variableInstance) ? (variableInstance.GetRawValue()) : (null);
                case Type.Variable:
                    return (variable) ? (variable.GetRawValue()) : (null);
            }

            return null;
        }

        public string GetName()
        {
            switch (type)
            {
                case Type.Float:
                    break;
                case Type.Integer:
                    break;
                case Type.VariableInstance:
                    if (variableInstance != null) return variableInstance.name;
                    break;
                case Type.Variable:
                    if (variable != null) return variable.name;
                    break;
                default:
                    break;
            }

            return "unnamed";
        }
    }


    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoLabelAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoFloatAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoIntegerAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoVariableInstanceAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoVariableAttribute : PropertyAttribute
    {
    }

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class OVNoFunctionAttribute : PropertyAttribute
    {
    }
}
