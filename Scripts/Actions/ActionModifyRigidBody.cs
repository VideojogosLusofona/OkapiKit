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
        switch (changeType)
        {
            case ChangeType.SetBodyType:
                return $"Changes body type of this object to {bodyType}";
            case ChangeType.Mass:
                return $"Changes mass of this object to {value}";
            case ChangeType.LinearDrag:
                return $"Changes linear drag of this object to {value}";
            case ChangeType.AngularDrag:
                return $"Changes angular drag of this object to {value}";
            case ChangeType.GravityScale:
                return $"Changes gravity scale of this object to {value}";
        }

        return "Changes something on rigid body (change type not defined)!";
    }
}
