using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static OkapiKit.MovementRotate;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Rotate Towards")]
    public class ActionRotateTowards : Action
    {
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField]
        private bool        hasMaxSpeed = false;
        [SerializeField]
        private float       speed = 200.0f;
        [SerializeField]
        private Axis        axisToAlign = Axis.UpAxis;
        [SerializeField]
        private Transform   targetObject;
        [SerializeField]
        private Hypertag    targetTag;

        Rigidbody2D rb;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            Transform targetTransform = null;

            if (targetTag)
            {
                var potentialObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(targetTag);
                var minDist = float.MaxValue;
                foreach (var obj in potentialObjects)
                {
                    var d = Vector3.Distance(obj.position, transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        targetTransform = obj;
                    }
                }
            }
            else if (targetObject)
            {
                targetTransform = targetObject;
            }

            if (targetTransform)
            {
                Vector2 dir = (targetTransform.position - transform.position);
                if (dir.sqrMagnitude > 1e-6)
                {
                    dir.Normalize();

                    if (axisToAlign == Axis.RightAxis) dir = new Vector2(-dir.y, dir.x);

                    if (hasMaxSpeed)
                    {
                        RotateTo(dir, speed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        RotateTo(dir, float.MaxValue);
                    }
                }
            }
        }

        protected void RotateTo(Vector2 upDir, float maxAngle)
        {
            if ((rb != null) && (!rb.freezeRotation))
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);

                rb.MoveRotation(newRotation);
            }
            else
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);
            }
        }

        public override string GetActionTitle() => "Rotate Towards";
        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            string axisName = (axisToAlign == Axis.UpAxis) ? ("up") : ("right");


            if (targetObject)
            {
                desc += $"This object will align its {axisName} axis with the direction towards {targetObject.name}";
            }
            else if (targetTag)
            {
                desc += $"This object will align its {axisName} axis with the direction towards the closest object tagged with {targetTag.name}";
            }
            else
            {
                desc += $"This object will align its {axisName} axis with the direction towards [UNDEFINED]";
            }
            if (hasMaxSpeed)
            {
                desc += $", at a maximum {speed} degrees per second.";
            }
            else
            {
                desc += ".";
            }

            return desc;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if ((targetObject == null) && (targetTag == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No target defined!", "We want to rotate the object towards something, but we didn't define what"));
            }
        }

        protected override void Awake()
        {
            base.Awake();

            rb = GetComponent<Rigidbody2D>();
        }
    }
}