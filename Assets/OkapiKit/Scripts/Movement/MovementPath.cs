using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static OkapiKit.MovementFollow;
using TMPro;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Movement/Path Movement")]
    public class MovementPath : Movement
    {
        public enum RotationBehaviour { None = 0, AlignX = 1, AlignY = 2 };

        [SerializeField]
        private float               speed = 200.0f;
        [SerializeField, ShowIf("needPath")]
        private Path                path;
        [SerializeField, ShowIf("needTag")]
        private Hypertag            taggedPath;
        [SerializeField]
        private bool                useDuration = false;
        [SerializeField]
        private float               pathDuration = 1.0f;
        [SerializeField]
        private bool                loop = false;
        [SerializeField]
        private bool                relativePath = false;
        [SerializeField]
        private RotationBehaviour   rotationBehaviour = RotationBehaviour.None;
        [SerializeField, ShowIf("hasRotation")]
        private bool                useFlip;
        [SerializeField, ShowIf("hasRotation")]
        private bool                hasMaxRotationSpeed = false;
        [SerializeField, ShowIf("hasMaxRotation")]
        private float               maxRotationSpeed = 360;

        private bool needPath => taggedPath == null;
        private bool needTag => path == null;
        private bool hasRotation => rotationBehaviour != RotationBehaviour.None;
        private bool hasMaxRotation => (hasRotation) && (hasMaxRotationSpeed);

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public string GetTitle() => "Follow Path";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            if (speed != 0.0f)
            {
                if (path != null)
                {
                    if (useDuration)
                        desc += $"Follows the [{path.name}] path, doing a full loop in {pathDuration} seconds.\n";
                    else
                        desc += $"Follows the [{path.name}] path, at {speed} units per second.\n";
                }
                else if (taggedPath != null)
                {
                    if (useDuration)
                    {
                        desc += $"Follows a path tagged with [{taggedPath.name}], in {pathDuration} seconds.\n";
                    }
                    else
                    {
                        desc += $"Follows a path tagged with [{taggedPath.name}], at {speed} units per second.\n";
                    }
                    desc += $"If multiple paths have the [{taggedPath.name}] tag, a random one is selected.\n";
                }
                else
                {
                    if (useDuration)
                        desc += $"Follows a path, doing a full loop in {pathDuration} seconds.\n";
                    else
                        desc += $"Follows a path, at {speed} units per second.\n";
                }
            }
            if (loop) desc += "The object will loop back to the beginning at the end of the path.\n";
            if (relativePath) desc += "The path will be relative to the initial position of the object.\n";
            if (rotationBehaviour != RotationBehaviour.None)
            {
                if (rotationBehaviour == RotationBehaviour.AlignX)
                    desc += "The object will align it's X axis to the path direction";
                if (rotationBehaviour == RotationBehaviour.AlignY)
                    desc += "The object will align it's Y axis to the path direction";
                if (useFlip)
                    desc += ", flipping along that axis if the path reverses direction.\n";
                else
                    desc += ".\n";
                if (hasMaxRotation)
                {
                    desc += $"It will turn at a maximum rate of {maxRotationSpeed} degrees per second.";
                }
            }
            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();
            
            if ((path == null) && (taggedPath == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Path not defined - use either tag or drag the path object to the slot below!", "This component follows a path, so we need to specify which one."));
            }
        }

        List<Vector3>   actualPath;
        int             pathIndex;
        Vector3         startScale;
        Vector3         offset;
        Path            activePath;
        float           t = 0.0f;
        float           tInc;

        protected override void Start()
        {
            base.Start();

            if (taggedPath)
            {
                var paths = gameObject.FindObjectsOfTypeWithHypertag<Path>(taggedPath);
                if (paths.Length > 0)
                {
                    int r = Random.Range(0, paths.Length);
                    activePath = paths[r];
                }
            }
            else if (path != null)
            {
                activePath = path;
            }
            if (activePath)
            {
                actualPath = activePath.GetPoints();
            }

            if (useDuration)
            {
                t = 0.0f;
                tInc = 1.0f / pathDuration;

                if (relativePath)
                {
                    Vector3 startPos = activePath.EvaluateWorld(0.0f);
                    offset = transform.position - startPos;
                }
                else
                {
                    offset = Vector3.zero;
                }
            }
            else
            {
                if ((actualPath != null) && (actualPath.Count > 0))
                {
                    pathIndex = 0;

                    if (relativePath)
                    {
                        offset = transform.position - actualPath[pathIndex];

                        pathIndex++;
                    }
                    else
                    {
                        transform.position = actualPath[pathIndex];

                        pathIndex++;
                    }
                }
            }

            startScale = transform.localScale;
        }

        void FixedUpdate()
        {
            if (actualPath == null) return;
            if (actualPath == null) return;
            if (pathIndex >= actualPath.Count) return;
            if (!isMovementActive()) return;

            if (useDuration)
            {
                Vector3 currentPosition = transform.position - offset;
                Vector3 targetPosition = activePath.EvaluateWorld(t);

                MoveDelta(targetPosition - currentPosition);

                t += tInc * Time.fixedDeltaTime;
                if (t > 1.0f)
                {
                    if (loop) t -= 1.0f;
                    else
                    {
                        StopMovement();
                        t = 1.0f;
                    }
                }
            }
            else
            {
                if ((activePath) && (activePath.isLocalSpace))
                {
                    actualPath = activePath.GetPoints();
                }

                float distanceToMove = Time.fixedDeltaTime * speed;

                int initialIndex = pathIndex;

                while (distanceToMove > 0)
                {
                    // Get next position
                    var targetPosition = actualPath[pathIndex];

                    var currentPosition = transform.position - offset;
                    var newPosition = Vector3.MoveTowards(currentPosition, targetPosition, distanceToMove);

                    var distanceMoved = Vector3.Distance(targetPosition, currentPosition);

                    MoveDelta(newPosition - currentPosition);

                    distanceToMove -= distanceMoved;

                    if (Vector3.Distance(targetPosition, newPosition) <= 0.0001f)
                    {
                        // Go to next point
                        pathIndex++;
                        if (pathIndex >= actualPath.Count)
                        {
                            if (loop)
                            {
                                pathIndex = 0;
                            }
                            else
                            {
                                StopMovement();
                                break;
                            }
                        }
                        if (initialIndex == pathIndex)
                        {
                            // Error in path
                            break;
                        }
                    }
                }
            }

            if (rotationBehaviour != RotationBehaviour.None)
            {
                var direction = lastDelta;
                if (direction.sqrMagnitude > 1e-6)
                {
                    direction = direction.normalized;

                    Quaternion targetRotation = transform.rotation;

                    if (rotationBehaviour == RotationBehaviour.AlignX)
                    {
                        if (useFlip)
                        {
                            if (direction.x < 0)
                            {
                                direction = -direction;
                                transform.localScale = new Vector3(-startScale.x, startScale.y, startScale.z);
                            }
                            else
                            {
                                transform.localScale = startScale;
                            }
                        }

                        targetRotation = Quaternion.LookRotation(Vector3.forward, new Vector3(-direction.y, direction.x));
                    }
                    else if (rotationBehaviour == RotationBehaviour.AlignY)
                    {
                        if (useFlip)
                        {
                            if (direction.y < 0)
                            {
                                direction = -direction;
                                transform.localScale = new Vector3(startScale.x, -startScale.y, startScale.z);
                            }
                            else
                            {
                                transform.localScale = startScale;
                            }
                        }

                        targetRotation = Quaternion.LookRotation(Vector3.forward, direction);
                    }

                    if (hasMaxRotation)
                    {
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.fixedDeltaTime);
                    }
                    else
                    {
                        transform.rotation = targetRotation;
                    }
                }
            }
        }
    }
}