using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static UnityEngine.CullingGroup;

public class ActionModifyTrailRenderer : Action
{
    [SerializeField] private enum ChangeType { Emitter };
    [SerializeField] private enum StateChange { Enable, Disable, Toggle};

    [SerializeField]
    private TrailRenderer       target;
    [SerializeField] 
    private ChangeType          changeType;
    [SerializeField, ShowIf("isEmitter")] 
    private StateChange         emitter;
    [SerializeField, ShowIf("needValue")]
    private float              value;

    private bool isEmitter => (changeType == ChangeType.Emitter);

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        TrailRenderer tr = target;
        if (tr == null) tr = GetComponent<TrailRenderer>();
        if (tr == null) return;

        switch (changeType)
        {
            case ChangeType.Emitter:
                if (emitter == StateChange.Enable) tr.emitting = true;
                else if (emitter == StateChange.Disable) tr.emitting = false;
                else tr.emitting = !tr.emitting;
                break;
            default:
                break;
        }
    }

    public override string GetRawDescription(string ident)
    {
        var desc = GetPreconditionsString();

        string targetName = (target) ? (target.name) : (name);
        switch (changeType)
        {
            case ChangeType.Emitter:
                switch (emitter)
                {
                    case StateChange.Enable:
                        desc += $"Enables {targetName}'s trail renderer emission";
                        break;
                    case StateChange.Disable:
                        desc += $"Disables {targetName}'s trail renderer emission";
                        break;
                    case StateChange.Toggle:
                        desc += $"Toggles {targetName}'s trail renderer emission";
                        break;
                }
                break;
        }

        return desc;
    }
}
