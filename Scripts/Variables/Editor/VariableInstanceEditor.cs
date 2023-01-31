using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using System.Linq;
using static UnityEditor.MaterialProperty;

[CustomEditor(typeof(VariableInstance))]
public class VariableInstanceEditor : Editor
{
    SerializedProperty propShowInfo;
    SerializedProperty propDescription;
    SerializedProperty propType;
    SerializedProperty propCurrentValue;
    SerializedProperty propDefaultValue;
    SerializedProperty propHasLimits;
    SerializedProperty propMinValue;
    SerializedProperty propMaxValue;

    protected virtual void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
        propDescription = serializedObject.FindProperty("_description");
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

    protected virtual bool WriteTitle()
    {
        VariableInstance varInstance = target as VariableInstance;
        if (varInstance == null) { return true; }

        GUIStyle styleTitle = GUIUtils.GetActionTitleStyle();
        GUIStyle explanationStyle = GUIUtils.GetActionExplanationStyle();

        var backgroundColor = GUIUtils.ColorFromHex("#fffaa7");
        var textColor = GUIUtils.ColorFromHex("#2f4858");
        var separatorColor = GUIUtils.ColorFromHex("#ffdf6e");

        string title = $"{varInstance.name} = {varInstance.GetValueString()}";

        // Compute explanation text height
        string explanation = propDescription.stringValue;
        int explanationLines = explanation.Count((c) => c == '\n') + 1;
        int explanationTextHeight = explanationLines * explanationStyle.fontSize + 6;

        var varTexture = GUIUtils.GetTexture("VariableTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("VariableTexture", new CodeBitmaps.Variable());
        }

        // Background and title
        float inspectorWidth = EditorGUIUtility.currentViewWidth - 20;
        Rect titleRect = EditorGUILayout.BeginVertical("box");
        Rect rect = new Rect(titleRect.x, titleRect.y, inspectorWidth - titleRect.x, styleTitle.fontSize + 16);
        Rect fullRect = rect;
        if (explanation != "")
        {
            fullRect.height = rect.height + 8 + explanationTextHeight;
        }
        EditorGUI.DrawRect(fullRect, backgroundColor);
        var prevColor = styleTitle.normal.textColor;
        styleTitle.normal.textColor = textColor;
        GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), varTexture, ScaleMode.ScaleToFit, true, 1.0f);
        EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 8, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), title, styleTitle);
        styleTitle.normal.textColor = prevColor;
        EditorGUILayout.EndVertical();
        EditorGUILayout.Space(fullRect.height);

        if (explanation != "")
        {
            // Separator
            Rect separatorRect = new Rect(titleRect.x + 4, titleRect.y + rect.height, inspectorWidth - 20 - 8, 4);
            EditorGUI.DrawRect(separatorRect, separatorColor);

            // Explanation
            EditorGUI.LabelField(new Rect(titleRect.x + 10, separatorRect.y + separatorRect.height + 4 , inspectorWidth - 20 - titleRect.x - 4, explanationTextHeight), explanation, explanationStyle);
        }


        bool toggle = false;
        if (propShowInfo.boolValue)
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose", BuildEyeClose));
        }
        else
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 28, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen", BuildEyeOpen));
        }
        if (toggle)
        {
            propShowInfo.boolValue = !propShowInfo.boolValue;

            Event e = Event.current;
            if (e.shift)
            {
                // Affect all the Actions in this object
                var variables = varInstance.GetComponents<VariableInstance>();
                foreach (var v in variables)
                {
                    v.showInfo = propShowInfo.boolValue;
                }
            }

            serializedObject.ApplyModifiedProperties();
        }

        return propShowInfo.boolValue;
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

        if (EditorGUI.EndChangeCheck())
        {
            serializedObject.ApplyModifiedProperties();
        }

        // Draw old editor, need it for now
        if (useOriginalEditor)
        {
            base.OnInspectorGUI();
        }

    }

    private void BuildEyeOpen(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeOpen());
    }

    private void BuildEyeClose(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeClose());
    }

    private void BuildTitleButton(string name, GUIBitmap bitmap)
    {
        Color iconColor = GUIUtils.ColorFromHex("#2f4858");
        Color borderColor = GUIUtils.ColorFromHex("#2f4858");
        Color normalBackColor = GUIUtils.ColorFromHex("#a8b591");
        Color hoverBackColor = GUIUtils.ColorFromHex("#cbdbaf");

        var bitmap_normal = new GUIBitmap(bitmap);
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_normal.FillAlpha(normalBackColor);
        GUIUtils.BitmapToTexture($"{name}:normal", bitmap_normal);

        var bitmap_highlight = new GUIBitmap(bitmap);
        bitmap_normal.Multiply(iconColor);
        bitmap_normal.Border(borderColor);
        bitmap_highlight.FillAlpha(hoverBackColor);
        GUIUtils.BitmapToTexture($"{name}:hover", bitmap_highlight);
    }

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
