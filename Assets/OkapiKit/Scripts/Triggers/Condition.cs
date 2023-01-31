using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Condition
{
    [System.Serializable] public enum ValueType { None, TagCount, 
                                                  WorldPositionX, WorldPositionY, RelativePositionX, RelativePositionY,
                                                  AbsoluteVelocityX, AbsoluteVelocityY };
    [System.Serializable] public enum Comparison { Equal, Less, LessEqual, Greater, GreaterEqual };

    public VariableInstance valueHandler;
    public Variable     variable;
    public ValueType    valueType;
    public Hypertag     tag;
    public Transform    sourceTransform;
    public Rigidbody2D  rigidBody;
    public Comparison   comparison;
    public float        value;
    public bool         percentageCompare;

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
            case ValueType.WorldPositionX:
                if (sourceTransform) return $"{sourceTransform.name}.x";
                return $"this.x";
            case ValueType.WorldPositionY:
                if (sourceTransform) return $"{sourceTransform.name}.y";
                return $"this.y";
            case ValueType.RelativePositionX:
                if (sourceTransform) return $"{sourceTransform.name}.rx";
                return $"this.rx";
            case ValueType.RelativePositionY:
                if (sourceTransform) return $"{sourceTransform.name}.ry";
                return $"this.ry";
            case ValueType.AbsoluteVelocityX:
                if (rigidBody) return $"{rigidBody.name}.velocity.x";
                return $"this.velocity.x";
            case ValueType.AbsoluteVelocityY:
                if (rigidBody) return $"{rigidBody.name}.velocity.y";
                return $"this.velocity.y";
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

    public bool Evaluate(GameObject gameObject)
    {
        var currentVar = GetVariable();

        float       currentValue = 0.0f;
        float       minValue = 0;
        float       maxValue = 0;
        Transform   t;
        Rigidbody2D rb;
        if (currentVar == null)
        {
            switch (valueType)
            {
                case Condition.ValueType.TagCount:
                    currentValue = HypertaggedObject.FindGameObjectsWithHypertag(tag).Count;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.WorldPositionX:
                    t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                    currentValue = t.position.x;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.WorldPositionY:
                    t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                    currentValue = t.position.y;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.RelativePositionX:
                    t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                    currentValue = t.localPosition.x;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.RelativePositionY:
                    t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                    currentValue = t.localPosition.y;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.AbsoluteVelocityX:
                    rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                    if (rb) currentValue = rb.velocity.x;
                    minValue = 0;
                    maxValue = float.MaxValue;
                    break;
                case Condition.ValueType.AbsoluteVelocityY:
                    rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                    if (rb) currentValue = rb.velocity.y;
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