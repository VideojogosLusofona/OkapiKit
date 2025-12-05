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

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (unityEvent.GetPersistentEventCount() == 0)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No Unity event declared!", "If we're using this, we want to call an event (which is just a functional call to some Unity script).\nIf you don't define anything, this does nothing."));
            }
            else
            {
                for (int i = 0; i < unityEvent.GetPersistentEventCount(); i++)
                {
                    if (unityEvent.GetPersistentTarget(i) == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"No receiver for Unity event on slot {i}!", "If you want to call a function on an object, you need to define which object."));
                    }
                    else if (unityEvent.GetPersistentMethodName(i) == "")
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"No function for Unity event on slot {i}!", "An object is selected as target for the event, but no function/event is selected."));
                    }
                }
            }
        }
    }
}