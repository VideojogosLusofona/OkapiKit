using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Combat Text")]
    public class ActionCombatText : Action
    {
        [SerializeField]
        private TargetTransform targetTransform;
        [SerializeField] 
        private string          text = "[UNDEFINED]";
        [SerializeField]
        private Color           color = Color.white;
        [SerializeField]
        private float           duration = 1.0f;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var t = targetTransform.GetTarget(gameObject);

            CombatTextManager.SpawnText(t.gameObject, text, color, color, duration);
        }

        public override string GetActionTitle() => "Combat Text";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            desc += $"Spawns a combat text with the text \"{text}\", at location {targetTransform.GetShortDescription(gameObject)}.";

            return desc;
        }

        protected override void CheckErrors(int level)
        {
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            targetTransform.CheckErrors(_logs, "target", gameObject);
        }
    }
};
