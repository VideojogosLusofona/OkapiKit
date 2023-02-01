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

    [SerializeField, HideInInspector]
    protected   bool            _showInfo = true;
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

    public bool showInfo
    {
        get { return _showInfo; }
        set { _showInfo = value; }
    }
    public bool isTriggerEnabled
    {
        get { return enableTrigger; }
        set { enableTrigger = value; }
    }

    public virtual string GetTriggerTitle() { return "Trigger"; }

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
                    desc += preConditions[i].GetRawDescription(gameObject) + " and ";
                }
            }
        }

        desc += GetRawDescription() + ":\n";

        if (actions != null)
        {
            List<ActionTrigger> sortedActions = new List<ActionTrigger>(actions);
            sortedActions.Sort((e1, e2) => (e1.delay == e2.delay)?(0): ((e1.delay < e2.delay) ? (-1):(1)));

            float lastTime = -float.MaxValue;
            for (int i = 0; i < sortedActions.Count; i++)
            {
                var action = sortedActions[i];
                string actionDesc = "[NULL]";
                string timeString = $" At {action.delay} seconds, \n";

                if (action.delay == 0)
                {
                    timeString = $" First, \n";
                }
                else
                {
                    if (i != 0) timeString = $" then, at {action.delay} seconds, \n";
                    else timeString = $" At {action.delay} seconds, \n";
                }

                string spaces = "";

                for (int k = 0; k < 10; k++) spaces += " ";

                if (lastTime == action.delay)
                {
                    timeString = spaces;
                }
                else
                {
                    timeString += spaces;
                }

                if (action.action != null)
                {
                    actionDesc = action.action.GetRawDescription("  ", gameObject);
                    actionDesc = actionDesc.Replace("\n", "\n" + spaces);
                }
                
                desc += $"{timeString}{actionDesc}\n";

                lastTime = action.delay;
            }
        }

        return desc;
    }

    public void UpdateExplanation()
    {
        _explanation = GetDescription();
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
