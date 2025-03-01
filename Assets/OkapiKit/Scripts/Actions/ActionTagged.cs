using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEditor;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Run Tagged Action")]
    public class ActionTagged : Action
    {
        public enum SearchType { Global = 0, Tagged = 1, Children = 2, WithinCollider = 3, CollisionObject = 4 };
        public enum TriggerType { All = 0, Sequence = 1, Random = 2 };

        [SerializeField]
        private SearchType searchType = SearchType.Global;
        [SerializeField]
        private Hypertag[] searchTags;
        [SerializeField]
        private Collider2D[] colliders;
        [SerializeField]
        private TriggerType triggerType = TriggerType.All;
        [SerializeField]
        private Hypertag[] triggerTags;

        private int sequenceIndex = 0;

        public override string GetActionTitle() => "Execute Tagged Action";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);

            if (searchType == SearchType.Global)
            {
                desc = "find actions tagged with any of [";
            }
            else if (searchType == SearchType.Children)
            {
                desc = "find actions underneath this object tagged with any of [";
            }
            else if (searchType == SearchType.Tagged)
            {
                desc = "find all objects tagged with any of [";

                if ((searchTags != null) && (searchTags.Length > 0))
                {
                    for (int i = 0; i < searchTags.Length; i++)
                    {
                        if (searchTags[i] != null)
                            desc += searchTags[i].name;
                        else
                            desc += "[UNDEFINED]";
                        if (i < searchTags.Length - 1) desc += ",";
                    }
                }
                else desc += "UNDEFINED";

                desc += "] and in them find actions tagged with any of ["; ;
            }
            else if (searchType == SearchType.WithinCollider)
            {
                desc = "find all objects tagged with any of [";

                if ((searchTags != null) && (searchTags.Length > 0))
                {
                    for (int i = 0; i < searchTags.Length; i++)
                    {
                        desc += searchTags[i].name;
                        if (i < searchTags.Length - 1) desc += ",";
                    }
                }
                else desc += "UNDEFINED";

                int nColliders = (colliders != null) ? (colliders.Length) : (0);
                if (nColliders == 0)
                {
                    desc += $"] within any of the undefined colliders and in them \nfind actions tagged with any of [";
                }
                else if (nColliders == 1)
                {
                    string cname = (colliders[0]) ? (colliders[0].name) : ("UNDEFINED");
                    desc += $"] inside the collider [{cname}] and in them \nfind actions tagged with any of [";
                }
                else
                {
                    desc += $"] inside any of the colliders defined and in them \nfind actions tagged with any of [";
                }
            }
            else if (searchType == SearchType.CollisionObject)
            {
                desc = "find actions underneath the object with which this one collided, tagged with any of [";
            }

            if ((triggerTags != null) && (triggerTags.Length > 0))
            {
                for (int i = 0; i < triggerTags.Length; i++)
                {
                    if (triggerTags[i] != null)
                    {
                        desc += triggerTags[i].name;
                    }
                    else
                    {
                        desc += "[UNDEFINED]";
                    }
                    if (i < triggerTags.Length - 1) desc += ",";
                }
            }
            else desc += "MISSING";

            switch (triggerType)
            {
                case TriggerType.All:
                    desc += "] and execute all of them.";
                    break;
                case TriggerType.Sequence:
                    desc += "] and execute each of them in sequence each time this action is executed.";
                    break;
                case TriggerType.Random:
                    desc += "] and choose one randomly to execute.";
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((triggerTags == null) || (triggerTags.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Trigger tags not defined!", "Trigger tags are the tags of the action you want to call.\nThis acts like a broadcast, i.e. all the actions that are not filtered out (by the search criteria) that are tagged with these tags will be executed when this action is executed"));
            }
            else
            {
                int index = 0;
                foreach (var tag in triggerTags)
                {
                    if (tag == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Tag slot is empty in tag list (index={index})!", "Empty tags are useless, fill it in, or delete it"));
                    }
                    index++;
                }
            }            
            if (searchType == SearchType.WithinCollider)
            {
                if ((colliders == null) || (colliders.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Colliders not defined!", "If we want to search for actions inside a particular area, we need to define that area by using the colliders"));
                }
                else
                {
                    int index = 0;
                    foreach (var collider in colliders)
                    {
                        if (collider == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Collider slot is empty in collider list (index={index})!", "Empty colliders are useless, remove it or fill it in"));
                        }
                        index++;
                    }
                }
            }
            if ((searchType == SearchType.Tagged) || (searchType == SearchType.WithinCollider))
            {
                if ((searchTags == null) || (searchTags.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Search tags not defined!", "If you want to search for an action tag inside objects tagged with another tag, you need to specify which ones you want to include in the search"));
                }
                else
                {
                    int index = 0;
                    foreach (var tag in searchTags)
                    {
                        if (tag == null)
                        {
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Tag slot is empty in search tag list (index={index})!", "Empty tags are useless, fill it in or remove it."));
                        }
                        index++;
                    }
                }
            }
        }

        List<Action> targetActions = null;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if ((targetActions == null) || (triggerType != TriggerType.Sequence) || (searchType == SearchType.CollisionObject))
            {
                RefreshActions();
            }

            if (targetActions != null)
            {
                if ((searchType == SearchType.Global) ||
                    (triggerType == TriggerType.All))
                {
                    foreach (var action in targetActions)
                    {
                        action.Execute();
                    }
                }
                else if (triggerType == TriggerType.Random)
                {
                    targetActions.RemoveAll((t) => (t == null) || (!t.isActionEnabled));
                    if (targetActions.Count > 0)
                    {
                        int r = Random.Range(0, targetActions.Count);
                        targetActions[r].Execute();
                    }
                }
                else if (triggerType == TriggerType.Sequence)
                {
                    bool actionDone = false;
                    bool restarted = false;
                    while (!actionDone)
                    {
                        var action = targetActions[sequenceIndex];
                        if ((action) && (action.isActionEnabled))
                        {
                            action.Execute();
                            actionDone = true;
                        }
                        sequenceIndex++;
                        if (sequenceIndex >= targetActions.Count)
                        {
                            if (restarted)
                            {
                                // Couldn't find a valid action
                                break;
                            }
                            else
                            {
                                RefreshActions();
                                restarted = true;
                            }
                        }
                    }
                }
            }
        }

        void RefreshActions()
        {
            if (searchType == SearchType.Global)
            {
                targetActions = new List<Action>(GameObject.FindObjectsByType<Action>(FindObjectsSortMode.None));

                targetActions.RemoveAll((action) => !action.HasTag(triggerTags));
            }
            else if (searchType == SearchType.Children)
            {
                targetActions = new List<Action>(GetComponentsInChildren<Action>());

                targetActions.RemoveAll((action) => !action.HasTag(triggerTags));
            }
            else if (searchType == SearchType.WithinCollider)
            {
                if (colliders.Length > 0)
                {
                    var contactFilter = new ContactFilter2D();
                    contactFilter.NoFilter();
                    contactFilter.useTriggers = true;

                    targetActions = new List<Action>();

                    List<Collider2D> results = new List<Collider2D>();

                    foreach (var c in colliders)
                    {
                        if (c == null) return;

                        if (Physics2D.OverlapCollider(c, contactFilter, results) > 0)
                        {
                            foreach (var r in results)
                            {
                                if (r.gameObject.HasHypertags(searchTags))
                                {
                                    var actions = r.GetComponents<Action>();

                                    foreach (var a in actions)
                                    {
                                        if (a.HasTag(triggerTags))
                                        {
                                            targetActions.Add(a);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
            else if (searchType == SearchType.CollisionObject)
            {
                targetActions = new List<Action>();

                var lastCollider = TriggerOnCollision.GetLastCollider();
                if (lastCollider != null)
                {
                    targetActions = new List<Action>(lastCollider.GetComponentsInChildren<Action>());

                    targetActions.RemoveAll((action) => !action.HasTag(triggerTags));
                }
            }
            else
            {
                targetActions = new List<Action>();

                var objects = HypertaggedObject.FindGameObjectsWithHypertag(searchTags);
                if (objects != null)
                {
                    foreach (var obj in objects)
                    {
                        var actions = obj.GetComponentsInChildren<Action>();
                        foreach (var action in actions)
                        {
                            if (action.isActionEnabled)
                            {
                                if (action.HasTag(triggerTags))
                                {
                                    if (targetActions.IndexOf(action) == -1)
                                    {
                                        targetActions.Add(action);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            sequenceIndex = 0;
        }
    }
}