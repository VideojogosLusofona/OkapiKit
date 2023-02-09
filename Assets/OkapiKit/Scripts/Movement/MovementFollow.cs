using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFollow : Movement
{
    public enum Axis { UpAxis = 0, RightAxis = 1 };

    [SerializeField] private float      speed = 200.0f;
    [SerializeField] private Hypertag   targetTag;
    [SerializeField] private Transform  targetObject;
    [SerializeField] private bool       relativeMovement = false;
    [SerializeField] private bool       rotateTowardsDirection = false;
    [SerializeField] private Axis       alignAxis = Axis.UpAxis;
    [SerializeField] private float      maxRotationSpeed = 360.0f;

    Transform   prevTransform;
    Vector3     offset;

    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    override public string GetTitle() => "Follow";

    public override string GetRawDescription()
    {
        string desc = "";
        string speedDesc = "";

        if (speed != 0.0f)
        {
            speedDesc = $", at {speed} units per second.\n";
        }
        else
        {
            speedDesc += $", although {speed} is set to zero!\n";
        }

        if (targetTag != null)
        {
            desc += $"Follows the closest object tagged with [{targetTag.name}]{speedDesc}";
        }
        else if (targetObject != null)
        {
            desc += $"Follows the {targetObject.name} object{speedDesc}";
        }
        else
        {
            desc += $"Need to set either a tag or an object to follow!";
        }
        if (relativeMovement)
        {
            desc += "The relative positions between objects will be preserved.\n";
        }
        if (rotateTowardsDirection)
        {
            string axisName = (alignAxis == Axis.UpAxis) ? ("up") : ("right");

            desc += $"This object's {axisName}'s axis will rotate towards the direction of movement, at a maximum speed of {maxRotationSpeed} degrees/sec.\n";
        }

        return desc;
    }

    protected override void Start()
    {
        base.Start();
        prevTransform = null;
    }

    void FixedUpdate()
    {
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

        if (targetTransform != null)
        {
            if (prevTransform != targetTransform)
            {
                offset = transform.position - targetTransform.position;
            }

            Vector3 delta = Vector3.MoveTowards(transform.position, targetTransform.position + offset, Time.fixedDeltaTime * speed) - transform.position;

            MoveDelta(delta);

            if (rotateTowardsDirection)
            {
                if (delta.sqrMagnitude > 1e-6)
                {
                    Vector2 dir = delta;
                    dir.Normalize();

                    if (alignAxis == Axis.RightAxis) dir = new Vector2(-dir.y, dir.x);

                    RotateTo(delta, maxRotationSpeed * Time.fixedDeltaTime);
                }
            }
        }

        prevTransform = targetTransform;
    }
}
