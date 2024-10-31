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

        [SerializeField] private Type               type;
        [SerializeField] private VariableInstance   variableInstance;
        [SerializeField] private Variable           variable;
        [SerializeField] private float              floatValue;
        [SerializeField] private int                intValue;
        private float v;

        public OkapiValue(float v) : this()
        {
            this.type = Type.Float;
            this.floatValue = v;
        }

        public OkapiValue(int v) : this()
        {
            this.type = Type.Integer;
            this.intValue = v;
        }

        public object GetRawValue(GameObject go)
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

        public float GetFloat(GameObject go)
        {
            switch (type)
            {
                case Type.Float:
                    return floatValue;
                case Type.Integer:
                    return intValue;
                case Type.VariableInstance:
                    return (variableInstance) ? (variableInstance.GetValue()) : (0.0f);
                case Type.Variable:
                    return (variable) ? (variable.currentValue) : (0.0f);
            }

            return 0.0f;
        }

        public string GetDescription()
        {
            switch (type)
            {
                case Type.Float:
                    return floatValue.ToString();
                case Type.Integer:
                    return intValue.ToString();
                case Type.VariableInstance:
                    if (variableInstance != null) return variableInstance.name;
                    break;
                case Type.Variable:
                    if (variable != null) return variable.name;
                    break;
                default:
                    break;
            }

            return "[UNDEFINED]";
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
