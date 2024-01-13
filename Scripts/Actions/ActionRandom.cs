using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Random")]
    public class ActionRandom : Action
    {
        [System.Serializable]
        public struct ActionProbability
        {
            public Action action;
            public float probability;
        }

        [SerializeField] private ActionProbability[] actions;

        public override string GetActionTitle() => "Choose Random Action";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            if ((actions == null) || (actions.Length == 0))
            {
                return $"{GetPreconditionsString(gameObject)}selects randomly between some actions, but no actions are defined!";
            }
            var desc = GetPreconditionsString(gameObject) + "select randomly between the following actions:\n";

            float total = 0;
            foreach (var action in actions)
            {
                total += action.probability;
            }
            if (total == 0) total = 1;

            for (int i = 0; i < actions.Length; i++)
            {
                if (actions[i].action != null)
                {
                    desc += $"{ident}  {i + 1}. {actions[i].action.GetRawDescription(ident + "    ", gameObject)} ({(int)(actions[i].probability * 100 / total)}%)";
                }
                else
                {
                    desc += $"{ident}  {i + 1}. {ident}    [UNDEFINED] ({(int)(actions[i].probability * 100 / total)}%)";
                }
                if (i < (actions.Length - 1)) desc += "\n";
            }

            return desc;
        }
        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((actions == null) || (actions.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No actions defined!"));
            }
            else
            {
                if (actions.Length == 1)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Only one action defined, no need for random choice!"));
                }

                int index = 0;
                foreach (var action in actions)
                {
                    if (action.action == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Empty action in action list (index={index})!"));
                    }
                    else
                    {
                        action.action.ForceCheckErrors();
                        var actionLogs = action.action.logs;
                        foreach (var log in actionLogs)
                        {
                            _logs.Add(new LogEntry(log.type, $"On action {index}: " + log.text, log.tooltip));
                        }
                    }
                    if (action.probability <= 0.0f)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Action with probability of zero or less - this will never be chosen (index={index})!"));
                    }
                    index++;
                }
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            List<ActionProbability> activeActions = new List<ActionProbability>();

            float sumProbability = 0;

            foreach (var action in actions)
            {
                if (action.action.isActionEnabled)
                {
                    sumProbability += action.probability;
                    activeActions.Add(action);
                }
            }


            float r = Random.Range(0.0f, sumProbability);
            foreach (var action in activeActions)
            {
                if (action.probability >= r)
                {
                    action.action.Execute();
                    break;
                }
                r -= action.probability;
            }
        }
    }
}