using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionPlayParticleSystem : Action
{
    [SerializeField] private ParticleSystem target;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (target == null)
        {
            target = GetComponent<ParticleSystem>();
        }
        if (target)
        {
            target.Play();
        }
    }

    public override string GetRawDescription(string ident)
    {
        string targetName = (target) ? (target.name) : (name);

        return $"{GetPreconditionsString()}Play particle system {targetName}";
    }
}
