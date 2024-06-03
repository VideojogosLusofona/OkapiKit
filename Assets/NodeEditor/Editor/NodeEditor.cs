using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEditor.PackageManager.UI;
using UnityEditor.Graphs;

namespace NodeEditor
{
    public abstract class BaseNodeEditor : EditorWindow
    {
        protected struct Line
        {
            public Vector2  p1, p2;
            public Color    color;
        };
        public class Theme
        {
            public Color    backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            public Color    selectionRectangleColor = new Color(0.4f, 1.0f, 1.0f, 0.25f);
            public bool     gridEnabled = true;
            public Color    gridColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            public float    gridSpacing = 200.0f;
            public bool     toolbarEnabled = true;
            public Color    toolbarBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public bool     toolbarEnableSnapToGrid = true;
            public bool     toolbarEnableSnapToNode = true;
            public bool     enableCornerSnap = true;
            public bool     menuAddNode = false;
            public int      panButton = 2;
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
        protected Texture2D                                 snapToGridTexture;
        protected Texture2D                                 snapToNodeTexture;
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
        protected DateTime[]                                timeOfClick = new DateTime[8];
        protected bool[]                                    clickOnNode = new bool[8];
        protected bool                                      isNodeMove = false;
        protected Dictionary<BaseNodeRenderer, Vector2>     originalPosition;
        protected Vector2                                   totalDelta;
        protected Vector2Int                                sortIndices = Vector2Int.zero;
        protected List<string>                              clipboard;
        protected bool                                      runPaste = false;
        protected int                                       pasteCount = 0;
        protected bool                                      snapToGrid = true;
        protected bool                                      snapToNode = true;
        protected GUIStyle                                  toggleButtonStyle;
        protected Texture2D                                 defaultToggleButtonTexture;
        protected Texture2D                                 selectedToggleButtonTexture;
        protected Texture2D                                 defaultHoverToggleButtonTexture;
        protected Texture2D                                 selectedHoverToggleButtonTexture;
        protected bool                                      initSkin = false;
        protected List<Line>                                renderSnapLines = new();
        
        protected bool isPanning
        {
            get { return _isPanning; }
            set { _isPanning = value; if (!_isPanning) lastPan = DateTime.Now; }
        }

        protected static T Init<T>(Theme theme) where T : BaseNodeEditor
        {
            EditorUtils.AddTexturePath("Assets/NodeEditor/UI/", "Packages/com.videojogoslusofona.nodeeditor/UI/");

            T window = (T)GetWindow(typeof(T));
            if (window == null)
            {
                window = (T)GetWindow(typeof(T), false, theme.windowName);
                window.Show();
            }

            window.theme = theme;
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
        protected abstract bool NodeExists(BaseNode node);

        protected bool shouldPan => (theme.panEdge) && ((isRectSelecting) || (isNodeMove));

        void OnGUI()
        {
            if (theme == null)
            {
                GUILayout.Label("Restart window, missing theme...");
                return;
            }

            initSkin &= (selectionTexture != null) && (backgroundTexture != null);

            if (!initSkin)
            {
                backgroundTexture = EditorUtils.GetColorTexture($"{theme.windowName}Background", theme.backgroundColor);
                toolbarTexture = EditorUtils.GetColorTexture($"{theme.windowName}ToolbarBackground", theme.toolbarBackgroundColor);
                selectionTexture = EditorUtils.GetColorTexture($"{theme.windowName}SelectionRectangle", theme.selectionRectangleColor);
                if (theme.toolbarEnableSnapToGrid)
                    snapToGridTexture = EditorUtils.GetTexture("snap_to_grid");
                if (theme.toolbarEnableSnapToNode)
                    snapToNodeTexture = EditorUtils.GetTexture("snap_to_node");
                toggleButtonStyle = new GUIStyle(EditorStyles.toolbarButton);
                defaultToggleButtonTexture = EditorUtils.GetColorTexture("defaultToggleButtonTexture", new Color(0.235f, 0.235f, 0.235f, 1.0f));
                defaultHoverToggleButtonTexture = EditorUtils.GetColorTexture("defaultHoverToggleButtonTexture", new Color(0.27f, 0.27f, 0.27f, 1.0f));
                selectedHoverToggleButtonTexture = EditorUtils.GetColorTexture("selectedHoverToggleButtonTexture", new Color(0.31451f, 0.41647f, 0.52627f, 1.0f));
                selectedToggleButtonTexture = EditorUtils.GetColorTexture("selectedToggleButtonTexture", new Color(0.27451f, 0.37647f, 0.48627f, 1.0f));

                initSkin = true;

                Debug.Log("Skin initialized...");
            }

            if (runPaste)
            {
                // A paste has been schedulled, but it should only be done on OnGUI
                ActualPaste();
                runPaste = false;
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
                        var op = panPosition;
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

                            if (isRectSelecting)
                            {
                                UpdateSelectionRect();
                            }
                            else if (isNodeMove)
                            {
                                totalDelta += op - panPosition;
                                MoveNodes();
                            }

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
            else if (isNodeMove)
            {
                if (!hasFocus)
                {
                    isNodeMove = false; // Reset panning if the window loses focus while panning
                }
                else
                {
                    EditorGUIUtility.AddCursorRect(new Rect(0, 0, position.width, position.height), MouseCursor.MoveArrow);
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
            // Check if nodes should be deleted
            Repaint();
        }

        void OnSelectionChange()
        {
            Repaint();
        }

        void DrawToolbar()
        {
            if (!theme.toolbarEnabled) return;

            GUI.DrawTexture(new Rect(0, 0, position.width, 22.0f), toolbarTexture);

            GUILayout.Space(1);
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
                    GUILayout.Space(2);
                    initSkin = false;
                }

                if (theme.toolbarEnableSnapToGrid)
                {
                    toggleButtonStyle.normal.background = (snapToGrid) ? selectedToggleButtonTexture : defaultToggleButtonTexture;
                    toggleButtonStyle.hover.background = (snapToGrid) ? selectedHoverToggleButtonTexture : defaultHoverToggleButtonTexture;
                    if (GUILayout.Button(new GUIContent(snapToGridTexture, "Snap to Grid"), toggleButtonStyle, GUILayout.Width(24.0f)))
                    {
                        snapToGrid = !snapToGrid;
                        Repaint();
                    }
                    GUILayout.Space(2);
                }
                if (theme.toolbarEnableSnapToNode)
                {
                    toggleButtonStyle.normal.background = (snapToNode) ? selectedToggleButtonTexture : defaultToggleButtonTexture;
                    toggleButtonStyle.hover.background = (snapToNode) ? selectedHoverToggleButtonTexture : defaultHoverToggleButtonTexture;
                    if (GUILayout.Button(new GUIContent(snapToNodeTexture, "Snap to Node"), toggleButtonStyle, GUILayout.Width(24.0f)))
                    {
                        snapToNode = !snapToNode;
                        Repaint();
                    }
                    GUILayout.Space(2);
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

            if ((renderSnapLines != null) && (isNodeMove))
            {
                foreach (var line in renderSnapLines)
                {
                    Handles.color = line.color;
                    Handles.DrawDottedLine(line.p1, line.p2, 2.0f);
                }
            }

            SetupNodes();
            var rendererNodes = renderers.Values.ToList();
            if (rendererNodes.Count > 0)
            {
                rendererNodes.Sort((o1, o2) => o1.sortOrder.CompareTo(o2.sortOrder));
                sortIndices = new Vector2Int(rendererNodes[0].sortOrder, rendererNodes[rendererNodes.Count - 1].sortOrder);

                foreach (var renderer in rendererNodes)
                {
                    renderer.Render(zoomScale);
                    renderer.sortOrder -= sortIndices.x;
                }
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

        BaseNodeRenderer CreateNodeRendererIfNeeded(BaseNode node)
        {
            if ((!renderers.TryGetValue(node, out BaseNodeRenderer renderer)) ||
                (renderer == null))
            {
                var rendererType = Info.GetRendererType(node.GetType());
                if (rendererType == null)
                {
                    Debug.LogWarning($"No renderer for node type {node.GetType()}!");
                    return null;
                }
                renderer = Activator.CreateInstance(rendererType) as BaseNodeRenderer;
                renderer.Init(node);
                renderer.sortOrder = sortIndices.y;
                sortIndices.y++;

                renderers[node] = renderer;
            }
            return renderer;
        }

        void SetupNodes()
        {
            var nodes = GetNodes();
            foreach (var node in nodes)
            {
                if (node == null) continue;

                CreateNodeRendererIfNeeded(node);
            }

            List<BaseNode> toRemove = null;
            foreach (var nodeEntry in renderers)
            {
                if (!NodeExists(nodeEntry.Key))
                {
                    if (toRemove == null) toRemove = new();
                    toRemove.Add(nodeEntry.Key);
                }
            }
            if (toRemove != null)
            {
                foreach (var node in toRemove)
                {
                    renderers.Remove(node);
                }
            }
        }

        protected virtual void ProcessEvents(Event e)
        {
            bool ctrl = (e.control || e.command); // Ctrl on Windows/Linux, Command on macOS

            switch (e.type)
            {
                case EventType.KeyDown:                    
                    if (e.keyCode == KeyCode.Delete) 
                    {
                        Debug.Log($"Delete pressed, focused key = [{GUI.GetNameOfFocusedControl()}], EditorGUIUtility.keyboardControl = {EditorGUIUtility.keyboardControl}");
                        if (selectedNodeCount > 0)
                        {
                            DeleteSelectedNodes();
                            Repaint();
                        }
                    }
                    if ((e.keyCode == KeyCode.C) && (ctrl))
                    {
                        CopyNodes();
                    }
                    if ((e.keyCode == KeyCode.X) && (ctrl))
                    {
                        CutNodes();
                    }
                    if ((e.keyCode == KeyCode.V) && (ctrl))
                    {
                        PasteNodes();
                    }
                    break;
                case EventType.MouseDown:
                    timeOfClick[e.button] = DateTime.Now;
                    clickOnNode[e.button] = (GetNodeAtMouse(false) != null);
                    break;
                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if ((!isPanning) && (!isNodeMove))
                        {
                            // On the title bar of a node, or on empty space?
                            var node = (isRectSelecting) ? (null) : (GetNodeAtMouse(true));
                            if (node != null)
                            {
                                // Start node move
                                isNodeMove = true;
                                // Check if this node is selected, if not, select it
                                if (!node.selected)
                                {
                                    if (!ctrl) ClearNodeSelection();
                                    AddNodeToSelection(node);
                                }

                                totalDelta = Vector2.zero;
                                originalPosition = new();
                                foreach (var selectedNode in nodeSelection)
                                {
                                    originalPosition[selectedNode] = selectedNode.node.position;
                                }
                            }
                            else if (!isRectSelecting)
                            {
                                // We can only start rect select outside of a node
                                if (!clickOnNode[e.button])
                                {
                                    if (!ctrl) { ClearNodeSelection(); initialSelection.Clear(); }
                                    else initialSelection = new(nodeSelection);

                                    // Start selection
                                    selectionRect = new Rect(GetMouseWorldPosition(), Vector2.zero);
                                    isRectSelecting = true;
                                }
                            }
                            else
                            {
                                UpdateSelectionRect();
                            }
                        }
                        if (isNodeMove)
                        {
                            // Move nodes
                            totalDelta += e.delta / zoomScale;
                            MoveNodes();
                            Repaint();
                        }
                    }
                    else if ((e.button == theme.panButton) && (!isRectSelecting))
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
                    float elapsedTime = 0.001f * (float)(DateTime.Now - timeOfClick[e.button]).TotalMilliseconds; 
                    if (e.button == 0)
                    {
                        if (isRectSelecting)
                        {
                            isRectSelecting = false;
                            Repaint();
                        }
                        else if (isNodeMove)
                        {
                            isNodeMove = false;
                            Repaint();
                        }
                        else if (!isPanning)
                        {
                            if (elapsedTime < 0.4f)
                            {
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
                        }
                    }
                    else if ((e.button == theme.panButton)  && (isPanning)) // End panning on mouse up
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
                        if ((!isPanning) && (!isRectSelecting) && ((DateTime.Now - lastPan).TotalMilliseconds > 100.0f))
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

        protected BaseNodeRenderer GetNodeAtMouse(bool forDragMove = false)
        {
            BaseNodeRenderer    ret = null;
            var                 pos = GetMouseWorldPosition();

            foreach (var nodeRenderer in renderers)
            {
                var node = nodeRenderer.Value;

                if (((forDragMove) && (node.IsHoveringForMove(pos)))  ||
                    ((!forDragMove) && (node.IsHovering(pos))))
                {
                    if (ret == null) ret = node;
                    else if (ret.sortOrder < node.sortOrder) ret = node;
                }
            }

            return ret;
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

            menu.AddSeparator("");

            if (selectedNodeCount > 0)
            {
                menu.AddItem(new GUIContent("Copy"), false, () => CopyNodes());
                menu.AddItem(new GUIContent("Cut"), false, () => CutNodes());
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Copy"), false);
                menu.AddDisabledItem(new GUIContent("Cut"), false);
            }
            if (clipboard != null)
            {
                menu.AddItem(new GUIContent("Paste"), false, () => PasteNodes());
            }
            else
            {
                menu.AddDisabledItem(new GUIContent("Paste"), false);
            }

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
                node.sortOrder = sortIndices.y + 1;
                sortIndices.y++;
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

        protected void MoveNodes()
        {
            var uniqueScripts = nodeSelection
                .Where(renderer => renderer != null && renderer.node != null && renderer.node.owner != null) // Filter out nulls
                .Select(renderer => renderer.node.owner)  // Select the owner from each node
                .Distinct()  
                .ToArray();

            if (renderSnapLines != null) renderSnapLines.Clear();
            else renderSnapLines = new();

            Undo.RecordObjects(uniqueScripts, "Move Nodes");
            foreach (var node in nodeSelection)
            {
                // Check if we have the original position
                node.node.position = Snap(node, originalPosition[node] + totalDelta, node.GetRect(), ref renderSnapLines);
            }
            foreach (var scripts in uniqueScripts)
            {
                EditorUtility.SetDirty(scripts);
            }
        }

        protected Vector2 Snap(BaseNodeRenderer excludeNodeRenderer, Vector2 sourcePosition, Rect rect, ref List<Line> lines)
        {
            Vector2 ret = Vector2.zero;
            ret.x = SnapHorizontal(excludeNodeRenderer, sourcePosition.x, sourcePosition.y, rect.width, ref lines);
            ret.y = SnapVertical(excludeNodeRenderer, sourcePosition.y, sourcePosition.x, rect.height, ref lines);

            return ret;
        }

        protected float SnapHorizontal(BaseNodeRenderer excludeNodeRenderer, float x, float baseY, float width, ref List<Line> lines)
        {
            if ((!theme.toolbarEnableSnapToGrid) || (!theme.gridEnabled)) return x;

            float       snapPos = x;
            float       snapPosDist = float.MaxValue;
            List<Line>  snapLines = null;
            float       tolerance = 20.0f / zoomScale;

            void SnapHorizontal(float testX, float offsetX, float candidateX, float y1, float y2, Color lineColor)
            {
                float testPos = testX + offsetX;
                float d = Mathf.Abs(testPos - candidateX);
                if ((d < tolerance) && (d < snapPosDist))
                {
                    snapPosDist = d;
                    snapPos = candidateX - offsetX;
                    snapLines = new List<Line>
                        {
                            new Line()
                            {
                                p1 = new Vector2(candidateX, y1),
                                p2 = new Vector2(candidateX, y2),
                                color = lineColor
                            }
                        };
                }
            }

            for (int i = 0; i < 2; i++)
            {
                float offset = 0;
                if (i == 1) offset = width;

                if (snapToGrid)
                {
                    // Check snap with grid lines
                    float closestGridLine = Mathf.Round((x + offset) / theme.gridSpacing) * theme.gridSpacing;

                    SnapHorizontal(x, offset, closestGridLine, baseY - 1000.0f, baseY + 1000.0f, Color.yellow);
                }
                if (snapToNode)
                {
                    // Check snap with nodes
                    foreach (var renderNode in renderers.Values)
                    {
                        if (renderNode == excludeNodeRenderer) continue;

                        var rect = renderNode.GetRect();

                        SnapHorizontal(x, offset, rect.xMin, baseY, rect.yMin, Color.red);
                        if (theme.enableCornerSnap)
                        {
                            SnapHorizontal(x, offset, rect.xMax, baseY, rect.yMin, Color.red);
                        }
                    }
                }

                if (!theme.enableCornerSnap) break;
            }

            if (snapLines != null)
            {
                lines.AddRange(snapLines);
            }
            return snapPos;
        }

        protected float SnapVertical(BaseNodeRenderer excludeNodeRenderer, float y, float baseX, float height, ref List<Line> lines)
        {
            if ((!theme.toolbarEnableSnapToGrid) || (!theme.gridEnabled)) return y;

            float       snapPos = y;
            float       snapPosDist = float.MaxValue;
            List<Line>  snapLines = null;
            float       tolerance = 20.0f / zoomScale;

            void SnapVertical(float testY, float offsetY, float candidateY, float x1, float x2, Color lineColor)
            {
                float testPos = testY + offsetY;
                float d = Mathf.Abs(testPos - candidateY);
                if ((d < tolerance) && (d < snapPosDist))
                {
                    snapPosDist = d;
                    snapPos = candidateY - offsetY;
                    snapLines = new List<Line>
                        {
                            new Line()
                            {
                                p1 = new Vector2(x1, candidateY),
                                p2 = new Vector2(x2, candidateY),
                                color = lineColor
                            }
                        };
                }
            }

            for (int i = 0; i < 2; i++)
            {
                float offset = 0;
                if (i == 1) offset = height;

                if (snapToGrid)
                {
                    // Check snap with grid lines
                    float closestGridLine = Mathf.Round((y + offset) / theme.gridSpacing) * theme.gridSpacing;

                    SnapVertical(y, offset, closestGridLine, baseX - 1000.0f, baseX + 1000.0f, Color.yellow);
                }
                if (snapToNode)
                {
                    // Check snap with nodes
                    foreach (var renderNode in renderers.Values)
                    {
                        if (renderNode == excludeNodeRenderer) continue;

                        var rect = renderNode.GetRect();

                        SnapVertical(y, offset, rect.yMin, baseX, rect.xMin, Color.red);
                        if (theme.enableCornerSnap)
                        {
                            SnapVertical(y, offset, rect.yMax, baseX, rect.xMin, Color.red);
                        }
                    }
                }

                if (!theme.enableCornerSnap) break;
            }

            if (snapLines != null)
            {
                lines.AddRange(snapLines);
            }
            return snapPos;
        }

        protected void CopyNodes()
        {
            clipboard = null;            
            foreach (var nodeRenderer in nodeSelection)
            {
                if (nodeRenderer != null)
                {
                    var node = nodeRenderer.node;
                    var str = JsonUtility.ToJson(node);
                    var type = node.GetType().ToString();
                    var nodeString = $"{type}|{str}";
                    if (!string.IsNullOrEmpty(nodeString))
                    {
                        if (clipboard == null) clipboard = new();
                        clipboard.Add(nodeString);
                    }
                }
            }
            pasteCount = 1;
        }
        protected void CutNodes()
        {
            CopyNodes();
            DeleteSelectedNodes();
        }

        protected void PasteNodes()
        {
            if (clipboard == null) return;

            runPaste = true;
            Repaint();
        }

        protected void ActualPaste()
        { 
            ClearNodeSelection();

            foreach (var elem in clipboard)
            {
                // Split name
                int index = elem.IndexOf('|');
                string typeString = elem.Substring(0, index);
                string json = elem.Substring(index + 1);

                Type nodeType = Info.GetNodeType(typeString);
                if (nodeType == null)
                {
                    Debug.LogError($"Unknown type {typeString}!");
                    continue;
                }
                BaseNode newNode = (BaseNode)JsonUtility.FromJson(json, nodeType);
                // Change owner (we might be on another object)
                newNode.owner = GetSelection();
                // Assume we are managing nodes in some way in your editor window
                // This is where you would add the newNode to your system
                OnNodeCreate(newNode, newNode.position + new Vector2(20.0f * pasteCount, 20.0f * pasteCount));

                var renderer = CreateNodeRendererIfNeeded(newNode);
                if (renderer != null)
                {
                    AddNodeToSelection(renderer);
                }
            }

            pasteCount++;
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
