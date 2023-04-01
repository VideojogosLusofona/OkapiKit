using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionDestroyObject : Action
    {
        public enum Target { This = 0, Parent = 1, Topmost = 2, Object = 3, Tag = 4 };

        [SerializeField]
        private Target target = Target.Object;

        [SerializeField, ShowIf("needsTarget")]
        private GameObject targetObject;
        [SerializeField, ShowIf("needsTags")]
        private Hypertag[] tags;

        private bool needsTarget => target == Target.Object;
        private bool needsTags => target == Target.Tag;

        public override string GetActionTitle() { return "Destroy Object"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            switch (target)
            {
                case Target.This:
                    desc += $"destroys this object";
                    break;
                case Target.Topmost:
                    desc += $"destroys the topmost object that contains this";
                    break;
                case Target.Parent:
                    desc += $"destroys the parent of this object";
                    break;
                case Target.Object:
                    if (targetObject != null) desc += $"Destroys object {targetObject.name}";
                    else desc += $"destroys this object";
                    break;
                case Target.Tag:
                    if (tags.Length > 0)
                    {
                        desc += "destroys objects with tags [";
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
                        desc += "destroys objects with tags [undefined]!";
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
                case Target.This:
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
}