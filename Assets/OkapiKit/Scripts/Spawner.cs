using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using static UnityEditor.Progress;

public class Spawner : MonoBehaviour
{
    [System.Serializable] enum SpawnPointType { Random, Sequence, All };
    [System.Serializable] enum Modifiers 
    {
        None = 0,
        Scale = 1 << 0,
        Speed = 1 << 1,
    }

    [SerializeField] 
    private bool            forceCount = false;
    [SerializeField, ShowIf("forceCount")] 
    private int             numberOfEntities = 1;
    [SerializeField, HideIf("forceCount")]
    private bool            usePulsePattern = false;
    [SerializeField, ShowIf("hasPulsePattern")]
    private string          pulsePattern = "xoxo";
    [SerializeField, ShowIf("hasPulsePattern")]
    private float           pulseTime = 0.1f;
    [SerializeField]
    private GameObject[]    prefabs;
    [SerializeField, ShowIf("needSpawnPoints")]
    private Transform[]     spawnPoints;
    [SerializeField, ShowIf("hasMultipleSpawnPoints")]
    private SpawnPointType  spawnPointType = SpawnPointType.Random;
    [SerializeField, EnumFlags]
    private Modifiers       modifiers = Modifiers.None;
    [SerializeField, MinMaxSlider(0.0f, 2.0f), ShowIf("modifierScale")]
    private Vector2         scaleVariance = new Vector2(1.0f, 1.0f);
    [SerializeField, MinMaxSlider(-2.0f, 2.0f), ShowIf("modifierSpeed")]
    private Vector2         speedVariance = new Vector2(1.0f, 1.0f);
    [SerializeField]
    private bool            setParent = false;

    private bool needSpawnPoints => GetComponent<BoxCollider2D>() == null;
    private bool modifierScale => (modifiers & Modifiers.Scale) != 0;
    private bool modifierSpeed => (modifiers & Modifiers.Speed) != 0;
    private bool hasPulsePattern => (!forceCount) && (usePulsePattern);

    private BoxCollider2D       spawnArea;
    private int                 spawnPointIndex;
    private List<GameObject>    items;

    private bool hasMultipleSpawnPoints => (spawnPoints != null) && (spawnPoints.Length > 1);

    private void Start()
    {
        items = new List<GameObject>();
        spawnArea = GetComponent<BoxCollider2D>();
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
                Vector3     position = Vector3.zero;
                Quaternion  rotation = Quaternion.identity;
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
                else if (spawnArea)
                {
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
}
