using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(MultiValueDisplayText))]
    public class MultiValueDisplayTextEditor : MultiValueDisplayEditor
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