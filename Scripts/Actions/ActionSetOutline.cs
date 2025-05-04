using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Set Outline")]
    public class ActionSetOutline : ActionEffect
    {
        [SerializeField] private Color      color = Color.black;
        [SerializeField] private float      thickness = 1.0f;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var target = GetSpriteEffect();
            if (target == null)
            {
                Debug.LogWarning("Can't find target renderer for set outline!");
                return;
            }


            if (thickness > 0.0f)
                target.SetOutline(thickness, color);
            else
                target.SetOutline(0.0f, color);
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = GetPreconditionsString(gameObject);
            
            if (thickness > 0.0f)
                desc += $"sets outline of {targetRenderer.GetRawDescription("target", gameObject)} to {thickness} units.";
            else
                desc += $"disables outline of {targetRenderer.GetRawDescription("target", gameObject)}.";

            return desc;
        }

        public override string GetActionTitle() { return "Set Outline"; }
    }
}
