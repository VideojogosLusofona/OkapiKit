using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class Probe : OkapiElement
{
    public enum Type { Raycast = 0, Circlecast = 1 };
    public enum Direction { Up = 0, Down = 1, Right = 2, Left = 3, TargetObject = 4, TargetTag = 5 }

    [SerializeField] private Type       type = Type.Raycast;
    [SerializeField] private Direction  direction = Direction.Up;
    [SerializeField] private float      radius = 20.0f;
    [SerializeField] private float      minDistance = 0.0f;
    [SerializeField] private float      maxDistance = 1000.0f;
    [SerializeField] private Hypertag[] tags;
    [SerializeField] private Transform  targetTransform;
    [SerializeField] private Transform  dirTransform;
    [SerializeField] private Hypertag   dirTag;

    private bool            intersectionState = false;
    private RaycastHit2D[]  intersectionResults = new RaycastHit2D[64];
    private Vector3         intersectionPoint = Vector3.zero;
    private float           closestIntersectionDistance;

    private bool hasMaxDistance => !((direction == Direction.TargetTag) || (direction == Direction.TargetObject));

    public string GetTags()
    {
        string desc = "";
        if ((tags != null) && (tags.Length > 0))
        {
            desc += $"[";
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == null) desc += "NULL";
                else desc += tags[i].name;
                if (i < tags.Length - 1) desc += ",";
            }
            desc += "]";
        }
        else desc = "[UNDEFINED]";

        return desc;
    }
    public override string GetRawDescription(string ident, GameObject refObject)
    {
        string desc = ident;

        string dirDesc = "";
        switch (direction)
        {
            case Direction.Up:
                dirDesc = "the up direction of this object";
                break;
            case Direction.Down:
                dirDesc = "the down direction of this object";
                break;
            case Direction.Right:
                dirDesc = "the right direction of this object";
                break;
            case Direction.Left:
                dirDesc = "the left direction of this object";
                break;
            case Direction.TargetObject:
                {
                    string objName = (dirTransform != null) ? (dirTransform.name) : ("UNDEFINED");
                    dirDesc = $"in the direction of the object [{objName}]";
                }
                break;
            case Direction.TargetTag:
                string tagName = (dirTag != null) ? (dirTag.name) : ("UNDEFINED");
                dirDesc = $"in the direction of the closest object with tag [{tagName}]";
                break;
            default:
                break;
        }

        switch (type)
        {
            case Type.Raycast:
                if (hasMaxDistance)
                {
                    if (minDistance > 0) desc += $"Casts a ray {dirDesc}, from {minDistance} units to {maxDistance} units away,\n";
                    else desc += $"Casts a ray {dirDesc}, up to {maxDistance} units away,\n";
                }
                else
                {
                    if (minDistance > 0) desc += $"Casts a ray {dirDesc}, from {minDistance} units away,\n";
                    else desc += $"Casts a ray {dirDesc},\n";
                }
                break;
            case Type.Circlecast:
                if (hasMaxDistance)
                {
                    if (minDistance > 0) desc += $"Casts a circle with radius {radius} {dirDesc}, from {minDistance} units to {maxDistance} units away,\n";
                    else desc += $"Casts a circle with radius {radius} {dirDesc}, up to {maxDistance} units away,\n";
                }
                else
                {
                    if (minDistance > 0) desc += $"Casts a circle with radius {radius} {dirDesc}, from {minDistance} units away,\n";
                    else desc += $"Casts a circle with radius {radius} {dirDesc},\n";
                }
                break;
            default:
                break;
        }
        if ((tags != null) && (tags.Length > 0))
        {
            desc += $"detecting intersections with objects with tags [";
            for (int i = 0; i < tags.Length; i++)
            {
                if (tags[i] == null) desc += "NULL";
                else desc += tags[i].name;
                if (i < tags.Length - 1) desc += ",";
            }
            desc += "]";
        }
        else desc += "although tags are not set (no collision will be detected)!";
        if (targetTransform != null)
        {
            desc += $"\nThe position of object {targetTransform.name} will be set to the intersection.";
        }        

        return desc;
    }

    public override string UpdateExplanation()
    {
        _explanation = "";
        if (description != "") _explanation += description + "\n----------------\n";

        _explanation += GetRawDescription("", gameObject);

        return _explanation;
    }

    public bool     GetIntersectionState() => intersectionState;
    public float    GetIntersectionDistance() => closestIntersectionDistance;
    public float    GetMinDistance() => minDistance;
    public float    GetMaxDistance()
    {
        switch (direction)
        {
            case Direction.TargetObject:
                if (dirTransform)
                {
                    return Vector3.Distance(dirTransform.position, transform.position);
                }
                break;
            case Direction.TargetTag:
                if (dirTag)
                {
                    var objects = this.FindObjectsOfTypeWithHypertag<Transform>(dirTag, true);
                    if ((objects != null) && (objects.Length > 0))
                    {
                        return Vector3.Distance(objects[0].position, transform.position);
                    }
                }
                break;
            default:
                break;
        }
        return maxDistance;
    }
    public Vector3  GetStart(Vector3 dir) => transform.position + minDistance * dir;
    public Vector3  GetEnd(Vector3 dir) => transform.position + GetMaxDistance() * dir;

    private void Update()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;

        var dir = GetDirection();

        int n = 0;
        if (type == Type.Raycast)
        {
            n = Physics2D.Raycast(GetStart(dir), dir, contactFilter, intersectionResults, (GetMaxDistance() - minDistance));
        }
        else if (type == Type.Circlecast)
        {
            n = Physics2D.CircleCast(GetStart(dir), radius, dir, contactFilter, intersectionResults, (GetMaxDistance() - minDistance));
        }
        if (n > 0)
        {
            intersectionState = false;
            closestIntersectionDistance = float.MaxValue;
            for (int i = 0; i < n; i++)
            {
                // Check if this objects has the tags
                if (intersectionResults[i].collider.gameObject.HasHypertags(tags))
                {
                    intersectionState = true;
                    if (intersectionResults[i].distance < closestIntersectionDistance)
                    {
                        closestIntersectionDistance = intersectionResults[i].distance;
                    }
                }
            }

            intersectionPoint = GetStart(dir) + closestIntersectionDistance * dir;
            if ((intersectionState) && (targetTransform))
            {
                targetTransform.position = intersectionPoint;
            }
        }
        else
        {
            intersectionState = false;
            intersectionPoint = Vector3.zero;
            closestIntersectionDistance = float.MaxValue;

            if (targetTransform)
            {
                targetTransform.position = GetEnd(dir);
            }
        }
    }

    private Vector3 GetDirection()
    {
        Vector3 dir = transform.up;
        switch (direction)
        {
            case Direction.Up: dir = transform.up; break;
            case Direction.Down: dir = -transform.up; break;
            case Direction.Right: dir = transform.right; break;
            case Direction.Left: dir = -transform.right; break;
            case Direction.TargetObject:
                if (dirTransform)
                {
                    dir = dirTransform.position - transform.position;
                    if (dir.sqrMagnitude > 1e-6) dir.Normalize();
                    else dir = transform.up;
                }
                break;
            case Direction.TargetTag:
                if (dirTag)
                {
                    var objects = this.FindObjectsOfTypeWithHypertag<Transform>(dirTag, true);
                    if ((objects != null) && (objects.Length > 0))
                    {
                        dir = objects[0].position - transform.position;
                        if (dir.sqrMagnitude > 1e-6) dir.Normalize();
                        else dir = transform.up;
                    }
                }
                break;
            default:
                break;
        }

        return dir;
    }

    private void OnDrawGizmos()
    {
        Vector3 dir = GetDirection();
        if (type == Type.Raycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(GetStart(dir), GetEnd(dir));
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetStart(dir), radius);
            Gizmos.DrawWireSphere(GetEnd(dir), radius);

            var perp = new Vector3(dir.y, -dir.x, dir.z);

            var p1 = GetStart(dir) - perp * radius;
            var p2 = GetStart(dir) + perp * radius;
            var p3 = GetEnd(dir) - perp * radius;
            var p4 = GetEnd(dir) + perp * radius;
            Gizmos.DrawLine(p1, p3);
            Gizmos.DrawLine(p2, p4);
        }

        if (intersectionState)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(intersectionPoint, radius);
        }
    }
}
