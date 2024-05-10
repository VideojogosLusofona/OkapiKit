using NodeEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OkapiKitV2
{
    public class OkapiScriptEditor : NodeEditor.NodeEditor<OkapiNode>
    {
        private OkapiScript currentScript = null;
        private string[]    subSelectorOptions;

        [MenuItem("Window/Okapi/Script Editor")]
        static void Init()
        {
            var theme = new Theme()
            {
                windowName = "Okapi Script",
                menuAddNode = true
            };
            var editor = Init<OkapiScriptEditor>(theme);
        }

        protected override void OnEnable()
        {
            base.OnEnable();

            Init();
        }

        protected override UnityEngine.Object GetSelection()
        {
            return currentScript;
        }

        protected override void SetActiveSelection()
        {
            if (Selection.count == 0)
            {
                currentScript = null;
                subSelectorOptions = null;
                disableReason = "Select an object with an Okapi object, or an Okapi script";
            }
            else if (Selection.count> 1)
            {
                currentScript = null;
                subSelectorOptions = null;
                disableReason = "Multi-object script editing not supported!";
            }
            else
            {
                if (Selection.activeGameObject != null)
                {
                    var okapiObject = Selection.activeGameObject.GetComponent<OkapiObject>();
                    if (okapiObject == null)
                    {
                        currentScript = null;
                        disableReason = "Select an object with an Okapi object!";
                    }
                    else
                    {
                        if (okapiObject.scriptCount == 1)
                        {
                            currentScript = okapiObject.scripts[0];
                        }
                        else
                        {
                            List<string> subSelectorOptionsList = new();
                            int          index = -1;
                            var          i = 0;
                            foreach (var script in okapiObject.scripts)
                            {
                                if (script != null)
                                {
                                    subSelectorOptionsList.Add(script.name);
                                    if (script == currentScript)
                                    {
                                        index = i;
                                    }
                                    ++i;
                                }
                            }
                            if (index == -1)
                            {
                                currentScript = null;
                                disableReason = "Select a script from the dropdown";
                            }

                            subSelectorOptions = subSelectorOptionsList.ToArray();
                        }
                    }
                }
                else 
                {
                    subSelectorOptions = null;
                    currentScript = Selection.activeObject as OkapiScript;
                    if (currentScript == null)
                    {
                        disableReason = "Select an object with an Okapi object, or an Okapi script";
                    }
                }
            }
        }

        protected override bool hasSelection => currentScript != null;
        protected override string[] GetSubSelectorOptions() => subSelectorOptions;
        protected override int GetSubSelectorSelected()
        {
            if (subSelectorOptions == null) return -1;
            if (currentScript == null) return -1;

            for (int i = 0; i < subSelectorOptions.Length; ++i)
            {
                if (subSelectorOptions[i] == currentScript.name) return i;
            }

            return -1;
        }
        protected override void SetSubSelector(string str)
        {
            var okapiObject = Selection.activeGameObject.GetComponent<OkapiObject>();
            foreach (var script in okapiObject.scripts)
            {
                if (script != null)
                {
                    if (script.name == str)
                    {
                        currentScript = script;
                        break;
                    }
                }
            }
        }

        protected override void OnNodeCreate(BaseNode newNode, Vector2 addPosition)
        {
            var okapiNode = newNode as OkapiNode;

            if (okapiNode == null)
            {
                Debug.LogError("Unexpected error after creating node!");
                return;
            }

            // Before I change the current script, record its state
            Undo.RecordObject(currentScript, "Add New Node");

            currentScript.Add(okapiNode);

            SetPositionOfNewNode(okapiNode, addPosition);

            EditorUtility.SetDirty(currentScript);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }



        protected override void OnNodeDelete(BaseNode newNode)
        {
            var okapiNode = newNode as OkapiNode;

            if (okapiNode == null)
            {
                Debug.LogError("Unexpected error after deleting node!");
                return;
            }

            // Before I change the current script, record its state
            Undo.RecordObject(currentScript, "Remove Node");

            currentScript.Remove(okapiNode);

            EditorUtility.SetDirty(currentScript);
            ActiveEditorTracker.sharedTracker.ForceRebuild();
        }

        void SetPositionOfNewNode(OkapiNode node, Vector2 addPosition)
        {
            node.position = addPosition;
        }

        protected override List<BaseNode> GetNodes()
        {
            return currentScript.GetNodes().Select(node => node as BaseNode).ToList();
        }

        protected override bool NodeExists(BaseNode node) => (currentScript != null) ? (currentScript.Exists(node as OkapiNode)) : (false); 
    }
}
