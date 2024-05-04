using UnityEngine;
using NodeEditor;

namespace OkapiKitV2
{
    [System.Serializable]
    public abstract class OkapiNode : BaseNode
    {
    }

    [System.Serializable]
    [NodeColor("#D0FFFF", "#2f4858", "#86CBFF")]
    public abstract class OkapiTriggerNode : OkapiNode
    {
    }

    [System.Serializable]
    [NodePath("Triggers/On Every Frame")]
    public class OkapiEveryFrameNode : OkapiTriggerNode
    {
        public int okapiNode1;
    }

    [System.Serializable]
    [NodeColor("#D7E8BA", "#2f4858", "#86CB92")]
    public abstract class OkapiAction : OkapiNode
    {
    }

    [System.Serializable]
    [NodePath("Actions/Flash")]
    public class OkapiFlashNode : OkapiAction
    {
        public string anotherNode2;
    }

    [System.Serializable]
    [NodeColor("#ffcaca", "#2f4858", "#ff6060")]
    public abstract class OkapiMovementNode : OkapiNode
    {
    }

    [System.Serializable]
    [NodePath("Movement/2D Movement")]
    public class Okapi2DMovementNode : OkapiMovementNode
    {
        public bool thirdTimeIsTheCharm;
    }
}
