using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace OkapiKitV2
{
    [CreateAssetMenu(menuName = "Okapi Kit/V2/Okapi Script")]

    public class OkapiScript : ScriptableObject
    {
        [SerializeReference]
        [SerializeField] private List<OkapiNode> nodes = new List<OkapiNode>();

        public int Add(OkapiNode node)
        {
            Undo.RecordObject(this, "Add Node");

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == null)
                {
                    nodes[i] = node;
                    return i;
                }
            }
            nodes.Add(node);

            return nodes.Count - 1;
        }

        public void Remove(OkapiNode node)
        {
            Undo.RecordObject(this, "Remove Node");

            for (int i = 0; i < nodes.Count; i++)
            {
                if (nodes[i] == node)
                {
                    nodes[i] = null;
                    return;
                }
            }
        }

        public List<OkapiNode> GetNodes() => nodes;
    }
}
