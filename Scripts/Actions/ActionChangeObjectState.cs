using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionChangeObjectState : Action
{
    [SerializeField] private enum StateChange { Enable, Disable, Toggle };

    [SerializeField] private GameObject     target;
    [SerializeField] private StateChange    state;

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        switch (state)
        {
            case StateChange.Enable:
                desc += (target) ? ($"Enables object {target.name}") : ("Enables this object");
                break;
            case StateChange.Disable:
                desc += (target) ? ($"Disables object {target.name}") : ("Disables this object");
                break;
            case StateChange.Toggle:
                desc += (target) ? ($"Toggles object {target.name}") : ("Toggles this object");
                break;
        }
        return desc;
    }

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        GameObject go = target;
        if (go == null) go = gameObject;

        switch (state)
        {
            case StateChange.Enable:
                go.SetActive(true);
                break;
            case StateChange.Disable:
                go.SetActive(false);
                break;
            case StateChange.Toggle:
                go.SetActive(!go.activeSelf);
                break;
            default:
                break;
        }
    }
}
