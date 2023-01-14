using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Action_DestroyObject : Action
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
        switch (target)
        {
            case Target.Self:
                return $"Destroys this object";
            case Target.Topmost:
                return $"Destroys the topmost object that contains this";
            case Target.Parent:
                return $"Destroys the parent of this object";
            case Target.Object:
                if (targetObject != null) return $"Destroys object {targetObject.name}";
                else return $"Destroys this object";
            case Target.Tag:
                if (tags.Length > 0)
                {
                    string desc = "Destroys objects with tags [";
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
                    return "Destroys objects with tags [undefined]!";
                }
            default:
                break;
        }

        return "Destroy this object";
    }
    public override void Execute()
    {
        if (!enableAction) return;

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
