using System.Collections.Generic;
using UnityEngine;

namespace OkapiKitV2
{

    [CreateAssetMenu(menuName = "Okapi Kit/V2/Hypertag")]
    public class OkapiTag : OkapiScriptableObject
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