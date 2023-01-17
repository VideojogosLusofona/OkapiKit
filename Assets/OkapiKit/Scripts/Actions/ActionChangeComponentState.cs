using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

public class ActionChangeComponentState : Action
{
    [SerializeField] private enum StateChange { Enable, Disable, Toggle };

    [SerializeField] private Behaviour      target;
    [SerializeField] private StateChange    state;

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        string targetDesc = "[Undefined] on [Undefined Object]";
        if (target != null)
        {
            targetDesc = $"{target} on {target.name}";
        }
        switch (state)
        {
            case StateChange.Enable:
                desc += $"Enables component {targetDesc}";
                break;
            case StateChange.Disable:
                desc += $"Disables component {targetDesc}";
                break;
            case StateChange.Toggle:
                desc += $"Toggles component {targetDesc}";
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
                target.enabled = true;
                break;
            case StateChange.Disable:
                target.enabled = false;
                break;
            case StateChange.Toggle:
                target.enabled = !target.enabled;
                break;
            default:
                break;
        }
        return;
    }
}
