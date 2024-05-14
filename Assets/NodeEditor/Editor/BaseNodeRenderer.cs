using System;
using System.Collections.Generic;
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
        public int      sortOrder = 0;
        public bool     selected = false;

        public void Init(BaseNode node)
        {
            this.node = node;
            OnInit();
        }
        public void Render(float zoomFactor)
        {
            EditorGUI.BeginChangeCheck();

            OnRender(zoomFactor);

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(node.owner, "Property Changed");
                CommitChanges();
                EditorUtility.SetDirty(node.owner);
            }
        }
        protected abstract void OnInit();
        protected abstract void OnRender(float zoomFactor);
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
        protected Color     bgColor = Color.black;
        protected Color     textColor = Color.white;
        protected float     width = 100.0f;
        protected GUIStyle  titleStyle;
        protected string    titleString;

        protected bool      newNodeEnabled;

        SerializedObject            serializedNode;
        SerializedProperty          nodeList;
        SerializedProperty          nodeProperty;

        protected class NodeProperty
        {
            public SerializedProperty  property;
        }

        protected List<NodeProperty>    nodeProperties;

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

            // Prepare data for display - This is kind of convoluted because
            // Unity expects UnityEngine.Object classes, but nodes aren't
            // So I need to use the scriptable object as the serialized object that gets modified, etc, and for the list of properties 
            // I have to search for a list of BaseNode or derived, and then go into that looking for the SerializedProperty that matches
            // this node, and then I can get the serialized properties that I actually need.
            serializedNode = new SerializedObject(node.owner);
            Type nodeOwnerType = node.owner.GetType();
            Type nodeType = node.GetType();
            nodeList = SearchForNodeList();
            if (nodeList == null)
            {
                Debug.LogWarning("No list of Nodes that can be analyzed on the owner object!");
                return;
            }
            // Find the node I want in the nodeList
            nodeProperty = FindNode(node);
            if (nodeProperty == null)
            {
                Debug.LogWarning($"Node could not be found in owner's node list (property {nodeList.name})!");
                return;
            }

            // Create list of properties
            nodeProperties = new();

            var     nodePropertyIterator = nodeProperty.Copy();
            bool    enterChildren = true;
            // Enter the children of the root property (the BaseNode itself)
            while (nodePropertyIterator.NextVisible(enterChildren))
            {
                // Stop when we get back to the parent depth
                if (nodePropertyIterator.depth == nodeProperty.depth + 1)
                {
                    enterChildren = false;

                    FieldInfo   fieldInfo = nodeType.GetFieldInfo(nodePropertyIterator.name);
                    Type        propertyOriginType = fieldInfo.DeclaringType;
                    bool        visible = false;
                    var         visProp = fieldInfo.GetCustomAttribute<NodePropertyVisibilityAttribute>();
                    if (visProp != null)
                    {
                        visible = visProp.nodeVisibility;
                    }
                    else
                    {
                        var defaultVisProp = propertyOriginType.GetCustomAttribute<NodeDefaultPropertyVisibilityAttribute>();
                        if (defaultVisProp != null)
                        {
                            visible = defaultVisProp.defaultVisibility;
                        }
                    }
                    if (visible)
                    {
                        nodeProperties.Add(new NodeProperty { property = nodePropertyIterator.Copy() });
                    }
                }
                else if (nodePropertyIterator.depth <= nodeProperty.depth)
                {
                    break; // We've iterated past the children we care about
                }
            }

            foreach (var p in nodeProperties)
            {
                Debug.Log($"Property {p.property.propertyPath} found");
            }
        }
        
        protected SerializedProperty SearchForNodeList()
        {
            SerializedProperty property = serializedNode.GetIterator();
            bool next = property.NextVisible(true);
            while (next)
            {
                if (property.IsListOfT<BaseNode>())
                { 
                    return property;
                }
                 
                next = property.NextVisible(false);
            }

            return null;
        }

        protected SerializedProperty FindNode(BaseNode node)
        {
            for (int i = 0; i < nodeList.arraySize; i++)
            {
                var element = nodeList.GetArrayElementAtIndex(i);

                // Check if node object is the same as pointed by the serialized property element
                if (element.GetSerializedPropertyValue() == node)
                {
                    // Return the serialized property pointing to this node
                    return element;
                }
            }

            return null;
        }

        protected override void OnRender(float zoomFactor)
        {
            // All the part below is non-interactive (background, border, labels)
            // Get color and width from node attributes
            Rect rect = new Rect(node.position.x, node.position.y, width, GetHeight());

            float outlineThickness = (selected) ? (2) : (1);
            outlineThickness /= zoomFactor;

            DrawBoxWithOutline(rect, new Color(0.2f, 0.2f, 0.2f, 1.0f), (selected) ? (Color.white) : (Color.black), outlineThickness);

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
