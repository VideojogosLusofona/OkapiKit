using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Destroy Object")]
    public class ActionDestroyObject : Action
    {
        public enum Target { Self = 0, Parent = 1, Topmost = 2, Object = 3, Tag = 4 };

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
                case Target.Self:
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
                            if (tags[i] != null)
                            {
                                desc += tags[i].name;
                                if (i < tags.Length - 1) desc += ",";
                            }
                            else
                            {
                                desc += "[UNDEFINED]";
                            }
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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((target == Target.Object) && (targetObject == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined target object - this will destroy this object - consider using Self as target!", "It's always better to explicitly set properties, instead of letting the system guess what we want!"));
            }
            if (target == Target.Tag)
            {
                if ((tags == null) || (tags.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Deletion by tag is selected, but no tags are selected!", "We want to delete all objects with certain tags, but you didn't define which ones!"));
                }
                else
                {
                    int index = 0;
                    foreach (var tag in tags)
                    {
                        if (tag == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Empty tag defined in slot {index} of the tag list!", "There's an empty tag defined, please define it or delete it."));
                        }
                        index++;
                    }
                }
            }
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
}