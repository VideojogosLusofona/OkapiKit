using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class Action : MonoBehaviour
{
    [SerializeField]
    protected bool      enableAction = true;
    [SerializeField]
    private Hypertag[]  tags;

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
        foreach (var t in this.tags)
        {
            if (t == tag) return true;
        }

        return false;
    }
}
