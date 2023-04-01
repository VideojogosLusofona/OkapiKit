using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class Spawner : OkapiElement
    {
        public enum SpawnPointType { Random = 0, Sequence = 1, All = 2 };
        public enum Modifiers
        {
            None = 0,
            Scale = 1 << 0,
            Speed = 1 << 1,
        }

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
        private SpawnPointType spawnPointType = SpawnPointType.Random;
        [SerializeField, EnumFlags]
        private Modifiers modifiers = Modifiers.None;
        [SerializeField, MinMaxSlider(0.0f, 2.0f)]
        private Vector2 scaleVariance = new Vector2(1.0f, 1.0f);
        [SerializeField, MinMaxSlider(-2.0f, 2.0f)]
        private Vector2 speedVariance = new Vector2(1.0f, 1.0f);
        [SerializeField]
        private bool setParent = false;

        private BoxCollider2D[] spawnAreas;
        private int spawnPointIndex;
        private List<GameObject> items;

        private bool hasPulsePattern => (!forceCount) && (usePulsePattern);

        private void Start()
        {
            items = new List<GameObject>();
            spawnAreas = GetComponents<BoxCollider2D>();
            spawnPointIndex = 0;
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
                if (spawnPointType == SpawnPointType.All) c = spawnPoints.Length;

                for (int i = 0; i < c; i++)
                {
                    Vector3 position = Vector3.zero;
                    Quaternion rotation = Quaternion.identity;
                    if ((spawnPoints != null) && (spawnPoints.Length > 0))
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
                    else if ((spawnAreas != null) && (spawnAreas.Length > 0))
                    {
                        var ra = Random.Range(0, spawnAreas.Length);
                        var spawnArea = spawnAreas[ra];

                        float x = 0.5f * Random.Range(-spawnArea.size.x, spawnArea.size.x) + spawnArea.offset.x;
                        float y = 0.5f * Random.Range(-spawnArea.size.y, spawnArea.size.y) + spawnArea.offset.y;

                        position = transform.TransformPoint(new Vector3(x, y, 0));
                        rotation = transform.rotation;
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

        public override string UpdateExplanation()
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
                _explanation += $"When triggered by a [Spawn] action, this component will create new objects.\n";
                if (usePulsePattern)
                {
                    _explanation += $"Every spawn trigger will generate {pulsePattern.Length} objects, ";
                    if (pulseTime > 0) _explanation += $"one every {pulseTime} seconds, following a\n[{pulsePattern}] pattern, where an 'x' is a spawn, and a 'o' is a pause.\n";
                    else _explanation += $"following a [{pulsePattern}] pattern,\nwhere an 'x' is a spawn, and a 'o' is a pause.\n";
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
                    if (prefabs.Length > 0)
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
            if ((colliders != null) && (colliders.Length > 0))
            {
                if (colliders.Length > 1)
                    _explanation += $"These objects will spawn anywhere in the area defined by any of the {colliders.Length}\nBoxCollider2D attached to this object.\n";
                else
                    _explanation += $"These objects will spawn inside the BoxCollider2D attached to this object.\n";
            }
            else if ((spawnPoints != null) && (spawnPoints.Length > 0))
            {
                if (spawnPoints.Length > 1)
                {
                    _explanation += "These objects will spawn on any of the positions:\n";
                    foreach (var spawnPoint in spawnPoints)
                    {
                        if (spawnPoint == null) _explanation += $"      [Nothing]\n";
                        else _explanation += $"      [{spawnPoint.name}]\n";
                    }
                }
                else
                {
                    if (spawnPoints[0])
                        _explanation += $"These objects will spawn will spawn at the position of [{spawnPoints[0].name}].\n";
                    else
                        _explanation += $"These objects will spawn will spawn at the position of this object.\n";
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

        public int GetSpawnPointCount()
        {
            if (spawnPoints == null) return 0;
            return spawnPoints.Length;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) Spawner.GetRawDescription";
        }
    }
}