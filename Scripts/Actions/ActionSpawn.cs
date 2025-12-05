using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
#if UNITY_EDITOR
using UnityEditor;
#endif
using System;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Spawn")]
    public class ActionSpawn : Action
    {
        public enum SpawnPosition { Default, This, Target, Tag };
        private Spawner spawner;

        [SerializeField, ShowIf("needObject")]
        private GameObject prefabObject;
        [SerializeField, ShowIf("hasPrefab")]
        private SpawnPosition spawnPosition = SpawnPosition.Default;
        [SerializeField, ShowIf("needsTransform")]
        private Transform targetPosition;
        [SerializeField, ShowIf("needsTag")]
        private Hypertag targetTag;
        [SerializeField, ShowIf("needParent")]
        private bool setParent = false;

        private bool hasPrefab => prefabObject != null;
        private bool needObject => GetComponent<Spawner>() == null;
        private bool needsTransform => (prefabObject != null) && (spawnPosition == SpawnPosition.Target);
        private bool needsTag => (prefabObject != null) && (spawnPosition == SpawnPosition.Tag);
        private bool needParent => spawnPosition != SpawnPosition.Default;

        public override string GetActionTitle() { return "Spawn"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            spawner = GetComponent<Spawner>();
            if (spawner != null)
            {
                desc += $"spawns an entity using spawner {name}";
            }
            else
            {
                if (prefabObject)
                {
                    desc += $"spawns prefab {prefabObject.name}";
                    switch (spawnPosition)
                    {
                        case SpawnPosition.Default:
                            desc += " at original position";
                            break;
                        case SpawnPosition.This:
                            desc += $" at the position of this object ({name})";
                            break;
                        case SpawnPosition.Target:
                            var targetName = (targetPosition) ? (targetPosition.name) : ("UNDEFINED");
                            desc += $" at the position of {targetName}";
                            break;
                        case SpawnPosition.Tag:
                            var tagName = (targetTag) ? (targetTag.name) : ("UNDEFINED");
                            desc += $" at the position of object with tag [{tagName}] (if multiple objects have the same tag, selects a random one)";
                            break;
                        default:
                            break;
                    }
                    if ((needParent) && (setParent))
                    {
                        desc += ", setting it as parent";
                    }
                }
                else
                {
                    desc += $"spawns an entity, but there's no prefab nor a spawner defined!";
                }
            }
            return desc;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            var spawner = GetComponent<Spawner>();
            if (spawner == null)
            {
                if (prefabObject == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Spawn prefab not defined!\nEither define a prefab to spawn, or add a Spawner system on this object!", "This action spawns (creates) a new object, so we need to define which object we want to create.\nWe can do that by defining a prefab object, or if we want to do something more complex (like selecting a random object from a list, or spawning randomly inside of an area, or at certain points), we need to create a Spawner system on this object. "));
                }
                else
                {
#if UNITY_EDITOR
                    if ((PrefabUtility.GetPrefabAssetType(prefabObject) == PrefabAssetType.NotAPrefab) ||
                        (prefabObject.scene == null) ||
                        (prefabObject.scene.rootCount != 0))
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Spawn object is not a prefab!", "Object needs to be a prefab, not an object that belongs to the scene, because those can be destroyed.\nA prefab is an object that doesn't belong to a scene, but belongs to the project (so it's on the Project view, not on the Hierarchy).\nTo create a new prefab, just drag the object from the hierarchy to the project.\nIf the object is already a prefab object by itself, select the original object on the project view, instead of the hierarchy view."));
                    }
#endif

                    if (spawnPosition == SpawnPosition.Target)
                    {
                        if (targetPosition == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, "Target position is not set!", "If you want to spawn the object at a particular point, you need to provide which point"));
                        }
                    }
                    if (spawnPosition == SpawnPosition.Tag)
                    {
                        if (targetTag == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, "Target tag is not set!", "If you want to spawn the object at the position of an object with a specific tag, please define what tag.\nIf there's multiple objects with the same tag, a random one will be chosen."));
                        }
                    }
                }
            }
            else
            {
                spawner.ForceCheckErrors(level + 1);
                var actionLogs = spawner.logs;
                foreach (var log in actionLogs)
                {
                    _logs.Add(new LogEntry(log.type, $"On spawner: " + log.text, log.tooltip));
                }
            }    
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if (spawner == null)
            {
                spawner = GetComponent<Spawner>();
            }
            if (spawner != null)
            {
                spawner.Spawn();
            }
            else
            {
                GameObject newObject = null;
                if (prefabObject)
                {
                    switch (spawnPosition)
                    {
                        case SpawnPosition.Default:
                            newObject = Instantiate(prefabObject);
                            break;
                        case SpawnPosition.This:
                            {
                                newObject = Instantiate(prefabObject, transform.position, transform.rotation);
                                if (setParent)
                                {
                                    newObject.transform.SetParent(transform);
                                }
                            }
                            break;
                        case SpawnPosition.Target:
                            {
                                newObject = Instantiate(prefabObject, targetPosition.position, targetPosition.rotation);
                                if (setParent)
                                {
                                    newObject.transform.SetParent(targetPosition.transform);
                                }
                            }
                            break;
                        case SpawnPosition.Tag:
                            {
                                var targetObjs = HypertaggedObject.FindGameObjectsWithHypertag(targetTag);
                                if ((targetObjs != null) && (targetObjs.Count > 0))
                                {
                                    var targetObj = targetObjs[UnityEngine.Random.Range(0, targetObjs.Count)];
                                    if (targetObj)
                                    {
                                        newObject = Instantiate(prefabObject, targetObj.transform.position, targetObj.transform.rotation);
                                        if (setParent)
                                        {
                                            newObject.transform.SetParent(targetObj.transform);
                                        }
                                    }
                                }
                            }
                            break;
                        default:
                            break;
                    }

                    if (newObject != null)
                    {
                        if (!setParent)
                        {
                            GridObject gridObject = newObject.GetComponent<GridObject>();
                            if (gridObject)
                            {
                                // This object has a grid object, need to parent it to something that has a grid
                                GridObject selfGridObject = GetComponent<GridObject>();
                                GridSystem parentGridObject;
                                if (selfGridObject == null) parentGridObject = GetComponentInParent<GridSystem>();
                                else parentGridObject = selfGridObject.grid;
                                if (parentGridObject == null) parentGridObject = FindAnyObjectByType<GridSystem>();
                                if (parentGridObject != null)
                                {
                                    newObject.transform.SetParent(parentGridObject.transform);
                                }
                            }
                        }
                    }

                    return;
                }
            }
        }
    }
}