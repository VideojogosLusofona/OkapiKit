using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionDestroyObject : Action
{
    [SerializeField] private enum Target { Self, Parent, Topmost, Object, Tag };

    [SerializeField]
    private Target      target = Target.Object;

    [SerializeField, ShowIf("needsTarget")]
    private GameObject  targetObject;
    [SerializeField, ShowIf("needsTags")]
    private Hypertag[]  tags;

    private bool needsTarget => target == Target.Object;
    private bool needsTags => target == Target.Tag;

    public override string GetRawDescription(string ident)
    {
        string desc = GetPreconditionsString();

        switch (target)
        {
            case Target.Self:
                desc += $"Destroys this object";
                break;
            case Target.Topmost:
                desc += $"Destroys the topmost object that contains this";
                break;
            case Target.Parent:
                desc += $"Destroys the parent of this object";
                break;
            case Target.Object:
                if (targetObject != null) desc += $"Destroys object {targetObject.name}";
                else desc += $"Destroys this object";
                break;
            case Target.Tag:
                if (tags.Length > 0)
                {
                    desc += "Destroys objects with tags [";
                    for (int i = 0; i < tags.Length; i++)
                    {
                        desc += tags[i].name;
                        if (i < tags.Length - 1) desc += ",";
                    }
                    desc += "] ";
                    return desc;
                }
                else
                {
                    desc += "Destroys objects with tags [undefined]!";
                }
                break;
            default:
                break;
        }

        return desc;
    }
    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        switch (target)
        {
            case Target.Self:
                Destroy(gameObject);
                break;
            case Target.Topmost:
                {
                    Transform o = gameObject.transform;
                    while (o.parent != null) 
                    {
                        o = o.parent;
                    }
                    Destroy(o.gameObject);
                }
                break;
            case Target.Parent:
                {
                    Transform p = gameObject.transform.parent;
                    if (p)
                    {
                        Destroy(p.gameObject);
                    }
                }
                break;
            case Target.Object:
                if (targetObject) Destroy(targetObject);
                else Destroy(gameObject);
                break;
            case Target.Tag:
                var objs = HypertaggedObject.FindGameObjectsWithHypertag(tags);
                foreach (var obj in objs)
                {
                    Destroy(obj);
                }
                break;
            default:
                break;
        }
    }
}
