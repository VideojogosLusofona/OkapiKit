using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(ActionSequence))]
    public class ActionSequenceEditor : ActionEditor
    {
        SerializedProperty propActions;

        protected override void OnEnable()
        {
            base.OnEnable();

            propActions = serializedObject.FindProperty("actions");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);

                var action = (target as ActionSequence);
                if (action == null) return;

                var actionsRect = GUILayoutUtility.GetLastRect();
                actionsRect = new Rect(actionsRect.xMin, actionsRect.yMax, actionsRect.width, 20.0f);

                TryDragActionToActionDelayList(actionsRect, propActions);

                EditorGUI.BeginChangeCheck();
                EditorGUILayout.PropertyField(propActions, new GUIContent("Action Sequence"));

                EditorGUI.EndChangeCheck();

                serializedObject.ApplyModifiedProperties();
                (target as Action).UpdateExplanation();
            }
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
    }
}