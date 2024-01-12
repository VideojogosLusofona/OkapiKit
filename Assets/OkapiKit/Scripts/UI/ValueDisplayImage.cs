using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/UI/Display Image")]
    public class ValueDisplayImage : ValueDisplay
    {
        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "This component turns the children of this object on/off depending on the value of the variable.";
        }

        // Update is called once per frame
        void Update()
        {
            var v = GetVariable();
            if (v == null) return;

            int i = 0;

            foreach (Transform t in transform)
            {
                t.gameObject.SetActive(i < v.currentValue);
                i++;
            }
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            var v = GetVariable();
            if (v != null)
            {
                if (v.type != Variable.Type.Integer)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "This type of display only supports integer values!"));
                }
            }
        }

    }
}