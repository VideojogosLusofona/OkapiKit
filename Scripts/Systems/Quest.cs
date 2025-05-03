using Mono.Cecil;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

    [CreateAssetMenu(fileName = "Quest", menuName = "Okapi Kit/Quest")]
    public class Quest : OkapiScriptableObject
    {
        [Flags]
        public enum Flags { 
            RemoveItemsOnComplete = 1, 
            RemoveTokensOnComplete = 2 
        };

        [Serializable]
        public class QuestObjective
        {
            public enum Type { Item, Token };

            public Type     type;
            public string   name;
            public Item     item;
            public Hypertag tag;
            public int      count;
        }

        public string       displayName;
        [TextArea] 
        public string           questText;
        public Quest[]          questsRequired;
        public QuestObjective[] questObjectives;
        public Flags            flags;

        public bool removeItemsOnComplete => (flags & Flags.RemoveItemsOnComplete) != 0;
        public bool removeTokensOnComplete => (flags & Flags.RemoveTokensOnComplete) != 0;
        public int objectiveCount => questObjectives?.Length ?? 0;
        public string GetObjectiveDescription(int index, QuestManager questManager)
        {
            string desc = "";
            if ((index >= 0) && (index < questObjectives.Length))
            {
                QuestObjective objective = questObjectives[index];  
                if (objective != null)
                {
                    int count = 0;
                    switch (objective.type)
                    {
                        case QuestObjective.Type.Item:
                            if (objective.item)
                            {
                                if (objective.count == 0)
                                {
                                    desc = $"  Not having a {objective.item.displayName}\n";
                                }
                                else 
                                {
                                    count = Mathf.Min(objective.count, questManager?.GetItemCount(objective.item) ?? 0);
                                    desc = $"  {objective.item.displayName} ({count}/{objective.count})\n";
                                }
                            }
                            break;
                        case QuestObjective.Type.Token:
                            if (objective.tag)
                            {
                                count = Mathf.Min(objective.count, questManager?.GetTokenCount(objective.tag) ?? 0);
                                desc = $"   {objective.name} ({count}/{objective.count})\n";
                            }
                            break;
                        default:
                            break;
                    }
                }
            }

            return desc;
        }

        public bool IsObjectiveCompleted(int index, QuestManager questManager)
        {
            if ((index >= 0) && (index < questObjectives.Length))
            {
                QuestObjective objective = questObjectives[index];
                if (objective != null)
                {
                    switch (objective.type)
                    {
                        case QuestObjective.Type.Item:
                            if (objective.item)
                            {
                                if (objective.count == 0)
                                {
                                    return (questManager?.GetItemCount(objective.item) ?? 0)== 0;
                                }
                                else
                                {
                                    return (questManager?.GetItemCount(objective.item) ?? 0) >= objective.count;
                                }
                            }
                            break;
                        case QuestObjective.Type.Token:
                            if (objective.tag)
                            {
                                return (questManager?.GetTokenCount(objective.tag) ?? 0) >= objective.count;
                            }
                            break;
                        default:
                            break;
                    }
                }
            }
            return false;
        }

        public bool IsCompleted(QuestManager questManager)
        {
            for (int i = 0; i < objectiveCount; i++)
            {
                if (!IsObjectiveCompleted(i, questManager)) return false;
            }

            return true;
        }


        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            string desc = "Requires the following quests:\n";

            foreach (var q in questsRequired)
            {
                if (q) desc += $"  {q.displayName}\n";
            }

            desc += "\nQuest Objectives:\n";
            for (int i = 0; i < objectiveCount; i++)
            {
                desc += GetObjectiveDescription(i, null);
            }
            

            return desc;
        }

        public bool CanActive(QuestManager questManager)
        {
            foreach (var q in questsRequired)
            {
                if (!questManager.IsQuestComplete(q)) return false;
            }

            return true;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (questObjectives == null || questObjectives.Length == 0)
            {
                _logs.Add(new LogEntry(
                    LogEntry.Type.Warning,
                    "No quest objectives defined.",
                    "This quest will immediately complete if no objectives are required."
                ));
                return;
            }

            for (int i = 0; i < questObjectives.Length; i++)
            {
                var obj = questObjectives[i];
                if (obj == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, $"Objective {i} is null.", "Remove or replace this objective."));
                    continue;
                }

                switch (obj.type)
                {
                    case QuestObjective.Type.Item:
                        if (obj.item == null)
                        {
                            _logs.Add(new LogEntry(
                                LogEntry.Type.Error,
                                $"Objective {i} of type Item has no item assigned.",
                                "Item objectives must have a valid item reference."
                            ));
                        }
                        else if (obj.count < 0)
                        {
                            _logs.Add(new LogEntry(
                                LogEntry.Type.Warning,
                                $"Objective {i} for item '{obj.item.name}' has a non-positive count.",
                                "Make sure the count is > 0 if this objective requires collecting items, or that it is equal to zero if the player can't have an object of that type with him."
                            ));
                        }
                        break;

                    case QuestObjective.Type.Token:
                        if (string.IsNullOrWhiteSpace(obj.name))
                        {
                            _logs.Add(new LogEntry(
                                LogEntry.Type.Warning,
                                $"Objective {i} of type Token has no display name.",
                                "Tokens should have a display name to describe them in the UI."
                            ));
                        }

                        if (obj.tag == null)
                        {
                            _logs.Add(new LogEntry(
                                LogEntry.Type.Error,
                                $"Objective {i} of type Token has no Hypertag assigned.",
                                "Token objectives must have a tag reference to match against."
                            ));
                        }

                        if (obj.count <= 0)
                        {
                            _logs.Add(new LogEntry(
                                LogEntry.Type.Warning,
                                $"Objective {i} for token '{obj.name}' has a non-positive count.",
                                "Ensure the token count is greater than zero to make it meaningful."
                            ));
                        }
                        break;
                }
            }
        }

        public void CompleteQuest(QuestManager questManager)
        {
            foreach (var objective in questObjectives)
            {
                switch (objective.type)
                {
                    case QuestObjective.Type.Item:
                        if (removeItemsOnComplete)
                        {
                            questManager.Inventory.Remove(objective.item, objective.count);
                        }
                        break;
                    case QuestObjective.Type.Token:
                        if (removeTokensOnComplete)
                        {
                            questManager.ChangeToken(objective.tag, -objective.count);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}