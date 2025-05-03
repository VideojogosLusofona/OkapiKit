using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace OkapiKit
{
    public class QuestDisplay : OkapiElement
    {
        [SerializeField] private TargetQuestManager sourceQuestManager;
        [SerializeField] private int                questIndex;
        [SerializeField] private TextMeshProUGUI    titleText;
        [SerializeField] private TextMeshProUGUI[]  objectiveText;
        [SerializeField] private Color              normalObjectiveColor = Color.yellow;
        [SerializeField] private Color              completedObjectiveColor = Color.green;

        QuestManager questManager;

        void Start()
        {
            questManager = sourceQuestManager.GetTarget(gameObject);
            if (questManager)
            {
                questManager.onQuestStateModified += UpdateDisplay;
            }

            UpdateDisplay();
        }

        void UpdateDisplay()
        {
            if (questManager == null) return;

            var quest = questManager.GetActiveQuest(questIndex);
            if (quest == null)
            {
                gameObject.SetActive(false);
            }
            else
            {
                gameObject.SetActive(true);

                if (titleText) titleText.text = quest.displayName;
                if ((objectiveText != null) && (objectiveText.Length > 0))
                {
                    for (int i = 0; i < objectiveText.Length; i++)
                    {
                        if (i >= quest.objectiveCount) 
                        {
                            objectiveText[i].gameObject.SetActive(false);
                        }
                        else
                        {
                            objectiveText[i].gameObject.SetActive(true);
                            objectiveText[i].text = quest.GetObjectiveDescription(i, questManager).Trim();
                            objectiveText[i].color = quest.IsObjectiveCompleted(i, questManager) ? completedObjectiveColor : normalObjectiveColor;
                        }
                    }
                }
            }

            RebuildHierarchy(GetComponent<RectTransform>());
        }

        private void RebuildHierarchy(RectTransform rectTransform)
        {
            LayoutRebuilder.ForceRebuildLayoutImmediate(rectTransform);

            var parent = rectTransform.parent;
            if (parent)
            {
                var parentRect = parent as RectTransform;
                if (parentRect)
                {
                    RebuildHierarchy(parentRect);
                }
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "";
        }
    }
}
