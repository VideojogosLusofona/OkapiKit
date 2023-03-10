using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Probe : OkapiElement
{
    public enum Type { Raycast = 0, Circlecast = 1 };

    [SerializeField] private Type       type = Type.Raycast;
    [SerializeField] private float      radius = 20.0f;
    [SerializeField] private float      minDistance = 0.0f;
    [SerializeField] private float      maxDistance = 1000.0f;
    [SerializeField] private Hypertag[] tags;
    [SerializeField] private Transform  targetTransform;

    private bool            intersectionState = false;
    private RaycastHit2D[]  intersectionResults = new RaycastHit2D[64];
    private float           closestIntersectionDistance;

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

        switch (type)
        {
            case Type.Raycast:
                desc += $"Casts a ray in the up direction of this object, from distance {minDistance} to {maxDistance},\n";
                break;
            case Type.Circlecast:
                desc += $"Casts a circle with radius {radius} in the up direction of this object, from distance {minDistance} to {maxDistance},\n";
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
    public Vector3  GetStart() => transform.position + minDistance * transform.up;
    public Vector3  GetEnd() => transform.position + maxDistance * transform.up;

    private void Update()
    {
        var contactFilter = new ContactFilter2D();
        contactFilter.useTriggers = true;

        int n = 0;
        if (type == Type.Raycast)
        {
            n = Physics2D.Raycast(GetStart(), transform.up, contactFilter, intersectionResults, (maxDistance - minDistance));
        }
        else if (type == Type.Circlecast)
        {
            n = Physics2D.CircleCast(GetStart(), radius, transform.up, contactFilter, intersectionResults, (maxDistance - minDistance));
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

            if ((intersectionState) && (targetTransform))
            {
                targetTransform.position = GetStart() + closestIntersectionDistance * transform.up;
            }
        }
        else
        {
            intersectionState = false;
            closestIntersectionDistance = float.MaxValue;

            if (targetTransform)
            {
                targetTransform.position = GetEnd();
            }
        }
    }

    private void OnDrawGizmos()
    {
        if (type == Type.Raycast)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(GetStart(), GetEnd());
        }
        else
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(GetStart(), radius);
            Gizmos.DrawWireSphere(GetEnd(), radius);

            var p1 = GetStart() - transform.right * radius;
            var p2 = GetStart() + transform.right * radius;
            var p3 = GetEnd() - transform.right * radius;
            var p4 = GetEnd() + transform.right * radius;
            Gizmos.DrawLine(p1, p3);
            Gizmos.DrawLine(p2, p4);
        }

        if (intersectionState)
        {
            var p = GetStart() + transform.up * closestIntersectionDistance;
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(p, radius);
        }
    }
}
