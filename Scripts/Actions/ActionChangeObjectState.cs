using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
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