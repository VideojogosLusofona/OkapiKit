using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Variable))]
    public class VariableEditor : OkapiBaseEditor
    {
        SerializedProperty propType;
        SerializedProperty propCurrentValue;
        SerializedProperty propDefaultValue;
        SerializedProperty propHasLimits;
        SerializedProperty propMinValue;
        SerializedProperty propMaxValue;

        protected override void OnEnable()
        {
            base.OnEnable();

            propType = serializedObject.FindProperty("_type");
            propCurrentValue = serializedObject.FindProperty("_currentValue");
            propDefaultValue = serializedObject.FindProperty("_defaultValue");
            propHasLimits = serializedObject.FindProperty("_hasLimits");
            propMinValue = serializedObject.FindProperty("_minValue");
            propMaxValue = serializedObject.FindProperty("_maxValue");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                StdEditor(false);
            }
        }

        protected void StdEditor(bool useOriginalEditor = true)
        {
            EditorGUI.BeginChangeCheck();

            EditorGUILayout.PropertyField(propType, new GUIContent("Type", "Type of variable."), true);
            EditorGUILayout.PropertyField(propCurrentValue, new GUIContent("Current Value", "Current value of this variable."), true);
            EditorGUILayout.PropertyField(propDefaultValue, new GUIContent("Default Value", "Default value of this variable.\nWhen variable is reset, application starts or object is spawned, it will be reset to this value."), true);
            EditorGUILayout.PropertyField(propHasLimits, new GUIContent("Has Limits", "If we want this value to have limits.\nTo use ValueDisplayProgress, numbers have to have limits.\nNumbers with limits can never be smaller or larger than its limits."), true);
            if (propHasLimits.boolValue)
            {
                EditorGUILayout.PropertyField(propMinValue, new GUIContent("Minimum Value", "Minimum value"), true);
                EditorGUILayout.PropertyField(propMaxValue, new GUIContent("Maximum Value", "Maximum value"), true);
            }
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiScriptableObject).UpdateExplanation();

            // Draw old editor, need it for now
            if (useOriginalEditor)
            {
                base.OnInspectorGUI();
            }

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
            Variable varInstance = target as Variable;

            return $"{varInstance.name} = {varInstance.GetValueString()}";
        }

        protected override Texture2D GetIcon()
        {
            var varTexture = GUIUtils.GetTexture("Variable");

            return varTexture;
        }

        protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#fffaa7"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ffdf6e"));



        /*[MenuItem("Assets/Okapi/Tool: Upgrade Variables")]
        static void UpgradeVariables()
        {
            var vars = FindObjectsOfType<VariableInstance>(true);
            foreach (var v in vars)
            {
                if (v.isInteger)
                {
                    v.type = Variable.Type.Integer;
                }
                else
                {
                    v.type = Variable.Type.Float;
                }
            }

            var allAssets = AssetDatabase.GetAllAssetPaths();
            foreach (var asset in allAssets)
            {
                var obj = AssetDatabase.LoadAssetAtPath<GameObject>(asset);
                if (obj)
                {
                    var comps = obj.GetComponents<VariableInstance>();
                    foreach (var v in comps)
                    {
                        Debug.Log($"Upgrading prefab {asset}:{v.name}...");
                        if (v.isInteger)
                        {
                            v.type = Variable.Type.Integer;
                            EditorUtility.SetDirty(obj);
                        }
                        else
                        {
                            v.type = Variable.Type.Float;
                            EditorUtility.SetDirty(obj);
                        }
                    }
                    comps = obj.GetComponentsInChildren<VariableInstance>(true);
                    foreach (var v in comps)
                    {
                        Debug.Log($"Upgrading prefab {asset}:{v.name}...");
                        if (v.isInteger)
                        {
                            v.type = Variable.Type.Integer;
                            EditorUtility.SetDirty(obj);
                        }
                        else
                        {
                            v.type = Variable.Type.Float;
                            EditorUtility.SetDirty(obj);
                        }
                    }
                }
                var scr = AssetDatabase.LoadAssetAtPath<Variable>(asset);
                if (scr)
                {
                    Debug.Log($"Upgrading scriptable object {asset}...");
                    if (scr.isInteger)
                    {
                        scr.type = Variable.Type.Integer;
                        EditorUtility.SetDirty(scr);
                    }
                }
            }
            AssetDatabase.SaveAssets();
        }//*/

    }
}