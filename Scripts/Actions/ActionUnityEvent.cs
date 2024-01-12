using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Run Unity Event")]
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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (unityEvent.GetPersistentEventCount() == 0)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No Unity event declared!"));
            }
            else
            {
                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                {
                    if (unityEvent.GetPersistentTarget(i) == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"No receiver for Unity event on slot {i}!"));
                    }
                    else if (unityEvent.GetPersistentMethodName(i) == "")
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"No function for Unity event on slot {i}!"));
                    }
                }
            }
        }
    }
}