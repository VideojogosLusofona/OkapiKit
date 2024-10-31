using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.UIElements;

namespace OkapiKit
{
    public class GridObject : OkapiElement
    {
        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = ident;

            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            Grid grid = GetComponentInParent<Grid>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid in parent", "Grid objects need to be a child of an object with a Grid component!"));
            }
        }
    }
}
