using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEditor;
using UnityEngine;

namespace OkapiKit.Editor
{
    [InitializeOnLoad] // Ensure the static constructor is called in the editor
    public static class OkapiSceneEditorGUI
    {
        static OkapiSceneEditorGUI()
        {
            SceneView.duringSceneGui += OnSceneGUI;
        }

        private static void OnSceneGUI(SceneView sceneView)
        {
            if (OkapiConfig.showTags)
            {
                // Get all tags
                var hos = GameObject.FindObjectsByType<HypertaggedObject>(FindObjectsInactive.Exclude, FindObjectsSortMode.None);
                foreach (var h in hos)
                {
                    var text = h.GetTagString();
                    if (text != "")
                    {
                        var pos = h.transform.position;

                        var sr = h.GetComponent<SpriteRenderer>();
                        if (sr)
                        {
                            pos = sr.bounds.center + sr.bounds.size.y * Vector3.up;
                        }

                        Handles.Label(pos, text, GUIUtils.GetCenteredLabelStyle("#00FFFFFF", GUIUtils.GetColorTexture("BlackBackground", Color.black)));
                    }
                }
            }
        }
    }
}
