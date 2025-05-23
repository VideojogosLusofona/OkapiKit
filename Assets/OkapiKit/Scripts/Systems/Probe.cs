using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Probe")]
    public class Probe : OkapiElement
    {
        public enum Type { Raycast = 0, Circlecast = 1, Colliders = 2 };
        public enum Direction { Up = 0, Down = 1, Right = 2, Left = 3, TargetObject = 4, TargetTag = 5, TargetObjectDirection = 6, TargetTagDirection = 7 }

        [SerializeField] private Type type = Type.Raycast;
        [SerializeField] private Direction direction = Direction.Up;
        [SerializeField, OVNoFunction] private OkapiValue radius = new OkapiValue(20.0f);
        [SerializeField, OVNoFunction] private OkapiValue minDistance = new OkapiValue(0.0f);
        [SerializeField, OVNoFunction] private OkapiValue maxDistance = new OkapiValue(1000.0f);
        [SerializeField] private Collider2D[] colliders;
        [SerializeField] private Hypertag[] tags;
        [SerializeField] private Transform targetTransform;
        [SerializeField] private Transform dirTransform;
        [SerializeField] private Hypertag dirTag;


        private bool intersectionState = false;
        private RaycastHit2D[] hitResults = new RaycastHit2D[64];
        private Collider2D[]   intersectionResults = new Collider2D[64];
        private Vector3 intersectionPoint = Vector3.zero;
        private float closestIntersectionDistance;

        private bool hasMaxDistance => !((direction == Direction.TargetTag) || (direction == Direction.TargetObject));
        private bool isCastProbe => (type == Type.Raycast) || (type == Type.Circlecast);

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

            if (isCastProbe)
            {
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
                            dirDesc = $"towards the object [{objName}]";
                        }
                        break;
                    case Direction.TargetTag:
                        {
                            string tagName = (dirTag != null) ? (dirTag.name) : ("UNDEFINED");
                            dirDesc = $"towards the closest object with tag [{tagName}]";
                        }
                        break;
                    case Direction.TargetObjectDirection:
                        {
                            string objName = (dirTransform != null) ? (dirTransform.name) : ("UNDEFINED");
                            dirDesc = $"in the direction of the object [{objName}]";
                        }
                        break;
                    case Direction.TargetTagDirection:
                        {
                            string tagName = (dirTag != null) ? (dirTag.name) : ("UNDEFINED");
                            dirDesc = $"in the direction of the closest object with tag [{tagName}]";
                        }
                        break;
                    default:
                        break;
                }

                switch (type)
                {
                    case Type.Raycast:
                        if (hasMaxDistance)
                        {
                            desc += $"Casts a ray {dirDesc}, from {minDistance.GetDescription()} units to {maxDistance.GetDescription()} units away,\n";
                        }
                        else
                        {
                            desc += $"Casts a ray {dirDesc}, from {minDistance.GetDescription()} units away,\n";
                        }
                        break;
                    case Type.Circlecast:
                        if (hasMaxDistance)
                        {
                            desc += $"Casts a circle with radius {radius.GetDescription()} {dirDesc}, from {minDistance.GetDescription()} units to {maxDistance.GetDescription()} units away,\n";
                        }
                        else
                        {
                            desc += $"Casts a circle with radius {radius.GetDescription()} {dirDesc}, from {minDistance.GetDescription()} units away,\n";
                        }
                        break;
                    default:
                        break;
                }
                
                if (targetTransform != null)
                {
                    desc += $"\nThe position of object {targetTransform.name} will be set to the intersection.";
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
            }
            else if (type == Type.Colliders)
            {
                desc += $"Detects if the following colliders:\n";
                foreach (var collider in colliders)
                {
                    desc += $"{ident}  - {collider.name}\n";
                }
                if ((tags != null) && (tags.Length > 0))
                {
                    desc += $"are intersecting objects with tags [";
                    for (int i = 0; i < tags.Length; i++)
                    {
                        if (tags[i] == null) desc += "NULL";
                        else desc += tags[i].name;
                        if (i < tags.Length - 1) desc += ",";
                    }
                    desc += "]";
                }
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (type == Type.Colliders)
            {
                if ((colliders == null) || (colliders.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Colliders not defined - these define the shape of the probe!", "Probes check for intersections, so we need to define what shape we want to intersect with the scene."));
                }
                else
                {
                    int index = 0;
                    int count = 0;
                    foreach (var collider in colliders)
                    {
                        if (collider == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Empty collider slot {index}!", "Empty colliders are useless, so remove the empty, or fill it in."));
                        }
                        else count++;
                        index++;
                    }

                    if (count == 0)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Colliders not defined - these define the shape of the probe!", "Probes check for intersections, so we need to define what shape we want to intersect with the scene."));
                    }
                }
            }

            if ((tags == null) || (tags.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Tags not defined - these define which objects the probe can detect!", "Probes check for intersections, so we need to define what objects can block/intersect with the probe. We define those using tags."));
            }
            else
            {
                int index = 0;
                foreach (var tag in tags)
                {
                    if (tag == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Empty tag slot {index}!", "Empty tags are useless, so remove the empty, or fill it in."));
                    }
                    index++;
                }
            }
            switch (direction)
            {
                case Direction.TargetObject:
                case Direction.TargetObjectDirection:
                    if (dirTransform == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Target object is not set!!", "If our probe is supposed to target a specific object, we need to define which one."));
                    }
                    break;
                case Direction.TargetTag:
                case Direction.TargetTagDirection:
                    if (dirTag== null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Target tag is not set!!", "If our probe is supposed to target the closest object with a specific tag, we need to define which tag."));
                    }
                    break;
            }
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        public bool GetIntersectionState() => intersectionState;
        public float GetIntersectionDistance() => closestIntersectionDistance;
        public float GetMinDistance() => minDistance.GetFloat(gameObject);
        public float GetMaxDistance()
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
            return maxDistance.GetFloat(gameObject);
        }
        public Vector3 GetStart(Vector3 dir) => transform.position + GetMinDistance() * dir;
        public Vector3 GetEnd(Vector3 dir) => transform.position + GetMaxDistance() * dir;

        private void Update()
        {
            var contactFilter = new ContactFilter2D();
            contactFilter.useTriggers = true;

            if (isCastProbe)
            {
                if ((direction == Direction.TargetObjectDirection) || (direction == Direction.TargetTagDirection))
                {
                    float dist = maxDistance.GetFloat(gameObject);
                    if (direction == Direction.TargetObjectDirection)
                    {
                        dist = (dirTransform.position - transform.position).magnitude;
                    }
                    else if (direction == Direction.TargetTagDirection)
                    {
                        var objects = this.FindObjectsOfTypeWithHypertag<Transform>(dirTag, true);
                        if ((objects != null) && (objects.Length > 0))
                        {
                            dist = (objects[0].position - transform.position).magnitude;
                        }
                    }

                    if ((dist < minDistance.GetFloat(gameObject)) || (dist > maxDistance.GetFloat(gameObject)))
                    {
                        intersectionState = true;
                        intersectionPoint = GetEnd(GetDirection());
                        closestIntersectionDistance = maxDistance.GetFloat(gameObject);
                        return;
                    }
                }
                var dir = GetDirection();

                int n = 0;
                if (type == Type.Raycast)
                {
                    n = Physics2D.Raycast(GetStart(dir), dir, contactFilter, hitResults, (GetMaxDistance() - GetMinDistance()));
                }
                else if (type == Type.Circlecast)
                {
                    n = Physics2D.CircleCast(GetStart(dir), radius.GetFloat(gameObject), dir, contactFilter, hitResults, (GetMaxDistance() - GetMinDistance()));
                }

                if (n > 0)
                {
                    intersectionState = false;
                    closestIntersectionDistance = float.MaxValue;
                    for (int i = 0; i < n; i++)
                    {
                        // Check if this objects has the tags
                        if (hitResults[i].collider.gameObject.HasHypertags(tags))
                        {
                            intersectionState = true;
                            if (hitResults[i].distance < closestIntersectionDistance)
                            {
                                closestIntersectionDistance = hitResults[i].distance;
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
            else if (type == Type.Colliders)
            {
                intersectionState = false;
                foreach (var collider in colliders)
                {
                    var c = collider;
                    if (c.composite)
                    {
                        c = c.composite;
                    }
                    int n = Physics2D.OverlapCollider(c, contactFilter, intersectionResults);
                    if (n > 0)
                    {
                        for (int i = 0; i < n; i++)
                        {
                            // Check if this objects has the tags
                            if (intersectionResults[i].gameObject.HasHypertags(tags))
                            {
                                intersectionState = true;
                                break;
                            }
                        }
                        if (intersectionState) break;
                    }
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
                case Direction.TargetObjectDirection:
                    if (dirTransform)
                    {
                        dir = dirTransform.position - transform.position;
                        if (dir.sqrMagnitude > 1e-6) dir.Normalize();
                        else dir = transform.up;
                    }
                    break;
                case Direction.TargetTag:
                case Direction.TargetTagDirection:
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

#if UNITY_EDITOR
        private Dictionary<Collider2D, Mesh> probeMeshes = new Dictionary<Collider2D, Mesh>();

        private void OnDrawGizmos()
        {
            Vector3 dir = GetDirection();
            float   r = 1.0f;
            if (type == Type.Raycast)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawLine(GetStart(dir), GetEnd(dir));
            }
            else if (type == Type.Circlecast)
            {
                r = radius.GetFloat(gameObject);

                Gizmos.color = Color.yellow;
                Gizmos.DrawWireSphere(GetStart(dir), r);
                Gizmos.DrawWireSphere(GetEnd(dir), r);

                var perp = new Vector3(dir.y, -dir.x, dir.z);

                var p1 = GetStart(dir) - perp * r;
                var p2 = GetStart(dir) + perp * r;
                var p3 = GetEnd(dir) - perp * r;
                var p4 = GetEnd(dir) + perp * r;
                Gizmos.DrawLine(p1, p3);
                Gizmos.DrawLine(p2, p4);
            }

            if (intersectionState)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawSphere(intersectionPoint, r);
            }

            if (type == Type.Colliders)
            {
                bool selected = false;
                foreach (var sel in UnityEditor.Selection.objects)
                {
                    if (sel == gameObject)
                    {
                        selected = true;
                        break;
                    }
                }

                if (!selected)
                {
                    probeMeshes.Clear();
                }
                else
                {
                    if (colliders != null)
                    {
                        foreach (var collider in colliders)
                        {
                            if (!probeMeshes.TryGetValue(collider, out var mesh))
                            {
                                var c = collider;
                                if (c.composite)
                                {
                                    c = c.composite;
                                }
                                mesh = c.CreateMesh(true, true);

                                if (mesh != null)
                                {
                                    probeMeshes[collider] = mesh;
                                }
                            }
                        }
                    }
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if (type == Type.Colliders)
            {
                if (colliders != null)
                {
                    foreach (var collider in colliders)
                    {
                        RenderCollider(collider);
                    }
                }
            }
        }

        void RenderCollider(Collider2D collider)
        {
            var c = collider;
            if (c.composite)
            {
                c = c.composite;
            }

            Gizmos.color = Color.yellow;
            foreach (var mesh in probeMeshes)
            {
                var tris = mesh.Value.triangles;
                var vertex = mesh.Value.vertices;
                for (int i = 0; i < tris.Length; i += 3)
                {
                    Gizmos.DrawLine(vertex[tris[i]], vertex[tris[i + 1]]);
                    Gizmos.DrawLine(vertex[tris[i + 1]], vertex[tris[i + 2]]);
                    Gizmos.DrawLine(vertex[tris[i + 2]], vertex[tris[i]]);
                }
            }
        }
#endif
    }
}