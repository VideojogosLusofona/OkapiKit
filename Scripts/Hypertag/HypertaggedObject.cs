using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HypertaggedObject : MonoBehaviour
{
    [SerializeField] private Hypertag[] hypertags;

    public bool Has(Hypertag tag)
    {
        foreach (var t in hypertags)
        {
            if (t == tag) return true;
        }

        return false;
    }

    public static List<GameObject> FindGameObjectsWithHypertag(Hypertag[] tags)
    {
        List<GameObject> ret = new List<GameObject>();

        var objs = FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objs)
        {
            foreach (var t in tags)
            {
                if (obj.Has(t))
                {
                    ret.Add(obj.gameObject);
                    break;
                }
            }
        }

        return ret;
    }

    public static List<GameObject> FindGameObjectsWithHypertag(Hypertag tag)
    {
        List<GameObject> ret = new List<GameObject>();

        var objs = FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objs)
        {
            if (obj.Has(tag))
            {
                ret.Add(obj.gameObject);
                break;
            }
        }

        return ret;
    }

    public static GameObject FindGameObjectWithHypertag(Hypertag tag)
    {
        var objs = FindObjectsOfType<HypertaggedObject>();
        foreach (var obj in objs)
        {
            if (obj.Has(tag))
            {
                return obj.gameObject;
            }
        }

        return null;
    }
}
