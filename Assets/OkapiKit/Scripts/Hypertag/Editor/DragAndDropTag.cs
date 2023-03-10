using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class DragAndDropTag
{
    static List<Hypertag> currentTags;

    static DragAndDropTag()
    {
        //SceneView.duringSceneGui += DragAndDropHypertag;
        EditorApplication.update += DragAndDropHypertag;
    }

    private static void DragAndDropHypertag()
    {
        if (EditorWindow.mouseOverWindow == null) return;

        if (EditorWindow.mouseOverWindow.titleContent.text == "Inspector")
        {
            // Check if an object is being dragged, and if it is an hypertag
            bool checkIfHypertag = true;
            List<Hypertag>  tags = new List<Hypertag>();
            foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
            {
                if (obj is not Hypertag)
                {
                    checkIfHypertag = false;
                    break;
                }
                else
                {
                    tags.Add(obj as Hypertag);
                }
            }

            if ((checkIfHypertag) && (tags.Count > 0))
            {
                if (Selection.gameObjects.Length > 0)
                {
                    currentTags = tags;
                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                }
            }
            else if (DragAndDrop.objectReferences.Length == 0)
            {
                if ((currentTags != null) && (Selection.gameObjects.Length > 0))
                {
                    // There's tags to drop on current object
                    foreach (var obj in Selection.gameObjects)
                    {
                        HypertaggedObject ho = obj.GetComponent<HypertaggedObject>();
                        if (ho == null)
                        {
                            Undo.RecordObject(obj, "Add Hypertag");
                            ho = Undo.AddComponent<HypertaggedObject>(obj);
                            Undo.RegisterCompleteObjectUndo(obj, "Add Hypertag");
                        }

                        Undo.RecordObject(ho, "Change Hypertags");
                        ho.AddTag(currentTags);
                        Undo.RegisterCompleteObjectUndo(ho, "Changed Hypertags");
                    }

                    Undo.FlushUndoRecordObjects();
                }
                currentTags = null;
            }
        }
        else
        {
            if (currentTags != null)
            {
                currentTags = null;
                DragAndDrop.visualMode = DragAndDropVisualMode.Rejected;
            }
        }
    }

    /*    private static void DragAndDropHypertag(SceneView sceneView)
        {
            Event evt = Event.current;
            if ((evt.type == EventType.DragUpdated || evt.type == EventType.DragPerform))
    //            && (rect.Contains(evt.mousePosition)))
            {
                bool checkIfHypertag = true;
                foreach (UnityEngine.Object obj in DragAndDrop.objectReferences)
                {
                    if (obj is not Hypertag)
                    {
                        checkIfHypertag = false;
                        break;
                    }
                }

                if (checkIfHypertag)
                {
                    Debug.Log("Dragging hypertag");

                    DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                    if (evt.type == EventType.DragPerform)
                    {
                        DragAndDrop.AcceptDrag();

                        foreach (Object obj in DragAndDrop.objectReferences)
                        {
                            if (obj is Action)
                            {
                                // Add element to the array
                                actionList.arraySize++;
                                var newElement = actionList.GetArrayElementAtIndex(actionList.arraySize - 1);
                                if (newElement != null)
                                {
                                    var propDelay = newElement.FindPropertyRelative("delay");
                                    var propAction = newElement.FindPropertyRelative("action");
                                    if (propDelay != null) propDelay.floatValue = d;
                                    if (propAction != null) propAction.objectReferenceValue = obj as Action;
                                }
                            }
                        }
                    }

                    evt.Use();
                }
            }
        }*/
}
