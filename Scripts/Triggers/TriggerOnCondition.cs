using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Trigger/On Condition")]
    public class TriggerOnCondition : Trigger
    {
        [SerializeField] private bool               continuous = false;
        [SerializeField] private Condition[]        conditions;
        [SerializeField] protected ActionTrigger[]  elseActions;

        private bool firstTime = true;
        private bool prevValue = false;

        public override string GetTriggerTitle() => "On Condition";

        protected override string Internal_UpdateExplanation()
        {
            base.Internal_UpdateExplanation();

            if ((elseActions != null) && (elseActions.Length > 0))
            {
                _explanation += "else\n" + GetDescriptionActions(elseActions);
            }

            if (continuous)
            {
                _explanation += "\nThis trigger will execute every single frame when the conditions are true.";
            }
            else
            {
                _explanation += "\nThis trigger will execute only when the conditions change and they are all true.";
            }

            return _explanation;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "When ";
            if ((conditions == null) || (conditions.Length == 0))
            {
                return "when [No conditions set]!";
            }
            for (int i = 0; i < conditions.Length; i++)
            {
                desc += conditions[i].GetRawDescription(gameObject);
                if (i < conditions.Length - 1) desc = desc + " and ";
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((conditions == null) || (conditions.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No conditions set!", "If there's no conditions, this trigger will be executed every frame, and if that's the desired option, it's better to use a OnEveryFrame trigger."));
            }
            else
            {
                var condLogs = new List<LogEntry>();
                int index = 0;
                foreach (var condition in conditions)
                {
                    condLogs.Clear();
                    condition.CheckErrors(gameObject, condLogs);
                    foreach (var l in condLogs)
                    {
                        _logs.Add(new LogEntry(l.type, $"Condition {index}: {l.text}", l.tooltip));
                    }
                    index++;
                }
            }
        }

        private void Update()
        {
            if (!isTriggerEnabled) return;
            if (!EvaluatePreconditions()) return;

            bool b = true;

            foreach (var condition in conditions)
            {
                b &= condition.Evaluate(gameObject);
                if (!b) break;
            }

            if (b)
            {
                if (firstTime) ExecuteTrigger();
                else if ((!prevValue) && (!continuous)) ExecuteTrigger();
                else ExecuteTrigger();
            }
            else
            {
                if (firstTime) ExecuteElseTrigger();
                else if ((prevValue) && (!continuous)) ExecuteElseTrigger();
                else ExecuteElseTrigger();
            }

            prevValue = b;
            firstTime = false;
        }

        public virtual void ExecuteElseTrigger()
        {
            if (!enableTrigger) return;

            foreach (var action in elseActions)
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
        }
    }
}