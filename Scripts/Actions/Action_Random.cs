using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Random : Action
{
    [System.Serializable]
    struct ActionProbability
    {
        public Action  action;
        public float   probability;
    }

    [SerializeField] private ActionProbability[]   actions;

    public override string GetRawDescription(string ident)
    {
        var desc = "Select randomly between the following actions:\n";

        float total = 0;
        foreach (var action in actions)
        {
            total += action.probability;
        }

        for (int i = 0; i < actions.Length; i++)
        {
            desc += $"{ident}{i + 1}. {actions[i].action.GetRawDescription(ident + "  ")} ({(int)(actions[i].probability * 100 / total)}%)\n";
        }

        return desc;
    }

    public override void Execute()
    {
        if (!enableAction) return;

        List<ActionProbability> activeActions = new List<ActionProbability>();

        float sumProbability = 0;

        foreach (var action in actions)
        {
            if (action.action.isActionEnabled)
            {
                sumProbability += action.probability;
                activeActions.Add(action);
            }
        }


        float r = Random.Range(0.0f, sumProbability);
        foreach (var action in activeActions) 
        { 
            if (action.probability > r)
            {
                action.action.Execute();
                break;
            }
            r -= action.probability;
        }
    }
}
