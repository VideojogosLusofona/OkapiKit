using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointMovement : Movement
{
    [SerializeField] enum MovementType { Sequence, Random };

    [SerializeField] private float          speed = 200.0f;
    [SerializeField] private MovementType   type;
    [SerializeField] private Transform[]    waypoints;

    private int         index = 0;
    private Transform   currentTarget;

    public override Vector2 GetSpeed() => new Vector2(speed, speed);
    public override void SetSpeed(Vector2 speed) { this.speed = speed.x; }

    private void FixedUpdate()
    {
        if (currentTarget)
        {
            MoveDelta(Vector3.MoveTowards(transform.position, currentTarget.position, speed * Time.fixedDeltaTime) - transform.position);
        }
    }

    void Update()
    {
        if (currentTarget)
        {
            // Check distance
            if (Vector3.Distance(transform.position, currentTarget.position) < 1)
            {
                currentTarget = null;
            }
        }

        if (currentTarget == null)
        {
            switch (type)
            {
                case MovementType.Sequence:
                    index = (index + 1) % waypoints.Length;
                    break;
                case MovementType.Random:
                    index = Random.Range(0, waypoints.Length);
                    break;
                default:
                    index = 0;
                    break;
            }
            if (index < waypoints.Length) 
            {
                currentTarget = waypoints[index];
            }
        }
    }
}
