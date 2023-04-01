using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    [CreateAssetMenu(menuName = "Okapi Kit/Variable")]
    public class Variable : ScriptableObject
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
        private float _minValue = -float.MaxValue;
        [SerializeField]
        private float _maxValue = float.MaxValue;

        public float currentValue { get { return _currentValue; } }
        public float minValue { get { return _minValue; } }
        public float maxValue { get { return _maxValue; } }

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

        public void SetProperties(Type type, float currentValue, float defaultValue, bool isInteger, bool hasLimits, float minValue, float maxValue)
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
    }
}