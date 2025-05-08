using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [CustomEditor(typeof(AI))]
    public class AIEditor : OkapiBaseEditor
    {
        // Serialized properties
        SerializedProperty agentRadiusProp, agentOffsetProp, flagsProp, obstacleMaskProp;
        SerializedProperty wanderRadiusProp, timeBetweenWandersProp, wanderSpeedModifierProp;
        SerializedProperty fleeProximityRadiusProp, fleeSpeedModifierProp, stopFleeingIntervalProp, fleeTagsProp;
        SerializedProperty chaseStopDistanceProp, chaseViewDistanceProp, chaseUseFOVProp, chaseFieldOfViewProp;
        SerializedProperty chaseSpeedModifierProp, chaseUseMaxRangeFromSpawnProp, chaseMaxRangeFromSpawnProp, chaseRequireLOSProp, chaseTagsProp;
        SerializedProperty searchSpeedModifierProp, searchMaxTimeProp, searchRadius;
        SerializedProperty patrolSpeedModifierProp, patrolModeProp, patrolPathProp, patrolPointsProp;

        AI ai;

        protected override void OnEnable()
        {
            base.OnEnable();

            ai = (AI)target;

            // General
            agentRadiusProp = serializedObject.FindProperty("agentRadius");
            agentOffsetProp = serializedObject.FindProperty("agentOffset");
            flagsProp = serializedObject.FindProperty("flags");
            obstacleMaskProp = serializedObject.FindProperty("obstacleMask");

            // Wander
            wanderRadiusProp = serializedObject.FindProperty("wanderRadius");
            timeBetweenWandersProp = serializedObject.FindProperty("timeBetweenWanders");
            wanderSpeedModifierProp = serializedObject.FindProperty("wanderSpeedModifier");

            // Flee
            fleeProximityRadiusProp = serializedObject.FindProperty("fleeProximityRadius");
            fleeSpeedModifierProp = serializedObject.FindProperty("fleeSpeedModifier");
            stopFleeingIntervalProp = serializedObject.FindProperty("stopFleeingInterval");
            fleeTagsProp = serializedObject.FindProperty("fleeTags");

            // Chase
            chaseStopDistanceProp = serializedObject.FindProperty("chaseStopDistance");
            chaseViewDistanceProp = serializedObject.FindProperty("chaseViewDistance");
            chaseUseFOVProp = serializedObject.FindProperty("chaseUseFOV");
            chaseFieldOfViewProp = serializedObject.FindProperty("chaseFieldOfView");
            chaseSpeedModifierProp = serializedObject.FindProperty("chaseSpeedModifier");
            chaseUseMaxRangeFromSpawnProp = serializedObject.FindProperty("chaseUseMaxRangeFromSpawn");
            chaseMaxRangeFromSpawnProp = serializedObject.FindProperty("chaseMaxRangeFromSpawn");
            chaseRequireLOSProp = serializedObject.FindProperty("chaseRequireLOS");
            chaseTagsProp = serializedObject.FindProperty("chaseTags");

            // Search
            searchSpeedModifierProp = serializedObject.FindProperty("searchSpeedModifier");
            searchMaxTimeProp = serializedObject.FindProperty("searchMaxTime");
            searchRadius = serializedObject.FindProperty("searchRadius");

            // Patrol
            patrolSpeedModifierProp = serializedObject.FindProperty("patrolSpeedModifier");
            patrolModeProp = serializedObject.FindProperty("patrolMode");
            patrolPathProp = serializedObject.FindProperty("patrolPath");
            patrolPointsProp = serializedObject.FindProperty("patrolPoints");
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            if (WriteTitle())
            {
                EditorGUILayout.PropertyField(agentRadiusProp, new GUIContent("Agent Radius", "Radius of the agent for collisions and LOS."));
                EditorGUILayout.PropertyField(agentOffsetProp, new GUIContent("Agent Offset", "Offset from the GameObject's origin."));
                EditorGUILayout.PropertyField(obstacleMaskProp, new GUIContent("Obstacle Mask", "Layer mask for line-of-sight blocking."));
                EditorGUILayout.PropertyField(flagsProp, new GUIContent("Enabled Behaviours", "Which AI behaviours are enabled."));

                // Wander section
                if (ai.canWander)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Wander", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(wanderRadiusProp, new GUIContent("Wander Radius", "How far the AI can wander from its spawn."));
                    EditorGUILayout.PropertyField(timeBetweenWandersProp, new GUIContent("Time Between Wanders", "Random time interval between wander targets."));
                    EditorGUILayout.PropertyField(wanderSpeedModifierProp, new GUIContent("Wander Speed Modifier", "Speed modifier when wandering."));
                }

                // Flee section
                if (ai.canFlee)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Flee", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(fleeProximityRadiusProp, new GUIContent("Proximity Radius", "Distance within which AI will flee from threats."));
                    EditorGUILayout.PropertyField(fleeSpeedModifierProp, new GUIContent("Flee Speed Modifier", "Speed modifier while fleeing."));
                    EditorGUILayout.PropertyField(stopFleeingIntervalProp, new GUIContent("Stop Fleeing Interval", "Delay before stopping fleeing."));
                    EditorGUILayout.PropertyField(fleeTagsProp, new GUIContent("Flee Tags", "Tags of objects that trigger fleeing."), true);
                }

                // Chase section
                if (ai.canChase)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Chase", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(chaseStopDistanceProp, new GUIContent("Stop Distance", "How close the target must be to stop chasing."));
                    EditorGUILayout.PropertyField(chaseViewDistanceProp, new GUIContent("View Distance", "How far the AI can detect chase targets."));
                    EditorGUILayout.PropertyField(chaseUseFOVProp, new GUIContent("Use Field of View", "Enable cone-based vision."));
                    if (chaseUseFOVProp.boolValue)
                        EditorGUILayout.PropertyField(chaseFieldOfViewProp, new GUIContent("Field of View", "Half-angle of the vision cone."));
                    EditorGUILayout.PropertyField(chaseRequireLOSProp, new GUIContent("Require Line of Sight", "If enabled, AI must see the target."));
                    EditorGUILayout.PropertyField(chaseUseMaxRangeFromSpawnProp, new GUIContent("Use Max Range", "Limit distance from spawn during chase."));
                    if (chaseUseMaxRangeFromSpawnProp.boolValue)
                        EditorGUILayout.PropertyField(chaseMaxRangeFromSpawnProp, new GUIContent("Max Range from Spawn", "Maximum distance from spawn while chasing."));
                    EditorGUILayout.PropertyField(chaseSpeedModifierProp, new GUIContent("Chase Speed Modifier", "Speed modifier when chasing."));
                    EditorGUILayout.PropertyField(chaseTagsProp, new GUIContent("Chase Tags", "Tags of objects the AI will chase."), true);
                }

                // Search section
                if (ai.canSearch)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Search", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(searchSpeedModifierProp, new GUIContent("Search Speed Modifier", "Speed modifier when searching."));
                    EditorGUILayout.PropertyField(searchRadius, new GUIContent("Search Radius", "Maximum radius to search for the lost target."));
                    EditorGUILayout.PropertyField(searchMaxTimeProp, new GUIContent("Search Max Time", "Maximum duration for search behavior."));
                }

                // Patrol section
                if (ai.canPatrol)
                {
                    EditorGUILayout.Space(6);
                    EditorGUILayout.LabelField("Patrol", EditorStyles.boldLabel);
                    EditorGUILayout.PropertyField(patrolSpeedModifierProp, new GUIContent("Patrol Speed Modifier", "Speed modifier when patrolling."));
                    EditorGUILayout.PropertyField(patrolPathProp, new GUIContent("Patrol Path", "Optional path component to define patrol route."));
                    if (patrolPathProp.objectReferenceValue == null)
                    {
                        EditorGUILayout.PropertyField(patrolModeProp, new GUIContent("Patrol Mode", "Random or Sequential mode."));
                        EditorGUILayout.PropertyField(patrolPointsProp, new GUIContent("Patrol Points", "Manual list of points to patrol if no path is assigned."), true);
                    }
                }

                serializedObject.ApplyModifiedProperties();
            }

            ai.UpdateExplanation();
        }

        // OkapiBaseEditor requirements:

        protected override string GetTitle() => "AI Controller";

        protected override Texture2D GetIcon() => GUIUtils.GetTexture("Brain"); // Replace with your icon if available

        protected override GUIStyle GetTitleSyle() => GUIUtils.GetActionTitleStyle();

        protected override GUIStyle GetExplanationStyle() => GUIUtils.GetActionExplanationStyle();

        protected override (Color, Color, Color) GetColors() => (
            GUIUtils.ColorFromHex("#E2D5F9"), // Light background (lavender)
            GUIUtils.ColorFromHex("#6E57A0"), // Medium primary (purple-gray)
            GUIUtils.ColorFromHex("#A18CCB")  // Highlight/accent (soft violet)
        );
    }
}
