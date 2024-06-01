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
                OnChanges();
                EditorUtility.SetDirty(node.owner);
            }
        }
        protected abstract void OnInit();
        protected abstract void OnRender(float zoomFactor);
        protected abstract void CommitChanges();
        protected virtual void OnChanges() { ; }

        protected void DrawBoxWithOutline(Rect rect, Color boxColor, Color outlineColor, float outlineThickness)
        {
            // Draw outline
            EditorGUI.DrawRect(new Rect(rect.x - outlineThickness, rect.y - outlineThickness, rect.width + (outlineThickness * 2), rect.height + (outlineThickness * 2)), outlineColor);

            // Draw box
            EditorGUI.DrawRect(rect, boxColor);
        }

        public virtual bool IsHovering(Vector2 pos)
        {
            return GetRect().Contains(pos, true);
        }

        public virtual bool IsHoveringForMove(Vector2 pos) => IsHovering(pos);

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
        protected Texture2D connectorTexture;

        SerializedObject            serializedNode;
        SerializedProperty          nodeList;
        SerializedProperty          nodeProperty;

        protected class NodeProperty
        {
            public const int None = 0;
            public const int Input = 1 << 0;  
            public const int Output = 1 << 1;

            public string              name;
            public SerializedProperty  property;
            public int                 flags = None;
            public float               height;
            public Type                propertyType;
            public Color               connectorColor;
        }

        protected List<NodeProperty>    nodeProperties;
        protected float                 propertyHeight;
        protected int                   propertyFlags;

        const float titleHeight = 30.0f;
        const float xPadding = 4.0f;
        const float titleToPropertySpacing = 8.0f;
        const float connectorWidth = 8.0f;
        const float connectorHeight = connectorWidth;
        const float connectorMargin = 4.0f;

        protected override void OnInit()
        {
            connectorTexture = EditorUtils.GetTexture("connector_node");

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
            width = Mathf.Max(titleStyle.CalcSize(label).x + xPadding * 2.0f, width);

            // Prepare data for display - This is kind of convoluted because
            // Unity expects UnityEngine.Object classes, but nodes aren't
            // So I need to use the scriptable object as the serialized object that gets modified, etc, and for the list of properties 
            // I have to search for a list of BaseNode or derived, and then go into that looking for the SerializedProperty that matches
            // this node, and then I can get the serialized properties that I actually need.
            serializedNode = new SerializedObject(node.owner);
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
                        int     f = NodeProperty.None;

                        var nodeInput = fieldInfo.GetCustomAttribute<NodeInputAttribute>();
                        if (nodeInput != null)
                        {
                            f |= NodeProperty.Input;
                        }
                        var nodeOutput = fieldInfo.GetCustomAttribute<NodeOutputAttribute>();
                        if (nodeOutput != null)
                        {
                            f |= NodeProperty.Output;
                        }

                        var nodeProperty = new NodeProperty
                        {
                            name = nodePropertyIterator.name,
                            property = nodePropertyIterator.Copy(),
                            flags = f,
                            propertyType = fieldInfo.FieldType,
                            connectorColor = Info.GetTypeColor(fieldInfo.FieldType)
                        };

                        nodeProperties.Add(nodeProperty);
                    }
                }
                else if (nodePropertyIterator.depth <= nodeProperty.depth)
                {
                    break; // We've iterated past the children we care about
                }
            }

            RefreshSizes();
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

        void RefreshSizes()
        {
            propertyHeight = 0;
            propertyFlags = 0;

            foreach (var property in nodeProperties)
            {
                property.height = EditorGUI.GetPropertyHeight(property.property);

                propertyHeight += property.height + EditorGUIUtility.standardVerticalSpacing;
                propertyFlags |= property.flags;
            }
        }

        protected override void OnChanges() 
        {
            RefreshSizes(); 
        }
        public override bool IsHoveringForMove(Vector2 pos)
        {
            var r = new Rect(node.position.x, node.position.y, width, titleHeight);
            return r.Contains(pos);
        }

        protected override void OnRender(float zoomFactor)
        {
            // Object to serialized property
            serializedNode.UpdateIfRequiredOrScript();

            // All the part below is non-interactive (background, border, labels)
            // Get color and width from node attributes
            Rect rect = new Rect(node.position.x, node.position.y, width, GetHeight());

            float outlineThickness = (selected) ? (2) : (1);
            outlineThickness /= zoomFactor;

            DrawBoxWithOutline(rect, new Color(0.2f, 0.2f, 0.2f, 1.0f), (selected) ? (Color.white) : (Color.black), outlineThickness);

            // Draw the node rectangle
            Rect titleRect = rect;
            titleRect.height = titleHeight;

            Color c = bgColor;
            if (!node.nodeEnabled) c = new Color(c.r * 0.5f, c.g * 0.5f, c.b * 0.5f, c.a);
            EditorGUI.DrawRect(titleRect, c);

            // Node label
            var label = new GUIContent(titleString);
            float height = titleStyle.CalcSize(label).y;
            Rect labelRect = titleRect;
            labelRect.x += xPadding;
            labelRect.y += labelRect.height * 0.5f - height * 0.5f;
            labelRect.width -= xPadding * 2.0f;
            labelRect.height = height;
            GUI.Label(labelRect, titleString, titleStyle);

            Rect propertyRect = rect;
            propertyRect.x += xPadding;
            propertyRect.y += titleHeight + titleToPropertySpacing;
            propertyRect.width -= xPadding * 2.0f;
            propertyRect.height -= titleHeight + titleToPropertySpacing;

            Rect inputRect = rect;
            inputRect.x += connectorMargin;
            inputRect.width = connectorWidth;
            inputRect.height = connectorWidth;
            Rect outputRect = rect;
            outputRect.x += outputRect.width - connectorWidth - connectorMargin;
            outputRect.width = connectorWidth;
            outputRect.height = connectorWidth;

            if ((propertyFlags & NodeProperty.Input) != 0)
            {
                propertyRect.x += connectorWidth + connectorMargin;
                propertyRect.width -= connectorWidth + connectorMargin * 2;
            }

            if ((propertyFlags & NodeProperty.Output) != 0)
            {
                propertyRect.width -= connectorWidth + connectorMargin * 2;
            }

            foreach (var property in nodeProperties)
            {
                propertyRect.height = EditorGUI.GetPropertyHeight(property.property);

                EditorGUI.PropertyField(propertyRect, property.property, true);

                if ((property.flags & NodeProperty.Input) != 0)
                {
                    inputRect.y = propertyRect.y + (property.height - connectorHeight) * 0.5f;
                    var prevColor = GUI.color;
                    GUI.color = property.connectorColor;
                    GUI.DrawTexture(inputRect, connectorTexture);
                    GUI.color = prevColor;
                }

                if ((property.flags & NodeProperty.Output) != 0)
                {
                    outputRect.y = propertyRect.y + (property.height - connectorHeight) * 0.5f;
                    var prevColor = GUI.color;
                    GUI.color = property.connectorColor;
                    GUI.DrawTexture(outputRect, connectorTexture);
                    GUI.color = prevColor;
                }

                propertyRect.y += propertyRect.height + EditorGUIUtility.standardVerticalSpacing;
            }
        }

        protected override void CommitChanges()
        {
            serializedNode.ApplyModifiedPropertiesWithoutUndo();
        }

        float GetHeight()
        {
            return titleHeight + propertyHeight + titleToPropertySpacing;
        }

        public override Rect GetRect()
        {
            return new Rect(node.position.x, node.position.y, width, GetHeight());
        }
    }
}
