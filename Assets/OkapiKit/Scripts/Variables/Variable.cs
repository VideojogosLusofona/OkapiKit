using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [CreateAssetMenu(menuName = "Okapi Kit/Variable")]
    public class Variable : OkapiScriptableObject
    {
        public enum Type { Float, Integer };

        [SerializeField]
        private Type _type;
        [SerializeField]
        private float _currentValue = 0;
        [SerializeField]
        private float _defaultValue = 0;
        [SerializeField]
        private bool _hasLimits = true;
        [SerializeField]
        private float _minValue = 0;
        [SerializeField]
        private float _maxValue = 1000.0f;

        public float currentValue { get { return _currentValue; } }
        public float minValue { get { return _minValue; } }
        public float maxValue { get { return _maxValue; } }
        public bool hasLimits => _hasLimits;

        public Type type
        {
            get { return _type; }
            set { _type = value; }
        }

        public void SetValue(float value)
        {
            _currentValue = (_hasLimits) ? (Mathf.Clamp(value, _minValue, _maxValue)) : (value);
            if (type == Type.Integer)
            {
                _currentValue = (int)_currentValue;
            }
        }

        public void ChangeValue(float value)
        {
            _currentValue = (_hasLimits) ? (Mathf.Clamp(_currentValue + value, _minValue, _maxValue)) : (_currentValue + value);
            if (type == Type.Integer)
            {
                _currentValue = (int)_currentValue;
            }
        }

        public void ResetValue()
        {
            _currentValue = _defaultValue;
        }

        public object GetRawValue()
        {
            if (_type == Type.Float) return _currentValue;

            return (int)_currentValue;
        }

        public void SetProperties(Type type, float currentValue, float defaultValue, bool hasLimits, float minValue, float maxValue)
        {
            this._type = type;
            this._currentValue = currentValue;
            this._defaultValue = defaultValue;
            this._hasLimits = hasLimits;
            this._minValue = minValue;
            this._maxValue = maxValue;
        }

        public string GetValueString()
        {
            if (type == Type.Integer) return ((int)currentValue).ToString();

            return currentValue.ToString();
        }

        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            string desc = "";

            switch (_type)
            {
                case Type.Float:
                case Type.Integer:
                    desc += $"Defines a global variable of type float, with a default value {_defaultValue}.\n";
                    desc += $"Currently it holds the value {_currentValue}.\n";
                    if (hasLimits)
                    {
                        desc += $"It's limited between {_minValue} and {_maxValue}.";
                    }
                    break;
                default:
                    break;
            }

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