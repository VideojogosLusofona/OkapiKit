using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChangeActionState : Action
{
    [SerializeField] private enum StateChange { Enable, Disable, Toggle };

    [SerializeField] private Action         target;
    [SerializeField] private StateChange    state;

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        switch (state)
        {
            case StateChange.Enable:
                desc += $"Enables action [{target.GetRawDescription(ident)}]";
                break;
            case StateChange.Disable:
                desc += $"Disables action [{target.GetRawDescription(ident)}]";
                break;
            case StateChange.Toggle:
                desc += $"Toggles action [{target.GetRawDescription(ident)}]";
                break;
        }
        return desc;
    }

    public override void Execute()
    {
        if (!enableAction) return;
        if (target == null) return;
        if (!EvaluatePreconditions()) return;

        switch (state)
        {
            case StateChange.Enable:
                target.isActionEnabled = true;
                break;
            case StateChange.Disable:
                target.isActionEnabled = false;
                break;
            case StateChange.Toggle:
                target.isActionEnabled = !target.isActionEnabled;
                break;
            default:
                break;
        }
    }
}
