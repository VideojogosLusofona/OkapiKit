using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class QuestManager : OkapiElement
    {
        public delegate void OnQuestEvent(Quest quest);
        public event OnQuestEvent onQuestStart;
        public event OnQuestEvent onQuestFailed;
        public event OnQuestEvent onQuestComplete;

        class QuestState
        {
        };

        [SerializeField] private Quest[]            startQuests;
        [SerializeField] private TargetInventory    targetInventory;
        [SerializeField] private TargetEquipment    targetEquipment;

        List<Quest> pendingQuests = new();
        List<Quest> activeQuests = new();
        List<Quest> completedQuests = new();
        List<Quest> failedQuests = new();

        Dictionary<Quest, QuestState> questState;

        public List<Quest> PendingQuests => pendingQuests;
        public List<Quest> ActiveQuests => activeQuests;
        public List<Quest> CompletedQuests => completedQuests;
        public List<Quest> FailedQuests => failedQuests;

        Inventory _inventory;
        Equipment _equipment;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "Quest system, use with actions such as Add/Remove/Fail Quest, conditions such as QuestComplete/Active/Failed, or events as QuestStarted/Completed";
        }

        void Start()
        {
            if (startQuests != null) pendingQuests = new List<Quest>(startQuests);

            _inventory = GetComponent<Inventory>();
            if (_inventory == null) _inventory = targetInventory.GetTarget(gameObject);

            _equipment = GetComponent<Equipment>();
            if (_equipment == null) _equipment = targetEquipment.GetTarget(gameObject);
        }

        void Update()
        {
            // Check if we can move any pending quest to active quest
            if ((pendingQuests != null) && (pendingQuests.Count > 0))
            {
                var checkQuests = new List<Quest>(pendingQuests);
                foreach (var q in checkQuests)
                {
                    if (q.CanActive(this))
                    {
                        pendingQuests.Remove(q);
                        StartQuest(q);
                    }
                }
            }

            // Check for completed quests
            if ((activeQuests != null) && (activeQuests.Count > 0))
            {
                var checkQuests = new List<Quest>(activeQuests);
                foreach (var q in checkQuests)
                {
                    if (q.IsCompleted(this))
                    {
                        CompleteQuest(q);
                    }
                }
            }
        }

        public void AddQuest(Quest quest)
        {
            // Quests are added to the pending quest list, in case they depend on other quests
            pendingQuests.Add(quest);
        }

        public void StartQuest(Quest q)
        {
            activeQuests.Add(q);
            onQuestStart?.Invoke(q);
        }

        public void CompleteQuest(Quest q)
        {
            activeQuests.Remove(q);
            pendingQuests.Remove(q);
            completedQuests.Add(q);

            onQuestComplete?.Invoke(q);
        }

        public void FailQuest(Quest quest)
        {
            activeQuests.Remove(quest);
            pendingQuests.Remove(quest);
            failedQuests.Add(quest);

            onQuestFailed?.Invoke(quest);
        }

        internal void RemoveQuest(Quest quest)
        {
            activeQuests.Remove(quest);
            pendingQuests.Remove(quest);
            failedQuests.Remove(quest);

            onQuestFailed?.Invoke(quest);
        }


        public bool IsQuestComplete(Quest quest)
        {
            if (quest == null) return true;
            if (completedQuests == null) return false;

            return (completedQuests.IndexOf(quest) != -1);
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            bool hasInventory = GetComponent<Inventory>() != null;
            bool hasEquipment = GetComponent<Equipment>() != null;

            if (!hasInventory)
            {
                targetInventory.CheckErrors(_logs, "inventory", gameObject);
            }

            if (!hasEquipment)
            {
                targetEquipment.CheckErrors(_logs, "inventory", gameObject);
            }
        }

        public int GetItemCount(Item item)
        {
            int count = _inventory.GetItemCount(item);
            if (count == 0)
            {
                if (_equipment.IsEquipped(item)) return 1;
            }
            return count;
        }

        public int GetTokenCount(Hypertag tag)
        {
            return 0;
        }
    }

    [Serializable]
    public class TargetQuestManager : OkapiTarget<QuestManager>
    {

    }
}
