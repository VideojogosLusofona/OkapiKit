using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [Serializable]
    public class OkapiTarget<T> where T : Component
    {
        public enum Type { Hypertag, Object, Self, Parent, Child, LastCollider };

        [SerializeField] protected Type       type = Type.Hypertag;
        [SerializeField] protected Hypertag   tag;
        [SerializeField] protected T          obj;

        public Type         targetType => type;
        public Hypertag     targetTag => tag;

        public bool         isDynamic => (type == Type.LastCollider) || (type == Type.Hypertag);

        public virtual T GetTarget(GameObject parentGameObject)
        {
            switch (type)
            {
                case Type.Hypertag:
                    if (tag)
                    {
                        return parentGameObject.FindObjectOfTypeWithHypertag<T>(tag);
                    }
                    break;
                case Type.Object:
                    if (obj)
                    {
                        return obj.GetComponent<T>();
                    }
                    break;
                case Type.Self:
                    return parentGameObject.GetComponent<T>();
                case Type.Parent:
                    return parentGameObject.GetComponentInParent<T>();
                case Type.Child:
                    return parentGameObject.GetComponentInChildren<T>();
                case Type.LastCollider:
                    var colliderObject = TriggerOnCollision.GetLastCollider();
                    if (colliderObject != null)
                    {
                        return colliderObject.GetComponent<T>();
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
                        ret.AddRange(parentGameObject.FindObjectsOfTypeWithHypertag<T>(tag));
                    }
                    break;
                case Type.Object:
                    if (obj)
                    {
                        ret.Add(obj);
                    }
                    break;
                case Type.Self:
                    ret.Add(parentGameObject.GetComponent<T>());
                    break;
                case Type.Parent:
                    ret.Add(parentGameObject.GetComponentInParent<T>());
                    break;
                case Type.Child:
                    ret.AddRange(parentGameObject.GetComponentsInChildren<T>());
                    break;
                case Type.LastCollider:
                    var colliderObject = TriggerOnCollision.GetLastCollider();
                    if (colliderObject != null)
                    {
                        ret.Add(colliderObject.GetComponent<T>());
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
                    if (parentGameObject.GetComponent<T>() == null)
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"This object doesn't have a target of type {typeof(T).Name}!"));
                    }
                    break;
                case Type.Parent:
                    if (parentGameObject.GetComponentInParent<T>() == null)
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"Can't find a target of type {typeof(T).Name} on the parents of this object!"));
                    }
                    break;
                case Type.Child:
                    if (parentGameObject.GetComponentInChildren<T>() == null)
                    {
                        logs.Add(new LogEntry(LogEntry.Type.Error, $"Can't find a target of type {typeof(T).Name} on the children of this object!"));
                    }
                    break;
                case Type.LastCollider:
                    break;
                default:
                    break;
            }
        }

        public string GetRawDescription(string propName, GameObject refObject)
        {
            switch (type)
            {
                case Type.Hypertag:
                    if (tag) return $"{propName} with tag [{tag.name}]";
                    else return $"{propName} with tag [UNDEFINED]";
                case Type.Object:
                    if (obj) return $"{propName} on {obj.name}";
                    else return $"{propName} on [UNDEFINED]";
                case Type.Self:
                    return $"{propName} on this object";
                case Type.Parent:
                    return $"{propName} on this object's parents";
                case Type.Child:
                    return $"{propName} on this object's children";
                case Type.LastCollider:
                    return $"{propName} on the colliding object";
                default:
                    break;
            }

            return "[UNDEFINED]";
        }

        public virtual string GetShortDescription(GameObject refObject)
        {
            switch (type)
            {
                case Type.Hypertag:
                    if (tag) return $"[{tag.name}]";
                    else return $"[UNDEFINED]";
                case Type.Object:
                    if (obj) return $"[{obj.name}]";
                    else return $"[UNDEFINED]";
                case Type.Self:
                    return $"";
                case Type.Parent:
                    return $"Parent";
                case Type.Child:
                    return $"Child";
                case Type.LastCollider:
                    return $"Collider";
                default:
                    break;
            }

            return "[UNDEFINED]";
        }
    }

    [Serializable]
    public class TargetRenderer : OkapiTarget<Renderer>
    {

    }
}
