using UnityEditor;
using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace NodeEditor
{

    public class AddNodePopup : EditorWindow
    {
        static protected GUIStyle      toolbarSkinStyle;
        static protected GUIStyle      toolbarSearchTextField;
        static protected GUIStyle      toolbarSearchCancelButton;
        static protected GUIStyle      clickableLabelStyle;
        static protected Texture2D     highlightTexture;
        static protected bool          skinCache = false;
        
        protected Vector2                       scrollPosition;
        protected string                        searchString = "";
        protected Action<Type, Vector2>         onTypeSelected;
        protected bool                          autoFocus = true;
        protected TreeNode                      rootNode;
        protected object                        lastHighlight;
        protected object                        newHighlight;
        protected Vector2                       addPosition;
        protected Dictionary<TreeNode, bool>    foldoutStates = new();

        protected class TreeNode
        {
            public string           name;
            public List<TreeNode>   children;
            public List<NodeType>   types;

            public bool isEmpty => ((children == null) || (children.Count == 0)) &&
                                   ((types == null) || (types.Count == 0));

            public void Add(string name, Type type)
            {
                int index = name.IndexOf('/');
                if (index != -1)
                {
                    if (children == null) children = new List<TreeNode>();

                    var localPath = name.Substring(0, index);
                    var remainingPath = name.Substring(index + 1);
                    foreach (var c in children)
                    {
                        if (c.name == localPath)
                        {
                            c.Add(remainingPath, type);
                            return;
                        }
                    }

                    var tn = new TreeNode();
                    tn.name = localPath;
                    children.Add(tn);
                    tn.Add(remainingPath, type);
                    
                }
                else
                {
                    if (types == null) types = new List<NodeType>();
                    types.Add(new NodeType(name, type));
                }
            }

            internal bool HasChildElement(string searchString)
            {
                if (types != null)
                {
                    foreach (var t in types)
                    {
                        if ((t.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0) ||
                            (t.type.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0))
                        {
                            return true;
                        }
                    }
                }

                if (children != null)
                {
                    foreach (var c in children)
                    {
                        if (c.HasChildElement(searchString)) return true;
                    }
                }

                return false;
            }
        }

        public static AddNodePopup Init(NodeType[] nodeTypes, Action<Type, Vector2> onTypeSelected, Vector2 position)
        {
            AddNodePopup window = GetWindow<AddNodePopup>(true, "Add Node");
            window.onTypeSelected = onTypeSelected;
            window.addPosition = position;
            window.Build(nodeTypes);
            window.ShowPopup();
            window.wantsMouseMove = true;

            return window;
        }

        void OnSelectionChange()
        {
            Close(); // Close if selection changed
        }

        void Build(NodeType[] nodeType)
        {
            rootNode = new TreeNode();
            rootNode.name = "";

            foreach (var node in nodeType)
            {
                rootNode.Add(node.name, node.type);
            }
        }

        private void DrawTree(TreeNode node)
        {
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                if (!node.HasChildElement(searchString))
                {
                    return;
                }
            }

            // Draw the node as a foldout if it has children or types
            if (!node.isEmpty)
            {
                if (node.name == "")
                {
                    // Recursively draw each child node
                    if (node.children != null)
                    {
                        foreach (TreeNode child in node.children)
                        {
                            DrawTree(child);
                        }
                    }

                    // Draw all types at this node as buttons
                    if (node.types != null)
                    {
                        foreach (NodeType type in node.types)
                        {
                            if ((string.IsNullOrWhiteSpace(searchString)) || 
                                (type.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                (type.type.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0))
                            {
                                DrawNodeType(type);
                            }
                        }
                    }
                }
                else
                {
                    bool foldoutState = false;
                    if (foldoutStates.ContainsKey(node))
                    {
                        foldoutState = foldoutStates[node];
                    }
                    else
                    {
                        foldoutState = true;
                    }

                    foldoutState = EditorGUILayout.Foldout(foldoutState, node.name, true);

                    // Save the current state back to the dictionary
                    foldoutStates[node] = foldoutState;

                    if (foldoutState)
                    {
                        EditorGUI.indentLevel++;

                        // Recursively draw each child node
                        if (node.children != null)
                        {
                            foreach (TreeNode child in node.children)
                            {
                                DrawTree(child);
                            }
                        }

                        // Draw all types at this node as buttons
                        if (node.types != null)
                        {
                            foreach (NodeType type in node.types)
                            {
                                if ((string.IsNullOrWhiteSpace(searchString)) || 
                                    (type.name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0) ||
                                    (type.type.Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0))
                                {
                                    DrawNodeType(type);
                                }
                            }
                        }

                        EditorGUI.indentLevel--;
                    }
                }
            }
        }

        private void DrawNodeType(NodeType nodeType)
        {
            var rect = GUILayoutUtility.GetRect(new GUIContent(nodeType.name), clickableLabelStyle);
            var indentedRect = EditorGUI.IndentedRect(rect);

            // Handle hover state manually
            if (indentedRect.Contains(Event.current.mousePosition))
            {
                EditorGUIUtility.AddCursorRect(rect, MouseCursor.Link);
                if (Event.current.type == EventType.MouseDown && Event.current.button == 0)
                {
                    onTypeSelected?.Invoke(nodeType.type, addPosition);
                    Close();
                    Event.current.Use(); // Consume the event
                }
                
                GUI.DrawTexture(rect, highlightTexture);
                if (lastHighlight != nodeType)
                {
                    newHighlight = nodeType;
                    Repaint();
                }
            }
            else
            {
                if (lastHighlight != null)
                {
                    Repaint();
                }
            }

            GUI.Label(indentedRect, nodeType.name, clickableLabelStyle);
        }

        private void OnGUI()
        {
            newHighlight = null;

            if (rootNode == null)
            {
                Debug.LogWarning("rootNode is null: reopen add node window!");
                Close();
                return;
            }

            if (!skinCache)
            {
                toolbarSkinStyle = GUI.skin.FindStyle("Toolbar");
                toolbarSearchTextField = GUI.skin.FindStyle("ToolbarSearchTextField");
                toolbarSearchCancelButton = GUI.skin.FindStyle("ToolbarSearchCancelButton");

                clickableLabelStyle = new GUIStyle(GUI.skin.label);
                clickableLabelStyle.normal.textColor = GUI.skin.button.normal.textColor;
                clickableLabelStyle.hover.textColor = GUI.skin.button.hover.textColor;
                clickableLabelStyle.padding = new RectOffset(0, 0, 2, 2);
                clickableLabelStyle.margin = new RectOffset(0, 0, 2, 2);
                clickableLabelStyle.alignment = TextAnchor.MiddleLeft;

                highlightTexture = EditorUtils.GetColorTexture("AddNodePopupHighlight", new Color(0.2f, 0.2f, 0.2f, 1.0f));

                skinCache = true;
            }
            // Search bar
            GUILayout.BeginHorizontal(toolbarSkinStyle);

            GUI.SetNextControlName("SearchField");

            searchString = GUILayout.TextField(searchString, toolbarSearchTextField);
            if (GUILayout.Button("", toolbarSearchCancelButton))
            {
                searchString = "";
                GUI.FocusControl(null);
            }
            GUILayout.EndHorizontal();

            if (autoFocus)
            {
                EditorGUI.FocusTextInControl("SearchField");
                autoFocus = false;  // Ensure it only happens once
            }

            // List of node types
            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            DrawTree(rootNode);
            GUILayout.EndScrollView();

            lastHighlight = newHighlight;
        }
    }
}
