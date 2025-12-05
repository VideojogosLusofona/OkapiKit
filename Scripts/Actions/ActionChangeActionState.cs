using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Action State")]
    public class ActionChangeActionState : Action
    {
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField] private Action target;
        [SerializeField] private StateChange state;

        public override string GetActionTitle() => "Change Action State";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetDesc = (target) ? (target.GetRawDescription(ident, gameObject)) : ("UNKNOWN");
            switch (state)
            {
                case StateChange.Enable:
                    desc += $"enables action [{targetDesc}]";
                    break;
                case StateChange.Disable:
                    desc += $"disables action [{targetDesc}]";
                    break;
                case StateChange.Toggle:
                    desc += $"toggles action [{targetDesc}]";
                    break;
            }
            return desc;
        }

        protected override void CheckErrors(int level)
        {
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (target == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target action", "You need to define which action you want to change the state of!"));
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (target == null) return;
            if (!EvaluatePreconditions()) return;

            switch (state)
            {
                case StateChange.Enable:
                    target.isActionEnabled = true;
                    break;
                case StateChange.Disable:
                    target.isActionEnabled = false;
                    break;
                case StateChange.Toggle:
                    target.isActionEnabled = !target.isActionEnabled;
                    break;
                default:
                    break;
            }
        }
    }
}