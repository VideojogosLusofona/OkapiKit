using UnityEngine;
using NodeEditor;

namespace OkapiKitV2
{
    [System.Serializable]
    public class OkapiNode
    {
        public Vector2  position;
    }

    [System.Serializable]
    [NodePath("Level 1/Okapi Node 1")]
    public class OkapiNode1 : OkapiNode
    {
        public int okapiNode1;
    }

    [System.Serializable]
    [NodePath("Another Node 2")]
    public class AnotherNode2 : OkapiNode
    {
        public string anotherNode2;
    }

    [System.Serializable]
    [NodePath("Level 1/Level 2/Third Time Is The Charm")]
    public class ThirdTimeIsTheCharm : OkapiNode
    {
        public bool thirdTimeIsTheCharm;
    }
}