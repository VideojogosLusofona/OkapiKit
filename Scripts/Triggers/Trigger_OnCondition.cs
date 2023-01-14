using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static UnityEngine.Rendering.DebugUI;
using static Trigger_OnCondition;

public class Trigger_OnCondition: Trigger
{
    [System.Serializable]
    public struct Condition
    {
        [System.Serializable] public enum ValueType { None, TagCount};
        [System.Serializable] public enum Comparison { Equal, Less, LessEqual, Greater, GreaterEqual };

        public ValueHandler valueHandler;
        public Variable     variable;
        public ValueType    valueType;
        public Hypertag     tag;
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
    }

    [SerializeField] private Condition[] conditions;

    private bool firstTime = true;
    private bool prevValue = false;

    protected override string GetRawDescription()
    {
        string desc = "When ";
        if ((conditions == null) || (conditions.Length == 0))
        {
            return "When [No conditions set]!";
        }
        for (int i = 0; i < conditions.Length; i++)
        {
            desc += conditions[i].GetRawDescription();
            if (i < conditions.Length - 1) desc = desc + " and ";
        }

        return desc;
    }

    private bool Evaluate(Condition condition)
    {
        var currentVar = condition.GetVariable();

        float currentValue;
        float minValue = 0;
        float maxValue = 0;
        if (currentVar == null)
        {
            switch (condition.valueType)
            {
                case Condition.ValueType.TagCount:
                    currentValue = HypertaggedObject.FindGameObjectsWithHypertag(condition.tag).Count;
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

        if (condition.percentageCompare)
        {
            currentValue = 100 * (currentValue - minValue) / (maxValue - minValue);
        }

        bool b = false;
        switch (condition.comparison)
        {
            case Condition.Comparison.Equal:
                b = (currentValue == condition.value);
                break;
            case Condition.Comparison.Less:
                b = (currentValue < condition.value);
                break;
            case Condition.Comparison.LessEqual:
                b = (currentValue <= condition.value);
                break;
            case Condition.Comparison.Greater:
                b = (currentValue > condition.value);
                break;
            case Condition.Comparison.GreaterEqual:
                b = (currentValue >= condition.value);
                break;
            default:
                break;
        }

        return b;
    }

    private void Update()
    {
        bool b = true;

        foreach (var condition in conditions) 
        {
            b &= Evaluate(condition);
            if (!b) break;
        }

        if (b) 
        {
            if (firstTime) ExecuteTrigger();
            else if (!prevValue) ExecuteTrigger();
        }

        prevValue = b;
        firstTime = false;
    }
}
