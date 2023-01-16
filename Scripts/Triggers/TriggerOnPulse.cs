using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerOnPulse : Trigger
{
    [SerializeField] public string pulseString = "1111111";
    [SerializeField] public float  pulseDuration = 0.1f;

    protected override string GetRawDescription()
    {
        return $"Triggers the actions with a pulse pattern of [{pulseString}]. Each pulse lasts for {pulseDuration} seconds";
    }

    public override void ExecuteTrigger()
    {
        StartCoroutine(ExecuteTriggerCR());
    }

    IEnumerator ExecuteTriggerCR()
    { 
        for (int i = 0; i < pulseString.Length; i++)
        {
            if ((pulseString[i] == '1') || (pulseString[i] == 'x'))
            {
                base.ExecuteTrigger();
            }
            yield return new WaitForSeconds(pulseDuration);
        }
    }
}
