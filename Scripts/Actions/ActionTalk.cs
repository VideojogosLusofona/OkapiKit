using UnityEngine;

namespace OkapiKit
{
    public class ActionTalk : Action
    {
        [SerializeField, DialogueKey] private string dialogueKey;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            DialogueManager.StartConversation(dialogueKey);
        }

        public override string GetActionTitle() 
        { 
            return $"Run Conversation {dialogueKey}";
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return $"Starts conversation {dialogueKey}.";
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (DialogueManager.Instance == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "No dialogue manager found!", "A dialogue manager must exist in the scene for the dialogue system to work!"));
            }
        }
    }
}
