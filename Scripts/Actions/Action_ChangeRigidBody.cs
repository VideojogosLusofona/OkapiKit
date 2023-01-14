using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Action_ChangeRigidBody : Action
{
    [SerializeField] private enum ChangeType { SetBodyType };

    [SerializeField] 
    private ChangeType         changeType;
    [SerializeField, ShowIf("needsBodyType")] 
    private RigidbodyType2D    bodyType;

    private bool needsBodyType => (changeType == ChangeType.SetBodyType);

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
        }

        return "Changes something on rigid body (change type not defined)!";
    }
}
