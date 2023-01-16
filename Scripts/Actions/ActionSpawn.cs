using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class ActionSpawn : Action
{
    [SerializeField] private enum SpawnPosition { Default, This, Target, Tag };
    private Spawner         spawner;

    [SerializeField]
    private GameObject      prefabObject;
    [SerializeField, ShowIf("hasPrefab")]
    private SpawnPosition   spawnPosition = SpawnPosition.Default;
    [SerializeField, ShowIf("needsTransform")]
    private Transform       targetPosition;
    [SerializeField, ShowIf("needsTag")]
    private Hypertag        targetTag;
    [SerializeField, ShowIf("needParent")]
    private bool            setParent = false;

    private bool hasPrefab => prefabObject != null;
    private bool needsTransform => (prefabObject != null) && (spawnPosition == SpawnPosition.Target);
    private bool needsTag => (prefabObject != null) && (spawnPosition == SpawnPosition.Tag);
    private bool needParent => spawnPosition != SpawnPosition.Default;

    public override string GetRawDescription(string ident)
    {
        if (prefabObject)
        {
            return $"Spawns prefab {prefabObject.name}";
        }
        spawner = GetComponent<Spawner>();
        if (spawner == null)
        {
            return $"Spawns an entity using spawner, but there's no spawner in object!";
        }
        return $"Spawns an entity using spawner {name}";
    }

    public override void Execute()
    {
        if (!enableAction) return;

        if (prefabObject)
        {
            switch (spawnPosition)
            {
                case SpawnPosition.Default:
                    Instantiate(prefabObject);
                    break;
                case SpawnPosition.This:
                    {
                        var obj = Instantiate(prefabObject, transform.position, transform.rotation);
                        if (setParent)
                        {
                            obj.transform.SetParent(transform);
                        }
                    }
                    break;
                case SpawnPosition.Target:
                    {
                        var obj = Instantiate(prefabObject, targetPosition.position, targetPosition.rotation);
                        if (setParent)
                        {
                            obj.transform.SetParent(targetPosition.transform);
                        }
                    }
                    break;
                case SpawnPosition.Tag:
                    {
                        var targetObj = HypertaggedObject.FindGameObjectWithHypertag(targetTag);
                        if (targetObj)
                        {
                            var obj = Instantiate(prefabObject, targetObj.transform.position, targetObj.transform.rotation);
                            if (setParent)
                            {
                                obj.transform.SetParent(targetObj.transform);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
            return;
        }

        if (spawner == null)
        {
            spawner = GetComponent<Spawner>();
        }
        if (spawner != null)
        {
            spawner.Spawn();
        }
    }
}
