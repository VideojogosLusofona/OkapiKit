using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TriggerOnCondition: Trigger
{
    [SerializeField] private Condition[] conditions;

    private bool firstTime = true;
    private bool prevValue = false;

    public override string GetTriggerTitle() => "On Condition";

    public override string GetRawDescription(string ident, GameObject refObject)
    {
        string desc = "When ";
        if ((conditions == null) || (conditions.Length == 0))
        {
            return "when [No conditions set]!";
        }
        for (int i = 0; i < conditions.Length; i++)
        {
            desc += conditions[i].GetRawDescription(gameObject);
            if (i < conditions.Length - 1) desc = desc + " and ";
        }

        return desc;
    }

    private void Update()
    {
        if (!isTriggerEnabled) return;
        if (!EvaluatePreconditions()) return;

        bool b = true;

        foreach (var condition in conditions) 
        {
            b &= condition.Evaluate(gameObject);
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
