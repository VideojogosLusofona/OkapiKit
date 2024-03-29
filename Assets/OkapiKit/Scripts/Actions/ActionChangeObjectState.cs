using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Object State")]
    public class ActionChangeObjectState : Action
    {
        [SerializeField] private enum StateChange { Enable, Disable, Toggle };

        [SerializeField] private GameObject target;
        [SerializeField] private StateChange state;

        public override string GetActionTitle() => "Change Object State";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            switch (state)
            {
                case StateChange.Enable:
                    desc += (target) ? ($"enables object {target.name}") : ("enables this object");
                    break;
                case StateChange.Disable:
                    desc += (target) ? ($"disables object {target.name}") : ("disables this object");
                    break;
                case StateChange.Toggle:
                    desc += (target) ? ($"toggles object {target.name}") : ("toggles this object");
                    break;
            }
            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined target object - object will change its own state", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            GameObject go = target;
            if (go == null) go = gameObject;

            switch (state)
            {
                case StateChange.Enable:
                    go.SetActive(true);
                    break;
                case StateChange.Disable:
                    go.SetActive(false);
                    break;
                case StateChange.Toggle:
                    go.SetActive(!go.activeSelf);
                    break;
                default:
                    break;
            }
        }
    }
}