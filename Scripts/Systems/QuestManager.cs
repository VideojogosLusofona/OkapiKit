using System;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class QuestManager : OkapiElement
    {
        public delegate void OnQuestListEvent();
        public delegate void OnQuestEvent(Quest quest);
        public event OnQuestEvent       onQuestStart;
        public event OnQuestEvent       onQuestFailed;
        public event OnQuestEvent       onQuestComplete;
        public event OnQuestListEvent   onQuestStateModified;

        class QuestState
        {
        };

        [SerializeField] private Quest[]            startQuests;
        [SerializeField] private TargetInventory    targetInventory;
        [SerializeField] private TargetEquipment    targetEquipment;
        [SerializeField] private bool               combatTextEnable;
        [SerializeField] private float              combatTextDuration = 1.0f;
        [SerializeField] private Color              combatTextActiveQuestColor = Color.yellow;
        [SerializeField] private Color              combatTextCompletedQuestColor = Color.green;

        List<Quest> pendingQuests = new();
        List<Quest> activeQuests = new();
        List<Quest> completedQuests = new();
        List<Quest> failedQuests = new();

        Dictionary<Quest, QuestState>   questState;
        Dictionary<Hypertag, int>       tokens;

        public List<Quest> PendingQuests => pendingQuests;
        public List<Quest> ActiveQuests => activeQuests;
        public List<Quest> CompletedQuests => completedQuests;
        public List<Quest> FailedQuests => failedQuests;
        
        public Dictionary<Hypertag, int>    Tokens => tokens;
        public Inventory Inventory => _inventory;

        Inventory _inventory;
        Equipment _equipment;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "Quest system, use with actions such as Add/Remove/Fail Quest, conditions such as QuestComplete/Active/Failed, or events as QuestStarted/Completed";
        }

        void Start()
        {
            if (startQuests != null)
            {
                pendingQuests = new List<Quest>(startQuests);
                onQuestStateModified?.Invoke();
            }

            _inventory = GetComponent<Inventory>();
            if (_inventory == null) _inventory = targetInventory.GetTarget(gameObject);
            if (_inventory != null) _inventory.onChange += Inventory_onChange;

            _equipment = GetComponent<Equipment>();
            if (_equipment == null) _equipment = targetEquipment.GetTarget(gameObject);
        }

        private void Inventory_onChange(bool add, Item item, int slot)
        {
            onQuestStateModified?.Invoke();
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
            // Check if quest is already pending/active
            if (pendingQuests.IndexOf(quest) != -1) return;
            if (activeQuests.IndexOf(quest) != -1) return;

            // Quests are added to the pending quest list, in case they depend on other quests
            pendingQuests.Add(quest);
        }

        public void StartQuest(Quest q)
        {
            activeQuests.Add(q);
            onQuestStart?.Invoke(q);
            onQuestStateModified?.Invoke();

            if (combatTextEnable)
            {
                CombatTextManager.SpawnText(gameObject, $"Quest \"{q.displayName}\" accepted!", combatTextActiveQuestColor, combatTextActiveQuestColor, combatTextDuration);
            }
        }

        public void CompleteQuest(Quest q)
        {
            activeQuests.Remove(q);
            pendingQuests.Remove(q);
            completedQuests.Add(q);

            q.CompleteQuest(this);

            onQuestComplete?.Invoke(q);
            onQuestStateModified?.Invoke();

            if (combatTextEnable)
            {
                CombatTextManager.SpawnText(gameObject, $"Quest \"{q.displayName}\" completed!", combatTextCompletedQuestColor, combatTextCompletedQuestColor, combatTextDuration);
            }
        }

        public void FailQuest(Quest quest)
        {
            activeQuests.Remove(quest);
            pendingQuests.Remove(quest);
            failedQuests.Add(quest);

            onQuestFailed?.Invoke(quest);
            onQuestStateModified?.Invoke();
        }

        internal void RemoveQuest(Quest quest)
        {
            activeQuests.Remove(quest);
            pendingQuests.Remove(quest);
            failedQuests.Remove(quest);

            onQuestStateModified?.Invoke();
        }


        public bool IsQuestComplete(Quest quest)
        {
            if (quest == null) return true;
            if (completedQuests == null) return false;

            return (completedQuests.IndexOf(quest) != -1);
        }

        public bool IsQuestPending(Quest quest)
        {
            if (quest == null) return true;
            if (pendingQuests == null) return false;

            return (pendingQuests.IndexOf(quest) != -1);
        }

        public bool IsQuestActive(Quest quest)
        {
            if (quest == null) return true;
            if (activeQuests == null) return false;

            return (activeQuests.IndexOf(quest) != -1);
        }

        public bool IsQuestFailed(Quest quest)
        {
            if (quest == null) return true;
            if (failedQuests == null) return false;

            return (failedQuests.IndexOf(quest) != -1);
        }

        public Quest GetActiveQuest(int questIndex)
        {
            if ((questIndex >= 0) && (questIndex < activeQuests.Count))
            {
                return activeQuests[questIndex];
            }

            return null;
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

            if (combatTextEnable)
            {
                if (!CombatTextManager.IsActive()) 
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Combat text is enabled, but there's no combat text manager!", "If you want to use combat text, you need to have an object with a combat text manager component."));
                }
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

        public int GetTokenCount(Hypertag token)
        {
            if (token == null) return 0;
            if (tokens == null) return 0;

            if (tokens.TryGetValue(token, out int current)) return current;

            return 0;
        }

        public void ChangeToken(Hypertag token, int quantity)
        {
            if (token == null) return;
            if (tokens == null) tokens = new();

            int current = 0;
            tokens.TryGetValue(token, out current);

            tokens[token] = current + quantity;
            
            if (tokens[token] <= 0)
            {
                tokens.Remove(token);
            }

            onQuestStateModified?.Invoke();
        }
    }

    [Serializable]
    public class TargetQuestManager : OkapiTarget<QuestManager>
    {

    }
}
