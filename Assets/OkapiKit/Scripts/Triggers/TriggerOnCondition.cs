using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class TriggerOnCondition: Trigger
{
    [SerializeField] private    Condition[]     conditions;
    [SerializeField] protected  ActionTrigger[] elseActions;

    private bool firstTime = true;
    private bool prevValue = false;

    public override string GetTriggerTitle() => "On Condition";

    public override string UpdateExplanation()
    {
        base.UpdateExplanation();

        if ((elseActions != null) && (elseActions.Length > 0)) 
        {
            _explanation += "else\n" + GetDescriptionActions(elseActions);
        }

        return _explanation;
    }

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
        else
        {
            if (prevValue) ExecuteElseTrigger();
        }

        prevValue = b;
        firstTime = false;
    }

    public virtual void ExecuteElseTrigger()
    {
        if (!enableTrigger) return;

        foreach (var action in elseActions)
        {
            if (action.action.isActionEnabled)
            {
                if (action.delay > 0)
                {
                    StartCoroutine(ExecuteTriggerCR(action));
                }
                else
                {
                    action.action.Execute();
                }
            }
        }
    }
}
