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
        switch (target)
        {
            case Target.None:
                return "Remove this object from parent";
            case Target.Object:
                if (targetObject) return $"Sets this object as a child of {targetObject.name}";
                else return "Remove this object from parent";
            case Target.Tag:
                if (tag) return $"Sets this object as a child of object with tag {tag.name}";
                else return $"Sets this object as a child of object with tag [Undefined]";
            default:
                break;
        }
        return "[Uknown target]";
    }
}
