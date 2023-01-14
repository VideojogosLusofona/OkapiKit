using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

abstract public class Action : MonoBehaviour
{
    [SerializeField]
    protected bool      enableAction = true;
    [SerializeField, FormerlySerializedAsAttribute("tags")]
    private Hypertag[]  actionTags;

    public bool isActionEnabled { get { return enableAction; } set { enableAction = value; } }

    public abstract string GetRawDescription(string ident);

    public abstract void Execute();

    public bool HasTag(Hypertag[] tags)
    {
        foreach (var t in tags)
        {
            if (HasTag(t)) return true;
        }

        return false;
    }

    public bool HasTag(Hypertag tag)
    {
        foreach (var t in actionTags)
        {
            if (t == tag) return true;
        }

        return false;
    }
}
