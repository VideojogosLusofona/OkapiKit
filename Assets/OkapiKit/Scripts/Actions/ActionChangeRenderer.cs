using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Renderer")]
    public class ActionChangeRenderer : Action
    {
        public enum ChangeType { Visibility = 0 };

        public enum BoolChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField]
        new private Renderer renderer;
        [SerializeField]
        private ChangeType changeType;
        [SerializeField, ShowIf("needVisibility")]
        private BoolChange visibility;

        private bool needVisibility => (changeType == ChangeType.Visibility);

        public override string GetActionTitle() { return "Change Renderer"; }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (renderer == null)
            {
                renderer = GetComponent<Renderer>();
            }
            if (renderer == null) return;

            switch (changeType)
            {
                case ChangeType.Visibility:
                    switch (visibility)
                    {
                        case BoolChange.Enable:
                            renderer.enabled = true;
                            break;
                        case BoolChange.Disable:
                            renderer.enabled = false;
                            break;
                        case BoolChange.Toggle:
                            renderer.enabled = !renderer.enabled;
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    break;
            }
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);
            var rendererName = (renderer) ? (renderer.name) : ("this");
            switch (changeType)
            {
                case ChangeType.Visibility:
                    switch (visibility)
                    {
                        case BoolChange.Enable:
                            desc += (renderer) ? ($"enables renderer {rendererName}") : ("enables this renderer");
                            break;
                        case BoolChange.Disable:
                            desc += (renderer) ? ($"disables renderer {rendererName}") : ("disables this renderer");
                            break;
                        case BoolChange.Toggle:
                            desc += (renderer) ? ($"toggles renderer {rendererName}") : ("toggles this renderer");
                            break;
                        default:
                            break;
                    }

                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (renderer == null)
            {
                if (GetComponent<Renderer>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target renderer", "We're changing something on a renderer, so we need a target so we know which renderer to change."));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Renderer to modify is this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
        }
    }
}