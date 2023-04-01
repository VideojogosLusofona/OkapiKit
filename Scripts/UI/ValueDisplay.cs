using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class ValueDisplay : OkapiElement
    {
        [SerializeField]
        protected VariableInstance valueHandler;
        [SerializeField]
        protected Variable variable;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "";
        }

        public override string UpdateExplanation()
        {
            _explanation = "";

            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        protected Variable GetVariable()
        {
            if (variable) return variable;

            if (valueHandler)
            {
                return valueHandler.GetVariable();
            }

            return null;
        }
    }
}