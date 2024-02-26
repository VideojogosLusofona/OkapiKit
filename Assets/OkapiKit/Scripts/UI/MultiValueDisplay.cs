using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public class MultiValueDisplay : OkapiElement
    {
        [SerializeField, OVNoLabel, OVNoFloat, OVNoInteger, OVNoFunction]
        protected OkapiValue[]    values;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "";
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";

            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        public string GetVariableNames()
        {
            string ret = "";

            if ((values == null) || (values.Length == 0))
            {
                ret = "[UNDEFINED]";
            }
            else
            {
                ret = "[";
                for (int i  = 0; i < values.Length; i++)
                {
                    if (i > 0) ret += ", ";
                    ret += values[i].GetName();
                }
                ret += "]";
            }

            return ret;
        }

        protected object[] GetVariables()
        {
            List<object> vars = new List<object>();

            foreach (var v in values)
            {
                vars.Add(v.GetRawValue());
            }

            return vars.ToArray();
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();
        }
    }
}