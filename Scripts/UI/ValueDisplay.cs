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

        protected override string Internal_UpdateExplanation()
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

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            if ((variable == null) && (valueHandler == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No variable is set!", "This will display the contents of a variable, so we need to define which one."));
            }
        }
    }
}