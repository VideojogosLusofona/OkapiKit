using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnUpdate : Trigger
{
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
