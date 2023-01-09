using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Action_UnityEvent : Action
{
    [SerializeField] private UnityEvent unityEvent;

    public override void Execute()
    {
        unityEvent?.Invoke();
    }

    public override string GetRawDescription(string ident)
    {
        return $"Execute Unity event";
    }
}
