using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionTagged : Action
{
    public enum SearchType { Global = 0, Tagged = 1, Children = 2 };
    public enum TriggerType { All = 0, Sequence = 1, Random = 2 };

    [SerializeField] 
    private SearchType  searchType = SearchType.Global;
    [SerializeField, ShowIf("needSearchTags")] 
    private Hypertag[]  searchTags;
    [SerializeField, ShowIf("needSearchTags")]
    private TriggerType triggerType = TriggerType.All;
    [SerializeField] 
    private Hypertag[] triggerTags;

    private bool needSearchTags => searchType == SearchType.Tagged;

    private int sequenceIndex = 0;

    public override string GetActionTitle() => "Execute Tagged Action";

    public override string GetRawDescription(string ident, GameObject gameObject)
    {
        var desc = GetPreconditionsString(gameObject);

        if (searchType == SearchType.Global)
        {
            desc = "find actions tagged with any of ["; ;
        }
        else if (searchType == SearchType.Children)
        {
            desc = "find actions underneath this object tagged with any of ["; ;
        }
        else if (searchType == SearchType.Tagged)
        {
            desc = "find all objects tagged with any of [";

            if ((searchTags != null) && (searchTags.Length > 0))
            {
                for (int i = 0; i < searchTags.Length; i++)
                {
                    desc += searchTags[i].name;
                    if (i < searchTags.Length - 1) desc += ",";
                }
            }
            else desc += "UNDEFINED";

            desc += "] and in them find actions tagged with any of ["; ;
        }
        
        if ((triggerTags != null) && (triggerTags.Length > 0))
        {
            for (int i = 0; i < triggerTags.Length; i++)
            {
                desc += triggerTags[i].name;
                if (i < triggerTags.Length - 1) desc += ",";
            }
        }
        else desc += "MISSING";

        switch (triggerType)
        {
            case TriggerType.All:
                desc += "] and execute all of them.";
                break;
            case TriggerType.Sequence:
                desc += "] and execute each of them in sequence each time this action is executed.";
                break;
            case TriggerType.Random:
                desc += "] and choose one randomly to execute.";
                break;
            default:
                break;
        }

        return desc;
    }

    List<Action> targetActions = null;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if ((targetActions == null) || (triggerType != TriggerType.Sequence))
        {
            RefreshActions();
        }

        if (targetActions != null)
        {
            if ((searchType == SearchType.Global) ||
                (triggerType == TriggerType.All))
            {
                foreach (var action in targetActions)
                {
                    action.Execute();
                }
            }
            else if (triggerType == TriggerType.Random)
            {
                targetActions.RemoveAll((t) => (t == null) || (!t.isActionEnabled));
                if (targetActions.Count > 0)
                {
                    int r = Random.Range(0, targetActions.Count);
                    targetActions[r].Execute();
                }
            }
            else if (triggerType == TriggerType.Sequence)
            {
                bool actionDone = false;
                bool restarted = false;
                while (!actionDone)
                {
                    var action = targetActions[sequenceIndex];
                    if ((action) && (action.isActionEnabled))
                    {
                        action.Execute();
                        actionDone = true;
                    }
                    sequenceIndex++;
                    if (sequenceIndex >= targetActions.Count)
                    {
                        if (restarted)
                        {
                            // Couldn't find a valid action
                            break;
                        }
                        else
                        {
                            RefreshActions();
                            restarted = true;
                        }
                    }
                }
            }
        }
    }

    void RefreshActions()
    {
        if (searchType == SearchType.Global)
        {
            targetActions = new List<Action>(GameObject.FindObjectsOfType<Action>());

            targetActions.RemoveAll((action) => !action.HasTag(triggerTags));
        }
        else if (searchType == SearchType.Children)
        {
            targetActions = new List<Action>(GetComponentsInChildren<Action>());

            targetActions.RemoveAll((action) => !action.HasTag(triggerTags));
        }
        else
        {
            targetActions = new List<Action>();

            var objects = HypertaggedObject.FindGameObjectsWithHypertag(searchTags);
            if (objects != null)
            {
                foreach (var obj in objects)
                {
                    var actions = obj.GetComponentsInChildren<Action>();
                    foreach (var action in actions)
                    {
                        if (action.isActionEnabled)
                        {
                            if (action.HasTag(triggerTags))
                            {
                                if (targetActions.IndexOf(action) == -1)
                                {
                                    targetActions.Add(action);
                                }
                            }
                        }
                    }
                }
            }
        }

        sequenceIndex = 0;
    }
}
