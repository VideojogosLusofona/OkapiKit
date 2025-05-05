using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class OkapiKitContext : DefaultExpressionContextEvaluator
    {
        [SerializeField] private List<Quest> quests;
        protected Dictionary<string, Quest> cachedQuests;

        public bool GiveQuest(string targetTagName, string questName)
        {
            Quest quest = GetQuestByName(questName);
            if (quest == null)
            {
                return false;
            }

            var targetTag = GetTagByName(targetTagName);
            if (targetTag == null)
            {
                return false;
            }
            var questManager = HypertaggedObject.FindObjectByHypertag<QuestManager>(targetTag);
            if (questManager == null) questManager = HypertaggedObject.FindObjectByHypertag<Transform>(targetTag)?.GetComponentInChildren<QuestManager>() ?? null;
            if (questManager == null)
            {
                Debug.LogError($"Can't find quest manager tagged with {targetTagName}");
                return false;
            }
            questManager.AddQuest(quest);
            return true;
        }

        public bool RemoveQuest(string targetTagName, string questName)
        {
            Quest quest = GetQuestByName(questName);
            if (quest == null)
            {
                return false;
            }

            var targetTag = GetTagByName(targetTagName);
            if (targetTag == null)
            {
                return false;
            }
            var questManager = HypertaggedObject.FindObjectByHypertag<QuestManager>(targetTag);
            if (questManager == null) questManager = HypertaggedObject.FindObjectByHypertag<Transform>(targetTag)?.GetComponentInChildren<QuestManager>() ?? null;
            if (questManager == null)
            {
                Debug.LogError($"Can't find quest manager tagged with {targetTagName}");
                return false;
            }
            questManager.RemoveQuest(quest);
            return true;
        }

        public bool FailQuest(string targetTagName, string questName)
        {
            Quest quest = GetQuestByName(questName);
            if (quest == null)
            {
                return false;
            }

            var targetTag = GetTagByName(targetTagName);
            if (targetTag == null)
            {
                return false;
            }
            var questManager = HypertaggedObject.FindObjectByHypertag<QuestManager>(targetTag);
            if (questManager == null) questManager = HypertaggedObject.FindObjectByHypertag<Transform>(targetTag)?.GetComponentInChildren<QuestManager>() ?? null;
            if (questManager == null)
            {
                Debug.LogError($"Can't find quest manager tagged with {targetTagName}");
                return false;
            }
            questManager.FailQuest(quest);
            return true;
        }

        public bool ChangeToken(string targetTagName, string tokenName, int quantity)
        {
            var tokenTag = GetTagByName(tokenName);
            if (tokenTag == null)
            {
                return false;
            }

            var targetTag = GetTagByName(targetTagName);
            if (targetTag == null)
            {
                return false;
            }
            var questManager = HypertaggedObject.FindObjectByHypertag<QuestManager>(targetTag);
            if (questManager == null) questManager = HypertaggedObject.FindObjectByHypertag<Transform>(targetTag)?.GetComponentInChildren<QuestManager>() ?? null;
            if (questManager == null)
            {
                Debug.LogError($"Can't find quest manager tagged with {targetTagName}");
                return false;
            }
            questManager.ChangeToken(tokenTag, quantity);
            return true;
        }

        protected Quest GetQuestByName(string name)
        {
            if (cachedQuests == null)
            {
                cachedQuests = new();
                foreach (var q in quests)
                {
                    cachedQuests.Add(q.name, q);
                    cachedQuests.Add(q.displayName, q);
                }
            }

            if (cachedQuests.TryGetValue(name, out var quest))
                return quest;

            Debug.LogError($"Can't find quest {name}!");
            return null;
        }

#if UNITY_EDITOR
        protected void AddAllQuests()
        {
            quests = new List<Quest>(AssetUtils.GetAll<Quest>());
        }
    }
#endif
}
