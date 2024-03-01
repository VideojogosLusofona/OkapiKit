using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Movement/Follow Movement")]
    public class MovementFollow : Movement
    {
        public enum TargetType { Tag = 0, Object = 1, Mouse = 2 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField] private float speed = 200.0f;
        [SerializeField] private TargetType targetType;
        [SerializeField] private Hypertag targetTag;
        [SerializeField] private Transform targetObject;
        [SerializeField] private float stoppingDistance = 0;
        [SerializeField] private bool relativeMovement = false;
        [SerializeField] private bool rotateTowardsDirection = false;
        [SerializeField] private Axis alignAxis = Axis.UpAxis;
        [SerializeField] private float maxRotationSpeed = 360.0f;
        [SerializeField] private Camera cameraObject;
        [SerializeField] private Hypertag cameraTag;

        Transform prevTransform;
        Vector3 offset;

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

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (targetType == TargetType.Tag)
            {
                if (targetTag == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target tag not defined!", "You want to follow an object with a specific tag, so you have to define which tag."));
                }
            }
            else if (targetType == TargetType.Object)
            {
                if (targetObject == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Target object not defined!", "You want to follow an specific object, so you have to define which one.\nNote that the prefered method is to follow a tag, not an object, which is more error prone."));
                }
            }
            else if (targetType == TargetType.Mouse)
            {
                if ((cameraObject == null) && (cameraTag == null))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Camera not defined - use either tag or drag the camera object to the slot below!", "Mouse cursor depends on a viewport on the screen, that's why we need to know what camera is the main one, either by using a tag, or a reference."));
                }
            }
        }

        protected override void Start()
        {
            base.Start();
            prevTransform = null;
        }

        void FixedUpdate()
        {
            if (!isMovementActive()) return;

            Vector3 target = Vector3.zero;
            Vector3 delta = Vector3.zero;

            if (targetType != TargetType.Mouse)
            {
                Transform targetTransform = null;

                if ((targetTag) && (targetType == TargetType.Tag))
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
                else if ((targetObject) && (targetType == TargetType.Object))
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
            else
            {
                StopMovement();
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
}