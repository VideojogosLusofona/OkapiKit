using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trigger_Randomizer : Trigger
{
    [SerializeField] Trigger[]  choiceTriggers;

    protected override string GetRawDescription()
    {
        string desc = "Choose a random trigger:\n";
        for (int i = 0; i < choiceTriggers.Length; i++)
        {
            desc += $"{i}. " + choiceTriggers[i].GetDescription() + "\n";
        }

        return desc;
    }

    public override void ExecuteTrigger()
    {
        base.ExecuteTrigger();

        List<Trigger> availableTriggers = new List<Trigger>(choiceTriggers);
        availableTriggers.RemoveAll((t) => !t.enableTrigger);

        if (availableTriggers.Count == 0) return;

        // Select one of the triggers
        int r = Random.Range(0, availableTriggers.Count);

        availableTriggers[r].ExecuteTrigger();
    }
}
