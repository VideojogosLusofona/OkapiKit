using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using System;

[CreateAssetMenu(menuName = "Okapi Kit/Variable")]
public class Variable : ScriptableObject
{
    [SerializeField]
    private float _currentValue = 0;
    [SerializeField]
    private float _defaultValue = 0;
    [SerializeField]
    private bool _isInteger = false;
    [SerializeField]
    private bool _hasLimits = true;
    [SerializeField, ShowIf("_hasLimits")]
    private float _minValue = -float.MaxValue;
    [SerializeField, ShowIf("_hasLimits")]
    private float _maxValue = float.MaxValue;

    public float currentValue { get { return _currentValue; } }
    public float minValue { get { return _minValue; } }
    public float maxValue { get { return _maxValue; } }

    public void SetValue(float value)
    {
        _currentValue = (_hasLimits) ? (Mathf.Clamp(value, _minValue, _maxValue)) : (value);
        if (_isInteger)
        {
            _currentValue = (int)_currentValue;
        }
    }

    public void ChangeValue(float value)
    {
        _currentValue = (_hasLimits) ? (Mathf.Clamp(_currentValue + value, _minValue, _maxValue)) : (_currentValue + value);
        if (_isInteger)
        {
            _currentValue = (int)_currentValue;
        }
    }

    public void ResetValue()
    {
        _currentValue = _defaultValue;
    }

    public void SetProperties(float currentValue, float defaultValue, bool isInteger, bool hasLimits, float minValue, float maxValue)
    {
        this._currentValue = currentValue;
        this._defaultValue = defaultValue;
        this._isInteger = isInteger;
        this._hasLimits = hasLimits;
        this._minValue = minValue;
        this._maxValue = maxValue;
    }
}
