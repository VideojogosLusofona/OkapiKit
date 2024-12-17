using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{

    [AddComponentMenu("Okapi/Action/Teleport")]
    public class ActionTeleport : Action
    {
        public enum TeleportSubject { Self, Target, Tag, Collider };
        public enum TeleportTarget { Target, Tag };

        [SerializeField]
        private TeleportSubject     subject;
        [SerializeField]
        private Transform           subjectTransform;
        [SerializeField]
        private Hypertag            subjectTag;

        [SerializeField]
        private TeleportTarget      teleportTarget;
        [SerializeField]
        private Transform[]         targetTransforms;
        [SerializeField]
        private Hypertag[]          targetTags;

        public override string GetActionTitle() => "Teleport";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            desc += "Teleports ";
            switch (subject)
            {
                case TeleportSubject.Self:
                    desc += "this object";
                    break;
                case TeleportSubject.Target:
                    if (subjectTransform != null) desc += $"object [{subjectTransform.name}]";
                    else desc += $"object [UNDEFINED]";
                    break;
                case TeleportSubject.Tag:
                    if (subjectTag != null) desc += $"object with tag [{subjectTag.name}]";
                    else desc += $"object with tag [UNDEFINED]";
                    break;
                case TeleportSubject.Collider:
                    desc += $"colliding object";
                    break;
                default:
                    break;
            }

            desc += " to ";

            switch (teleportTarget)
            {
                case TeleportTarget.Target:
                    if ((targetTransforms == null) || (targetTransforms.Length == 0) || (targetTransforms[0] == null))
                        desc += "an [UNDEFINED] position";
                    else if (targetTransforms.Length > 1)
                    {
                        desc += "a randomly position selected from [";
                        for (int i = 0; i < targetTransforms.Length; i++)
                        {
                            if (targetTransforms[i] == null) desc += "NULL";
                            else desc += $"{targetTransforms[i].name}";
                            if (i < targetTransforms.Length - 1) desc += ", ";
                        }

                        desc += "]";
                    }
                    else
                    {
                        desc += $"the position of {targetTransforms[0].name}";
                    }
                    break;
                case TeleportTarget.Tag:
                    if ((targetTags == null) || (targetTags.Length == 0) || (targetTags[0] == null))
                        desc += "an [UNDEFINED] tag";
                    else 
                    {
                        desc += "a randomly position selected from objects with tags [";
                        for (int i = 0; i < targetTags.Length; i++)
                        {
                            if (targetTags[i] == null) desc += "NULL";
                            else desc += $"{targetTags[i].name}";
                            if (i < targetTags.Length - 1) desc += ", ";
                        }

                        desc += "]";
                    }
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            switch (subject)
            {
                case TeleportSubject.Target:
                    if (subjectTransform == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Subject transform is not set!", "You need to select what object to teleport."));
                    }
                    break;
                case TeleportSubject.Tag:
                    if (subjectTag == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Subject tag is not set!", "You need to select what object to teleport."));
                    }
                    break;
                default:
                    break;
            }

            switch (teleportTarget)
            {
                case TeleportTarget.Target:
                    if ((targetTransforms == null) || (targetTransforms.Length == 0) || (targetTransforms[0] == null))
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Teleport target is not set!", "You need to select where the subject will teleport to."));
                    else
                    {
                        foreach (var t in targetTransforms)
                        {
                            if (t == null)
                            {
                                _logs.Add(new LogEntry(LogEntry.Type.Error, "Target is undefined", "This will lead to wrong behaviours"));
                            }
                        }
                    }
                    break;
                case TeleportTarget.Tag:
                    if ((targetTags == null) || (targetTags.Length == 0) || (targetTags[0] == null))
                        _logs.Add(new LogEntry(LogEntry.Type.Error, "Teleport tag is not set!", "You need to select where the subject will teleport to."));
                    else
                    {
                        foreach (var t in targetTags)
                        {
                            if (t == null)
                            {
                                _logs.Add(new LogEntry(LogEntry.Type.Error, "Target is undefined", "This will lead to wrong behaviours"));
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            Transform objToTeleport = null;
            switch (subject)
            {
                case TeleportSubject.Self:
                    objToTeleport = transform;
                    break;
                case TeleportSubject.Target:
                    objToTeleport = subjectTransform;
                    break;
                case TeleportSubject.Tag:
                    var taggedObjects = HypertaggedObject.FindObjectsByHypertag<Transform>(subjectTag);
                    if ((taggedObjects != null) && (taggedObjects.Count > 0))
                    {
                        objToTeleport = taggedObjects[Random.Range(0, taggedObjects.Count)];
                    }
                    break;
                case TeleportSubject.Collider:
                    var collider = TriggerOnCollision.GetLastCollider();
                    if (collider) objToTeleport = collider.transform;
                    break;
                default:
                    break;
            }

            if (objToTeleport == null) return;

            Transform target = null;
            switch (teleportTarget)
            {
                case TeleportTarget.Target:
                    if ((targetTransforms != null) && (targetTransforms.Length > 0))
                    {
                        target = targetTransforms[Random.Range(0, targetTransforms.Length)];
                    }
                    break;
                case TeleportTarget.Tag:
                    var taggedObjects = HypertaggedObject.FindObjectsByHypertag<Transform>(subjectTag);
                    if ((taggedObjects != null) && (taggedObjects.Count > 0))
                    {
                        target = taggedObjects[Random.Range(0, taggedObjects.Count)];
                    }
                    break;
                default:
                    break;
            }

            if (target == null) return;

            var gridObject = objToTeleport.GetComponent<GridObject>();
            if (gridObject)
            {
                gridObject.TeleportTo(target.position);
            }
            else
            {
                objToTeleport.position = target.position;
                objToTeleport.rotation = target.rotation;
            }
        }
    }
}