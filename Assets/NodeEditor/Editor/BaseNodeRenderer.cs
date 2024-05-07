using System;
using System.Reflection;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEngine;
using static UnityEditor.PlayerSettings;

namespace NodeEditor
{
    public abstract class BaseNodeRenderer
    {
        public BaseNode node;
        public bool     selected = false;

        public void Init(BaseNode node)
        {
            this.node = node;
            OnInit();
        }
        public void Render()
        {
            EditorGUI.BeginChangeCheck();

            OnRender();

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(node.owner, "Property Changed");
                CommitChanges();
                EditorUtility.SetDirty(node.owner);
            }
        }
        protected abstract void OnInit();
        protected abstract void OnRender();
        protected abstract void CommitChanges();

        protected void DrawBoxWithOutline(Rect rect, Color boxColor, Color outlineColor, float outlineThickness)
        {
            // Draw outline
            EditorGUI.DrawRect(new Rect(rect.x - outlineThickness, rect.y - outlineThickness, rect.width + (outlineThickness * 2), rect.height + (outlineThickness * 2)), outlineColor);

            // Draw box
            EditorGUI.DrawRect(rect, boxColor);
        }

        public bool IsHovering(Vector2 pos)
        {
            return GetRect().Contains(pos, true);
        }

        public bool IsOnRect(Rect rect)
        {
            Rect nodeRect = GetRect();

            return nodeRect.Overlaps(rect, true);
        }

        public abstract Rect GetRect();
        public virtual void OnSelect() { }
        public virtual void OnDeselect() { }
    }

    [NodeRenderer(typeof(BaseNode))]
    public class BasicNodeRenderer : BaseNodeRenderer
    {
        Color       bgColor = Color.black;
        Color       textColor = Color.white;
        float       width = 100.0f;
        GUIStyle    titleStyle;
        string      titleString;

        bool        newNodeEnabled;

        const float titleHeight = 30.0f;
        const float enableMargin = 5.0f;
        const float enableWidth = 21.0f;
        const float enableSpacing = 5.0f;
        const float rightMargin = 50.0f;

        protected override void OnInit()
        {
            var type = node.GetType();
            var colorAttr = type.GetCustomAttribute<NodeColorAttribute>();
            if (colorAttr != null)
            {
                bgColor = colorAttr.bgColor;
                textColor = colorAttr.textColor;
            }
            var widthAttr = type.GetCustomAttribute<NodeWidthAttribute>();
            if (widthAttr != null) width = widthAttr.width;

            var pathAttr = type.GetCustomAttribute<NodePathAttribute>();
            if (pathAttr != null) titleString = pathAttr.name;
            else titleString = type.Name;

            titleStyle = new GUIStyle(GUI.skin.label);
            titleStyle.fontSize = 24;
            titleStyle.fixedHeight = 24;
            titleStyle.normal.textColor = textColor;
            titleStyle.hover.textColor = textColor;
            titleStyle.active.textColor = textColor;
            titleStyle.clipping = TextClipping.Overflow;
            titleStyle.wordWrap = false;

            var label = new GUIContent(titleString);
            width = Mathf.Max(titleStyle.CalcSize(label).x + (enableMargin + enableWidth + enableSpacing + rightMargin), width);
        }

        protected override void OnRender()
        {
            // All the part below is non-interactive (background, border, labels)
            // Get color and width from node attributes
            Rect rect = new Rect(node.position.x, node.position.y, width, GetHeight());

            DrawBoxWithOutline(rect, new Color(0.2f, 0.2f, 0.2f, 1.0f), (selected) ? (Color.white) : (Color.black), (selected) ? (2) : (1));

            // Draw the node rectangle
            Rect titleRect = rect;
            titleRect.height = titleHeight;
            EditorGUI.DrawRect(titleRect, bgColor);

            // Node label
            var label = new GUIContent(titleString);
            float height = titleStyle.CalcSize(label).y;
            Rect labelRect = titleRect;
            labelRect.x += enableMargin + enableWidth + enableSpacing;
            labelRect.y += labelRect.height * 0.5f - height * 0.5f;
            labelRect.width -= enableMargin + enableWidth + enableSpacing;
            labelRect.height = height;
            GUI.Label(labelRect, titleString, titleStyle);

            // Node enable toggle
            Rect toggleRect = titleRect;
            toggleRect.x += enableMargin;
            toggleRect.y += toggleRect.height * 0.5f - 10.0f;
            toggleRect.width = toggleRect.height = enableWidth;
            newNodeEnabled = EditorGUI.Toggle(toggleRect, "", node.nodeEnabled);
        }

        protected override void CommitChanges()
        {
            node.nodeEnabled = newNodeEnabled;
        }

        float GetHeight()
        {
            return titleHeight + 50.0f;
        }

        public override Rect GetRect()
        {
            return new Rect(node.position.x, node.position.y, width, GetHeight());
        }
    }
}
