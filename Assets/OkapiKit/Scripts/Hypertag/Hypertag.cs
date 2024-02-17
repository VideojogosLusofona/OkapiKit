using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{

    [CreateAssetMenu(menuName = "Okapi Kit/Hypertag")]
    public class Hypertag : OkapiScriptableObject
    {
        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            string desc = $"Defines hypertag {name}";

            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", this);

            return _explanation;
        }
    }
}