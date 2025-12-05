using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Action))]
    public class ActionEditor : OkapiBaseEditor
    {
        SerializedProperty propEnableAction;
        SerializedProperty propHasTags;
        SerializedProperty propHasConditions;
        SerializedProperty propTags;
        SerializedProperty propConditions;

        protected override void OnEnable()
        {
            base.OnEnable();
            propEnableAction = serializedObject.FindProperty("enableAction");
            propHasTags = serializedObject.FindProperty("hasTags");
            propHasConditions = serializedObject.FindProperty("hasConditions");
            propTags = serializedObject.FindProperty("actionTags");
            propConditions = serializedObject.FindProperty("actionConditions");
        }


        protected override bool WriteTitle()
        {
            bool ret = base.WriteTitle();

            var action = target as Action;
            if (action != null)
            {
                var width = EditorGUIUtility.currentViewWidth;

                if ((action.isTagged) && (width > 400.0f))
                {
                    var tags = action.GetActionTags();
                    var tagTexture = GUIUtils.GetTexture("Tag");
                    var x = titleRect.x + width - 48 - 200.0f;
                    GUIStyle explanationStyle = GetExplanationStyle();

                    float y = titleRect.y = titleRect.y + 5;
                    foreach (var t in tags)
                    {
                        if (t == null) continue;

                        GUI.DrawTexture(new Rect(x, y, 16, 16), tagTexture, ScaleMode.ScaleToFit, true, 1.0f);
                        EditorGUI.LabelField(new Rect(x + 18, y, width, 20.0f), t.name, explanationStyle);

                        y += 18;
                    }
                }
            }

            return ret;
        }

        public override void OnInspectorGUI()
        {
            if (WriteTitle())
            {
                StdEditor();
            }
        }

        public virtual void OnSceneGUI()
        {
            if (OkapiConfig.showConditions)
            {
                // Make sure to update the serializedObject to reflect the latest data
                serializedObject.Update();

                if (propHasConditions == null) return;
                if (!propHasConditions.boolValue) return;

                // Iterate through all elements in the propConditions array
                for (int i = 0; i < propConditions.arraySize; i++)
                {
                    SerializedProperty conditionElement = propConditions.GetArrayElementAtIndex(i);

                    // Render the property we want 
                    ConditionDrawer.OnSceneGUI(serializedObject, conditionElement);
                }
            }
        }


        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Action");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors()
        {
            if (propEnableAction.boolValue)
            {
                return (GUIUtils.ColorFromHex("#D7E8BA"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#86CB92"));
            }
            else
            {
                return (GUIUtils.ColorFromHex("#94ad69"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#3e894b"));
            }
        }

        protected void StdEditor(bool useOriginalEditor = true)
        {
            EditorGUI.BeginChangeCheck();

            Rect rect = EditorGUILayout.BeginHorizontal();
            rect.height = 20;
            float totalWidth = rect.width;
            float elemWidth = totalWidth / 3;
            propEnableAction.boolValue = CheckBox("Active", "If active, this action can be triggered. If disabled, this action can never be triggered.", rect.x, rect.y, elemWidth, propEnableAction.boolValue);
            propHasTags.boolValue = CheckBox("Tags", "If active, you can tag this action so that it can be triggered by an unlinked 'Action Tagged' action.", rect.x + elemWidth, rect.y, elemWidth, propHasTags.boolValue);
            propHasConditions.boolValue = CheckBox("Conditions", "If active, you can specify conditions. Only if these conditions are all true will this action trigger, even if explicitly told to.", rect.x + elemWidth * 2, rect.y, elemWidth, propHasConditions.boolValue);
            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(rect.height);

            if (propHasTags.boolValue)
            {
                // Display tags
                EditorGUILayout.PropertyField(propTags, new GUIContent("Tags", "These define the tags for this action itself, so it can be triggered by an 'Action Tagged' action."), true);
            }

            if (propHasConditions.boolValue)
            {
                // Display tags
                EditorGUILayout.PropertyField(propConditions, new GUIContent("Conditions", "These define the conditions that have to be true for this action to be able to be triggered."), true);
            }

            serializedObject.ApplyModifiedProperties();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

            if (EditorGUI.EndChangeCheck())
            {
                (target as OkapiElement).UpdateExplanation();
            }

        }

        protected bool CheckBox(string label, string tooltip, float x, float y, float width, bool initialValue)
        {
            GUIStyle style = GUI.skin.toggle;
            Vector2 size = style.CalcSize(new GUIContent(label, tooltip));

            EditorGUI.LabelField(new Rect(x, y, size.x, 20), new GUIContent(label, tooltip));
            float offsetX = size.x + 1;

            if (offsetX + 20 > width) offsetX = width - 20;

            bool ret = EditorGUI.Toggle(new Rect(x + offsetX, y, 20, 20), new GUIContent("", tooltip), initialValue);

            return ret;
        }

        protected override GUIStyle GetTitleSyle()
        {
            return GUIUtils.GetActionTitleStyle();
        }

        protected override GUIStyle GetExplanationStyle()
        {
            return GUIUtils.GetActionExplanationStyle();
        }

        protected override string GetTitle()
        {
            return (target as Action).GetActionTitle();
        }
    }
}