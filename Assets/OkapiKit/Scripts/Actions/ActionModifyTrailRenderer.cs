using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionModifyTrailRenderer : Action
    {
        public enum ChangeType { Emitter = 0 };
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField]
        private TrailRenderer target;
        [SerializeField]
        private ChangeType changeType;
        [SerializeField, ShowIf("isEmitter")]
        private StateChange emitter;

        private bool isEmitter => (changeType == ChangeType.Emitter);

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            TrailRenderer tr = target;
            if (tr == null) tr = GetComponent<TrailRenderer>();
            if (tr == null) return;

            switch (changeType)
            {
                case ChangeType.Emitter:
                    if (emitter == StateChange.Enable) tr.emitting = true;
                    else if (emitter == StateChange.Disable) tr.emitting = false;
                    else tr.emitting = !tr.emitting;
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Modify Trail Renderer";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);

            string targetName = (target) ? (target.name) : (name);
            switch (changeType)
            {
                case ChangeType.Emitter:
                    switch (emitter)
                    {
                        case StateChange.Enable:
                            desc += $"enables {targetName}'s trail renderer emission";
                            break;
                        case StateChange.Disable:
                            desc += $"disables {targetName}'s trail renderer emission";
                            break;
                        case StateChange.Toggle:
                            desc += $"toggles {targetName}'s trail renderer emission";
                            break;
                    }
                    break;
            }

            return desc;
        }
    }
}