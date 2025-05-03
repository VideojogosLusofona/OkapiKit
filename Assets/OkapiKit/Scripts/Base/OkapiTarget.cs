using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [Serializable]
    public class OkapiTarget<T> where T : Component
    {
        [Flags]
        public enum Flags
        {
            IncChildren = 1,
            IncParent = 2
        };
        public enum Type { Hypertag, Object, Self, Collider, ColliderChildTag };

        [SerializeField] protected Type         type = Type.Hypertag;
        [SerializeField] protected Hypertag     tag;
        [SerializeField] protected T            obj;
        [SerializeField] protected Flags        flags;

        public Type         targetType => type;
        public Hypertag     targetTag => tag;

        public bool         isDynamic => (type == Type.Collider) || (type == Type.Hypertag) || (type == Type.ColliderChildTag);
        public bool         incChildren => (flags & Flags.IncChildren) != 0;
        public bool         incParent => (flags & Flags.IncParent) != 0;

        public virtual T GetTarget(GameObject parentGameObject)
        {
            switch (type)
            {
                case Type.Hypertag:
                    if (tag)
                    {
                        var allObjects = parentGameObject.FindObjectsOfTypeWithHypertag<HypertaggedObject>(tag);
                        foreach (var obj in allObjects)
                        {
                            var t = obj.GetComponent<T>();
                            if (t) return t;

                            if (incChildren) t = obj.GetComponentInChildren<T>();
                            if (t) return t;

                            if (incParent) t = obj.GetComponentInParent<T>();
                            if (t) return t;
                        }
                    }
                    break;
                case Type.Object:
                    if (obj)
                    {
                        var t = obj.GetComponent<T>();
                        if (t) return t;

                        if (incChildren) t = obj.GetComponentInChildren<T>();
                        if (t) return t;

                        if (incParent) t = obj.GetComponentInParent<T>();
                        if (t) return t;
                    }
                    break;
                case Type.Self:
                    {
                        var obj = parentGameObject;
                        var t = obj.GetComponent<T>();
                        if (t) return t;

                        if (incChildren) t = obj.GetComponentInChildren<T>();
                        if (t) return t;

                        if (incParent) t = obj.GetComponentInParent<T>();
                        if (t) return t;
                    }
                    break;
                case Type.Collider:
                    {
                        var colliderObject = TriggerOnCollision.GetLastCollider();
                        if (colliderObject != null)
                        {
                            var obj = colliderObject;
                            var t = obj.GetComponent<T>();
                            if (t) return t;

                            if (incChildren) t = obj.GetComponentInChildren<T>();
                            if (t) return t;

                            if (incParent) t = obj.GetComponentInParent<T>();
                            if (t) return t;
                        }
                    }
                    break;
                case Type.ColliderChildTag:
                    {
                        var colliderObject = TriggerOnCollision.GetLastCollider();
                        if (colliderObject != null)
                        {
                            var obj = colliderObject;
                            var tt = obj.GetComponentsInChildren<HypertaggedObject>();
                            foreach (var tmp in tt)
                            {
                                if (tmp.Has(tag))
                                {
                                    var t = tmp.GetComponent<T>();
                                    if (t) return t;

                                    if (incChildren) t = tmp.GetComponentInChildren<T>();
                                    if (t) return t;

                                    if (incParent) t = tmp.GetComponentInParent<T>();
                                    if (t) return t;
                                }                                
                            }

                        }
                    }
                    break;
                default:
                    break;
            }

            return null;
        }

        public virtual List<T> GetTargets(GameObject parentGameObject)
        {
            List <T> ret = new List<T>();
            switch (type)
            {
                case Type.Hypertag:
                    if (tag)
                    {
                        var allObjects = parentGameObject.FindObjectsOfTypeWithHypertag<HypertaggedObject>(tag);
                        foreach (var obj in allObjects)
                        {
                            var t = obj.GetComponent<T>();
                            T[] tt = null;
                            if (t) ret.Add(t);

                            if (incChildren) tt = obj.GetComponentsInChildren<T>();
                            if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                            if (incParent) t = obj.GetComponentInParent<T>();
                            if (t) ret.Add(t);
                        }
                    }
                    break;
                case Type.Object:
                    if (obj)
                    {
                        if (obj) ret.Add(obj);

                        T   t = null;
                        T[] tt = null;
                        if (incChildren) tt = obj.GetComponentsInChildren<T>();
                        if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                        if (incParent) t = obj.GetComponentInParent<T>();
                        if (t) ret.Add(t);
                    }
                    break;
                case Type.Self:
                    {
                        T t = null;

                        t = parentGameObject.GetComponent<T>();
                        if (t) ret.Add(t);

                        T[] tt = null;
                        if (incChildren) tt = parentGameObject.GetComponentsInChildren<T>();
                        if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                        if (incParent) t = parentGameObject.GetComponentInParent<T>();
                        if (t) ret.Add(t);
                    }
                    break;
                case Type.Collider:
                    {
                        var colliderObject = TriggerOnCollision.GetLastCollider();
                        if (colliderObject != null)
                        {
                            T t = null;

                            t = colliderObject.GetComponent<T>();
                            if (t) ret.Add(t);

                            T[] tt = null;
                            if (incChildren) tt = colliderObject.GetComponentsInChildren<T>();
                            if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                            if (incParent) t = colliderObject.GetComponentInParent<T>();
                            if (t) ret.Add(t);
                        }
                    }
                    break;
                case Type.ColliderChildTag:
                    {
                        var colliderObject = TriggerOnCollision.GetLastCollider();
                        if (colliderObject != null)
                        {
                            var obj = colliderObject;
                            var allObjs = obj.GetComponentsInChildren<HypertaggedObject>();
                            foreach (var tmp in allObjs)
                            {
                                if (tmp.Has(tag))
                                {
                                    var tt = tmp.GetComponents<T>();
                                    if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                                    if (incChildren) tt = tmp.GetComponentsInChildren<T>();
                                    if ((tt != null) && (tt.Length > 0)) ret.AddRange(tt);

                                    T t = null;
                                    if (incParent) t = tmp.GetComponentInParent<T>();
                                    if (t) ret.Add(t);
                                }
                            }

                        }
                    }
                    break;
                default:
                    break;
            }

            return ret;
        }

        public void CheckErrors(List<LogEntry> logs, string propName, GameObject parentGameObject)
        {
            switch (type)
            {
                case Type.Hypertag:
                    if (tag == null)
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"No target tag defined to search for {propName}!", $"An object of type {propName} will be searched on any object with the given tag."));
                    break;
                case Type.Object:
                    if (obj == null)
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"No target defined for {propName}!"));
                    break;
                case Type.Self:
                    if ((parentGameObject.GetComponent<T>() == null) && (flags == 0))
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"This object doesn't have a target of type {typeof(T).Name}!"));
                    }
                    break;
                case Type.ColliderChildTag:
                    if (tag == null)
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"No target tag defined to search for {propName}!", $"An object of type {propName} will be searched on the last collider with the given tag."));
                    break;
                default:
                    break;
            }
        }

        public string GetRawDescription(string propName, GameObject refObject)
        {
            string desc = "";

            switch (type)
            {
                case Type.Hypertag:
                    desc = $"{propName} with tag [{tag?.name ?? "UNDEFINED"}]";
                    break;
                case Type.Object:
                    if (obj) desc = $"{propName} on {obj.name}";
                    else desc = $"{propName} on [UNDEFINED]";
                    break;
                case Type.Self:
                    desc = $"{propName} on this object";
                    break;
                case Type.Collider:
                    desc = $"{propName} on the colliding object";
                    break;
                case Type.ColliderChildTag:
                    desc = $"{propName} on the colliding object children with tag [{tag?.name ?? "UNDEFINED"}]";
                    break;
                default:
                    break;
            }
            if ((incChildren) && (incParent)) desc += "(inc. children & parent)";
            else if (incChildren) desc += "(inc. children)";
            else if (incParent) desc += "(inc. parent)";

            return desc;
        }

        public virtual string GetShortDescription(GameObject refObject)
        {
            string desc = "";

            switch (type)
            {
                case Type.Hypertag:
                    if (tag) desc = $"[{tag.name}]";
                    else desc = $"[UNDEFINED]";
                    break;
                case Type.Object:
                    if (obj) desc = $"[{obj.name}]";
                    else desc = $"[UNDEFINED]";
                    break;
                case Type.Self:
                    desc = $"this";
                    break;
                case Type.Collider:
                    desc = $"Collider";
                    break;
                case Type.ColliderChildTag:
                    desc = $"Collider[{tag?.name ?? "UNDEFINED"}]";
                    break;
                default:
                    break;
            }
            if ((incChildren) && (incParent)) desc += "(inc. children & parent)";
            else if (incChildren) desc += "(inc. children)";
            else if (incParent) desc += "(inc. parent)";

            return desc;
        }
    }

    [Serializable]
    public class TargetRenderer : OkapiTarget<Renderer>
    {

    }

    [Serializable]
    public class TargetTransform : OkapiTarget<Transform>
    {

    }
}
