using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

namespace OkapiKit
{
    public class ActionChangeComponentState : Action
    {
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField] private Behaviour target;
        [SerializeField] private StateChange state;

        public override string GetActionTitle() { return "Change Component State"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetDesc = "[Undefined] on [Undefined Object]";
            if (target != null)
            {
                targetDesc = $"{target} on {target.name}";
            }
            switch (state)
            {
                case StateChange.Enable:
                    desc += $"enables component {targetDesc}";
                    break;
                case StateChange.Disable:
                    desc += $"disables component {targetDesc}";
                    break;
                case StateChange.Toggle:
                    desc += $"toggles component {targetDesc}";
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
                    target.enabled = true;
                    break;
                case StateChange.Disable:
                    target.enabled = false;
                    break;
                case StateChange.Toggle:
                    target.enabled = !target.enabled;
                    break;
                default:
                    break;
            }
            return;
        }
    }
}