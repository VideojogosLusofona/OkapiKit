using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.CullingGroup;

public class ActionBlink : Action
{
    [SerializeField] private Renderer   target;
    [SerializeField] private float      blinkTimeOn = 0.2f;
    [SerializeField] private float      blinkTimeOff = 0.2f;
    [SerializeField] private float      duration = 2.0f;

    bool    startState;
    float   timer;
    float   blinkPhaseTimer;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;
        if (target == null) return;

        timer = duration;
        startState = target.enabled;
        target.enabled = !startState;
        timer = duration;
        blinkPhaseTimer = (target.enabled) ? (blinkTimeOn) : (blinkTimeOff);
    }

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        if (target == null)
        {
            desc += $"Blinks this renderer for {duration} seconds";
        }
        else
        {
            desc += $"Blinks renderer {target.name} for {duration} seconds";
        }

        return desc;
    }

    void Start()
    {
        if (target == null)
        {
            target = GetComponent<Renderer>();
            if (target == null)
            {
                target = GetComponentInChildren<Renderer>();
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null) return;

        if (duration > 0)
        {
            duration -= Time.deltaTime;
            if (duration <= 0)
            {
                target.enabled = startState;
            }
            else
            {
                blinkPhaseTimer -= Time.deltaTime;
                if (blinkPhaseTimer <= 0)
                {
                    target.enabled = !target.enabled;
                    blinkPhaseTimer = (target.enabled) ? (blinkTimeOn) : (blinkTimeOff);
                }
            }
        }
    }
}
