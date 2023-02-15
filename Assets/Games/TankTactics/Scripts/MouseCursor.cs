using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursor : MonoBehaviour
{
    public Texture2D cursorTexture;
    public CursorMode cursorMode = CursorMode.Auto;
    public Vector2 hotSpot = Vector2.zero;
    public bool mouseVisible = true;

    void Start()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
        //Cursor.visible = mouseVisible;
        //Cursor.lockState = CursorLockMode.Confined;
    }

    void Update()
    {
        //this.transform.localPosition = Input.mousePosition;
    }

    void OnMouseEnter()
    {
        Cursor.SetCursor(cursorTexture, hotSpot, cursorMode);
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(null, Vector2.zero, cursorMode);
    }
}
