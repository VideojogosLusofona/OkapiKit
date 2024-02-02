using UnityEngine;
using UnityEditor;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(Spawner))]
    public class SpawnerEditor : OkapiBaseEditor
    {
        SerializedProperty propSpawnsPerTrigger;
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
        SerializedProperty propSpawnPathSpacing;

        protected override void OnEnable()
        {
            base.OnEnable();

            propSpawnsPerTrigger = serializedObject.FindProperty("spawnsPerTrigger");
            propForceCount = serializedObject.FindProperty("forceCount");
            propNumberOfEntities = serializedObject.FindProperty("numberOfEntities");
            propUsePulsePattern = serializedObject.FindProperty("usePulsePattern");
            propPulsePattern = serializedObject.FindProperty("pulsePattern");
            propPulseTime = serializedObject.FindProperty("pulseTime");
            propPrefabs = serializedObject.FindProperty("prefabs");
            propSpawnPoints = serializedObject.FindProperty("spawnPoints");
            propSpawnPointType = serializedObject.FindProperty("spawnPointType");
            propSpawnPathSpacing = serializedObject.FindProperty("spawnPathSpacing");
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

            EditorGUILayout.PropertyField(propDescription, new GUIContent("Description", "This is for you to leave a comment for yourself or others."));
            EditorGUILayout.Space(10);
            EditorGUILayout.PropertyField(propSpawnsPerTrigger, new GUIContent("Spawns per trigger", "Everytime this spawner is triggered, how many entities will it trigger at one time.\nNotice that if you have a pulse patterns, it will be this ammount of patters of spawning, so if your pulse pattern is xxx and this number is 3, it will trigger a total of 9 times!"));
            EditorGUILayout.PropertyField(propForceCount, new GUIContent("Force Count?", "If true, a certain number of objects will be spawned and guaranteed to be present in the scene at any time.\nIf any object spawned by this is destroyed, another gets spawned in."));
            if (propForceCount.boolValue)
            {
                EditorGUILayout.PropertyField(propNumberOfEntities, new GUIContent("Number Of Entities", "How many spawned objects should be active at any time."));
            }
            else
            {
                EditorGUILayout.PropertyField(propUsePulsePattern, new GUIContent("Use Pulse Pattern?", "If true, we can specify how the spawning should happen.\nThis is useful to spawn shot patterns, for example."));
                if (propUsePulsePattern.boolValue)
                {
                    EditorGUILayout.PropertyField(propPulsePattern, new GUIContent("Pulse Pattern", "What's the spawn pattern. Every 'x' is a spawn, an 'o' a pause."));
                    EditorGUILayout.PropertyField(propPulseTime, new GUIContent("Pulse Time", "How much time does each symbol in the pulse pattern represent."));
                }
            }
            EditorGUILayout.PropertyField(propPrefabs, new GUIContent("Prefabs", "Which prefabs should be spawned.\nIf more than one, a random one from this list will be chosen each time a spawn should happen."));

            var colliders = spawner.GetComponents<BoxCollider2D>();
            if ((colliders != null) && (colliders.Length > 0))
            {

            }
            else
            {
                var path = spawner.GetComponent<Path>();
                if (path)
                {
                    EditorGUILayout.PropertyField(propSpawnPointType, new GUIContent("Spawn Point Sequence", "How to choose a spawn point?\nRandom: A random point in the path is selected\nSequence: Points are chosen in sequence in a path, with the given spacing\nAll: Objects will spawn all over the path, with the given spacing"));

                    if (propSpawnPointType.enumValueIndex != (int)Spawner.SpawnPointType.Random)
                    {
                        EditorGUILayout.PropertyField(propSpawnPathSpacing, new GUIContent("Point Spacing", "What is the spacing between spawn points on the path, in parametric space (percentage of the whole path - i.e. 0.5 means that there will be a point in the beginning, one in the middle, one at the end)"));
                    }
                }
                else
                {
                    EditorGUILayout.PropertyField(propSpawnPoints, new GUIContent("Spawn Points", "Where should the objects spawn?\nIf no points, objects will spawn at this position.\nIf more than one point, a point is chosen based on Spawn Point Sequence."));

                    if (spawner.GetSpawnPointCount() > 1)
                    {
                        EditorGUILayout.PropertyField(propSpawnPointType, new GUIContent("Spawn Point Sequence", "How to choose a spawn point?\nRandom: A random one is selected\nSequence: Points are chosen in sequence (i.e. first object is spawned at the first point, second object at the second point, etc)\nAll: Objects are spawned at the same time in all points (i.e. if there's 5 points, 5 objects will spawn, one in each point)"));
                    }
                }
            }
            EditorGUILayout.PropertyField(propModifiers, new GUIContent("Modifiers", "What kind of per-spawn modifiers we want?\nScale: Every spawn has a different scale modifier applied\nSpeed: Every spawn has a different speed modifier applied"));
            if ((propModifiers.enumValueFlag & (int)Spawner.Modifiers.Scale) != 0)
            {
                EditorGUILayout.PropertyField(propScaleVariance, new GUIContent("Scale Variance", "Spawned objects will vary in scale from X to Y"));
            }
            if ((propModifiers.enumValueFlag & (int)Spawner.Modifiers.Speed) != 0)
            {
                EditorGUILayout.PropertyField(propSpeedVariance, new GUIContent("Speed Variance", "Spawned objects will vary in speed from X to Y (only if they have a Movement component)"));
            }

            EditorGUILayout.PropertyField(propSetParent, new GUIContent("Set Parent?", "If active, the objects will spawn as children of this one."));

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
            Vector2 size = style.CalcSize(new GUIContent(label));

            EditorGUI.LabelField(new Rect(x, y, size.x, 20), label);
            float offsetX = size.x + 1;

            if (offsetX + 20 > width) offsetX = width - 20;

            bool ret = EditorGUI.Toggle(new Rect(x + offsetX, y, 20, 20), initialValue);

            return ret;
        }
    }
}