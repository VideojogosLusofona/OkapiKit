using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnUpdate : Trigger
{
    public override string GetTriggerTitle() => "On Update";

    protected override string GetRawDescription()
    {
        return "Every frame";
    }

    private void Update()
    {
        if (!EvaluatePreconditions()) return;

        ExecuteTrigger();
    }
}
