using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Rigid Body")]
    public class ActionChangeRigidBody : Action
    {
        public enum ChangeType { SetBodyType = 0, Mass = 1, LinearDrag = 2, AngularDrag = 3, GravityScale = 4 };

        [SerializeField]
        private Rigidbody2D     target;
        [SerializeField]
        private ChangeType      changeType;
        [SerializeField, ShowIf("needsBodyType")]
        private RigidbodyType2D bodyType;
        [SerializeField, ShowIf("needValue")]
        private float value;

        private bool needsBodyType => (changeType == ChangeType.SetBodyType);
        private bool needValue => (changeType == ChangeType.Mass) || (changeType == ChangeType.LinearDrag) || (changeType == ChangeType.AngularDrag) || (changeType == ChangeType.GravityScale);

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            Rigidbody2D rb = target;
            if (rb == null) rb = GetComponent<Rigidbody2D>();
            if (rb == null) return;

            switch (changeType)
            {
                case ChangeType.SetBodyType:
                    rb.bodyType = bodyType;
                    break;
                case ChangeType.Mass:
                    rb.mass = value;
                    break;
                case ChangeType.LinearDrag:
                    rb.drag = value;
                    break;
                case ChangeType.AngularDrag:
                    rb.angularDrag = value;
                    break;
                case ChangeType.GravityScale:
                    rb.gravityScale = value;
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Change Rigid Body";
        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);
            switch (changeType)
            {
                case ChangeType.SetBodyType:
                    desc += $"changes body type of this object to {bodyType}";
                    break;
                case ChangeType.Mass:
                    desc += $"changes mass of this object to {value}";
                    break;
                case ChangeType.LinearDrag:
                    desc += $"changes linear drag of this object to {value}";
                    break;
                case ChangeType.AngularDrag:
                    desc += $"changes angular drag of this object to {value}";
                    break;
                case ChangeType.GravityScale:
                    desc += $"changes gravity scale of this object to {value}";
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (target == null)
            {
                if (GetComponent<Rigidbody2D>() == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Undefined target rigid body", "We're changing something on a rigid body, so we need a target so we know which rigid bodyto change."));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Rigid body to modify is on this object, but it should be explicitly linked!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                }
            }
        }
    }
}