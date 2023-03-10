using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class HypertaggedObject : OkapiElement
{
    [SerializeField] 
    private List<Hypertag>  hypertags;

    public string GetTagString()
    {
        if ((hypertags == null) || (hypertags.Count == 0)) return "Hypertag";

        string ret = "";
        foreach (var tag in hypertags)
        {
            if (tag)
            {
                if (ret != "") ret += ", ";
                ret += tag.name;
            }
        }

        return ret;
    }

    public void AddTag(Hypertag tag)
    {
        if (hypertags == null) hypertags = new List<Hypertag>();

        if (!hypertags.Contains(tag)) hypertags.Add(tag);
    }

    public void AddTag(Hypertag[] tags)
    {
        foreach (var t in tags)
        {
            AddTag(t);
        }
    }

    public void AddTag(List<Hypertag> tags)
    {
        foreach (var t in tags)
        {
            AddTag(t);
        }
    }



    public bool Has(Hypertag tag)
    {
        if (hypertags == null) return false;
        foreach (var t in hypertags)
        {
            if (t == tag) return true;
        }

        return false;
    }

    public bool Has(Hypertag[] tags)
    {
        if (hypertags == null) return false;
        if (tags == null) return false;

        foreach (var t1 in tags)
        {
            foreach (var t2 in hypertags)
            {
                if (t1 == t2) return true;
            }
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

    public override string GetRawDescription(string ident, GameObject refObject)
    {
        if (description != "") return description;
        
        return "";
    }

    public override string UpdateExplanation()
    {
        string e = GetRawDescription("", gameObject);

        _explanation = "";
        if (e != null)
        {
            for (int i = 0; i < e.Length; i++)
            {
                if (i != ' ')
                {
                    _explanation += char.ToUpper(e[i]) + e.Substring(i + 1);
                    break;
                }
                else
                {
                    _explanation += e[i];
                }
            }
        }
        return _explanation;
    }
}
