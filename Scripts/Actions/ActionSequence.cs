using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Sequence")]
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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((actions == null) || (actions.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No actions defined!", "If no actions are defined, this action will not do anything, even if it is executed!"));
            }
            else 
            {
                if (actions.Length == 1)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Only one action defined, no need for sequence!", "Only one action defined, no need for sequence"));
                }

                int index = 0;
                foreach (var action in actions)
                {
                    if (action.action == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Empty action in action list (index={index})!", "Empty actions don't do anything"));
                    }
                    else
                    {
                        action.action.ForceCheckErrors();
                        var actionLogs = action.action.logs;
                        foreach (var log in actionLogs)
                        {
                            _logs.Add(new LogEntry(log.type, $"On action {index}: " + log.text, log.tooltip));
                        }

                        var destroyObjectAction = action.action as ActionDestroyObject;
                        if (destroyObjectAction != null)
                        {
                            if (destroyObjectAction.WillDestroyThis(gameObject))
                            {
                                // This is a destroy action
                                if (index < actions.Length - 1)
                                {
                                    _logs.Add(new LogEntry(LogEntry.Type.Warning, $"Action on index {index} is a Destroy Object action!", "Destroy actions should always be the last actions of the sequence of action, because they will destroyu the object and stop other actions after from being executed most of the times!"));
                                }
                            }
                        }
                    }
                    index++;
                }
            }
        }
    }
}