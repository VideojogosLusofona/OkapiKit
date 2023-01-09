using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_OnReset : Trigger
{
    protected override string GetRawDescription()
    {
        return "When Start is called";
    }

    private void Start()
    {
        ExecuteTrigger();
    }
}
