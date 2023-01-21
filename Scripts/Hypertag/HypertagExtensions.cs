using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class HypertaggedExtension
{
    public static HypertaggedObject FindObjectWithHypertag(this Object go, Hypertag tag)
    {
        var objects = Object.FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objects)
        {
            if (obj.Has(tag)) return obj;
        }

        return null;
    }
    public static T FindObjectOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
    {
        var objects = Object.FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objects)
        {
            if (obj.Has(tag))
            {
                var ret = obj.GetComponent<T>();
                if (ret)
                {
                    return ret;
                }
            }
        }

        return null;
    }

    public static T[] FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
    {
        var objects = Object.FindObjectsOfType<HypertaggedObject>();
        var ret = new List<T>();
        foreach (var obj in objects)
        {
            if (obj.Has(tag))
            {
                var comp = obj.GetComponent<T>();
                if (comp)
                {
                    ret.Add(comp);
                }
            }
        }

        return ret.ToArray();
    }

    public static T FindObjectOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag tag) where T : Component
    {
        var objects = Object.FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objects)
        {
            if (obj.Has(tag))
            {
                var ret = obj.GetComponent<T>();
                if (ret)
                {
                    return ret;
                }
            }
        }

        return null;
    }

    public static T[] FindObjectsOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag tag) where T : Component
    {
        var objects = Object.FindObjectsOfType<HypertaggedObject>();
        var ret = new List<T>();
        foreach (var obj in objects)
        {
            if (obj.Has(tag))
            {
                var comp = obj.GetComponent<T>();
                if (comp)
                {
                    ret.Add(comp);
                }
            }
        }

        return ret.ToArray();
    }

    public static bool HasHypertags(this GameObject go, Hypertag[] tags)
    {
        if (tags == null) return true;
        if (tags.Length == 0) return true;

        foreach (var tag in tags)
        {
            if (go.HasHypertag(tag)) return true;
        }

        return false;
    }

    public static bool HasHypertag(this GameObject go, Hypertag tag)
    {
        var hos = go.GetComponentsInChildren<HypertaggedObject>();
        if (hos == null)
        {
            return false;
        }

        foreach (var ho in hos)
        {
            if (ho.Has(tag)) return true;
        }

        return false;
    }
}
