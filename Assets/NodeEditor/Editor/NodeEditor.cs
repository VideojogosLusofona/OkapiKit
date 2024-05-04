using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public abstract class BaseNodeEditor : EditorWindow
    {
        public class Theme
        {
            public Color    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            public bool     gridEnabled = true;
            public Color    gridColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            public float    gridSpacing = 100.0f;
            public bool     toolbarEnabled = true;
            public Color    toolbarBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public bool     menuAddNode = false;
            public string   windowName = "NodeEditor";
        }

        protected Theme                                     theme;
        protected Vector2                                   gridPanPosition = Vector2.zero;
        protected float                                     zoomScale = 1.0f;
        protected Vector2                                   zoomLimits = new Vector2(0.2f, 3.0f);   // Minimum zoom scale
        protected Texture2D                                 backgroundTexture;
        protected Texture2D                                 toolbarTexture;
        protected string                                    disableReason = "";
        protected Rect                                      worldSpaceExtents;
        protected bool                                      isPanning = false;
        protected Matrix4x4                                 currentMatrix;
        protected Matrix4x4                                 invCurrentMatrix;
        protected Dictionary<Type, Type>                    cachedRendererTypes;
        protected Dictionary<BaseNode, BaseNodeRenderer>    renderers = new();

        protected static T Init<T>(Theme theme) where T : BaseNodeEditor
        {
            T window = (T)GetWindow(typeof(T));
            if (window == null)
            {
                window = (T)GetWindow(typeof(T), false, theme.windowName);
                window.Show();
            }

            window.theme = theme;
            window.backgroundTexture = EditorUtils.GetColorTexture($"{theme.windowName}Background", theme.backgroundColor);
            window.toolbarTexture = EditorUtils.GetColorTexture($"{theme.windowName}ToolbarBackground", theme.toolbarBackgroundColor);
            window.ComputeMatrix();

            return window;
        }

        protected abstract UnityEngine.Object GetSelection();
        protected abstract void SetActiveSelection();
        protected virtual bool hasSelection => false;
        protected virtual string[] GetSubSelectorOptions() => null;
        protected virtual int GetSubSelectorSelected() => -1;
        protected virtual void SetSubSelector(string str) { }
        protected abstract void AddNode(Vector2 position);
        protected abstract void OnNodeCreate(object newNode, Vector2 addPosition);
        protected abstract List<BaseNode> GetNodes();

        void OnGUI()
        {
            if (theme == null)
            {
                GUILayout.Label("Restart window, missing theme...");
                return;
            }

            SetActiveSelection();

            // Content drawing will go here
            if (hasSelection)
            {
                Event e = Event.current;
                if (e != null) ProcessEvents(e);

                Clear();

                // Remove the group that already is set by Unity itself when it calls this function, which stops me from using negative
                // values.
                GUI.EndGroup();

                RenderWorkArea();

                if (!docked)
                {
                    GUIContent content = new GUIContent("Okapi Script Editor", "Okapi Script Editor");
                    var size = EditorStyles.miniButton.CalcSize(content);
                    GUI.Label(new Rect(0, 0, size.x, size.y), content, EditorStyles.miniButton);
                }

                // Restart the group (Unity is going to want to end the group itself, besides it's easier to handle toolbars, etc)
                GUI.BeginGroup(new Rect(0, 21.0f, Screen.width, Screen.height));
            }

            DrawToolbar();

            if (!hasSelection)
            {
                GUILayout.Label(disableReason);
            }

            if (isPanning)
            {
                if (!hasFocus)
                {
                    isPanning = false; // Reset panning if the window loses focus while panning
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.Pan);
                }
            }
        }

        protected virtual void OnEnable()
        {
            // Subscribe to the undo/redo event
            Undo.undoRedoPerformed += OnUndoRedo;

            Debug.Log("Registering undo performed");
        }

        private void OnDisable()
        {
            // Unsubscribe when the window is closed to clean up
            Undo.undoRedoPerformed -= OnUndoRedo;

            Debug.Log("Unregistering undo performed");
        }

        private void OnUndoRedo()
        {
            // Repaint the window whenever an undo or redo operation is performed
            Repaint();
            Debug.Log("Undo/redo performed");
        }

        void OnSelectionChange()
        {
            Repaint();  // This will refresh the editor window when the selection changes
        }

        void DrawToolbar()
        {
            if (!theme.toolbarEnabled) return;

            GUI.DrawTexture(new Rect(0, 0, position.width, 21.0f), toolbarTexture);

            GUILayout.BeginHorizontal(EditorStyles.toolbar);

            string[] subSelectorOptions = GetSubSelectorOptions();

            if (subSelectorOptions != null)
            {
                float maxWidth = 0.0f;
                foreach (var sso in subSelectorOptions)
                {
                    maxWidth = Mathf.Max(maxWidth, 10.0f + EditorStyles.toolbarDropDown.CalcSize(new GUIContent(sso, sso)).x);
                }
                int original = GetSubSelectorSelected();
                int selected = EditorGUILayout.Popup("", original, subSelectorOptions, EditorStyles.toolbarDropDown, GUILayout.Width(maxWidth));
                if (selected != original)
                {
                    SetSubSelector(subSelectorOptions[selected]);
                    Repaint();
                }
            }
            if (hasSelection)
            {
                var resetGUIContent = new GUIContent("Reset", "Reset View");
                if (GUILayout.Button(resetGUIContent, EditorStyles.toolbarButton, GUILayout.Width(EditorStyles.toolbarButton.CalcSize(resetGUIContent).x)))
                {
                    gridPanPosition = Vector2.zero;
                    zoomScale = 1.0f;
                    ComputeMatrix();
                    Repaint();
                }
            }
            GUILayout.EndHorizontal();
        }


        protected void Clear()
        {
            if (backgroundTexture != null)
            {
                GUI.DrawTexture(new Rect(0, 0, position.width, position.height), backgroundTexture);
            }
        }
        protected void RenderWorkArea()
        {
            float posY = (theme.toolbarEnabled) ? 21.0f : 0.0f;

            // Apply the combined transformation matrix
            var prevMatrix = GUI.matrix;
            GUI.matrix = currentMatrix;

            ComputeWorldSpaceExtents();

            if (theme.gridEnabled)
            {
                DrawGrid();
            }

            var nodes = GetNodes();
            foreach (var node in nodes)
            {
                DrawNode(node);
            }

            // Restore the original GUI matrix and end the group
            GUI.matrix = prevMatrix;
        }

        void DrawGrid()
        {
            Handles.BeginGUI();
            Handles.color = theme.gridColor;

            var rect = GetWorldSpaceExtents();

            rect.x = (Mathf.FloorToInt(rect.x / theme.gridSpacing) - 1) * theme.gridSpacing;
            rect.y = (Mathf.FloorToInt(rect.y / theme.gridSpacing) - 1) * theme.gridSpacing;
            rect.width = (Mathf.CeilToInt(rect.width / theme.gridSpacing) + 2) * theme.gridSpacing;
            rect.height = (Mathf.CeilToInt(rect.height / theme.gridSpacing) + 2) * theme.gridSpacing;

            for (float x = rect.xMin; x <= rect.xMax; x += theme.gridSpacing)
            {
                Handles.DrawLine(new Vector3(x, rect.y, 0), new Vector3(x, rect.y + rect.height, 0));
            }

            for (float y = rect.yMin; y <= rect.yMax; y += theme.gridSpacing)
            {
                Handles.DrawLine(new Vector3(rect.x, y, 0), new Vector3(rect.x + rect.width, y, 0));
            }
            Handles.EndGUI();
        }

        void DrawNode(BaseNode node)
        {
            if ((!renderers.TryGetValue(node, out BaseNodeRenderer renderer)) ||
                (renderer == null))
            {
                var rendererType = GetRendererType(node.GetType());
                if (rendererType == null)
                {
                    Debug.LogWarning($"No renderer for node type {node.GetType()}!");
                    return;
                }
                renderer = Activator.CreateInstance(rendererType) as BaseNodeRenderer;
                renderer.Init(node);

                renderers[node] = renderer;
            }
            
            renderer.Render();
        }

        Type GetRendererType(Type type)
        {
            if ((cachedRendererTypes == null) || (cachedRendererTypes.Count == 0))
            {
                cachedRendererTypes = new();

                foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
                {
                    foreach (var t in assembly.GetTypes())
                    {
                        if (t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(BaseNodeRenderer)))
                        {
                            var attr = t.GetCustomAttribute<NodeRendererAttribute>();
                            if (attr != null)
                            {
                                cachedRendererTypes.Add(attr.type, t);
                            }
                        }
                    }
                }
            }

            if (cachedRendererTypes.TryGetValue(type, out var rendererType))
            {
                return rendererType;
            }

            if (type.BaseType != null)
            {
                if (type.BaseType.IsClass)
                {
                    rendererType = GetRendererType(type.BaseType);
                    if (rendererType != null)
                    {
                        cachedRendererTypes[type] = rendererType;
                        return rendererType;
                    }
                }
            }

            return null;
        }

        protected virtual void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 1)
                    {
                        gridPanPosition += e.delta / zoomScale;
                        ComputeMatrix();
                        Repaint();
                        isPanning = true;
                    }
                    break;
                case EventType.ScrollWheel:
                    HandleZoom(e);
                    ComputeMatrix();
                    Repaint();
                    break;
                case EventType.MouseUp:
                    if (e.button == 1)  // End panning on mouse up
                    {
                        if (isPanning)
                        {
                            isPanning = false;
                            Repaint();
                        }
                    }
                    break;
                case EventType.Ignore: // Potentially handle ending a drag when mouse goes out of window
                    if (isPanning)
                    {
                        isPanning = false;
                        Repaint();
                    }
                    break;
                case EventType.ContextClick:
                    {
                        Vector2 mousePosition = Event.current.mousePosition; // Get the position of the mouse
                        if (ShowContextMenu(mousePosition))
                        {
                            Event.current.Use(); // Mark the event as used so it doesn't propagate further
                        }
                    }
                    break;
            }
        }

        void HandleZoom(Event e)
        {
            float oldZoom = zoomScale;
            zoomScale -= e.delta.y * 0.05f;
            zoomScale = Mathf.Clamp(zoomScale, zoomLimits.x, zoomLimits.y);

            // Optional: Adjust grid pan to zoom into the mouse position
            Vector2 screenCoordsMousePos = Event.current.mousePosition;
            Vector2 delta = screenCoordsMousePos - new Vector2(position.width / 2, position.height / 2);
            float diff = zoomScale - oldZoom;
            gridPanPosition -= delta * diff;

            e.Use();
        }

        Rect GetWorldSpaceExtents() => worldSpaceExtents;

        void ComputeWorldSpaceExtents()
        {
            var invMatrix = invCurrentMatrix;

            var c1 = invMatrix * new Vector4(0, 0, 0, 1);
            var c2 = invMatrix * new Vector4(position.width, position.height, 0, 1);

            worldSpaceExtents = new Rect(c1.x, c1.y, c2.x - c1.x, c2.y - c1.y);
        }

        void ComputeMatrix()
        {
            float posY = (theme.toolbarEnabled) ? 21.0f : 0.0f;

            // Prepare the transformation matrix
            Matrix4x4 translation = Matrix4x4.TRS(gridPanPosition, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * zoomScale);
            Matrix4x4 pivot = Matrix4x4.TRS(new Vector3(position.width / 2, position.height / 2 - posY, 0), Quaternion.identity, Vector3.one);

            // Apply the combined transformation matrix
            currentMatrix = pivot * scale * translation * pivot.inverse;
            invCurrentMatrix = currentMatrix.inverse;
        }

        bool ShowContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();

            // Add menu items
            if (theme.menuAddNode)
            {
                menu.AddItem(new GUIContent("Add Node"), false, () => AddNode(invCurrentMatrix * new Vector4(position.x, position.y, 0, 1)));
            }

            if (menu.GetItemCount() > 0)
            {
                // Show the menu at the mouse position
                menu.ShowAsContext();

                return true;
            }

            return false;
        }
    }

    public abstract class NodeEditor<T> : BaseNodeEditor where T : BaseNode
    {
        protected NodeType[] cachedNodeTypes = null;

        protected override void AddNode(Vector2 position)
        {
            if (cachedNodeTypes == null)
            {
                cachedNodeTypes = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(assembly => assembly.GetTypes())
                    .Where(t => t.IsClass && !t.IsAbstract && t.IsSubclassOf(typeof(T))) // Corrected BaseNode to OkapiNode based on your context
                    .Select(t => new NodeType
                    {
                        type = t,
                        name = GetNodeName(t)
                    })
                    .ToArray();
            }

            AddNodePopup.Init(cachedNodeTypes, CreateNodeOfType, position);
        }

        private void CreateNodeOfType(Type nodeType, Vector2 addPosition)
        {
            BaseNode newNode = (BaseNode)Activator.CreateInstance(nodeType);
            newNode.owner = GetSelection();
            // Assume we are managing nodes in some way in your editor window
            // This is where you would add the newNode to your system
            OnNodeCreate(newNode, addPosition);
        }

        private string GetNodeName(Type type)
        {
            // Attempt to get the NodePath attribute from the type
            var nodePathAttr = type.GetCustomAttribute<NodePathAttribute>();
            if (nodePathAttr != null)
            {
                return nodePathAttr.fullPath; // Use the path as the name if attribute exists
            }
            else
            {
                return EditorUtils.ToReadableName(type.Name); // Default to the type's name if no attribute is found
            }
        }
    }
}
