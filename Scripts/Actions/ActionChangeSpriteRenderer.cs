using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Sprite Renderer")]
    public class ActionChangeSpriteRenderer : Action
    {
        public enum ChangeType { Sprite = 0, Color = 1 };
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField]
        private SpriteRenderer target;
        [SerializeField]
        private ChangeType changeType;
        [SerializeField]
        private Sprite sprite;
        [SerializeField]
        private Color color = Color.white;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            SpriteRenderer sr = target;
            if (sr == null) sr = GetComponent<SpriteRenderer>();
            if (sr == null) return;

            switch (changeType)
            {
                case ChangeType.Sprite:
                    sr.sprite = sprite;
                    break;
                case ChangeType.Color:
                    sr.color = color;
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Change Sprite Renderer";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);

            string targetName = (target) ? (target.name) : (name);

            switch (changeType)
            {
                case ChangeType.Sprite:
                    string spriteName = (sprite) ? (sprite.name) : ("UNDEFINED");
                    desc += $"sets {targetName}'s sprite to [{spriteName}]";
                    break;
                case ChangeType.Color:
                    desc += $"sets {targetName}'s sprite color to {color}";
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                if (GetComponent<SpriteRenderer>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target sprite renderer!", "We're changing something on a renderer, so we need a target so we know which renderer to change."));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Sprite renderer to modify is this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
            if (changeType == ChangeType.Sprite)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined sprite to change sprite renderer!", "We want to change the sprite of an object, so we need to know which object (more precisely, which sprite renderer) to change"));
            }
        }
    }
}