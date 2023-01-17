using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Condition
{
    [System.Serializable] public enum ValueType { None, TagCount };
    [System.Serializable] public enum Comparison { Equal, Less, LessEqual, Greater, GreaterEqual };

    public ValueHandler valueHandler;
    public Variable variable;
    public ValueType valueType;
    public Hypertag tag;
    public Comparison comparison;
    public float value;
    public bool percentageCompare;

    public Variable GetVariable()
    {
        if (variable) return variable;

        if (valueHandler)
        {
            return valueHandler.GetVariable();
        }
        return null;
    }

    public string GetVariableName()
    {
        if (variable) return variable.name;
        if (valueHandler) return valueHandler.name;
        switch (valueType)
        {
            case ValueType.TagCount:
                if (tag) return $"TagCount({tag.name})";
                return "TagCount([Unknown])";
        }

        return "[Unknown]";
    }

    public string GetRawDescription()
    {
        string desc = $"({GetVariableName()}";
        switch (comparison)
        {
            case Comparison.Equal: desc += " == "; break;
            case Comparison.Less: desc += " < "; break;
            case Comparison.LessEqual: desc += " <= "; break;
            case Comparison.Greater: desc += " > "; break;
            case Comparison.GreaterEqual: desc += " >= "; break;
            default:
                break;
        }
        desc += value;
        if (percentageCompare) desc += "%";
        desc += ")";

        return desc;
    }

    public bool Evaluate()
    {
        var currentVar = GetVariable();

        float currentValue;
        float minValue = 0;
        float maxValue = 0;
        if (currentVar == null)
        {
            switch (valueType)
            {
                case Condition.ValueType.TagCount:
                    currentValue = HypertaggedObject.FindGameObjectsWithHypertag(tag).Count;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                default:
                    return false;
            }
        }
        else
        {
            currentValue = currentVar.currentValue;
            minValue = currentVar.minValue;
            maxValue = currentVar.maxValue;
        }

        if (percentageCompare)
        {
            currentValue = 100 * (currentValue - minValue) / (maxValue - minValue);
        }

        bool b = false;
        switch (comparison)
        {
            case Condition.Comparison.Equal:
                b = (currentValue == value);
                break;
            case Condition.Comparison.Less:
                b = (currentValue < value);
                break;
            case Condition.Comparison.LessEqual:
                b = (currentValue <= value);
                break;
            case Condition.Comparison.Greater:
                b = (currentValue > value);
                break;
            case Condition.Comparison.GreaterEqual:
                b = (currentValue >= value);
                break;
            default:
                break;
        }

        return b;
    }
}