using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Dash")]
    public class ActionDash : Action
    {
        public enum RelativeTo { Right, Up, LocalRight, LocalUp };

        [SerializeField] 
        private Transform  target;
        [SerializeField, OVNoFunction] 
        private OkapiValue speed = new OkapiValue(100.0f);
        [SerializeField] 
        private RelativeTo direction;
        [SerializeField, OVNoFunction] 
        private OkapiValue angle = new OkapiValue(0.0f);
        [SerializeField, OVNoFunction] 
        private OkapiValue duration = new OkapiValue(0.1f);

        float timer = 0;
        float actualSpeed;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;
            if (target == null) return;

            timer = duration.GetFloat(gameObject);
            actualSpeed = speed.GetFloat(gameObject);
        }

        public override string GetActionTitle() => "Dash";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            if (target)
            {
                desc += $"Moves object {target.name} ";
            }
            else
            {
                desc += "Moves this object ";
            }

            switch (direction)
            {
                case RelativeTo.Right:
                    desc += "in the right direction";
                    break;
                case RelativeTo.Up:
                    desc += "in the up direction";
                    break;
                case RelativeTo.LocalRight:
                    desc += "in the local right direction";
                    break;
                case RelativeTo.LocalUp:
                    desc += "in the local up direction";
                    break;
                default:
                    break;
            }

            desc += $", plus {angle.GetDescription()} degrees, ";
            desc += $"at a speed of {speed.GetDescription()} units/second, ";
            desc += $"for {duration.GetDescription()} seconds.";

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Transform to modify is this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected void FixedUpdate()
        {
            timer -= Time.fixedDeltaTime;
            if (timer > 0)
            {
                target.position += GetDirection() * actualSpeed * Time.fixedDeltaTime;
            }
        }

        Vector3 GetDirection()
        {
            var dir = Vector2.zero;
            switch (direction)
            {
                case RelativeTo.Right:
                    dir = Vector2.right;
                    break;
                case RelativeTo.Up:
                    dir = Vector2.up;
                    break;
                case RelativeTo.LocalRight:
                    dir = target.right;
                    break;
                case RelativeTo.LocalUp:
                    dir = target.up;
                    break;
                default:
                    break;
            }

            var newDir = Vector2.zero;
            var radAngle = Mathf.Deg2Rad * angle.GetFloat(gameObject);
            var c = Mathf.Cos(radAngle);
            var s = Mathf.Sin(radAngle);

            newDir.x = dir.x * c - dir.y * s;
            newDir.y = dir.x * s + dir.y * c;

            dir = newDir;

            return dir;
        }
    }
};
