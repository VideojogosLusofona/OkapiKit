using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using NaughtyAttributes;
using Unity.VisualScripting;

public abstract class Trigger : MonoBehaviour
{
    [System.Serializable]
    public struct ActionTrigger
    {
        public float    delay;
        public Action   action;
    }

    [SerializeField, ResizableTextArea]
    public      string          description;
    [SerializeField]
    public      bool            enableTrigger = true;
    [SerializeField]
    public      bool            allowRetrigger = true;
    [SerializeField]
    private     bool            hasPreconditions = false;
    [SerializeField, ShowIf("hasPreconditions")] 
    private     Condition[]     preConditions;
    [SerializeField]
    protected   ActionTrigger[] actions;

    [SerializeField, ResizableTextArea, ReadOnly]
    private string _explanation;

    private bool alreadyTriggered = false;

    [Button("Update Explanation")]
    private void UpdateExplanation()
    {
        _explanation = GetDescription();
    }

    public string GetDescription()
    {
        string desc = "";
        if (description != "") desc += description + "\n----------------\n";

        if (hasPreconditions)
        {
            if ((preConditions != null) && (preConditions.Length > 0))
            {
                desc += "If ";
                for (int i = 0; i < preConditions.Length; i++)
                {
                    desc += preConditions[i].GetRawDescription() + " and ";
                }
            }
        }

        desc += GetRawDescription() + ":\n";

        if (actions != null)
        {
            foreach (var action in actions)
            {
                string actionDesc = "[NULL]";
                if (action.action != null) actionDesc = action.action.GetRawDescription("  ");
                desc += $" At {action.delay} seconds, {actionDesc}\n";
            }
        }

        return desc;
    }

    protected abstract string GetRawDescription();

    protected bool EvaluatePreconditions()
    {
        if (preConditions == null) return true;
        if (!hasPreconditions) return true;

        foreach (var condition in preConditions)
        {
            if (!condition.Evaluate(gameObject)) return false;
        }

        return true;
    }

    public virtual void ExecuteTrigger()
    {
        if (!enableTrigger) return;
        if ((!allowRetrigger) && (alreadyTriggered)) return;

        foreach (var action in actions)
        {
            if (action.action.isActionEnabled)
            {
                if (action.delay > 0)
                {
                    StartCoroutine(ExecuteTriggerCR(action));
                }
                else
                {
                    action.action.Execute();
                }
            }
        }

        alreadyTriggered = true;
    }

    IEnumerator ExecuteTriggerCR(ActionTrigger action)
    {
        yield return new WaitForSeconds(action.delay);

        action.action.Execute();
    }
}
