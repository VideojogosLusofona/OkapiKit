using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Action_Tagged : Action
{
    [SerializeField] private Hypertag[] triggerTags;

    public override string GetRawDescription(string ident)
    {
        var desc = "Find actions tagged with any of [";
        for (int i = 0; i < triggerTags.Length; i++)
        {
            desc += triggerTags[i].name;
            if (i < triggerTags.Length - 1) desc += ",";
        }
        desc += "] and execute them.";
        return desc;
    }

    public override void Execute()
    {
        if (!enableAction) return;
        
        var allActions = GameObject.FindObjectsOfType<Action>();
        foreach (var action in allActions) 
        {
            if (action.isActionEnabled)
            {
                if (action.HasTag(triggerTags))
                {
                    action.Execute();
                }
            }
        }
    }
}
