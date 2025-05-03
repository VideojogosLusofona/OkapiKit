using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Quest Fail")]
    public class ActionQuestFail : Action
    {
        [SerializeField]
        private TargetQuestManager  questManager;
        [SerializeField]
        private Quest               quest;

        public override string GetActionTitle() { return "Fail Quest"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            var questName = quest?.displayName ?? "UNDEFINED";
            var targetName = questManager.GetShortDescription(gameObject);
            if (targetName != "") targetName = $"on {targetName}";
            desc += $"Fail quest \"{questName}\" {targetName}";

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (quest == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined target quest!", "You need to define the quest to fail!"));
            }
            questManager.CheckErrors(_logs, "quest manager", gameObject);
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var qm = questManager.GetTarget(gameObject);
            if (qm == null)
            {
                Debug.LogError("Can't find quest manager!");
                return;
            }
            if (quest != null)
            {
                qm.FailQuest(quest);
            }
        }
    }
}