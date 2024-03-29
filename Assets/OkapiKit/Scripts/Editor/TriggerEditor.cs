using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Trigger))]
    public class TriggerEditor : OkapiBaseEditor
    {
        protected SerializedProperty propEnableTrigger;
        protected SerializedProperty propAllowRetrigger;
        protected SerializedProperty propHasConditions;
        protected SerializedProperty propPreConditions;
        protected SerializedProperty propActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            propEnableTrigger = serializedObject.FindProperty("enableTrigger");
            propAllowRetrigger = serializedObject.FindProperty("allowRetrigger");
            propHasConditions = serializedObject.FindProperty("hasPreconditions");
            propPreConditions = serializedObject.FindProperty("preConditions");
            propActions = serializedObject.FindProperty("actions");
        }

        public override void OnInspectorGUI()
        {
            if (WriteTitle())
            {
                StdEditor();
            }
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Trigger");

            return varTexture;
        }

        protected void StdEditor(bool useOriginalEditor = true, bool allowConditions = true)
        {
            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.height = 20;
            float totalWidth = rect.width;
            float elemWidth = totalWidth / 3;
            propEnableTrigger.boolValue = CheckBox("Active", "If active, this trigger can be triggered. If not, this trigger is disabled.", rect.x, rect.y, elemWidth, propEnableTrigger.boolValue);
            propAllowRetrigger.boolValue = CheckBox("Allow Retrigger", "If true, this trigger can retrigger, otherwise it disables itself after the first trigger", rect.x + elemWidth, rect.y, elemWidth, propAllowRetrigger.boolValue);
            if (allowConditions)
            {
                propHasConditions.boolValue = CheckBox("Conditions", "If active, this trigger can only be triggered if some conditions are met", rect.x + elemWidth * 2, rect.y, elemWidth, propHasConditions.boolValue);
            }
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(rect.height);
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));

            if (propHasConditions.boolValue)
            {
                // Display tags
                EditorGUILayout.PropertyField(propPreConditions, new GUIContent("Conditions", "These conditions have to be met for this trigger to be active."), true);
            }

            serializedObject.ApplyModifiedProperties();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

        }

        public virtual void OnSceneGUI()
        {
            if (OkapiConfig.showConditions)
            {
                // Make sure to update the serializedObject to reflect the latest data
                serializedObject.Update();

                // Iterate through all elements in the propConditions array
                for (int i = 0; i < propPreConditions.arraySize; i++)
                {
                    SerializedProperty conditionElement = propPreConditions.GetArrayElementAtIndex(i);

                    // Render the property we want 
                    ConditionDrawer.OnSceneGUI(serializedObject, conditionElement);
                }
            }
        }


        protected void ActionPanel(string title = "Actions")
        {
            serializedObject.ApplyModifiedProperties();
            serializedObject.Update();

            var actionsRect = GUILayoutUtility.GetLastRect();
            actionsRect = new Rect(actionsRect.xMin, actionsRect.yMax, actionsRect.width, 20.0f);

            TryDragActionToActionDelayList(actionsRect, propActions);

            EditorGUILayout.PropertyField(propActions, new GUIContent(title, "These actions will be performed when this trigger is executed."), true);

            serializedObject.ApplyModifiedProperties();
            (target as Trigger).UpdateExplanation();
        }

        protected void TryDragActionToActionDelayList(Rect rect, SerializedProperty actionList)
        {
            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform) &&
                (rect.Contains(evt.mousePosition)))
            {
                bool checkIfAction = true;
                foreach (Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is not Action)
                    {
                        checkIfAction = false;
                        break;
                    }
                }

                if (checkIfAction)
                {
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        // Get max delay
                        float d = 0.0f;
                        for (int i = 0; i < actionList.arraySize; i++)
                        {
                            var elem = actionList.GetArrayElementAtIndex(i);
                            var propDelay = elem.FindPropertyRelative("delay");
                            if (propDelay != null)
                            {
                                if (d < propDelay.floatValue) d = propDelay.floatValue;
                            }
                        }

                        foreach (Object obj in DragAndDrop.objectReferences)
                        {
                            if (obj is Action)
                            {
                                // Add element to the array
                                actionList.arraySize++;
                                var newElement = actionList.GetArrayElementAtIndex(actionList.arraySize - 1);
                                if (newElement != null)
                                {
                                    var propDelay = newElement.FindPropertyRelative("delay");
                                    var propAction = newElement.FindPropertyRelative("action");
                                    if (propDelay != null) propDelay.floatValue = d;
                                    if (propAction != null) propAction.objectReferenceValue = obj as Action;
                                }
                            }
                        }
                    }

                    evt.Use();
                }
            }
        }

        private bool CheckBox(string label, string tooltip, float x, float y, float width, bool initialValue)
        {
            GUIStyle style = GUI.skin.toggle;
            Vector2 size = style.CalcSize(new GUIContent(label));

            EditorGUI.LabelField(new Rect(x, y, size.x, 20), new GUIContent(label, tooltip));
            float offsetX = size.x + 1;

            if (offsetX + 20 > width) offsetX = width - 20;

            bool ret = EditorGUI.Toggle(new Rect(x + offsetX, y, 20, 20), initialValue);

            return ret;
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetTriggerTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetTriggerExplanationStyle();
        }

        protected override string GetTitle()
        {
            return (target as Trigger).GetTriggerTitle();
        }

        protected override (Color, Color, Color) GetColors()
        {
            if (propEnableTrigger == null) OnEnable();

            if (propEnableTrigger.boolValue)
            {
                return (GUIUtils.ColorFromHex("#D0FFFF"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#86CBFF"));
            }
            else
            {
                return (GUIUtils.ColorFromHex("#80c5c5"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#4e7694"));
            }
        }
    }
}