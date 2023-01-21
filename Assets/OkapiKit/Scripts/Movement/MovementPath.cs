using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MovementPath : Movement
{
    [SerializeField] private enum RotationBehaviour { None, AlignX, AlignY };

    [SerializeField] 
    private float               speed = 200.0f;
    [SerializeField, ShowIf("needPath")] 
    private Path                path;
    [SerializeField, ShowIf("needTag")]
    private Hypertag            taggedPath;
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

    List<Vector3>   actualPath;
    int             pathIndex;
    Vector3         startScale;
    Vector3         offset;

    void Start()
    {
        if (taggedPath)
        {
            var paths = gameObject.FindObjectsOfTypeWithHypertag<Path>(taggedPath);
            if (paths.Length > 0)
            {
                int r = Random.Range(0, paths.Length);
                actualPath = paths[r].GetPoints();
            }
        }
        else if (path != null)
        {
            actualPath = path.GetPoints();
        }

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

        startScale = transform.localScale;
    }

    void FixedUpdate()
    {
        if (actualPath == null) return;
        if (actualPath == null) return;
        if (pathIndex >= actualPath.Count) return;

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
                    else break;
                }
                if (initialIndex == pathIndex)
                {
                    // Error in path
                    break;
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
