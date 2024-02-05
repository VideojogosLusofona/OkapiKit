using System.Collections;
using System.Collections.Generic;
using UnityEditor.TerrainTools;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

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
            if (PrefabStageUtility.GetCurrentPrefabStage() == null)
            {
                if (OkapiConfig.showTags)
                {
                    // Get all tags
                    var hos = HypertaggedObject.GetAll();
                    foreach (var h in hos)
                    {
                        var text = h.GetTagString();
                        if (text != "")
                        {
                            var pos = h.transform.position;

                            var rr = h.GetComponent<Renderer>();
                            if (rr)
                            {
                                pos = rr.bounds.center + rr.bounds.extents.y * Vector3.up;
                            }

                            Handles.Label(pos, text, GUIUtils.GetCenteredLabelStyle("#00FFFFFF", GUIUtils.GetColorTexture("BlackBackground", Color.black)));
                        }
                    }
                }
            }
        }
    }
}
