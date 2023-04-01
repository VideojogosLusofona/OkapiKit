using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionModifyRigidBody : Action
    {
        public enum ChangeType { SetBodyType = 0, Mass = 1, LinearDrag = 2, AngularDrag = 3, GravityScale = 4 };

        [SerializeField]
        private ChangeType changeType;
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

            Rigidbody2D rb = GetComponent<Rigidbody2D>();
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

        public override string GetActionTitle() => "Modify Rigid Body";
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
    }
}