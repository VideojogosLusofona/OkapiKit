using UnityEngine;
using UnityEditor;
using System.Linq;

[CustomEditor(typeof(Spawner))]
public class SpawnerEditor : OkapiBaseEditor
{
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

    protected override void OnEnable()
    {
        base.OnEnable();

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

    protected override Texture2D GetIcon()
    {
        var varTexture = GUIUtils.GetTexture("Spawner");

        return varTexture;
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
        return "Spawner";
    }

    protected override (Color, Color, Color) GetColors() => (GUIUtils.ColorFromHex("#ffeffc"), GUIUtils.ColorFromHex("#2f4858"), GUIUtils.ColorFromHex("#ff5a5a"));

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
}
