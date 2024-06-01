using UnityEngine;

namespace NodeEditor
{
    [System.Serializable]
    [NodeWidth(200.0f)]
    [NodeDefaultPropertyVisibility(false)]
    public class BaseNode 
    {
        public Object   owner;
        public Vector2  position = Vector2.zero;
        [NodePropertyVisibility(true), NodeInput]
        public bool     nodeEnabled = true;
    }
}
