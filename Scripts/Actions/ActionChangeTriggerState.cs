using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ActionChangeTriggerState : Action
    {
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField] private Trigger target;
        [SerializeField] private StateChange state;

        public override string GetActionTitle() => "Change Trigger State";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetName = (target) ? ($"{target.name}.{target.GetTriggerTitle()}") : ("UNDEFINED");

            switch (state)
            {
                case StateChange.Enable:
                    desc += $"enables trigger {targetName}";
                    break;
                case StateChange.Disable:
                    desc += $"disables trigger {targetName}";
                    break;
                case StateChange.Toggle:
                    desc += $"toggles trigger {targetName}";
                    break;
            }
            return desc;
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (target == null) return;
            if (!EvaluatePreconditions()) return;

            switch (state)
            {
                case StateChange.Enable:
                    target.isTriggerEnabled = true;
                    break;
                case StateChange.Disable:
                    target.isTriggerEnabled = false;
                    break;
                case StateChange.Toggle:
                    target.isTriggerEnabled = !target.isTriggerEnabled;
                    break;
                default:
                    break;
            }
        }
    }
}