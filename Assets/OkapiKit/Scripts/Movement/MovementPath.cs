using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class MovementPath : Movement
{
    [SerializeField] 
    private float       speed = 200.0f;
    [SerializeField, ShowIf("needPath")] 
    private Path        path;
    [SerializeField, ShowIf("needTag")]
    private Hypertag    taggedPath;
    [SerializeField]
    private bool        loop = false;
    [SerializeField]
    private bool        forceInitialPosition = true;

    private bool needPath => taggedPath == null;
    private bool needTag => path == null;

    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    Path    actualPath;
    int     pathIndex;

    void Start()
    {
        actualPath = path;

        if (taggedPath)
        {
            actualPath = gameObject.FindObjectOfTypeWithHypertag<Path>(taggedPath);
        }

        if (actualPath)
        {
            pathIndex = 0;

            if (forceInitialPosition)
            {
                transform.position = actualPath.GetWorldPosition(pathIndex);

                pathIndex++;
            }
        }
    }

    void FixedUpdate()
    {
        if (actualPath == null) return;
        if (pathIndex >= actualPath.GetPathSize()) return;

        float distanceToMove = Time.fixedDeltaTime * speed;

        int initialIndex = pathIndex;

        while (distanceToMove > 0)
        {
            // Get next position
            var targetPosition = actualPath.GetWorldPosition(pathIndex);

            var currentPosition = transform.position;
            var newPosition = Vector3.MoveTowards(currentPosition, targetPosition, distanceToMove);

            var distanceMoved = Vector3.Distance(targetPosition, currentPosition);

            MoveDelta(newPosition - currentPosition);

            distanceToMove -= distanceMoved;

            if (Vector3.Distance(targetPosition, newPosition) <= 0.0001f)
            {
                // Go to next point
                pathIndex++;
                if (pathIndex >= actualPath.GetPathSize())
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
    }
}
