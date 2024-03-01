using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Collider")]
    public class ActionChangeCollider : Action
    {
        public enum State { IsTrigger = 0 };
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField] private Collider2D     target;
        [SerializeField] private State          state;
        [SerializeField] private StateChange    changeState;

        public override string GetActionTitle() => "Change Collider";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetDesc = (target) ? (target.name) : ("[undefined]");
            switch (state)
            {
                case State.IsTrigger:
                    {
                        switch (changeState)
                        {
                            case StateChange.Enable:
                                desc += $"enables IsTrigger flag of collider {targetDesc}";
                                break;
                            case StateChange.Disable:
                                desc += $"disables IsTrigger flag of collider {targetDesc}";
                                break;
                            case StateChange.Toggle:
                                desc += $"toggles IsTrigger flag of collider {targetDesc}";
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target collider", "You need to define which collider you want to change the state of!"));
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (target == null) return;
            if (!EvaluatePreconditions()) return;

            switch (state)
            {
                case State.IsTrigger:
                    {
                        switch (changeState)
                        {
                            case StateChange.Enable:
                                target.isTrigger = true;
                                break;
                            case StateChange.Disable:
                                target.isTrigger = false;
                                break;
                            case StateChange.Toggle:
                                target.isTrigger = !target.isTrigger;
                                break;
                            default:
                                break;
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
}