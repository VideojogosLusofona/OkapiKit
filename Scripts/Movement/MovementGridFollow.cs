using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Movement/Follow Grid Movement")]
    public class MovementGridFollow : MovementGrid
    {
        public enum TargetType { Tag = 0, Object = 1, Mouse = 2 };
        public enum Axis { UpAxis = 0, RightAxis = 1 };

        [SerializeField] private float      speed = 200.0f;
        [SerializeField] private TargetType targetType;
        [SerializeField] private Hypertag   targetTag;
        [SerializeField] private Transform  targetObject;
        [SerializeField] private bool       relativeMovement = false;
        [SerializeField] private bool       rotateTowardsDirection = false;
        [SerializeField] private Axis       alignAxis = Axis.UpAxis;
        [SerializeField] private float      maxRotationSpeed = 360.0f;
        [SerializeField] private Camera     cameraObject;
        [SerializeField] private Hypertag   cameraTag;

        Transform prevTransform;
        Vector3 offset;

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public string GetTitle() => "Grid Follow";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            string speedDesc = "";
            float gridSpeed = speed / gridSize.x;

            if (speed != 0.0f)
            {
                speedDesc = $", at {speed} units per second ({gridSpeed} cells/sec).\n";
            }
            else
            {
                speedDesc += ", teleporting immediately to the target's position.\n";
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
            if (cooldown > 0.0f)
            {
                desc += $"The movement steps can only happen once every {cooldown} seconds.\n";
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
            if (pushStrength > 0)
            {
                desc += $"This object can push objects as long as their mass combined doesn't exceed {pushStrength}.\n";
            }
            else
            {
                desc += $"This object can not push objects.\n";
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

            if (gridObject.isMoving)
            {
                if (rotateTowardsDirection)
                {
                    if (gridObject.lastDelta.sqrMagnitude > 1e-6)
                    {
                        Vector3 upAxis = gridObject.lastDelta.normalized;

                        if (alignAxis == Axis.RightAxis) upAxis = new Vector3(-upAxis.y, upAxis.x);

                        Quaternion targetRotation = Quaternion.LookRotation(Vector3.forward, upAxis);

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.fixedDeltaTime);
                    }
                }
            }
            else if (moveCooldownTimer <= 0.0f)
            {
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

                target = transform.position + delta;
                if (speed == 0.0f)
                {
                    // Move to the target immediately
                    gridObject.TeleportTo(target);
                    moveCooldownTimer = cooldown;
                }
                else if (delta.sqrMagnitude > 1e-3)
                {
                    moveCooldownTimer = cooldown; 

                    // Given the target, see in what direction we should move
                    Vector2Int currentPos = grid.WorldToGrid(transform.position);
                    Vector2Int moveVector = Vector2Int.zero;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        // More to move in X, prioritize that
                        moveVector = new Vector2Int((int)Mathf.Sign(delta.x), 0);
                        if (!gridObject.MoveToGrid(currentPos + moveVector, GetSpeed(), pushStrength))
                        {
                            // Can't move in X, try to move in Y
                            moveVector = new Vector2Int(0, (int)Mathf.Sign(delta.y));
                            if (!gridObject.MoveToGrid(currentPos + moveVector, GetSpeed(), pushStrength))
                            {
                                // Can't move, try later
                                moveCooldownTimer = 0.0f;
                            }
                        }
                    }
                    else
                    {
                        // More to move in Y, prioritize that
                        moveVector = new Vector2Int(0, (int)Mathf.Sign(delta.y));
                        if (!gridObject.MoveToGrid(currentPos + moveVector, GetSpeed(), pushStrength))
                        {
                            // Can't move in Y, try to move in X
                            moveVector = new Vector2Int((int)Mathf.Sign(delta.x), 0);
                            if (!gridObject.MoveToGrid(currentPos + moveVector, GetSpeed(), pushStrength))
                            {
                                // Can't move, try later
                                moveCooldownTimer = 0.0f;
                            }
                        }
                    }
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