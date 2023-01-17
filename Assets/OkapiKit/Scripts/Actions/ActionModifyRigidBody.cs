using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionModifyRigidBody : Action
{
    [SerializeField] private enum ChangeType { SetBodyType, Mass, LinearDrag, AngularDrag, GravityScale };

    [SerializeField] 
    private ChangeType         changeType;
    [SerializeField, ShowIf("needsBodyType")] 
    private RigidbodyType2D    bodyType;
    [SerializeField, ShowIf("needValue")]
    private float              value;

    private bool needsBodyType => (changeType == ChangeType.SetBodyType);
    private bool needValue => (changeType == ChangeType.Mass) || (changeType == ChangeType.LinearDrag) || (changeType == ChangeType.AngularDrag) || (changeType == ChangeType.GravityScale);

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb == null) return;

        switch (changeType)
        {
            case ChangeType.SetBodyType:
                rb.bodyType = bodyType;
                break;
            case ChangeType.Mass:
                rb.mass= value;
                break;
            case ChangeType.LinearDrag:
                rb.drag = value;
                break;
            case ChangeType.AngularDrag:
                rb.angularDrag = value;
                break;
            case ChangeType.GravityScale:
                rb.gravityScale = value;
                break;
            default:
                break;
        }
    }

    public override string GetRawDescription(string ident)
    {
        var desc = GetPreconditionsString();
        switch (changeType)
        {
            case ChangeType.SetBodyType:
                desc += $"Changes body type of this object to {bodyType}";
                break;
            case ChangeType.Mass:
                desc += $"Changes mass of this object to {value}";
                break;
            case ChangeType.LinearDrag:
                desc += $"Changes linear drag of this object to {value}";
                break;
            case ChangeType.AngularDrag:
                desc += $"Changes angular drag of this object to {value}";
                break;
            case ChangeType.GravityScale:
                desc += $"Changes gravity scale of this object to {value}";
                break;
        }

        return desc;
    }
}
