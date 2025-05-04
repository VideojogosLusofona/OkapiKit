using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Flash")]
    public class ActionFlashV2 : ActionEffect
    {
        public enum Mode { ColorFlash, InvertColor, SmoothInvert };

        [SerializeField] private Mode       mode = Mode.ColorFlash;
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

            switch (mode)
            {
                case Mode.ColorFlash:
                    target.FlashColor(duration, color);
                    break;
                case Mode.InvertColor:
                    target.FlashInvert(duration);
                    break;
                case Mode.SmoothInvert:
                    target.SmoothInvert(duration);
                    break;
                default:
                    break;
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = GetPreconditionsString(gameObject);

            switch (mode)
            {
                case Mode.ColorFlash:
                    desc += $"flashes renderer {targetRenderer.GetRawDescription("target", gameObject)} with a color for {duration} seconds";
                    break;
                case Mode.InvertColor:
                    desc += $"flashes renderer {targetRenderer.GetRawDescription("target", gameObject)}, inverting the colors for {duration} seconds";
                    break;
                case Mode.SmoothInvert:
                    desc += $"smoothly inverts colors in and out, on renderer {targetRenderer.GetRawDescription("target", gameObject)}, during {duration} seconds";
                    break;
                default:
                    break;
            }
            

            return desc;
        }

        public override string GetActionTitle() 
        {
            switch (mode)
            {
                case Mode.ColorFlash:
                    return "Flash";
                case Mode.InvertColor:
                    return "Invert Colors";
                case Mode.SmoothInvert:
                    return "Invert Colors";
                default:
                    break;
            }
            return "[UNDEFINED]"; 
        }
    }
}
