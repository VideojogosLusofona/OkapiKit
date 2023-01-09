using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static UnityEngine.Rendering.DebugUI;

public class Trigger_OnCondition: Trigger
{
    [System.Serializable]
    public struct Condition
    {
        [System.Serializable] public enum Comparison { Equal, Less, LessEqual, Greater, GreaterEqual };

        [SerializeField, ShowIf("needValueHandler")]
        public ValueHandler valueHandler;
        [SerializeField, ShowIf("needValueVariable")]
        public Variable variable;
        [SerializeField]
        public Comparison comparison;
        [SerializeField]
        public float value;
        [SerializeField]
        public bool percentageCompare;

        private bool needValueHandler => (variable == null);
        private bool needValueVariable => (valueHandler == null);

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
        for (int i = 0; i < conditions.Length; i++)
        {
            desc += conditions[i].GetRawDescription();
            if (i < conditions.Length - 1) desc = desc + " and ";
        }

        return desc;
    }

    private bool Evaluate(Condition condition)
    {
        var v = condition.GetVariable();
        if (v == null) return false;

        float currentValue = v.currentValue;
        if (condition.percentageCompare)
        {
            currentValue = 100 * (currentValue - v.minValue) / (v.maxValue - v.minValue);
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
        bool b = false;

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
