using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

namespace OkapiKit
{
    [System.Serializable]
    public struct OkapiValue
    {
        public enum Type { Float = 0, Integer = 1, VariableInstance = 2, Variable = 3 };

#pragma warning disable CS0414
        [SuppressMessage("Compiler", "CS0414", Justification = "Used via reflection in the Editor - don't remove even if it seems it's not used"), SerializeField]
        private bool               init;
#pragma warning restore CS0414

        [SerializeField] private Type               type;
        [SerializeField] private VariableInstance   variableInstance;
        [SerializeField] private Variable           variable;
        [SerializeField] private float              floatValue;
        [SerializeField] private int                intValue;
        private float v;

        public OkapiValue(float v) : this()
        {
            this.init = true;
            this.type = Type.Float;
            this.floatValue = v;
        }

        public OkapiValue(int v) : this()
        {
            this.init = true;
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

        public void CheckErrors(List<LogEntry> logs, string propName, GameObject parentGameObject)
        {
            switch (type)
            {
                case Type.Float:
                    break;
                case Type.Integer:
                    break;
                case Type.VariableInstance:
                    if (variableInstance != null)
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"Undefined source variable instance for {propName}!", "You need to define the variable from which to fetch a value!"));
                    }
                    break;
                case Type.Variable:
                    if (variable != null)
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"Undefined source variable for {propName}!", "You need to define the variable from which to fetch a value!"));
                    }
                    break;
                default:
                    break;
            }
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
