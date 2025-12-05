using UnityEngine;

namespace OkapiKit
{
    public class ActionTalk : Action
    {
        [SerializeField, DialogueKey] private string dialogueKey;

        float timeOfLastDialogue;

        private void Start()
        {
            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.onDialogueEnd += Instance_onDialogueEnd;
            }
        }

        private void OnDisable()
        {
            if (DialogueManager.Instance)
            {
                DialogueManager.Instance.onDialogueEnd -= Instance_onDialogueEnd;
            }
        }

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            if ((Time.time - timeOfLastDialogue) < 0.25f)
            {
                return;
            }

            DialogueManager.StartConversation(dialogueKey);
        }

        private void Instance_onDialogueEnd()
        {
            timeOfLastDialogue = Time.time;
        }

        public override string GetActionTitle() 
        { 
            return $"Run Conversation [{dialogueKey}]";
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return $"Starts conversation [{dialogueKey}].";
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if (DialogueManager.Instance == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "No dialogue manager found!", "A dialogue manager must exist in the scene for the dialogue system to work!"));
            }
        }
    }
}
