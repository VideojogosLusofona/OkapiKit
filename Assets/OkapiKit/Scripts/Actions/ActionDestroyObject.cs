using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Destroy Object")]
    public class ActionDestroyObject : Action
    {
        public enum Target { Self = 0, Parent = 1, Topmost = 2, Object = 3, Tag = 4, Collider = 5 };

        [SerializeField]
        private Target target = Target.Self;

        [SerializeField, ShowIf("needsTarget")]
        private GameObject targetObject;
        [SerializeField, ShowIf("needsTags")]
        private Hypertag[] tags;

        private bool needsTarget => target == Target.Object;
        private bool needsTags => target == Target.Tag;

        public override string GetActionTitle() { return "Destroy Object"; }

        public bool WillDestroyThis(GameObject otherGameObject)
        {
            switch (target)
            {
                case Target.Self:
                    if (otherGameObject == gameObject) return true;
                    if (IsChild(gameObject, otherGameObject)) return true;
                    break;
                case Target.Parent:
                    if (transform.parent != null)
                    {
                        if (otherGameObject == transform.parent.gameObject) return true;
                        if (IsChild(transform.parent.gameObject, otherGameObject)) return true;
                    }
                    break;
                case Target.Topmost:
                    if (otherGameObject == GetTopMost(gameObject)) return true;
                    if (IsChild(GetTopMost(gameObject), otherGameObject)) return true;
                    break;
                case Target.Object:
                    if (targetObject != null)
                    {
                        if (otherGameObject == targetObject) return true;
                        if (IsChild(targetObject, otherGameObject)) return true;
                    }
                    else
                    {
                        if (otherGameObject == gameObject) return true;
                        if (IsChild(gameObject, otherGameObject)) return true;
                    }
                    break;
                case Target.Collider:
                    return false;
            }

            return false;
        }

        static bool IsChild(GameObject baseObject, GameObject otherGameObject)
        {            
            for (int i = 0; i < baseObject.transform.childCount; i++)
            {
                var childTransform = baseObject.transform.GetChild(i);
                if (childTransform.gameObject == otherGameObject) return true;
                if (IsChild(childTransform.gameObject, otherGameObject)) return true;
            }

            return false;
        }

        static GameObject GetTopMost(GameObject baseObject)
        {
            Transform o = baseObject.transform;
            while (o.parent != null)
            {
                o = o.parent;
            }
            return o.gameObject;
        }

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
                case Target.Collider:
                    desc += "destroys object that collided with this - only valid when called from a collision event";
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors(int level)
        {
             base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (target == Target.Object)
            {
                if (targetObject == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined target object - this will destroy this object - consider using Self as target!", "It's always better to explicitly set properties, instead of letting the system guess what we want!"));
                }
                else if (targetObject == gameObject)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "You selected this object as target - consider using Self as target!", "Self represents the object that contains this action, so you can use that instead of using the reference to him"));
                }
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
                        GameObject topMost = GetTopMost(gameObject);
                        Destroy(topMost);
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
                case Target.Collider:
                    var collider = TriggerOnCollision.GetLastCollider();
                    if (collider != null)
                    {
                        Destroy(collider);
                    }
                    break;
                default:
                    break;
            }
        }
    }
}