using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ValueDisplayImage))]
    public class ValueDisplayImageEditor : ValueDisplayEditor
    {
        protected override string typeOfDisplay { get => "a set of images"; }

        protected override void StdEditor(bool useOriginalEditor = true, bool isFinal = true)
        {
            base.StdEditor(useOriginalEditor, isFinal);
        }

    }
}