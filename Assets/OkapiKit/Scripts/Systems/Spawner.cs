using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System.IO;
using static Codice.Client.Commands.WkTree.WorkspaceTreeNode;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Spawner")]
    public class Spawner : OkapiElement
    {
        public enum SpawnPointType { Random = 0, Sequence = 1, All = 2 };
        public enum SpawnRotation { Default = 0, This = 1, AlignWithDirection = 2, AlignWithInverseDirection = 3, AlignWithPerpendicular = 4, AlignWithInversePerpendicular = 5 };
        public enum AlignAxis { Right = 0, Up = 1 };

        public enum Modifiers
        {
            None = 0,
            Scale = 1 << 0,
            Speed = 1 << 1,
        }

        [SerializeField]
        private int  spawnsPerTrigger = 1;
        [SerializeField]
        private bool forceCount = false;
        [SerializeField]
        private int numberOfEntities = 1;
        [SerializeField]
        private bool usePulsePattern = false;
        [SerializeField]
        private string pulsePattern = "xoxo";
        [SerializeField]
        private float pulseTime = 0.1f;
        [SerializeField]
        private GameObject[] prefabs;
        [SerializeField]
        private Transform[] spawnPoints;
        [SerializeField]
        private SpawnPointType  spawnPointType = SpawnPointType.Random;
        [SerializeField, Range(0.0f, 1.0f)] 
        private float           spawnPathSpacing = 0.25f;
        [SerializeField]
        private SpawnRotation   spawnRotation = SpawnRotation.Default;
        [SerializeField]
        private AlignAxis       spawnAlignmentAxis = AlignAxis.Right;
        [SerializeField, EnumFlags]
        private Modifiers modifiers = Modifiers.None;
        [SerializeField, MinMaxSlider(0.0f, 2.0f)]
        private Vector2 scaleVariance = new Vector2(1.0f, 1.0f);
        [SerializeField, MinMaxSlider(-2.0f, 2.0f)]
        private Vector2 speedVariance = new Vector2(1.0f, 1.0f);
        [SerializeField]
        private bool setParent = false;

        private BoxCollider2D[]     spawnAreas;
        private int                 spawnPointIndex;
        private Path                spawnPath;
        private float               spawnPointT;
        private List<GameObject>    items;

        private bool hasPulsePattern => (!forceCount) && (usePulsePattern);

        private void Start()
        {
            items = new List<GameObject>();
            spawnAreas = GetComponents<BoxCollider2D>();
            spawnPointIndex = 0;
            spawnPointT = 0.0f;
            spawnPath = GetComponent<Path>();
        }

        public void Update()
        {
            if (forceCount)
            {
                items.RemoveAll((go) => go == null);

                if (items.Count < numberOfEntities)
                {
                    for (int i = items.Count; i < numberOfEntities; i++)
                    {
                        SingleSpawn();
                    }
                }
            }
        }

        public void Spawn()
        {
            for (int j = 0; j < spawnsPerTrigger; j++)
            {
                if (hasPulsePattern)
                {
                    if (pulseTime > 0)
                    {
                        StartCoroutine(SpawnCR());
                    }
                    else
                    {
                        for (int i = 0; i < pulsePattern.Length; i++)
                        {
                            if (pulsePattern[i] == 'x') SingleSpawn();
                        }
                    }
                }
                else
                {
                    SingleSpawn();
                }
            }
        }

        IEnumerator SpawnCR()
        {
            for (int i = 0; i < pulsePattern.Length; i++)
            {
                if (pulsePattern[i] == 'x') SingleSpawn();
                yield return new WaitForSeconds(pulseTime);
            }
        }

        void SingleSpawn()
        {
            int r = Random.Range(0, prefabs.Length);
            var prefab = prefabs[r];
            if (prefab != null)
            {
                int c = 1;
                if ((spawnAreas == null) || (spawnAreas.Length == 0))
                {
                    if (spawnPointType == SpawnPointType.All) c = spawnPoints.Length;
                }
                if (spawnPath != null)
                {
                    if (spawnPointType == SpawnPointType.All) c = Mathf.FloorToInt(1.0f / spawnPathSpacing);
                }

                for (int i = 0; i < c; i++)
                {
                    Vector3 position = Vector3.zero;
                    Quaternion rotation = Quaternion.identity;
                    if ((spawnAreas != null) && (spawnAreas.Length > 0))
                    {
                        var ra = Random.Range(0, spawnAreas.Length);
                        var spawnArea = spawnAreas[ra];

                        float x = 0.5f * Random.Range(-spawnArea.size.x, spawnArea.size.x) + spawnArea.offset.x;
                        float y = 0.5f * Random.Range(-spawnArea.size.y, spawnArea.size.y) + spawnArea.offset.y;

                        position = transform.TransformPoint(new Vector3(x, y, 0));
                        rotation = transform.rotation;
                    }
                    else if (spawnPath != null)
                    {
                        if(spawnPointType == SpawnPointType.Random)
                        {
                            spawnPointT = Random.Range(0.0f, 1.0f);
                        }

                        position = spawnPath.EvaluateWorld(spawnPointT);

                        switch (spawnRotation)
                        {
                            case SpawnRotation.Default:
                                rotation = prefab.transform.rotation;
                                break;
                            case SpawnRotation.This:
                                rotation = transform.rotation;
                                break;
                            case SpawnRotation.AlignWithDirection:
                            case SpawnRotation.AlignWithInverseDirection:
                            case SpawnRotation.AlignWithPerpendicular:
                            case SpawnRotation.AlignWithInversePerpendicular:
                                {
                                    Vector2 dir = Vector2.zero;
                                    Vector2 up = Vector2.zero;

                                    GetWorldDirUp(spawnPath, spawnPointT, ref dir, ref up);
                                    if (spawnAlignmentAxis == AlignAxis.Up)
                                    {
                                        (dir, up) = (up, dir);
                                    }

                                    rotation = Quaternion.LookRotation(Vector3.forward, up);
                                }
                                break;
                        }
                        if (spawnPointT < 1.0f)
                        {
                            spawnPointT += spawnPathSpacing;
                            if (Mathf.Abs(1.0f - spawnPointT) < 1e-6) spawnPointT = 1.0f;
                            else if (spawnPointT >= 1.0f) spawnPointT = 0.0f;
                        }
                        else
                        {
                            spawnPointT = 0.0f;
                        }
                    }
                    else if ((spawnPoints != null) && (spawnPoints.Length > 0))
                    {
                        int p;
                        if (spawnPointType == SpawnPointType.All)
                        {
                            p = i;
                        }
                        else if (spawnPointType == SpawnPointType.Sequence)
                        {
                            p = spawnPointIndex;
                            spawnPointIndex = (spawnPointIndex + 1) % spawnPoints.Length;
                        }
                        else
                        {
                            p = Random.Range(0, spawnPoints.Length);
                        }

                        position = spawnPoints[p].position;
                        rotation = spawnPoints[p].rotation;
                    }
                    else
                    {
                        position = transform.position;
                        rotation = transform.rotation;
                    }


                    GameObject newObject = Instantiate(prefab, position, rotation);
                    if (forceCount) items.Add(newObject);
                    if (setParent)
                    {
                        newObject.transform.SetParent(transform);
                    }

                    if ((modifiers & Modifiers.Scale) != 0)
                    {
                        float s = Random.Range(scaleVariance.x, scaleVariance.y);
                        newObject.transform.localScale = newObject.transform.localScale * s;
                    }

                    if ((modifiers & Modifiers.Speed) != 0)
                    {
                        Movement movement = newObject.GetComponent<Movement>();
                        if (movement)
                        {
                            float s = Random.Range(speedVariance.x, speedVariance.y);

                            var speed = movement.GetSpeed() * s;
                            movement.SetSpeed(speed);
                        }
                    }
                }
            }
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            if (forceCount)
            {
                _explanation += $"This spawner will guarantee that at least {numberOfEntities} objects generated by him will \n";
                _explanation += "be present in the scene. When an object created by him is destroyed, another\n";
                _explanation += "will immediately take his place. This spawner can also be triggered manually.\n";
            }
            else
            {
                if (usePulsePattern)
                {
                    _explanation += $"When triggered by a [Spawn] action, this component will create execute the pulse pattern, {spawnsPerTrigger} times.\n";
                    _explanation += $"Every spawn trigger will generate {pulsePattern.Length} objects, ";
                    if (pulseTime > 0) _explanation += $"one every {pulseTime} seconds, following a\n[{pulsePattern}] pattern, where an 'x' is a spawn, and a 'o' is a pause.\n";
                    else _explanation += $"following a [{pulsePattern}] pattern,\nwhere an 'x' is a spawn, and a 'o' is a pause.\n";
                }
                else
                {
                    _explanation += $"When triggered by a [Spawn] action, this component will create {spawnsPerTrigger} objects.\n";
                }
            }
            if (prefabs != null)
            {
                if (prefabs.Length > 1)
                {
                    _explanation += "The generated object will be chosen randomly from:\n";
                    foreach (var prefab in prefabs)
                    {
                        if (prefab == null) _explanation += $"      [Nothing]\n";
                        else _explanation += $"      [{prefab.name}]\n";
                    }
                }
                else
                {
                    if ((prefabs.Length > 0) && (prefabs[0] != null))
                    {
                        _explanation += $"The generated object will be [{prefabs[0].name}].\n";
                    }
                    else
                    {
                        _explanation += "There's currently no defined object to spawn.\n";
                    }
                }
            }
            else
            {
                _explanation += "There's currently no defined object to spawn.\n";
            }
            var colliders = GetComponents<BoxCollider2D>();
            var path = GetComponent<Path>();
            if ((colliders != null) && (colliders.Length > 0))
            {
                if (colliders.Length > 1)
                    _explanation += $"These objects will spawn anywhere in the area defined by any of the {colliders.Length}\nBoxCollider2D attached to this object.\n";
                else
                    _explanation += $"These objects will spawn inside the BoxCollider2D attached to this object.\n";
            }
            else if (path != null)
            {
                if (spawnPointType == SpawnPointType.Random)
                {
                    _explanation += $"These objects will spawn on a random position of the path attached to this object.\n";
                }
                else if (spawnPointType == SpawnPointType.Sequence)
                {
                    _explanation += $"Each spawn will happen on a position on the path attached to this object, in a sequence, \nwith spacing {spawnPathSpacing * 100}% of the path length.\n";
                }
                else if (spawnPointType == SpawnPointType.All)
                {
                    _explanation += $"Every spawn will fill the path attached to this object, with spacing {spawnPathSpacing * 100}% of the path length.\n";
                }

                string axisName = (spawnAlignmentAxis == AlignAxis.Right) ? ("right axis") : ("up axis");
                switch (spawnRotation)
                {
                    case SpawnRotation.Default:
                        _explanation += $"The new object will spawn with the original rotation.\n";
                        break;
                    case SpawnRotation.This:
                        _explanation += $"The new object will spawn with the rotation of this object.\n";
                        break;
                    case SpawnRotation.AlignWithDirection:
                        _explanation += $"The new object will align it's {axisName} with the orientation of the path.\n";
                        break;
                    case SpawnRotation.AlignWithInverseDirection:
                        _explanation += $"The new object will align it's {axisName} with the inverse of the orientation of the path.\n";
                        break;
                    case SpawnRotation.AlignWithPerpendicular:
                        _explanation += $"The new object will align it's {axisName} with the outside direction of the path.\n";
                        break;
                    case SpawnRotation.AlignWithInversePerpendicular:
                        _explanation += $"The new object will align it's {axisName} with the inside direction of the path.\n";
                        break;
                    default:
                        break;
                }
            }
            else if ((spawnPoints != null) && (spawnPoints.Length > 0))
            {
                if (spawnPoints.Length > 1)
                {
                    if (spawnPointType == SpawnPointType.Random)
                    {
                        _explanation += "Each object will spawn at a random selection between these positions/rotations:\n";
                    }
                    else if (spawnPointType == SpawnPointType.Sequence)
                    {
                        _explanation += "Each object will spawn on one of these positions/rotations, in sequence:\n";
                    }
                    else if (spawnPointType == SpawnPointType.All)
                    {
                        _explanation += "An object will spawn at each of these positions/rotations:\n";
                    }
                    foreach (var spawnPoint in spawnPoints)
                    {
                        if (spawnPoint == null) _explanation += $"      [Nothing]\n";
                        else _explanation += $"      [{spawnPoint.name}]\n";
                    }
                }
                else
                {
                    if (spawnPoints[0])
                    {
                        _explanation += $"These objects will spawn will spawn at the position of [{spawnPoints[0].name}].\n";
                    }
                    else
                    {
                        _explanation += $"These objects will spawn will spawn at the position of this object.\n";
                    }
                }
            }
            else
            {
                _explanation += $"These objects will spawn will spawn at the position of this object.\n";
            }
            if (modifiers != 0)
            {
                if ((modifiers & Modifiers.Scale) != 0)
                {
                    if ((modifiers & Modifiers.Speed) != 0)
                    {
                        _explanation += $"The new objects will vary in scale from {scaleVariance.x * 100}% to {scaleVariance.y * 100}%,\n";
                        _explanation += $"and in speed from {speedVariance.x * 100}% to {speedVariance.y * 100}%.\n";
                    }
                    else
                    {
                        _explanation += $"The new objects will vary in scale from {scaleVariance.x * 100}% to {scaleVariance.y * 100}%.\n";
                    }
                }
                else
                {
                    _explanation += $"The new objects will vary in speed from {speedVariance.x * 100}% to {speedVariance.y * 100}%.\n";
                }
            }
            if (setParent)
            {
                _explanation += "They will also be placed as children of this object.";
            }

            return _explanation;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

#if UNITY_EDITOR
            if ((prefabs == null) || (prefabs.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Spawn prefabs not defined!", "This system spawns (creates) a new object, so we need to define which object we want to create.\nWe can do that by defining a list of prefab objects, and a random one will be chosen everytime a spawn should be performed."));
            }
            else
            {
                int index = 0;
                foreach (var prefab in prefabs)
                {
                    if (prefab == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Prefab slot is empty in prefab list (index={index})!", "Empty can't be spawned, so we should define something"));
                    }
                    else if ((PrefabUtility.GetPrefabAssetType(prefab) == PrefabAssetType.NotAPrefab) ||
                             (prefab.scene == null) ||
                             (prefab.scene.rootCount != 0))
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Spawn object in slot {index} is not a prefab!", "Object needs to be a prefab, not an object that belongs to the scene, because those can be destroyed.\nA prefab is an object that doesn't belong to a scene, but belongs to the project (so it's on the Project view, not on the Hierarchy).\nTo create a new prefab, just drag the object from the hierarchy to the project.\nIf the object is already a prefab object by itself, select the original object on the project view, instead of the hierarchy view."));
                    }
                    index++;
                }
            }
#endif

            // Check if there's colliders
            var collider = GetComponent<BoxCollider2D>();
            if (collider == null)
            {
                var path = GetComponent<Path>();
                if (path != null)
                {
                    if (spawnPointType != SpawnPointType.Random)
                    {
                        if (spawnPathSpacing == 0.0f)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Spacing is equal to zero, it should be larger than zero!", "To spawn objects along the curve, we need to know the spacing between objects!"));
                        }
                    }
                }
                else if((spawnPoints == null) || (spawnPoints.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "Missing explicit definition of spawn position.", "As it is, the objects will spawn at the position of this object. If that's what's intended, you can just move the GameObject to the Spawn Points list below.\nYou have different alternatives to spawn objects:\nPoints: Objects spawn at the position of a randomly chosen point in the Spawn Points list.\nBox Collider: You can add BoxCollider2d components and those will define the areas where the objects will spawn.\nPath: You can add a Path component on this object, and the objects will spawn along that path, in the defined fashion."));
                }
                else
            {
                    int index = 0;
                    foreach (var spawnPoint in spawnPoints)
                    {
                        if (spawnPoint == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Spawn point is empty in prefab list (index={index})!", "An empty point will cause the object not to spawn if this empty slot is chosen during object spawn."));
                        }
                        index++;
                    }
                }
            }
        }

        public void ForceCheckErrors()
        {
            _logs.Clear();
            CheckErrors();
        }

        public int GetSpawnPointCount()
        {
            if (spawnPoints == null) return 0;
            return spawnPoints.Length;
        }

        void GetWorldDirUp(Path path, float t, ref Vector2 dir, ref Vector2 up)
        {
            switch (spawnRotation)
            {
                case SpawnRotation.AlignWithDirection:
                    dir = path.EvaluateWorldDir(t);
                    up = new Vector2(dir.y, -dir.x);
                    break;
                case SpawnRotation.AlignWithInverseDirection:
                    dir = -path.EvaluateWorldDir(t);
                    up = new Vector2(dir.y, -dir.x);
                    break;
                case SpawnRotation.AlignWithPerpendicular:
                    up = path.EvaluateWorldDir(t);
                    dir = new Vector2(up.y, -up.x);
                    break;
                case SpawnRotation.AlignWithInversePerpendicular:
                    up = path.EvaluateWorldDir(t);
                    dir = new Vector2(-up.y, up.x);
                    break;
                default:
                    break;
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) Spawner.GetRawDescription";
        }

        private void OnDrawGizmosSelected()
        {
            const float circleSize = 5.0f;
            const float arrowSize = 10.0f;

            var spawnAreas = GetComponents<BoxCollider2D>();
            if ((spawnAreas != null) && (spawnAreas.Length > 0))
            {
                foreach (var collider in spawnAreas)
                {
                    Vector3 pA = collider.offset - collider.size * 0.5f;

                    var p1 = transform.transform.TransformPoint(pA);
                    var p2 = transform.transform.TransformPoint(pA + collider.size.x * Vector3.right);
                    var p3 = transform.transform.TransformPoint(pA + collider.size.x * Vector3.right + collider.size.y * Vector3.up);
                    var p4 = transform.transform.TransformPoint(pA + collider.size.y * Vector3.up);

                    Gizmos.color = Color.cyan;
                    int c = 10;
                    float t = 0.0f;
                    float tInc = 1.0f / c;

                    for (int i = 0; i < c; i++)
                    {
                        Vector3 pa = Vector3.Lerp(p1, p2, t);
                        Vector3 pb = Vector3.Lerp(p3, p2, t);

                        Gizmos.DrawLine(pa, pb);

                        pa = Vector3.Lerp(p3, p4, t);
                        pb = Vector3.Lerp(p1, p4, t);

                        Gizmos.DrawLine(pa, pb);

                        t += tInc;
                    }

                    Gizmos.DrawLine(p1, p2);
                    Gizmos.DrawLine(p2, p3);
                    Gizmos.DrawLine(p3, p4);
                    Gizmos.DrawLine(p4, p1);
                }
            }
            else 
            {
                var path = GetComponent<Path>();
                if (path)
                {
                    if (spawnPathSpacing > 0.0f)
                    {
                        if ((spawnPointType == SpawnPointType.Sequence) ||
                            (spawnPointType == SpawnPointType.All))
                        {
                            float t = 0.0f;
                            while (t <= 1.0f)
                            {
                                var pt = path.EvaluateWorld(t);

                                Gizmos.color = Color.cyan;
                                Gizmos.DrawWireSphere(pt, circleSize);

                                Gizmos.color = (spawnAlignmentAxis == AlignAxis.Right) ? Color.red : Color.green;

                                Vector2 dir = Vector2.zero;
                                Vector2 up = Vector2.zero;

                                GetWorldDirUp(path, t, ref dir, ref up);

                                DrawGizmoArrow(pt, dir, up, arrowSize);

                                t += spawnPathSpacing;
                            }
                        }
                    }
                }
                else
                {
                    foreach (var pt in spawnPoints)
                    {
                        Gizmos.color = Color.cyan;
                        Gizmos.DrawWireSphere(pt.position, circleSize);

                        Gizmos.color = Color.green;
                        DrawGizmoArrow(pt.position, pt.right, pt.up, arrowSize);
                    }
                }
            }            
        }

        void DrawGizmoArrow(Vector2 pt, Vector2 dir, Vector2 up, float arrowSize)
        {
            Vector2 arrowTip = pt + dir * arrowSize * 2.0f;
            Gizmos.DrawLine(pt, arrowTip);
            Gizmos.DrawLine(arrowTip, arrowTip - dir * arrowSize * 0.5f + up * arrowSize * 0.5f);
            Gizmos.DrawLine(arrowTip, arrowTip - dir * arrowSize * 0.5f - up * arrowSize * 0.5f);
        }
    }
}