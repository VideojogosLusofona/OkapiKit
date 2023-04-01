using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class TriggerOnEveryFrame : Trigger
    {
        public override string GetTriggerTitle() => "Every frame";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "Every frame";
        }

        private void Update()
        {
            if (!isTriggerEnabled) return;
            if (!EvaluatePreconditions()) return;

            ExecuteTrigger();
        }
    }
}