using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Trigger/On Quest Event")]
    public class TriggerOnQuestEvent : Trigger
    {
        public enum QuestEvent { Started, Failed, Complete};

        [SerializeField]
        private QuestEvent          eventType;
        [SerializeField]
        private TargetQuestManager  questManager;
        [SerializeField]
        private Quest               quest;

        public override string GetTriggerTitle() 
        {
            switch (eventType)
            {
                case QuestEvent.Started:
                    return "On Quest Started";
                case QuestEvent.Failed:
                    return "On Quest Failed";
                case QuestEvent.Complete:
                    return "On Quest Complete";
                default:
                    break;
            }
            return "On Quest Event"; 
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            var questName = quest?.displayName ?? null;
            var desc = "";
            switch (eventType)
            {
                case QuestEvent.Started:
                    if (questName != null)
                        desc = $"When quest {questName} starts";
                    else
                        desc = $"When any quest starts";
                    break;
                case QuestEvent.Failed:
                    if (questName != null)
                        desc = $"When quest {questName} fails";
                    else
                        desc = $"When any quest fails";
                    break;
                case QuestEvent.Complete:
                    if (questName != null)
                        desc = $"When quest {questName} completes";
                    else
                        desc = $"When any quest completes";
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (GetComponent<QuestManager>() == null)
            {
                questManager.CheckErrors(_logs, "quest manager", gameObject);
            }
        }

        private void Start()
        {
            var qm = GetComponent<QuestManager>();
            if (qm == null)
            {
                qm = questManager.GetTarget(gameObject);
            }
            if (qm == null)
            {
                Debug.LogError("Quest manager for On Quest Event not found!");
            }
            else
            {
                qm.onQuestStart += OnQuestStart;
                qm.onQuestFailed += OnQuestFail;
                qm.onQuestComplete += OnQuestComplete;
            }
        }

        private void OnQuestStart(Quest quest)
        {
            if (eventType != QuestEvent.Started) return;
            if ((this.quest != null) && (this.quest != quest)) return;

            if (EvaluatePreconditions())
            {
                ExecuteTrigger();
            }
        }

        private void OnQuestFail(Quest quest)
        {
            if (eventType != QuestEvent.Failed) return;
            if ((this.quest != null) && (this.quest != quest)) return;

            if (EvaluatePreconditions())
            {
                ExecuteTrigger();
            }
        }

        private void OnQuestComplete(Quest quest)
        {
            if (eventType != QuestEvent.Complete) return;
            if ((this.quest != null) && (this.quest != quest)) return;

            if (EvaluatePreconditions())
            {
                ExecuteTrigger();
            }
        }
    }
}
