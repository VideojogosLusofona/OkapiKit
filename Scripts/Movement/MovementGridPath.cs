using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [RequireComponent(typeof(GridObject))]
    [AddComponentMenu("Okapi/Movement/Path Grid Movement")]
    public class MovementGridPath : MovementGrid
    {
        public enum RotationBehaviour { None = 0, AlignX = 1, AlignY = 2 };

        [SerializeField]
        private float               speed = 200.0f;
        [SerializeField, ShowIf("needPath")]
        private Path                path;
        [SerializeField, ShowIf("needTag")]
        private Hypertag            taggedPath;
        [SerializeField]
        private bool                loop = false;
        [SerializeField]
        private bool                displayPath = false;
        [SerializeField]
        private bool                relativePath = false;
        [SerializeField]
        private RotationBehaviour   rotationBehaviour = RotationBehaviour.None;
        [SerializeField, ShowIf("hasRotation")]
        private bool                useFlip;
        [SerializeField, ShowIf("hasRotation")]
        private float               maxRotationSpeed = 360;
        [SerializeField]
        private bool                useAnimator = false;
        [SerializeField]
        private Animator            animator;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string              horizontalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string              absoluteHorizontalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string              verticalVelocityParameter;
        [SerializeField, AnimatorParam("animator", AnimatorControllerParameterType.Float)]
        private string              absoluteVerticalVelocityParameter;

        private bool needPath => taggedPath == null;
        private bool needTag => path == null;
        private bool hasRotation => rotationBehaviour != RotationBehaviour.None;

        public override Vector2 GetSpeed() => new Vector2(speed, speed);
        public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

        override public string GetTitle() => "Follow Grid Path";

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "";
            if (speed != 0.0f)
            {
                float gridSpeed = speed / gridSize.x;
                if (path != null)
                {
                    desc += $"Follows the [{path.name}] path, at {speed} units per second ({gridSpeed} cells/sec).\n";
                }
                else if (taggedPath != null)
                {
                    desc += $"Follows a path tagged with [{taggedPath.name}], at {speed} units per second ({gridSpeed} cells/sec).\n";
                    desc += $"If multiple paths have the [{taggedPath.name}] tag, a random one is selected.\n";
                }
                else
                {
                    desc += $"Follows a path, at {speed} units per second ({gridSpeed} cells/sec).\n";
                }
            }
            if (cooldown > 0.0f)
            {
                desc += $"The movement steps can only happen once every {cooldown} seconds.\n";
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
                desc += $"It will turn at a maximum rate of {maxRotationSpeed} degrees per second.\n";
            }
            if (pushStrength > 0)
            {
                desc += $"This object can push objects as long as their mass combined doesn't exceed {pushStrength}.\n";
            }
            else
            {
                desc += $"This object can not push objects.\n";
            }

            string animDesc = "";
            if (useAnimator)
            {
                Animator anim = animator;
                if (anim == null) anim = GetComponent<Animator>();
                if (anim)
                {
                    animDesc += $"Some values will be set on animator {anim.name}:\n";
                    if (horizontalVelocityParameter != "") animDesc += $"Horizontal velocity will be set to parameter {horizontalVelocityParameter}.\n";
                    if (absoluteHorizontalVelocityParameter != "") animDesc += $"Absolute horizontal velocity will be set to parameter {absoluteHorizontalVelocityParameter}.\n";
                    if (verticalVelocityParameter != "") animDesc += $"Vertical velocity will be set to parameter {verticalVelocityParameter}.\n";
                    if (absoluteVerticalVelocityParameter != "") animDesc += $"Absolute vertical velocity will be set to parameter {absoluteVerticalVelocityParameter}.\n";
                }
            }

            if (animDesc != "")
            {
                desc += animDesc;
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

            if (useAnimator)
            {
                Animator anm = animator;
                if (anm == null)
                {
                    anm = GetComponent<Animator>();
                    if (anm == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Animator not defined!", "If we want to drive an animator with the properties of the movement, we need to define which animator to use."));
                    }
                    else
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Warning, "Animator exists, but it should be linked explicitly!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                    }
                }
                if (anm != null)
                {
                    if (anm.runtimeAnimatorController == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Animator controller is not set!", "There's an animator, but it doesn't have a controller setup. Creat one and set it on the animator."));
                    }
                    else
                    {
                        Helpers.CheckErrorAnim(anm, "horizontal velocity", horizontalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "absolute horizontal velocity", absoluteHorizontalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "vertical velocity", verticalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                        Helpers.CheckErrorAnim(anm, "absolute vertical velocity", absoluteVerticalVelocityParameter, AnimatorControllerParameterType.Float, _logs);
                    }
                }
            }
        }

        List<Vector2Int>    actualPath;
        int                 pathIndex;
        Vector3             startScale;
        Vector2Int          offset;
        Path                activePath;

        protected override void Start()
        {
            base.Start();

            UpdatePath();

            if ((actualPath != null) && (actualPath.Count > 0))
            {
                pathIndex = 0;

                if (relativePath)
                {
                    transform.position = gridObject.Snap(transform.position);

                    offset = grid.WorldToGrid(transform.position) - actualPath[pathIndex];

                    pathIndex++;
                }
                else
                {
                    transform.position = gridObject.GridToWorld(actualPath[pathIndex]);

                    pathIndex++;
                }
            }

            startScale = transform.localScale;
        }

        void FixedUpdate()
        {
            if (actualPath == null) return;
            if (actualPath == null) return;
            if (!isMovementActive()) return;

            if ((activePath) && (activePath.isLocalSpace))
            {
                CreatePathFromPoints(activePath.GetPoints());
            }

            if (gridObject.isMoving)
            {
                if (rotationBehaviour != RotationBehaviour.None)
                {
                    var direction = gridObject.lastDelta;
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

                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, maxRotationSpeed * Time.fixedDeltaTime);
                    }
                }
            }
            else if (moveCooldownTimer <= 0.0f)
            {
                var nextPos = actualPath[pathIndex];
                if (relativePath)
                {
                    nextPos = nextPos + offset;
                }

                if (gridObject.MoveToGrid(nextPos, GetSpeed(), pushStrength))
                {
                    NextWaypoint();
                    moveCooldownTimer = cooldown;
                }
            }

            if ((useAnimator) && (animator))
            {
                currentVelocity = gridObject.lastDelta / Time.fixedDeltaTime;

                if (horizontalVelocityParameter != "") animator.SetFloat(horizontalVelocityParameter, currentVelocity.x);
                if (absoluteHorizontalVelocityParameter != "") animator.SetFloat(absoluteHorizontalVelocityParameter, Mathf.Abs(currentVelocity.x));
                if (verticalVelocityParameter != "") animator.SetFloat(verticalVelocityParameter, currentVelocity.y);
                if (absoluteVerticalVelocityParameter != "") animator.SetFloat(absoluteVerticalVelocityParameter, Mathf.Abs(currentVelocity.y));
            }
        }

        void NextWaypoint()
        {
            if (pathIndex >= actualPath.Count - 1)
            {
                if (!loop)
                {
                    pathIndex = actualPath.Count - 1;
                    return;
                }
            }

            for (int i = 0; i < actualPath.Count; i++)
            {
                pathIndex = (pathIndex + 1) % actualPath.Count;
                var nextPos = actualPath[pathIndex];
                if (relativePath)
                {
                    nextPos += offset;
                }
                if (grid.WorldToGrid(transform.position) != nextPos)
                {
                    return;
                }
            }
        }

        void CreatePathFromPoints(List<Vector3> points)
        {
            if (points.Count < 2) return;

            actualPath = new()
            {
                grid.WorldToGrid(points[0])
            };

            var prevPoint = actualPath[actualPath.Count - 1];
            for (int i = 1; i < points.Count; i++)
            {
                var nextPoint = grid.WorldToGrid(points[i]);
                var currentPoint = prevPoint;

                while (currentPoint != nextPoint)
                {
                    var delta = nextPoint - currentPoint;
                    if (Mathf.Abs(delta.x) > Mathf.Abs(delta.y))
                    {
                        currentPoint.x += (int)Mathf.Sign(delta.x);
                    }
                    else
                    {
                        currentPoint.y += (int)Mathf.Sign(delta.y);
                    }
                    if (currentPoint != prevPoint)
                    {
                        actualPath.Add(currentPoint);
                        prevPoint = currentPoint;
                    }
                }
            }

            if (loop)
            {
                // if last point equals first, remove it
                if (actualPath[0] == actualPath[actualPath.Count - 1])
                {
                    actualPath.RemoveAt(actualPath.Count - 1);
                }
            }
        }

        void UpdatePath()
        {
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
                CreatePathFromPoints(activePath.GetPoints());
            }
        }

        private void OnDrawGizmos()
        {
            if (!displayPath) return;

            UpdatePath();
            if (actualPath != null)
            {
                if (!Application.isPlaying)
                {
                    offset = grid.WorldToGrid(transform.position) - actualPath[0];
                }

                Gizmos.color = Color.red;
                int nPoints = actualPath.Count - 1;
                if (loop) nPoints = actualPath.Count;

                Vector2Int o = (relativePath) ? (offset) : (Vector2Int.zero);

                for (int i = 0; i < nPoints; i++)
                {
                    Vector3 p1 = gridObject.GridToWorld(actualPath[i] + o);
                    Vector3 p2 = gridObject.GridToWorld(actualPath[(i + 1) % actualPath.Count] + o);
                    Gizmos.DrawLine(p1, p2);
                }
            }
        }
    }
}