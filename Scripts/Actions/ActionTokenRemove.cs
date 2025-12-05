using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Token Remove")]
    public class ActionTokenRemove : Action
    {
        [SerializeField]
        private TargetQuestManager questManager;
        [SerializeField]
        private Hypertag token;
        [SerializeField]
        private int quantity = 1;

        public override string GetActionTitle()
        {
            if (token != null)
            {
                if (quantity > 1)
                    return $"Remove {quantity}x {token.name}";
                else
                    return $"Remove token {token.name}";
            }
            return "Remove Token";
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            var tokenName = token?.name ?? "UNDEFINED";
            var targetName = questManager.GetShortDescription(gameObject);
            if (targetName != "") targetName = $"to {targetName}";
            if (quantity > 1)
                desc += $"Remove {quantity}x \"{tokenName}\" {targetName}";
            else
                desc += $"Remove token \"{tokenName}\" {targetName}";

            return desc;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (quantity < 1)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Quantity < 1", "You probably want to remove 1 or more tokens"));
            }
            if (token == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Undefined token!", "You need to define the token to give!"));
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
            if (token != null)
            {
                qm.ChangeToken(token, -quantity);
            }
        }
    }
}