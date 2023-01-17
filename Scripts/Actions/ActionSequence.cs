using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSequence : Action
{
    [System.Serializable]
    public struct ActionTrigger
    {
        public float delay;
        public Action action;
    }

    [SerializeField]
    protected ActionTrigger[] actions;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (actions == null) return;

        foreach (var action in actions)
        {
            if (action.delay > 0)
            {
                StartCoroutine(ExecuteCR(action));
            }
            else
            {
                action.action.Execute();
            }
        }
    }

    IEnumerator ExecuteCR(ActionTrigger action)
    {
        yield return new WaitForSeconds(action.delay);

        action.action.Execute();
    }

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        if (actions != null)
        {
            desc += ":\n";
            foreach (var action in actions)
            {
                desc += $"{ident}After {action.delay} seconds, {action.action.GetRawDescription(ident + " ")}\n";
            }
        }

        return desc;
    }
}
