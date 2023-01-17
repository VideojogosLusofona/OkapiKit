using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionModifyRenderer : Action
{
    [SerializeField] private enum ChangeType { Visibility };

    [SerializeField] private enum BoolChange { Enable, Disable, Toggle };

    [SerializeField]
    new private Renderer    renderer;
    [SerializeField] 
    private ChangeType  changeType;
    [SerializeField, ShowIf("needVisibility")]
    private BoolChange  visibility;

    private bool needVisibility => (changeType == ChangeType.Visibility);

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (renderer == null)
        {
            renderer = GetComponent<Renderer>();
        }
        if (renderer == null) return;

        switch (changeType)
        {
            case ChangeType.Visibility:
                switch (visibility)
                {
                    case BoolChange.Enable:
                        renderer.enabled = true;
                        break;
                    case BoolChange.Disable:
                        renderer.enabled = false;
                        break;
                    case BoolChange.Toggle:
                        renderer.enabled = !renderer.enabled;
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
        var rendererName = (renderer) ? (renderer.name) : ("this");
        switch (changeType)
        {
            case ChangeType.Visibility:
                switch (visibility)
                {
                    case BoolChange.Enable:
                        desc += $"Enables renderer {rendererName}";
                        break;
                    case BoolChange.Disable:
                        desc += $"Disables renderer {rendererName}";
                        break;
                    case BoolChange.Toggle:
                        desc += $"Toggles renderer {rendererName}";
                        break;
                    default:
                        break;
                }
                
                break;
        }

        return desc;
    }
}
