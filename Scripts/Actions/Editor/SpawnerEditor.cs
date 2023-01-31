using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using static UnityEngine.Rendering.DebugUI.MessageBox;
using System.Linq;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : Editor
{
    SerializedProperty propShowInfo;
    SerializedProperty propExplanation;
    SerializedProperty propDescription;
    SerializedProperty propForceCount;
    SerializedProperty propNumberOfEntities;
    SerializedProperty propUsePulsePattern;
    SerializedProperty propPulsePattern;
    SerializedProperty propPulseTime;
    SerializedProperty propPrefabs;
    SerializedProperty propSpawnPoints;
    SerializedProperty propSpawnPointType;
    SerializedProperty propModifiers;
    SerializedProperty propScaleVariance;
    SerializedProperty propSpeedVariance;
    SerializedProperty propSetParent;

    protected virtual void OnEnable()
    {
        propShowInfo = serializedObject.FindProperty("_showInfo");
        propExplanation = serializedObject.FindProperty("_explanation");

        propDescription = serializedObject.FindProperty("description");
        propForceCount = serializedObject.FindProperty("forceCount");
        propNumberOfEntities = serializedObject.FindProperty("numberOfEntities");
        propUsePulsePattern = serializedObject.FindProperty("usePulsePattern");
        propPulsePattern = serializedObject.FindProperty("pulsePattern");
        propPulseTime = serializedObject.FindProperty("pulseTime");
        propPrefabs = serializedObject.FindProperty("prefabs");
        propSpawnPoints = serializedObject.FindProperty("spawnPoints");
        propSpawnPointType = serializedObject.FindProperty("spawnPointType");
        propModifiers = serializedObject.FindProperty("modifiers");
        propScaleVariance = serializedObject.FindProperty("scaleVariance");
        propSpeedVariance = serializedObject.FindProperty("speedVariance");
        propSetParent = serializedObject.FindProperty("setParent");
    }

    public override void OnInspectorGUI()
    {
        if (WriteTitle())
        {
            StdEditor(false);
        }
    }

    public virtual Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("SpawnerTexture");
        if (varTexture == null)
        {
            varTexture = GUIUtils.AddTexture("SpawnerTexture", new CodeBitmaps.Spawner());
        }

        return varTexture;
    }

    protected virtual bool WriteTitle()
    {
        Spawner spawner = target as Spawner;
        if (spawner == null) { return true; }

        GUIStyle styleTitle = GUIUtils.GetActionTitleStyle();
        GUIStyle explanationStyle = GUIUtils.GetActionExplanationStyle();

        var backgroundColor = GUIUtils.ColorFromHex("#ffeffc");
        var textColor = GUIUtils.ColorFromHex("#2f4858");
        var separatorColor = GUIUtils.ColorFromHex("#ff5a5a");

        // Compute explanation text height
        string explanation = propExplanation.stringValue;
        int explanationLines = explanation.Count((c) => c == '\n');
        explanationLines += 1;
        int explanationTextHeight = explanationLines * (explanationStyle.fontSize + 2) + 6;

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
        GUI.DrawTexture(new Rect(titleRect.x + 10, titleRect.y + 4, 32, 32), GetIcon(), ScaleMode.ScaleToFit, true, 1.0f);
        EditorGUI.LabelField(new Rect(titleRect.x + 50, titleRect.y + 6, inspectorWidth - 20 - titleRect.x - 4, styleTitle.fontSize), "Spawner", styleTitle);
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
        bool refreshExplanation = false;
        if (propShowInfo.boolValue)
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeClose", BuildEyeClose));
        }
        else
        {
            toggle = GUI.Button(new Rect(rect.x + rect.width - 48, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("EyeOpen", BuildEyeOpen));
        }
        refreshExplanation = GUI.Button(new Rect(rect.x + rect.width - 26, rect.y + rect.height * 0.5f - 10, 20, 20), "", GUIUtils.GetButtonStyle("Refresh", BuildRefresh));
        if (toggle)
        {
            refreshExplanation = true;
            propShowInfo.boolValue = !propShowInfo.boolValue;

            Event e = Event.current;
            if (e.shift)
            {
                // Affect all the Actions in this object
                var allActions = spawner.GetComponents<Action>();
                foreach (var a in allActions)
                {
                    a.showInfo = propShowInfo.boolValue;
                    a.UpdateExplanation();
                }
            }

            serializedObject.ApplyModifiedProperties();
        }
        if (refreshExplanation)
        {
            spawner.UpdateExplanation();
        }

        return propShowInfo.boolValue;
    }


    protected void StdEditor(bool useOriginalEditor = true)
    {
        Spawner spawner = target as Spawner;
        if (spawner == null) return;

        EditorGUI.BeginChangeCheck();

        EditorGUILayout.PropertyField(propDescription, new GUIContent("Description"));
        EditorGUILayout.PropertyField(propForceCount, new GUIContent("Force Count?"));
        if (propForceCount.boolValue)
        {
            EditorGUILayout.PropertyField(propNumberOfEntities, new GUIContent("Number Of Entities"));
        }
        else
        {
            EditorGUILayout.PropertyField(propUsePulsePattern, new GUIContent("Use Pulse Pattern?"));
            if (propUsePulsePattern.boolValue)
            {
                EditorGUILayout.PropertyField(propPulsePattern, new GUIContent("Pulse Pattern"));
                EditorGUILayout.PropertyField(propPulseTime, new GUIContent("Pulse Time"));
            }
        }
        EditorGUILayout.PropertyField(propPrefabs, new GUIContent("Prefabs"));

        var colliders = spawner.GetComponents<BoxCollider2D>();
        if ((colliders != null) && (colliders.Length > 0))
        {

        }
        else
        {
            EditorGUILayout.PropertyField(propSpawnPoints, new GUIContent("Spawn Points"));
            
            if (spawner.GetSpawnPointCount() > 1)
            {
                EditorGUILayout.PropertyField(propSpawnPointType, new GUIContent("Spawn Point Sequence"));
            }
        }
        EditorGUILayout.PropertyField(propModifiers, new GUIContent("Modifiers"));
        if ((propModifiers.enumValueFlag & (int)Spawner.Modifiers.Scale) != 0)
        {
            EditorGUILayout.PropertyField(propScaleVariance, new GUIContent("Scale Variance"));
        }
        if ((propModifiers.enumValueFlag & (int)Spawner.Modifiers.Speed) != 0)
        {
            EditorGUILayout.PropertyField(propSpeedVariance, new GUIContent("Speed Variance"));
        }

        EditorGUILayout.PropertyField(propSetParent, new GUIContent("Set Parent?"));

        serializedObject.ApplyModifiedProperties();

        if (EditorGUI.EndChangeCheck())
        {
            spawner.UpdateExplanation();
        }

        // Draw old editor, need it for now
        if (useOriginalEditor)
        {
            base.OnInspectorGUI();
        }

    }

    protected bool CheckBox(string label, float x, float y, float width, bool initialValue)
    {
        GUIStyle style = GUI.skin.toggle;
        Vector2  size = style.CalcSize(new GUIContent(label));

        EditorGUI.LabelField(new Rect(x, y, size.x, 20), label);
        float offsetX = size.x + 1;

        if (offsetX + 20 > width) offsetX = width - 20;

        bool ret = EditorGUI.Toggle(new Rect(x + offsetX, y, 20, 20), initialValue);

        return ret;
    }

    private void BuildEyeOpen(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeOpen());
    }

    private void BuildEyeClose(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.EyeClose());
    }

    private void BuildRefresh(string name)
    {
        BuildTitleButton(name, new CodeBitmaps.Refresh());
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

}
