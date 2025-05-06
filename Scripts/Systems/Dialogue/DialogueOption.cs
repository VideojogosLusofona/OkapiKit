using System;
using UnityEngine;

namespace OkapiKit
{

    public abstract class DialogueOption : OkapiElement
    {
        public abstract void Show(string text);
        public abstract void Hide();
        public abstract void Select();
        public abstract void Deselect();

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "This element represents a text option for dialogues.";
        }

    }
}