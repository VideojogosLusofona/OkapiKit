using NaughtyAttributes;
using UnityEngine;

namespace OkapiKit
{

    [CreateAssetMenu(fileName = "ResourceType", menuName = "Okapi Kit/Resource Type")]
    public class ResourceType : OkapiScriptableObject
    {
        public string   displayName;
        public Color    displaySpriteColor = Color.white;
        public Sprite   displaySprite;
        public Color    displayTextColor = Color.white;
        public Color    displayNegativeTextColor = Color.red;
        public Color    displayBarColor = Color.white;
        public float    defaultValue = 100.0f;
        public float    maxValue = 100.0f;

        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            return "Resource Type";
        }        
    }
}