using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnReset : Trigger
{
    public override string GetTriggerTitle() => "On Reset";

    protected override string GetRawDescription()
    {
        return "When Start is called";
    }

    private void Start()
    {
        if (!EvaluatePreconditions()) return;

        ExecuteTrigger();
    }
}
