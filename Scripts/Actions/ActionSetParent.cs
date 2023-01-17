using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionSetParent : Action
{
    [SerializeField] private enum Target { None, Object, Tag };

    [SerializeField]
    private Target      target = Target.Object;

    [SerializeField, ShowIf("needsTarget")]
    private GameObject  targetObject;
    [SerializeField, ShowIf("needsTags")]
    new private Hypertag    tag;

    private bool needsTarget => target == Target.Object;
    private bool needsTags => target == Target.Tag;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        switch (target)
        {
            case Target.None:
                transform.SetParent(null);
                break;
            case Target.Object:
                if (targetObject) transform.SetParent(targetObject.transform);
                else transform.SetParent(null);
                break;
            case Target.Tag:
                var obj = HypertaggedObject.FindGameObjectWithHypertag(tag);
                if (obj) transform.SetParent(obj.transform);
                else transform.SetParent(null);
                break;
            default:
                break;
        }
    }

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        switch (target)
        {
            case Target.None:
                desc += "Remove this object from parent";
                break;
            case Target.Object:
                if (targetObject) desc += $"Sets this object as a child of {targetObject.name}";
                else desc += "Remove this object from parent";
                break;
            case Target.Tag:
                if (tag) desc += $"Sets this object as a child of object with tag {tag.name}";
                else desc += $"Sets this object as a child of object with tag [Undefined]";
                break;
            default:
                break;
        }
        return desc;
    }
}
