using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OkapiKit
{
    public class ActionUnityEvent : Action
    {
        [SerializeField] private UnityEvent unityEvent;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            unityEvent?.Invoke();
        }

        public override string GetActionTitle() => "Unity Event";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            return $"{GetPreconditionsString(gameObject)}execute Unity event";
        }
    }
}