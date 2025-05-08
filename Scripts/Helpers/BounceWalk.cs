using UnityEngine;

namespace OkapiKit
{
    public class BounceWalk : OkapiElement
    {
        [SerializeField] private TargetTransform    target;
        [SerializeField] private float              stepTime = 0.1f;
        [SerializeField] private float              stepHeight = 5.0f;
        [SerializeField] private float              teleportDistance = 50.0f;

        Vector3     basePos;
        Vector3     prevPos;
        float       angle;
        float       prevAngularSpeed;
        Transform   targetTransform;

        void Start()
        {
            prevPos = transform.position;

            targetTransform = target.GetTarget(gameObject);
            basePos = targetTransform?.localPosition ?? Vector3.zero;
        }

        void FixedUpdate()
        {
            if (targetTransform == null) return;

            Vector3 deltaPos = transform.position.xy0() - prevPos.xy0();
            float   distance = deltaPos.magnitude;
            
            if (distance > teleportDistance)
            {
                prevPos = transform.position;
                angle = 0.0f;
            }
            else
            {
                if (distance < 1e-3)
                {
                    // Stopped - return to rest
                    if ((angle > 0.0f) && (angle < Mathf.PI * 0.5f))
                    {
                        angle -= prevAngularSpeed;
                        if (angle < 0.0f) angle = 0.0f;
                    }
                    else if (angle >= Mathf.PI * 0.5f)
                    {
                        angle += prevAngularSpeed;
                        if (angle > Mathf.PI) angle = 0.0f;
                    }
                }
                else
                {
                    prevAngularSpeed = Time.fixedDeltaTime * Mathf.PI / stepTime;
                    angle += prevAngularSpeed;
                    if (angle > Mathf.PI) angle -= Mathf.PI;
                }
            }

            targetTransform.localPosition = basePos + Vector3.up * Mathf.Sin(angle) * stepHeight;
            prevPos = transform.position;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";

            string targetName = target != null
                ? target.GetRawDescription("target", refObject)
                : "no target selected";

            desc += $"Applies a vertical bounce to {targetName} based on this object's movement.";

            desc += $" One bounce cycle takes {stepTime:0.###} seconds and reaches a height of {stepHeight:0.##} units.";

            if (teleportDistance > 0f)
            {
                desc += $" If the object teleports more than {teleportDistance:0.##} units, the bounce resets.";
            }

            return desc;
        }
    }
}
