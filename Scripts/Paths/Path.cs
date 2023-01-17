using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class Path : MonoBehaviour
{
    [SerializeField] private List<Vector3>  points;

    [SerializeField] private bool           worldSpace = true;
    [SerializeField] private bool           editMode = false;

    public List<Vector3>    GetPoints() => new List<Vector3>(points);
    public void             SetPoints(List<Vector3> inPoints) => points = new List<Vector3>(inPoints);

    public bool             isEditMode => editMode;
    public bool             isWorldSpace => worldSpace;
    public bool             isLocalSpace => !worldSpace;

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

    public Vector3 GetWorldPosition(int index)
    {
        if ((index < 0) || (index >= points.Count)) return Vector3.zero;

        Vector3 pt = points[index];

        if (isLocalSpace)
        {
            pt = transform.TransformPoint(pt);
        }

        return pt;
    }

    public int GetPathSize() => (points != null) ? (points.Count) : (0);
}
