using System;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;

[InitializeOnLoad]
public class OkapiDisplayHierarchy 
{
    static OkapiDisplayHierarchy()
    {
        EditorApplication.hierarchyWindowItemOnGUI += HierarchyItemCB;
    }

    private static void HierarchyItemCB(int instanceID, Rect selectionRect)
    {
        if (selectionRect.width < 200) return;

        float controlsWidth = 6 * 10;
        float xBase = selectionRect.x + selectionRect.width - controlsWidth;

        if (selectionRect.Contains(Event.current.mousePosition))
        {
            GUI.tooltip = "";
        }

        GameObject go = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
        if (go != null)
        {
            var variable = go.GetComponent<VariableInstance>();
            var movement = go.GetComponent<Movement>();
            var triggers = go.GetComponents<Trigger>();
            var actions = go.GetComponents<Action>();
            var path = go.GetComponent<Path>();
            var probes = go.GetComponents<Probe>();

            if (variable) DrawCircle(xBase, selectionRect.y, GUIUtils.ColorFromHex("#fffaa7"));
            if (movement) DrawCircle(xBase + 10, selectionRect.y, GUIUtils.ColorFromHex("#ffcaca"));
            if ((triggers != null) && (triggers.Length > 0)) DrawCircle(xBase + 20, selectionRect.y, GUIUtils.ColorFromHex("#D0FFFF"));
            if ((actions != null) && (actions.Length > 0)) DrawCircle(xBase + 30, selectionRect.y, GUIUtils.ColorFromHex("#D7E8BA"));
            if ((probes != null) && (probes.Length > 0)) DrawCircle(xBase + 40, selectionRect.y, GUIUtils.ColorFromHex("#ffd283"));
            if (path) DrawCircle(xBase + 50, selectionRect.y, GUIUtils.ColorFromHex("#ff8040"));

            var tags = go.GetTagsString();
            if (tags != "")
            {
                Rect rect = new Rect(xBase - 12, selectionRect.y + 4, 10, 10);
                GUI.DrawTexture(rect, GUIUtils.GetTexture("Tag"), ScaleMode.ScaleToFit, true, 1.0f, Color.white, 0.0f, 0.0f);
            }

            if ((variable) && (HasTooltip(xBase, selectionRect.y))) SetTooltip(GUIUtils.ColorFromHex("#fffaa7"), "Variable", $"{variable.name}");
            if ((movement) && (HasTooltip(xBase + 10, selectionRect.y))) SetTooltip(GUIUtils.ColorFromHex("#ffcaca"), $"{movement.GetTitle()}", $"Movement: {movement.UpdateExplanation()}");
            if ((triggers != null) && (triggers.Length > 0) && (HasTooltip(xBase + 20, selectionRect.y)))
            {
                string tooltip = "";
                for (int i = 0; i < triggers.Length; i++)
                {
                    if (tooltip != "") tooltip += $"\n";
                    tooltip += $"Trigger {i + 1}:\n";
                    tooltip += triggers[i].UpdateExplanation();
                }
                tooltip.Remove(tooltip.Length - 1);

                SetTooltip(GUIUtils.ColorFromHex("#D0FFFF"), "Triggers", tooltip);
            }
            if ((actions != null) && (actions.Length > 0) && (HasTooltip(xBase + 30, selectionRect.y)))
            {
                string tooltip = "";
                for (int i = 0; i < actions.Length; i++)
                {
                    tooltip += $"{i + 1}. {actions[i].UpdateExplanation()}\n";
                }
                tooltip = tooltip.Remove(tooltip.Length - 1);

                SetTooltip(GUIUtils.ColorFromHex("#D7E8BA"), "Actions", tooltip);
            }
            if ((probes != null) && (probes.Length > 0) && (HasTooltip(xBase + 40, selectionRect.y)))
            {
                string tooltip = "";
                for (int i = 0; i < probes.Length; i++)
                {
                    if (tooltip != "") tooltip += $"\n\n";
                    tooltip += $"Probe {i + 1}:\n";
                    tooltip += probes[i].UpdateExplanation();
                }
                tooltip.Remove(tooltip.Length - 1);

                SetTooltip(GUIUtils.ColorFromHex("#d1d283"), "Probes", tooltip);
            }
            if ((path) && (HasTooltip(xBase + 50, selectionRect.y))) SetTooltip(GUIUtils.ColorFromHex("#ffcaca"), "Path", "");

            if (tags != "")
            {
                if (HasTooltip(xBase - 12, selectionRect.y))
                {
                    tags = tags.Replace(", ", "\n");
                    SetTooltip(GUIUtils.ColorFromHex("#fdd0f6"), "Hypertags", tags);
                }
            }
        }
    }

    private static void DrawCircle(float x, float y, Color color)
    {
        Rect rect = new Rect(x, y + 4, 10, 10);
        GUI.DrawTexture(rect, GUIUtils.GetTexture("FullCircle"), ScaleMode.ScaleToFit, true, 1.0f, color, 0.0f, 0.0f);
    }

    private static bool HasTooltip(float x, float y)
    {
        Rect rect = new Rect(x, y + 4, 10, 10);
        return rect.Contains(Event.current.mousePosition);
    }

    private static void SetTooltip(Color color, string title, string text)
    {
        var window = EditorWindow.GetWindow(typeof(OkapiTooltipWindow), true, title, false) as OkapiTooltipWindow;
        if (window == null) return;

        window.Show(color, text);
    }
}
