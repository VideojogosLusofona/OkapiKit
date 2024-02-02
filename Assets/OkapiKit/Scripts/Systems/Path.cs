using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static OkapiKit.CameraFollow2d;
using UnityEditor;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Path Object")]
    public class Path : OkapiElement
    {
        [SerializeField] public enum Type { Linear = 0, Smooth = 1, Circle = 2, Arc = 3, Polygon = 4};

        [SerializeField]
        private Type    type = Type.Linear;
        [SerializeField]
        private bool        closed = false;
        [SerializeField, Range(3, 30)]
        private int         nSides = 4;
        [SerializeField]
        private List<Vector3> points;

        [SerializeField]
        private bool        worldSpace = true;
        [SerializeField]
        private bool        editMode = false;

        [SerializeField]
        private bool        onlyDisplayWhenSelected = true;
        [SerializeField]
        private Color       displayColor = Color.white;

        private bool isSmooth => type == Type.Smooth;

        private float           primaryRadius;
        private Vector2         primaryDir;
        private float           perpRadius;
        private Vector2         perpDir;
        private float           secondaryRadius;
        private Vector2         secondaryDir;
        private float           startAngle;
        private float           endAngle;
        private bool            dirty = true;
        private List<Vector3>   fullPoints;
        private float           linearLength;

        public List<Vector3> GetEditPoints() => (points == null) ? (null) : (new List<Vector3>(points));
        public void SetEditPoints(List<Vector3> inPoints)
        {
            points = new List<Vector3>(inPoints);

            dirty = true;
        }
        public int GetEditPointsCount() => points.Count;

        public Type GetPathType() => type;
        public void SetPathType(Type t) => type = t;
        public void SetWorldSpace(bool ws) => worldSpace = ws;

        public bool isEditMode => editMode;
        public bool isWorldSpace => worldSpace;
        public bool isLocalSpace => !worldSpace;

        public void AddPoint()
        {
            if (points == null) points = new List<Vector3>();

            if (points.Count >= 2)
            {
                // Get last two points and make the new point in that direction
                Vector3 delta = points[points.Count - 1] - points[points.Count - 2];

                points.Add(points[points.Count - 1] + delta);
            }
            else if (points.Count >= 1)
            {
                // Create a point next to the last point
                points.Add(points[points.Count - 1] + new Vector3(20, 20, 0));
            }
            else
            {
                // Create point in (0,0,0)
                points.Add(Vector3.zero);
            }

            dirty = true;
        }

        static private Vector3 ComputeBezier(Vector3[] pt, float t)
        {
            float it = (1 - t);
            float t2 = t * t;
            float t3 = t2 * t;
            float it2 = it * it;
            float it3 = it2 * it;

            return pt[0] * it3 + 3 * pt[1] * it2 * t + 3 * pt[2] * it * t2 + pt[3] * t3;
        }

        private static Vector3 EvaluateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;
            float t3 = t2 * t;

            Vector3 a = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
            Vector3 b = p0 - 2.5f * p1 + 2f * p2 - 0.5f * p3;
            Vector3 c = -0.5f * p0 + 0.5f * p2;
            Vector3 d = p1;

            return a * t3 + b * t2 + c * t + d;
        }

        private static Vector3 EvaluateCatmullRomDerivative(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
        {
            float t2 = t * t;

            Vector3 a = -0.5f * p0 + 1.5f * p1 - 1.5f * p2 + 0.5f * p3;
            Vector3 b = p0 - 2.5f * p1 + 2f * p2 - 0.5f * p3;
            Vector3 c = -0.5f * p0 + 0.5f * p2;

            // Derivative calculation
            Vector3 derivative = 3 * a * t2 + 2 * b * t + c;

            return derivative;
        }

        public Vector2 EvaluateWorld(float t)
        {
            return transform.TransformPoint(EvaluateLocal(t));
        }

        public Vector2 EvaluateLocal(float t)
        {
            switch (type)
            {
                case Type.Linear:
                    {
                        float d = t * linearLength;
                        
                        for (int i = 0; i < points.Count - ((closed)?(0):(1)); i++)
                        {
                            var delta = points[(i + 1) % points.Count] - points[i];
                            float thisDistance = delta.magnitude;
                            if (thisDistance >= d)
                            {
                                return points[i] + d * (delta / thisDistance);
                            }
                            else d -= thisDistance;
                        }

                        return points[0];
                    }
                case Type.Smooth:
                    {
                        int numPoints = points.Count;
                        int lastIndex = closed ? numPoints : numPoints - 2;
                        float scaledT = t * lastIndex; // Scale t to the number of segments
                        int i = Mathf.FloorToInt(scaledT) % numPoints; // Segment index, wrap around for closed curve

                        // Calculate t for the specific segment
                        t = scaledT - Mathf.FloorToInt(scaledT);

                        // The three points for the segment
                        Vector3 p0 = points[(i - 1 + numPoints) % numPoints];
                        Vector3 p1 = points[i];
                        Vector3 p2 = points[(i + 1) % numPoints];

                        // Generate a fake fourth point for Catmull-Rom equation
                        Vector3 p3 = points[(i + 2) % numPoints];

                        return EvaluateCatmullRom(p0, p1, p2, p3, t);
                    }
                case Type.Circle:
                    {
                        float   angle = t * Mathf.PI * 2.0f;
                        Vector2 center = points[0];

                        return center + primaryRadius * primaryDir * Mathf.Cos(angle) + perpRadius * perpDir * Mathf.Sin(angle);
                    }
                case Type.Arc:
                    {
                        float angle = Mathf.Lerp(startAngle, endAngle, t);
                        float radius = Mathf.Lerp(primaryRadius, secondaryRadius, t);
                        Vector2 center = points[0];

                        return center + Vector2.right * radius * Mathf.Cos(angle) + Vector2.up * radius * Mathf.Sin(angle);
                    }
                case Type.Polygon:
                    { 
                        float   totalSides = (nSides - ((closed) ? (0) : (1)));
                        int     side = Mathf.FloorToInt(totalSides * t);
                        float   tInc = 1.0f / totalSides;
                        float   angleInc = Mathf.PI * 2.0f / (totalSides + ((closed)?(0):(1)));
                        float   angle = side * angleInc;
                        Vector2 center = points[0];

                        Vector3 p1 = center + primaryRadius * primaryDir * Mathf.Cos(angle) + perpRadius * perpDir * Mathf.Sin(angle);
                        Vector3 p2 = center + primaryRadius * primaryDir * Mathf.Cos(angle + angleInc) + perpRadius * perpDir * Mathf.Sin(angle + angleInc);

                        float remainingT = (t - side * tInc) / tInc;

                        return p1 + (p2 - p1) * remainingT;
                    }
                default:
                    break;
            }

            return Vector2.zero;
        }

        public Vector2 EvaluateWorldDir(float t)
        {
            return transform.TransformVector(EvaluateLocalDir(t));
        }

        public Vector2 EvaluateLocalDir(float t)
        {
            switch (type)
            {
                case Type.Linear:
                    {
                        float d = t * linearLength;

                        for (int i = 0; i < points.Count - ((closed) ? (0) : (1)); i++)
                        {
                            var delta = points[(i + 1) % points.Count] - points[i];
                            float thisDistance = delta.magnitude;
                            if (thisDistance >= d)
                            {
                                return delta / thisDistance;
                            }
                            else d -= thisDistance; 
                        }

                        if (points.Count > 2)
                        {
                            if (closed)
                            {
                                var delta = points[0] - points[points.Count - 1];
                                return delta.normalized;
                            }
                            else
                            {
                                var delta = points[points.Count - 1] - points[points.Count - 2];
                                return delta.normalized;
                            }
                        }

                        return points[0];
                    }
                case Type.Smooth:
                    {
                        int numPoints = points.Count;
                        int lastIndex = closed ? numPoints : numPoints - 2;
                        float scaledT = t * lastIndex; // Scale t to the number of segments
                        int i = Mathf.FloorToInt(scaledT) % numPoints; // Segment index, wrap around for closed curve

                        // Calculate t for the specific segment
                        t = scaledT - Mathf.FloorToInt(scaledT);

                        // The three points for the segment
                        Vector3 p0 = points[(i - 1 + numPoints) % numPoints];
                        Vector3 p1 = points[i];
                        Vector3 p2 = points[(i + 1) % numPoints];

                        // Generate a fake fourth point for Catmull-Rom equation
                        Vector3 p3 = points[(i + 2) % numPoints];

                        return EvaluateCatmullRomDerivative(p0, p1, p2, p3, t).normalized;
                    }
                case Type.Circle:
                    {
                        float angle = t * Mathf.PI * 2.0f;

                        return (-primaryDir * Mathf.Sin(angle) + perpDir * Mathf.Cos(angle)).normalized;
                    }
                case Type.Arc:
                    {
                        float angle = Mathf.Lerp(startAngle, endAngle, t);
                        float radius = Mathf.Lerp(primaryRadius, secondaryRadius, t);

                        return (Vector2.up * radius * Mathf.Cos(angle) - Vector2.right * radius * Mathf.Sin(angle)).normalized;
                    }
                case Type.Polygon:
                    {
                        float totalSides = (nSides - ((closed) ? (0) : (1)));
                        int side = Mathf.FloorToInt(totalSides * t);
                        float tInc = 1.0f / totalSides;
                        float angleInc = Mathf.PI * 2.0f / (totalSides + ((closed) ? (0) : (1)));
                        float angle = side * angleInc;
                        Vector2 center = points[0];

                        Vector3 p1 = center + primaryRadius * primaryDir * Mathf.Cos(angle) + perpRadius * perpDir * Mathf.Sin(angle);
                        Vector3 p2 = center + primaryRadius * primaryDir * Mathf.Cos(angle + angleInc) + perpRadius * perpDir * Mathf.Sin(angle + angleInc);

                        float remainingT = (t - side * tInc) / tInc;

                        return (p2 - p1).normalized;
                    }
                default:
                    break;
            }

            return Vector2.zero;
        }

        public void InvertPath()
        {
            if ((type == Path.Type.Linear) || (type == Path.Type.Smooth))
            {
                var newPoints = new List<Vector3>();
                for (int i = points.Count - 1; i >= 0; i--)
                {
                    newPoints.Add(points[i]);
                }

                points = newPoints;
            }
            else if (type == Path.Type.Arc)
            {
                if (points.Count > 1)
                {
                    (points[1], points[2]) = (points[2], points[1]);
                }
            }
        }

        public void CenterPath()
        {
            if ((points == null) || (points.Count == 0)) return;

            Vector3 delta = Vector3.zero;

            if ((type == Path.Type.Linear) || (type == Path.Type.Smooth))
            {
                // Get extents of object
                Vector3 min = points[0];
                Vector3 max = points[0];

                foreach (var pt in points)
                {
                    min.x = Mathf.Min(min.x, pt.x);
                    min.y = Mathf.Min(min.y, pt.y);
                    min.z = Mathf.Min(min.z, pt.z);
                    max.x = Mathf.Max(max.x, pt.x);
                    max.y = Mathf.Max(max.y, pt.y);
                    max.z = Mathf.Max(max.z, pt.z);
                }

                delta = new Vector3(-(max.x + min.x) * 0.5f, -(max.y + min.y) * 0.5f, -(max.z + min.z) * 0.5f);
            }
            else if ((type == Path.Type.Arc) || (type == Path.Type.Circle) || (type == Path.Type.Polygon))
            {
                delta = -points[0];
            }

            for (int i = 0; i < points.Count; i++)
            {
                points[i] = points[i] + delta;
            }
        }

        public List<Vector3> GetPoints()
        {
            if ((!dirty) && (fullPoints == null)) return fullPoints;

            ComputeVariables();

            fullPoints = new List<Vector3>();
            if (points == null) return fullPoints;

            int steps = 20;
            switch (type)
            {
                case Type.Linear:
                    steps = 0;
                    fullPoints = new List<Vector3>(points);
                    if ((closed) && (points.Count > 1)) fullPoints.Add(points[0]);
                    break;
                case Type.Smooth:
                    if (points.Count < 3)
                    {
                        steps = 0;
                        fullPoints = new List<Vector3>(points);
                    }
                    else
                    {
                        steps = points.Count * 20;
                    }
                    break;
                case Type.Circle:
                case Type.Arc:
                    {
                        if (points.Count < ((type == Type.Circle) ? (2) : (3)))
                        {
                            steps = 0;
                            fullPoints = new List<Vector3>(points);
                        }
                        else
                        {
                            steps = 180;
                        }
                    }
                    break;
                case Type.Polygon:
                    {
                        if (points.Count < 2)
                        {
                            steps = 0;
                            fullPoints = new List<Vector3>(points);
                        }
                        else
                        {
                            steps = 0;

                            float angle = 0.0f;
                            float angleInc = Mathf.PI * 2.0f / nSides;
                            for (int i = 0; i < nSides; i++)
                            {
                                Vector2 center = points[0];
                                fullPoints.Add(center + primaryDir * primaryRadius * Mathf.Cos(angle) + perpDir * perpRadius * Mathf.Sin(angle));
                                angle += angleInc;
                            }
                            if (closed)
                            {
                                fullPoints.Add(fullPoints[0]);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }

            if (steps > 0)
            {
                var prevPoint = EvaluateLocal(0.0f);
                fullPoints.Add(prevPoint);

                float tInc = 1.0f / steps;
                float t = tInc;

                for (int i = 0; i < steps; i++)
                {
                    var newPt = EvaluateLocal(t);
                    if (Vector3.SqrMagnitude(newPt - prevPoint) > 1.0f)
                    {
                        fullPoints.Add(newPt);
                        prevPoint = newPt;
                    }

                    t += tInc;
                }
            }

            if (isLocalSpace)
            {
                for (int i = 0; i < fullPoints.Count; i++)
                {
                    fullPoints[i] = transform.TransformPoint(fullPoints[i]);
                }
            }

            dirty = false;

            return fullPoints;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) Path.GetRawDescription";
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((points == null) || (points.Count == 0))
            {
                if (type == Type.Circle)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No points defined: for a circle we need at least 2 points!", "For a circle we need at least 2 points (center and a radius)"));
                }
                else if (type == Type.Arc)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No points defined: for an arc we need at least 3 points!", "For an arc we need at least 3 points (center and two positions for the radius and directions of the arc)"));
                }
                else if (type == Type.Polygon)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No points defined: for a polygon we need at least 2 points!", "For a polygon we need at least 2 points (center and a radius)"));
                }
                else
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No points defined: add points!", "To have a path, we need to have some points..."));
                }
            }
            else
            {
                if ((type == Type.Circle) && (points.Count < 2))
                    {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Not enough points defined: for a circle we need at least 2 points!", "For a circle we need at least 2 points (center and a radius)"));
                }
                else if ((type == Type.Arc) && (points.Count < 3))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Not enough points defined: for an arc we need at least 3 points!", "For an arc we need at least 3 points (center and two positions for the radius and directions of the arc)"));
                }
                else if ((type == Type.Polygon) && (points.Count < 2))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Not enough points defined: for a polygon we need at least 2 points!", "For a polygon we need at least 2 points (center and a radius)"));
                }
                else if (points.Count < 2)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Need more points: add points!", "To have a path, we need to have more than one point..."));
                }
            }
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation = description;

            return _explanation;
        }

        private void OnDrawGizmos()
        {
            if (onlyDisplayWhenSelected) return;

            foreach (var obj in Selection.gameObjects)
            {
                if (obj == gameObject) return;
            }

            Handles.color = displayColor;

            var renderPoints = GetPoints();
            for (int i = 1; i < renderPoints.Count; i++)
            {
                Handles.DrawLine(renderPoints[i - 1], renderPoints[i], 1.0f);
            }
        }

        private void ComputeVariables()
        {
            // Compute length
            linearLength = 0.0f;
            for (int i = 0; i < points.Count - ((closed)?(0):(1)); i++)
            {
                linearLength += Vector3.Distance(points[i], points[(i + 1) % points.Count]);
            }

            // Compute generic values for circle, arc and polygon
            if (points.Count >= 2)
            {
                primaryRadius = Vector3.Distance(points[0], points[1]);
                primaryDir = (points[1] - points[0]).normalized;
                perpDir = new Vector2(primaryDir.y, -primaryDir.x);
                perpRadius = primaryRadius;
                if (points.Count >= 3)
                {
                    secondaryDir = points[2] - points[0];
                    if (Vector2.Dot(perpDir, secondaryDir) < 0) perpDir = -perpDir;
                    perpRadius = Vector2.Dot(perpDir, secondaryDir);

                    secondaryRadius = secondaryDir.magnitude;
                    secondaryDir /= secondaryRadius;

                    startAngle = Mathf.Atan2(primaryDir.y, primaryDir.x);
                    endAngle = Mathf.Atan2(secondaryDir.y, secondaryDir.x);

                    if (endAngle < startAngle) endAngle += Mathf.PI * 2.0f;
                }
            }
        }

        public Vector3 upAxis => primaryDir;
        public float   upExtent => primaryRadius;
        public Vector3 rightAxis => perpDir;
        public float rightExtent => perpRadius;

        public void SetDirty()
        {
            dirty = true;
        }
    }
}
