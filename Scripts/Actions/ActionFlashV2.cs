using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Flash")]
    public class ActionFlashV2 : ActionEffect
    {
        [SerializeField] private Gradient   color;
        [SerializeField] private float      duration = 1.0f;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var target = GetSpriteEffect();
            if (target == null)
            {
                Debug.LogWarning("Can't find target renderer for flash!");
                return;
            }

            target.FlashColor(duration, color);
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = GetPreconditionsString(gameObject);
            
            desc += $"flashes renderer {targetRenderer.GetRawDescription("target", gameObject)} for {duration} seconds";

            return desc;
        }
    }
}
