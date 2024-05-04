using System;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace NodeEditor
{
    public abstract class BaseNodeEditor : EditorWindow
    {
        public class Theme
        {
            public Color backgroundColor = new Color(0.2f, 0.2f, 0.2f, 1.0f);
            public bool gridEnabled = true;
            public Color gridColor = new Color(0.3f, 0.3f, 0.3f, 1.0f);
            public float gridSpacing = 100.0f;
            public bool toolbarEnabled = true;
            public Color toolbarBackgroundColor = new Color(0.1f, 0.1f, 0.1f, 1.0f);
            public bool toolbarAddNode = false;
            public string windowName = "NodeEditor";
        }

        protected Theme         theme;
        protected Vector2       gridPanPosition = Vector2.zero;
        protected float         zoomScale = 1.0f;
        protected Vector2       zoomLimits = new Vector2(0.2f, 3.0f);   // Minimum zoom scale
        protected Texture2D     backgroundTexture;
        protected Texture2D     toolbarTexture;
        protected string        disableReason = "";
        protected Rect          worldSpaceExtents;

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

            return window;
        }

        protected abstract void SetActiveSelection();
        protected virtual bool hasSelection => false;
        protected virtual string[] GetSubSelectorOptions() => null;
        protected virtual int GetSubSelectorSelected() => -1;
        protected virtual void SetSubSelector(string str) { }
        protected abstract void AddNode();
        protected abstract void OnNodeCreate(object newNode);

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
                int selected = 0;
                selected = EditorGUILayout.Popup("", original, subSelectorOptions, EditorStyles.toolbarDropDown, GUILayout.Width(maxWidth));
                if (selected != original)
                {
                    SetSubSelector(subSelectorOptions[selected]);
                    Repaint();
                }
            }
            if (hasSelection)
            {
                if (theme.toolbarAddNode)
                {
                    if (GUILayout.Button(new GUIContent("+", "Add node"), EditorStyles.toolbarButton, GUILayout.Width(21.0f)))
                    {
                        AddNode();
                    }
                }
                var resetGUIContent = new GUIContent("Reset", "Reset View");
                if (GUILayout.Button(resetGUIContent, EditorStyles.toolbarButton, GUILayout.Width(EditorStyles.toolbarButton.CalcSize(resetGUIContent).x)))
                {
                    gridPanPosition = Vector2.zero;
                    zoomScale = 1.0f;
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

            // Prepare the transformation matrix
            Matrix4x4 translation = Matrix4x4.TRS(gridPanPosition, Quaternion.identity, Vector3.one);
            Matrix4x4 scale = Matrix4x4.Scale(Vector3.one * zoomScale);
            Matrix4x4 pivot = Matrix4x4.TRS(new Vector3(position.width / 2, position.height / 2 - posY, 0), Quaternion.identity, Vector3.one);

            // Apply the combined transformation matrix
            GUI.matrix = pivot * scale * translation * pivot.inverse;

            ComputeWorldSpaceExtents();

            if (theme.gridEnabled)
            {
                DrawGrid();
            }

            // Restore the original GUI matrix and end the group
            GUI.matrix = Matrix4x4.identity;
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

        protected virtual void ProcessEvents(Event e)
        {
            switch (e.type)
            {
                case EventType.MouseDrag:
                    if (e.button == 1)
                    {
                        gridPanPosition += e.delta / zoomScale;
                        Repaint();
                    }
                    break;
                case EventType.ScrollWheel:
                    HandleZoom(e);
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
            var invMatrix = GUI.matrix.inverse;

            var c1 = invMatrix * new Vector4(0, 0, 0, 1);
            var c2 = invMatrix * new Vector4(position.width, position.height, 0, 1);

            worldSpaceExtents = new Rect(c1.x, c1.y, c2.x - c1.x, c2.y - c1.y);
        }
    }

    public abstract class NodeEditor<T> : BaseNodeEditor where T : class
    {
        protected NodeType[] cachedNodeTypes = null;

        protected override void AddNode()
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

            AddNodePopup.Init(cachedNodeTypes, CreateNodeOfType);
        }

        private void CreateNodeOfType(Type nodeType)
        {
            var newNode = Activator.CreateInstance(nodeType);
            // Assume we are managing nodes in some way in your editor window
            // This is where you would add the newNode to your system
            OnNodeCreate(newNode);
        }

        private string GetNodeName(Type type)
        {
            // Attempt to get the NodePath attribute from the type
            var nodePathAttr = type.GetCustomAttribute<NodePathAttribute>();
            if (nodePathAttr != null)
            {
                return nodePathAttr.Path; // Use the path as the name if attribute exists
            }
            else
            {
                return EditorUtils.ToReadableName(type.Name); // Default to the type's name if no attribute is found
            }
        }
    }
}
