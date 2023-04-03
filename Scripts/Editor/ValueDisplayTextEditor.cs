using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ValueDisplayText))]
    public class ValueDisplayTextEditor : ValueDisplayEditor
    {
        protected override string typeOfDisplay { get => "text"; }

        protected override void OnEnable()
        {
            base.OnEnable();
        }

        protected override void StdEditor(bool useOriginalEditor = true, bool isFinal = true)
        {
            base.StdEditor(useOriginalEditor, true);
        }

    }
}