using UnityEngine;

namespace NodeEditor
{
    [System.Serializable]
    [NodeWidth(200.0f)]
    public class BaseNode
    {
        public Object   owner;
        public Vector2  position = Vector2.zero;
        public bool     nodeEnabled = true;
    }
}
