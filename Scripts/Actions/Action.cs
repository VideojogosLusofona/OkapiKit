using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Action")]
    abstract public class Action : OkapiElement
    {
        [SerializeField]
        protected bool enableAction = true;
        [SerializeField]
        protected bool hasTags = false;
        [SerializeField, FormerlySerializedAsAttribute("tags"), ShowIf("hasTags")]
        private Hypertag[] actionTags;
        [SerializeField]
        private bool hasConditions = false;
        [SerializeField, ShowIf("hasConditions")]
        private Condition[] actionConditions;

        public bool isActionEnabled { get { return enableAction; } set { enableAction = value; } }
        public bool isTagged
        {
            get
            {
                if (hasTags)
                {
                    if (actionTags == null) return false;
                    return (actionTags.Length > 0);
                }
                return false;
            }
        }

        public Hypertag[] GetActionTags() => actionTags;

        public abstract void Execute();

        public virtual string GetActionTitle() { return "Action"; }

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

        protected string GetPreconditionsString(GameObject gameObject)
        {
            if (!hasConditions) return "";

            string desc = "";
            if ((actionConditions != null) && (actionConditions.Length > 0))
            {
                desc += "if ";
                for (int i = 0; i < actionConditions.Length; i++)
                {
                    desc += actionConditions[i].GetRawDescription(gameObject);
                    if (i < actionConditions.Length - 1) desc += " and ";
                }
                desc += " ";
            }
            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            string e = GetRawDescription("", gameObject);

            _explanation = "";
            for (int i = 0; i < e.Length; i++)
            {
                if (i != ' ')
                {
                    _explanation += char.ToUpper(e[i]) + e.Substring(i + 1);
                    break;
                }
                else
                {
                    _explanation += e[i];
                }
            }
            return _explanation;
        }

        public void ForceCheckErrors(int level)
        {
            if (level > OkapiElement.CheckErrorsMaxLevel) return;

            _logs.Clear();
            CheckErrors(level);
        }

        protected override void CheckErrors(int level)
        {
            // Max depth of 5 levels for checking errors
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (hasTags)
            {
                if ((actionTags == null) || (actionTags.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Action is flagged as tagged, but no tags defined!", "If you want to tag an action so you can call it/identify it from somewhere else, you need to assign at least one tags!"));
                }
                else
                {
                    foreach (var tag in actionTags)
                    {
                        if (tag == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, "Empty tag defined in tags list!", "If you want to tag an action so you can call it/identify it from somewhere else, you need to assign at least one tags!"));
                        }
                    }
                }
            }

            if (hasConditions)
            {
                if ((actionConditions == null) || (actionConditions.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Conditions active, but no conditions defined!", "If you want this action to only execute given some conditions, you have to define those conditions!"));
                }
                else
                {
                    var condLogs = new List<LogEntry>();
                    int index = 0;
                    foreach (var condition in actionConditions)
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
        }
    }
};