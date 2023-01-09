using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_DestroyObject : Action
{
    [SerializeField] GameObject target;

    public override string GetRawDescription(string ident)
    {
        if (target)
        {
            return $"Destroy object {target.name}";
        }

        return "Destroy this object";
    }
    public override void Execute()
    {
        if (!enableAction) return;

        if (target == null)
        {
            Destroy(gameObject);
        }
        else
        {
            Destroy(target.gameObject);
        }
    }
}
