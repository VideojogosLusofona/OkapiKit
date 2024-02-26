using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Variable Instance")]
    public class VariableInstance : OkapiElement
    {
        [SerializeField]
        private Variable.Type type = Variable.Type.Float;
        [SerializeField]
        private float currentValue = 0;
        [SerializeField]
        private float defaultValue = 0;
        [SerializeField]
        private bool hasLimits = false;
        [SerializeField, ShowIf("hasLimits")]
        private float minValue = -float.MaxValue;
        [SerializeField, ShowIf("hasLimits")]
        private float maxValue = float.MaxValue;

        private Variable value;

        void Start()
        {
            value = ScriptableObject.CreateInstance<Variable>();
            value.SetProperties(type, defaultValue, defaultValue, hasLimits, minValue, maxValue);
        }

        public Variable.Type GetValueType() => type;
        public object GetRawValue()
        {
            if (value)
            {
                return value.GetRawValue();
            }

            if (type == Variable.Type.Float) return currentValue;

            return (int)currentValue;
        }

        public Variable GetVariable()
        {
            return value;
        }

        public float GetValue()
        {
            if (value != null) return value.currentValue;
            else return currentValue;
        }

        public void SetValue(float value)
        {
            this.value.SetValue(value);
        }

        public void Reset()
        {
            if (value)
            {
                value.ResetValue();
            }
        }

        public void ChangeValue(float value)
        {
            this.value.ChangeValue(value);
        }

        public string GetValueString()
        {
            if (value == null)
            {
                if (type == Variable.Type.Integer) return ((int)defaultValue).ToString();
                else return defaultValue.ToString();
            }

            return value.GetValueString();
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            if (description != "") return description;

            return "";
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = GetRawDescription("", gameObject);
            return _explanation;
        }
    }
}