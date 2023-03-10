using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementFollow : Movement
{
    public enum TargetType { Tag = 0, Object = 1, Mouse = 2 };
    public enum Axis { UpAxis = 0, RightAxis = 1 };

    [SerializeField] private float      speed = 200.0f;
    [SerializeField] private TargetType targetType;
    [SerializeField] private Hypertag   targetTag;
    [SerializeField] private Transform  targetObject;
    [SerializeField] private float      stoppingDistance = 0;
    [SerializeField] private bool       relativeMovement = false;
    [SerializeField] private bool       rotateTowardsDirection = false;
    [SerializeField] private Axis       alignAxis = Axis.UpAxis;
    [SerializeField] private float      maxRotationSpeed = 360.0f;
    [SerializeField] private Camera     cameraObject;
    [SerializeField] private Hypertag   cameraTag;

    Transform   prevTransform;
    Vector3     offset;

    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    override public string GetTitle() => "Follow";

    public override string GetRawDescription(string ident, GameObject refObject)
    {
        string desc = "";
        string speedDesc = "";

        if (speed != 0.0f)
        {
            if ((stoppingDistance > 0) && (!relativeMovement))
                speedDesc = $", at {speed} units per second, up to a distance of {stoppingDistance} units.\n";
            else
                speedDesc = $", at {speed} units per second.\n";
        }
        else
        {
            speedDesc += ", immediately.\n";
        }

        if (targetType == TargetType.Tag)
        {
            if (targetTag != null)
                desc += $"Follows the closest object tagged with [{targetTag.name}]{speedDesc}";
            else
                desc += $"Follows the closest object tagged with [UNDEFINED]{speedDesc}";
        }
        else if (targetType == TargetType.Object)
        {
            if (targetObject != null)
                desc += $"Follows the {targetObject.name} object{speedDesc}";
            else
                desc += $"Follows the [UNDEFINED] object{speedDesc}";
        }
        else if (targetType == TargetType.Mouse)
        {
            desc += $"Follows the mouse{speedDesc}";
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
        Vector3     target = Vector3.zero;
        Vector3     delta = Vector3.zero;

        if (targetType != TargetType.Mouse)
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
                    if (relativeMovement)
                    {
                        offset = transform.position - targetTransform.position;
                    }
                    else
                    {
                        offset = Vector3.zero;
                    }
                }

                target = targetTransform.position + offset;

                if (speed == 0.0f)
                {
                    delta = target - transform.position;
                }
                else
                {
                    delta = Vector3.MoveTowards(transform.position, target, Time.fixedDeltaTime * speed) - transform.position;
                }
            }

            prevTransform = targetTransform;
        }
        else
        {
            Camera camera = GetCamera();
            if (camera != null)
            {
                Vector3 mp = camera.ScreenToWorldPoint(Input.mousePosition);
                mp.z = transform.position.z;
                if (speed == 0.0f)
                    delta = mp - transform.position;
                else
                    delta = Vector3.MoveTowards(transform.position, mp, Time.fixedDeltaTime * speed) - transform.position;
            }
        }

        if ((Vector3.Distance(transform.position, target) > stoppingDistance) || (speed == 0.0f) || (relativeMovement))
        {
            MoveDelta(delta);
        }

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

    Camera GetCamera()
    {
        if (cameraTag)
        {
            return gameObject.FindObjectOfTypeWithHypertag<Camera>(cameraTag);
        }

        return cameraObject;
    }
}
