using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Quest Remove")]
    public class ActionQuestRemove : Action
    {
        [SerializeField]
        private TargetQuestManager  questManager;
        [SerializeField]
        private Quest               quest;

        public override string GetActionTitle() { return "Remove Quest"; }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            var questName = quest?.displayName ?? "UNDEFINED";
            var targetName = questManager.GetShortDescription(gameObject);
            if (targetName != "") targetName = $"to {targetName}";
            desc += $"Remove quest \"{questName}\" {targetName}";

            return desc;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (quest == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined target quest!", "You need to define the quest to remove!"));
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
                qm.RemoveQuest(quest);
            }
        }
    }
}