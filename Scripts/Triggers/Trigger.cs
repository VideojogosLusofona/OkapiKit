using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using NaughtyAttributes;

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
    protected   ActionTrigger[] actions;

    [SerializeField, ResizableTextArea, ReadOnly]
    private string _explanation;

    [Button("Update Explanation")]
    private void UpdateExplanation()
    {
        _explanation = GetDescription();
    }

    public string GetDescription()
    {
        string desc = "";
        if (description != "") desc += description + "\n----------------\n";

        desc += GetRawDescription() + ":\n";

        foreach (var action in actions)
        {
            string actionDesc = "[NULL]";
            if (action.action != null) actionDesc = action.action.GetRawDescription("  ");
            desc += $"  At {action.delay} seconds, {actionDesc}\n";
        }

        return desc;
    }

    protected abstract string GetRawDescription();

    public virtual void ExecuteTrigger()
    {
        if (!enableTrigger) return;

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

        if (!allowRetrigger)
        {
            Destroy(this);
        }
    }

    IEnumerator ExecuteTriggerCR(ActionTrigger action)
    {
        yield return new WaitForSeconds(action.delay);

        action.action.Execute();
    }
}
