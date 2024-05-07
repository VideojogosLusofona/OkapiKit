using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEditor.PlayerSettings;
using UnityEditor.PackageManager.UI;

namespace NodeEditor
{
    public abstract class BaseNodeEditor : EditorWindow
    {
        public class Theme
        {
            public Color    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            public Color    selectionRectangleColor = new Color(0.4f, 1.0f, 1.0f, 0.25f);
            public bool     gridEnabled = true;
            public Color    gridColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            public float    gridSpacing = 100.0f;
            public bool     toolbarEnabled = true;
            public Color    toolbarBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public bool     menuAddNode = false;
            public bool     panEdge = true;
            public float    panMargin = 20.0f;
            public float    panSpeed = 2.0f;
            public string   windowName = "NodeEditor";
        }

        protected Theme                                     theme;
        protected Vector2                                   panPosition = Vector2.zero;
        protected float                                     zoomScale = 1.0f;
        protected Vector2                                   zoomLimits = new Vector2(0.2f, 3.0f);   // Minimum zoom scale
        protected Texture2D                                 backgroundTexture;
        protected Texture2D                                 toolbarTexture;
        protected Texture2D                                 selectionTexture;
        protected string                                    disableReason = "";
        protected Rect                                      worldSpaceExtents;
        protected bool                                      isRectSelecting = false;
        protected Rect                                      selectionRect;
        protected bool                                      _isPanning = false;
        protected DateTime                                  lastPan;
        protected Matrix4x4                                 currentMatrix;
        protected Matrix4x4                                 invCurrentMatrix;
        protected Dictionary<BaseNode, BaseNodeRenderer>    renderers = new();
        protected List<BaseNodeRenderer>                    nodeSelection = new();
        protected List<BaseNodeRenderer>                    initialSelection = new();
        

        protected bool isPanning
        {
            get { return _isPanning; }
            set { _isPanning = value; if (!_isPanning) lastPan = DateTime.Now; }
        }

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
            window.selectionTexture = EditorUtils.GetColorTexture($"{theme.windowName}SelectionRectangle", theme.selectionRectangleColor);
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
        protected abstract void OnNodeCreate(BaseNode newNode, Vector2 addPosition);
        protected abstract void OnNodeDelete(BaseNode newNode);
        protected abstract List<BaseNode> GetNodes();

        protected bool shouldPan => (theme.panEdge) && (isRectSelecting);

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
                if (e != null)
                {
                    ProcessEvents(e);

                    // Run edge pan
                    if (shouldPan)
                    {
                        var mp = e.mousePosition;
                        bool shouldUpdate = false;
                        float ps = theme.panSpeed / zoomScale;
                        if (mp.x < theme.panMargin) { panPosition.x += ps; shouldUpdate = true; }
                        if (mp.y < theme.panMargin) { panPosition.y += ps; shouldUpdate = true; }
                        if (mp.x > position.width - theme.panMargin) { panPosition.x -= ps; shouldUpdate = true; }
                        if (mp.y > position.height - theme.panMargin) { panPosition.y -= ps; shouldUpdate = true; }

                        if (shouldUpdate)
                        {
                            ComputeMatrix();

                            UpdateSelectionRect();

                            Repaint();
                        }
                    }
                }

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
        }

        private void OnDisable()
        {
            // Unsubscribe when the window is closed to clean up
            Undo.undoRedoPerformed -= OnUndoRedo;
        }

        private void OnUndoRedo()
        {
            // Repaint the window whenever an undo or redo operation is performed
            Repaint();
        }

        void OnSelectionChange()
        {
            Repaint();
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
                    panPosition = Vector2.zero;
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

            if (isRectSelecting)
            {
                GUI.DrawTexture(selectionRect, selectionTexture);

                // Check who is highlighted
                var nodesOnCursor = GetNodesAtRect(selectionRect);
                foreach (var nodeRenderer in nodesOnCursor)
                {
                    if (!nodeRenderer.selected)
                    {
                        AddNodeToSelection(nodeRenderer);
                    }
                }
                List<BaseNodeRenderer> toRemove = new();
                foreach (var nodeRenderer in nodeSelection)
                {
                    if ((initialSelection.IndexOf(nodeRenderer) < 0) &&
                        (nodesOnCursor.IndexOf(nodeRenderer) < 0))
                    {
                        toRemove.Add(nodeRenderer);
                    }
                }
                foreach (var nodeRenderer in toRemove)
                {
                    RemoveNodeFromSelection(nodeRenderer);
                }

                Repaint();
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
            if (node == null) return;
            if ((!renderers.TryGetValue(node, out BaseNodeRenderer renderer)) ||
                (renderer == null))
            {
                var rendererType = Info.GetRendererType(node.GetType());
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

        protected virtual void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if ((e.button == 0) && (!isPanning) && (!isRectSelecting))
                    {
                        bool ctrl = (e.control || e.command); // Ctrl on Windows/Linux, Command on macOS

                        // Can be a selection
                        var node = GetNodeAtMouse();

                        if (!ctrl) ClearNodeSelection();
                        if (node != null)
                        {
                            if (ctrl) ToggleNodeSelection(node);
                            else AddNodeToSelection(node);
                        }
                        Repaint();
                    }
                    break;
                case EventType.MouseDrag:
                    if ((e.button == 0) && (!isPanning))
                    {
                        if (!isRectSelecting)
                        {
                            // Start selection
                            selectionRect = new Rect(GetMouseWorldPosition(), Vector2.zero); 
                            isRectSelecting = true;
                            initialSelection = new(nodeSelection);
                        }
                        else
                        {
                            UpdateSelectionRect();
                        }
                    }
                    else if ((e.button == 1) && (!isRectSelecting))
                    {
                        panPosition += e.delta / zoomScale;
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
                    if ((e.button == 0) && (isRectSelecting))
                    {
                        isRectSelecting = false;
                        Repaint();
                    }
                    else if ((e.button == 1)  && (isPanning)) // End panning on mouse up
                    {
                        isPanning = false;
                        Repaint();
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
                        if ((!isPanning) && (!isRectSelecting) && ((DateTime.Now - lastPan).Milliseconds > 100.0f))
                        {
                            Vector2 mousePosition = Event.current.mousePosition; // Get the position of the mouse
                            if (ShowContextMenu(mousePosition))
                            {
                                Event.current.Use(); // Mark the event as used so it doesn't propagate further
                            }
                        }
                    }
                    break;
            }
        }

        void UpdateSelectionRect()
        {
            Vector2 currentPos = GetMouseWorldPosition();
            selectionRect.width = currentPos.x - selectionRect.x;
            selectionRect.height = currentPos.y - selectionRect.y;

        }

        protected Vector2 GetMouseWorldPosition()
        {
            var pos = Event.current.mousePosition;

            var deltaY = (theme.toolbarEnabled) ? (21.0f) : (0.0f);

            // Convert position to world coordinates
            var worldPos4 = invCurrentMatrix * new Vector4(pos.x, pos.y + deltaY, 0, 1);

            return new Vector2(worldPos4.x, worldPos4.y);
        }

        protected BaseNodeRenderer GetNodeAtMouse()
        {
            var pos = GetMouseWorldPosition();

            foreach (var nodeRenderer in renderers)
            {
                var node = nodeRenderer.Value;

                if (node.IsHovering(pos))
                {
                    return node;
                }
            }

            return null;
        }

        protected List<BaseNodeRenderer> GetNodesAtRect(Rect rect)
        {
            var ret = new List<BaseNodeRenderer>();

            foreach (var nodeRenderer in renderers)
            {
                var node = nodeRenderer.Value;

                if (node.IsOnRect(rect))
                {
                    ret.Add(node);
                }
            }

            return ret;
        }

        void HandleZoom(Event e)
        {
            float oldZoom = zoomScale;
            zoomScale -= e.delta.y * 0.05f;
            zoomScale = Mathf.Clamp(zoomScale, zoomLimits.x, zoomLimits.y);

            // Optional: Adjust grid pan to zoom into the mouse position
            Vector2 screenCoordsMousePos = Event.current.mousePosition;
            //Vector4 worldCoordsMousePos4 = invCurrentMatrix * new Vector4(screenCoordsMousePos.x, screenCoordsMousePos.y, 0, 1);
            //Vector2 worldCoordsMousePos = new Vector2(worldCoordsMousePos4.x, worldCoordsMousePos4.y);
            Vector2 delta = screenCoordsMousePos - new Vector2(position.width / 2, position.height / 2);
            float diff = zoomScale - oldZoom;
            panPosition -= delta * diff;

            e.Use();
        }

        Rect GetWorldSpaceExtents() => worldSpaceExtents;

        void ComputeWorldSpaceExtents()
        {
            var c1 = invCurrentMatrix * new Vector4(0, 0, 0, 1);
            var c2 = invCurrentMatrix * new Vector4(position.width, position.height, 0, 1);

            worldSpaceExtents = new Rect(c1.x, c1.y, c2.x - c1.x, c2.y - c1.y);
        }

        void ComputeMatrix()
        {
            // Prepare the transformation matrix
            Matrix4x4 translation = Matrix4x4.TRS(panPosition, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * zoomScale);
            Matrix4x4 pivot = Matrix4x4.TRS(new Vector3(position.width / 2, position.height / 2, 0), Quaternion.identity, Vector3.one);

            // Apply the combined transformation matrix
            currentMatrix = pivot * scale * translation * pivot.inverse;
            invCurrentMatrix = currentMatrix.inverse;
        }

        bool ShowContextMenu(Vector2 position)
        {
            GenericMenu menu = new GenericMenu();

            var hoverNode = GetNodeAtMouse();

            // Add nodes option
            if (theme.menuAddNode)
            {
                if (hoverNode == null)
                    menu.AddItem(new GUIContent("Add Node"), false, () => AddNode(invCurrentMatrix * new Vector4(position.x, position.y, 0, 1)));
                else
                    menu.AddDisabledItem(new GUIContent("Add Node"), false);
            }

            // Delete node option
            var delName = "Delete Node";
            var delEnable = false;

            if (hoverNode != null)
            {
                if (hoverNode.selected)
                {
                    delEnable = true;
                    if (selectedNodeCount > 1)
                    {
                        delName = "Delete Nodes";
                    }
                }
            }
            else
            {
                if (selectedNodeCount > 0)
                {
                    delEnable = true;
                    if (selectedNodeCount > 1)
                        delName = "Delete Nodes";
                }
            }
            if (delEnable)
                menu.AddItem(new GUIContent(delName), false, () => DeleteSelectedNodes());
            else
                menu.AddDisabledItem(new GUIContent(delName), false);

            if (menu.GetItemCount() > 0)
            {
                // Show the menu at the mouse position
                menu.ShowAsContext();

                return true;
            }

            return false;
        }

        public int selectedNodeCount => nodeSelection.Count;
        public void ClearNodeSelection()
        {
            var tmp = nodeSelection;
            nodeSelection = new();
            foreach (var n in tmp)
            {
                n.OnDeselect();
                n.selected = false;
            }
        }

        public void AddNodeToSelection(BaseNodeRenderer node)
        {
            // Get node renderer
            if ((node != null) && (!node.selected))
            {
                nodeSelection.Add(node);
                node.selected = true;
                node.OnSelect();
            }
        }

        public void RemoveNodeFromSelection(BaseNodeRenderer node)
        {
            if ((node != null) && (node.selected))
            {
                nodeSelection.Remove(node);
                node.selected = false;
                node.OnDeselect();
            }
        }

        public void ToggleNodeSelection(BaseNodeRenderer node)
        {
            if (node != null)
            {
                if (node.selected)
                {
                    RemoveNodeFromSelection(node);
                }
                else
                {
                    AddNodeToSelection(node);
                }
            }
        }

        protected void DeleteSelectedNodes()
        {
            var tmp = nodeSelection;
            nodeSelection = new();
            ClearNodeSelection();

            foreach (var node in tmp)
            {
                renderers.Remove(node.node);
                OnNodeDelete(node.node);
            }
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
