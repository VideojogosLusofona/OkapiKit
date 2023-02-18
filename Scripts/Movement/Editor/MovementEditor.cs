using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Movement))]
public class MovementEditor : OkapiBaseEditor
{
    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        if (WriteTitle())
        {
            StdEditor(false);
        }
    }

    protected void StdEditor(bool useOriginalEditor = true)
    {
        // Draw old editor, need it for now
        if (useOriginalEditor)
        {
            base.OnInspectorGUI();
        }

    }

    protected override GUIStyle GetTitleSyle()
    {
        return GUIUtils.GetActionTitleStyle();
    }

    protected override GUIStyle GetExplanationStyle()
    {
        return GUIUtils.GetActionExplanationStyle();
    }

    protected override string GetTitle()
    {
        return (target as Movement).GetTitle();
    }

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Movement");
        return varTexture;
    }

    protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffcaca"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff6060"));
}
