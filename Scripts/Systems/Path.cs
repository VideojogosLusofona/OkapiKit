using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class Path : OkapiElement
    {
        [SerializeField] public enum PathType { Linear, Smooth };

        [SerializeField]
        private PathType type = PathType.Linear;
        [SerializeField, Range(0.0f, 2.0f)]
        private float tension = 1.0f;
        [SerializeField]
        private List<Vector3> points;

        [SerializeField]
        private bool worldSpace = true;
        [SerializeField]
        private bool editMode = false;

        private bool isSmooth => type == PathType.Smooth;

        public List<Vector3> GetEditPoints() => (points == null) ? (null) : (new List<Vector3>(points));
        public void SetEditPoints(List<Vector3> inPoints) => points = new List<Vector3>(inPoints);
        public PathType GetPathType() => type;
        public void SetPathType(PathType t) => type = t;
        public float GetTension() => tension;
        public void SetTension(float t) => tension = t;
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
                points.Add(points[points.Count - 1] + new Vector3(1, 1, 0));
            }
            else
            {
                // Create point in (0,0,0)
                points.Add(Vector3.zero);
            }
        }

        private Vector3 ComputeBezier(Vector3[] pt, float t)
        {
            float it = (1 - t);
            float t2 = t * t;
            float t3 = t2 * t;
            float it2 = it * it;
            float it3 = it2 * it;

            return pt[0] * it3 + 3 * pt[1] * it2 * t + 3 * pt[2] * it * t2 + pt[3] * t3;
        }

        public List<Vector3> GetPoints()
        {
            List<Vector3> ret;
            if (points == null) return new List<Vector3>();

            if ((type == PathType.Linear) || (points.Count < 3))
            {
                ret = new List<Vector3>(points);
            }
            else
            {
                ret = new List<Vector3>();

                if (points.Count < 3) return points;
                Vector3[] pt = null;
                for (int i = 0; i < points.Count - 2; i += 2)
                {
                    if (i > 0)
                    {
                        pt = new Vector3[] { points[i],
                                         points[i] + tension * (points[i] - points[i - 1]),
                                         points[i + 2] + tension * (points[i + 1] - points[i + 2]),
                                         points[i + 2] };
                    }
                    else
                    {
                        pt = new Vector3[] { points[i],
                                         points[i] + tension * (points[i + 1] - points[i]),
                                         points[i + 2] + tension * (points[i + 1] - points[i + 2]),
                                         points[i + 2] };
                    }

                    // Compute bezier
                    float t = 0.0f;
                    float tInc = 1.0f / (float)20;

                    ret.Add(ComputeBezier(pt, t));
                    t += tInc;
                    while (t <= 1.0f)
                    {
                        ret.Add(ComputeBezier(pt, t));

                        t += tInc;
                    }
                }
                Vector3 lastPoint = ComputeBezier(pt, 1.0f);
                if (Vector3.Distance(lastPoint, ret[ret.Count - 1]) > 1e-6)
                {
                    ret.Add(lastPoint);
                }
            }

            if (isLocalSpace)
            {
                for (int i = 0; i < ret.Count; i++)
                {
                    ret[i] = transform.TransformPoint(ret[i]);
                }
            }

            return ret;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) Path.GetRawDescription";
        }

        public override string UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation = description;

            return _explanation;
        }
    }
}