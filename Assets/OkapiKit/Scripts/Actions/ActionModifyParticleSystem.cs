using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Android;

public class ActionModifyParticleSystem : Action
{
    [SerializeField] private enum ChangeType { Emission };

    [SerializeField] private enum BoolChange { Enable, Disable, Toggle };

    [SerializeField]
    new private ParticleSystem    particleSystem;
    [SerializeField] 
    private ChangeType          changeType;
    [SerializeField, ShowIf("needEmission")]
    private BoolChange          emission;

    private bool needEmission => (changeType == ChangeType.Emission);

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }
        if (particleSystem == null) return;

        ParticleSystem.EmissionModule   emissionModule;

        switch (changeType)
        {
            case ChangeType.Emission:
                switch (emission)
                {
                    case BoolChange.Enable:
                        emissionModule = particleSystem.emission;
                        emissionModule.enabled = true;
                        break;
                    case BoolChange.Disable:
                        emissionModule = particleSystem.emission;
                        emissionModule.enabled = false;
                        break;
                    case BoolChange.Toggle:
                        emissionModule = particleSystem.emission;
                        emissionModule.enabled = !emissionModule.enabled;
                        break;
                    default:
                        break;
                }
                break;
            default:
                break;
        }
    }

    public override string GetRawDescription(string ident)
    {
        var desc = GetPreconditionsString();
        var psName = (particleSystem) ? (particleSystem.name) : ("this");
        switch (changeType)
        {
            case ChangeType.Emission:
                switch (emission)
                {
                    case BoolChange.Enable:
                        desc += $"Enables emission of particle system {psName}";
                        break;
                    case BoolChange.Disable:
                        desc += $"Disables emission of particle system {psName}";
                        break;
                    case BoolChange.Toggle:
                        desc += $"Toggles  emission of particle system {psName}";
                        break;
                    default:
                        break;
                }
                
                break;
        }

        return desc;
    }
}
