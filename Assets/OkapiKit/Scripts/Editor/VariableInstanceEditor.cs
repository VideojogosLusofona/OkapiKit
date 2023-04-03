using UnityEngine;
using UnityEditor;
using System.Linq;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(VariableInstance))]
    public class VariableInstanceEditor : OkapiBaseEditor
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

            propType = serializedObject.FindProperty("type");
            propCurrentValue = serializedObject.FindProperty("currentValue");
            propDefaultValue = serializedObject.FindProperty("defaultValue");
            propHasLimits = serializedObject.FindProperty("hasLimits");
            propMinValue = serializedObject.FindProperty("minValue");
            propMaxValue = serializedObject.FindProperty("maxValue");
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

            EditorGUILayout.PropertyField(propType, new GUIContent("Type"), true);
            EditorGUILayout.PropertyField(propCurrentValue, new GUIContent("Current Value"), true);
            EditorGUILayout.PropertyField(propDefaultValue, new GUIContent("Default Value"), true);
            EditorGUILayout.PropertyField(propHasLimits, new GUIContent("Has Limits"), true);
            if (propHasLimits.boolValue)
            {
                EditorGUILayout.PropertyField(propMinValue, new GUIContent("Minimum Value"), true);
                EditorGUILayout.PropertyField(propMaxValue, new GUIContent("Maximum Value"), true);
            }
            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"), true);

            EditorGUI.EndChangeCheck();

            serializedObject.ApplyModifiedProperties();
            (target as OkapiElement).UpdateExplanation();

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
            VariableInstance varInstance = target as VariableInstance;

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