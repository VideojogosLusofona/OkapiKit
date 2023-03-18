using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionSequence : Action
{
    [SerializeField]
    protected ActionTrigger[] actions;

    public override void Execute()
    {
        if (!enableAction) return;
        if (!EvaluatePreconditions()) return;

        if (actions == null) return;

        foreach (var action in actions)
        {
            if (action.delay > 0)
            {
                StartCoroutine(ExecuteCR(action));
            }
            else
            {
                action.action.Execute();
            }
        }
    }

    IEnumerator ExecuteCR(ActionTrigger action)
    {
        yield return new WaitForSeconds(action.delay);

        action.action.Execute();
    }

    public override string GetActionTitle() => "Sequence";

    public override string GetRawDescription(string ident, GameObject gameObject)
    {
        string desc = GetPreconditionsString(gameObject);

        if (actions != null)
        {
            List<ActionTrigger> sortedActions = new List<ActionTrigger>(actions);
            sortedActions.Sort((e1, e2) => (e1.delay == e2.delay) ? (0) : ((e1.delay < e2.delay) ? (-1) : (1)));

            float lastTime = -float.MaxValue;
            for (int i = 0; i < sortedActions.Count; i++)
            {
                var action = sortedActions[i];
                string actionDesc = "[NULL]";
                string timeString = $" At {action.delay} seconds, \n";

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
}
