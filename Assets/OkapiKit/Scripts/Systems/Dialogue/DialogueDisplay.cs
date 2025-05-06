using UnityEngine;

namespace OkapiKit
{

    public abstract class DialogueDisplay : OkapiElement
    {
        protected CanvasGroup canvasGroup;

        private void Start()
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup)
                canvasGroup.alpha = 0.0f;
            else
                gameObject.SetActive(false);
        }

        public abstract void Display(DialogueData.DialogueElement dialogue);
        public abstract void Clear();
        public abstract void Skip();
        public abstract bool isDisplaying();
        public abstract void SetInput(Vector2 moveVector);
        public abstract int GetSelectedOption();

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "This element will be used to render the dialogues.";
        }
    }
}