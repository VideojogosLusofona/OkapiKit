using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;

abstract public class Action : MonoBehaviour
{
    [SerializeField, HideInInspector]
    protected bool      _showInfo = true;
    [SerializeField]
    protected bool      enableAction = true;
    [SerializeField]
    protected bool      hasTags = false;
    [SerializeField, FormerlySerializedAsAttribute("tags"), ShowIf("hasTags")]
    private Hypertag[]  actionTags;
    [SerializeField]
    private bool        hasConditions = false;
    [SerializeField, ShowIf("hasConditions")]
    private Condition[] actionConditions;

    [SerializeField, ResizableTextArea, ReadOnly]
    private string      _explanation;

    public bool isActionEnabled { get { return enableAction; } set { enableAction = value; } }

    public abstract string GetRawDescription(string ident);

    public abstract void Execute();

    public virtual string GetActionTitle() { return "Action"; }

    public bool showInfo
    {
        get { return _showInfo; }
        set { _showInfo = value; }
    }

    public bool HasTag(Hypertag[] tags)
    {
        if (!hasTags) return false;

        foreach (var t in tags)
        {
            if (HasTag(t)) return true;
        }

        return false;
    }

    public bool HasTag(Hypertag tag)
    {
        if (!hasTags) return false;

        foreach (var t in actionTags)
        {
            if (t == tag) return true;
        }

        return false;
    }

    protected bool EvaluatePreconditions()
    {
        if (!hasConditions) return true;
        if (actionConditions == null) return true;

        foreach (var condition in actionConditions)
        {
            if (!condition.Evaluate(gameObject)) return false;
        }

        return true;
    }

    protected string GetPreconditionsString()
    {
        if (!hasConditions) return "";

        string desc = "";
        if ((actionConditions != null) && (actionConditions.Length > 0))
        {
            desc += "If ";
            for (int i = 0; i < actionConditions.Length; i++)
            {
                desc += actionConditions[i].GetRawDescription();
                if (i < actionConditions.Length - 1) desc += " and ";
            }
            desc += " ";
        }
        return desc;
    }

    [Button("Update Explanation")]
    private void UpdateExplanation()
    {
        _explanation = GetRawDescription("");
    }

}
