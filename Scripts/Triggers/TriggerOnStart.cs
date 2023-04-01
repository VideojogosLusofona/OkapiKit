using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class TriggerOnStart : Trigger
    {
        public override string GetTriggerTitle() => "On Start";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "When this object is created";
        }

        private void Start()
        {
            if (!isTriggerEnabled) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }
    }
}