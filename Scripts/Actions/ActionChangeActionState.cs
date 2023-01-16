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
        switch (state)
        {
            case StateChange.Enable:
                return $"Enables action [{target.GetRawDescription(ident)}]";
            case StateChange.Disable:
                return $"Disables action [{target.GetRawDescription(ident)}]";
            case StateChange.Toggle:
                return $"Toggles action [{target.GetRawDescription(ident)}]";
        }
        return "";
    }

    public override void Execute()
    {
        if (!enableAction) return;
        if (target == null) return;

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
