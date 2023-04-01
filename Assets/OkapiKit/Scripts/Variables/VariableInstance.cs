using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class VariableInstance : OkapiElement
    {
        public Variable.Type type = Variable.Type.Float;
        public float currentValue = 0;
        public float defaultValue = 0;
        public bool isInteger = false;
        public bool hasLimits = true;
        [SerializeField, ShowIf("hasLimits")]
        public float minValue = -float.MaxValue;
        [SerializeField, ShowIf("hasLimits")]
        public float maxValue = float.MaxValue;

        private Variable value;

        void Start()
        {
            value = ScriptableObject.CreateInstance<Variable>();
            value.SetProperties(type, currentValue, defaultValue, isInteger, hasLimits, minValue, maxValue);
        }

        public Variable GetVariable()
        {
            return value;
        }

        public void SetValue(float value)
        {
            float delta = value - this.value.currentValue;
            ChangeValue(value);
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
            //Debug.Log($"Change value {name}: Old = {this.value.currentValue}, New = {this.value.currentValue + value}");

            float prevValue = this.value.currentValue;

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

        public override string UpdateExplanation()
        {
            _explanation = GetRawDescription("", gameObject);
            return _explanation;
        }
    }
}