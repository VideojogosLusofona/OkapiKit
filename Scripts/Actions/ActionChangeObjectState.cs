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
        switch (state)
        {
            case StateChange.Enable:
                return (target) ? ($"Enables object {target.name}") : ("Enables this object");
            case StateChange.Disable:
                return (target) ? ($"Disables object {target.name}") : ("Disables this object");
            case StateChange.Toggle:
                return (target) ? ($"Toggles object {target.name}") : ("Toggles this object");
        }
        return "";
    }

    public override void Execute()
    {
        if (!enableAction) return;

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
