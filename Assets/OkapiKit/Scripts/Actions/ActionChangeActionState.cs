using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ActionChangeActionState : Action
    {
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField] private Action target;
        [SerializeField] private StateChange state;

        public override string GetActionTitle() => "Change Action State";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            switch (state)
            {
                case StateChange.Enable:
                    desc += $"enables action [{target.GetRawDescription(ident, gameObject)}]";
                    break;
                case StateChange.Disable:
                    desc += $"disables action [{target.GetRawDescription(ident, gameObject)}]";
                    break;
                case StateChange.Toggle:
                    desc += $"toggles action [{target.GetRawDescription(ident, gameObject)}]";
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