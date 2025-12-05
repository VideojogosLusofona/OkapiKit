using UnityEngine;

namespace OkapiKit
{
    public abstract class ActionEffect : Action
    {
        [SerializeField]
        protected TargetRenderer    targetRenderer;

        SpriteEffect effect;

        protected override void CheckErrors(int level)
        {
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            targetRenderer.CheckErrors(_logs, "renderer", gameObject);
        }

        protected SpriteEffect GetSpriteEffect()
        {
            if ((effect == null) || (targetRenderer.isDynamic))
            {
                var target = targetRenderer.GetTarget(gameObject);
                if (target != null)
                {
                    effect = target.GetComponent<SpriteEffect>();
                    if (effect == null)
                    {
                        // Need a SpriteEffect to be able to make effects
                        effect = target.gameObject.AddComponent<SpriteEffect>();
                    }
                }
            }

            return effect;
        }
    }
}
