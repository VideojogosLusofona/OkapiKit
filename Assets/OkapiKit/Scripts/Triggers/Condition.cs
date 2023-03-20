using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public struct Condition
{
    [System.Serializable] public enum ValueType { None = 0, TagCount = 1, 
                                                  WorldPositionX = 2, WorldPositionY = 3, RelativePositionX = 4, RelativePositionY = 5,
                                                  AbsoluteVelocityX = 6, AbsoluteVelocityY = 7,
                                                  Distance = 8, Angle = 9,
                                                  Probe = 10, ProbeDistance = 11,
                                                  IsGrounded = 12, IsGliding = 13 };
    [System.Serializable] public enum Comparison { Equal = 0, Less = 1, LessEqual = 2, Greater = 3, GreaterEqual = 4, Different = 5};
    [System.Serializable] public enum Axis { UpAxis = 0, RightAxis = 1 };

    [System.Serializable] public enum DataType { Number = 0, Boolean = 1};

    public bool                 negate;
    public VariableInstance     valueHandler;
    public Variable             variable;
    public ValueType            valueType;
    public Hypertag             tag;
    public Transform            sourceTransform;
    public Rigidbody2D          rigidBody;
    public Axis                 axis;
    public Probe                probe;
    public MovementPlatformer   movementPlatformer;
    public Comparison           comparison;
    public float                value;
    public bool                 percentageCompare;

    public DataType GetDataType()
    {
        if (valueType == ValueType.Probe) return DataType.Boolean;
        if (valueType == ValueType.IsGrounded) return DataType.Boolean;
        if (valueType == ValueType.IsGliding) return DataType.Boolean;
        else return DataType.Number;
    }

    public Variable GetVariable()
    {
        if (variable) return variable;

        if (valueHandler)
        {
            return valueHandler.GetVariable();
        }
        return null;
    }

    public string GetVariableName(GameObject gameObject)
    {
        if (variable) return variable.name;
        if (valueHandler) return valueHandler.name;
        switch (valueType)
        {
            case ValueType.TagCount:
                if (tag) return $"TagCount({tag.name})";
                return "TagCount([Unknown])";
            case ValueType.WorldPositionX:
                if (sourceTransform) return $"{sourceTransform.name}.x";
                return $"{gameObject.name}.x";
            case ValueType.WorldPositionY:
                if (sourceTransform) return $"{sourceTransform.name}.y";
                return $"{gameObject.name}.y";
            case ValueType.RelativePositionX:
                if (sourceTransform) return $"{sourceTransform.name}.rx";
                return $"{gameObject.name}.rx";
            case ValueType.RelativePositionY:
                if (sourceTransform) return $"{sourceTransform.name}.ry";
                return $"{gameObject.name}.ry";
            case ValueType.AbsoluteVelocityX:
                if (rigidBody) return $"{rigidBody.name}.velocity.x";
                return $"{gameObject.name}.velocity.x";
            case ValueType.AbsoluteVelocityY:
                if (rigidBody) return $"{rigidBody.name}.velocity.y";
                return $"{gameObject.name}.velocity.y";
            case ValueType.Distance:
                if (tag != null) return $"DistanceTo(Tag[{tag.name}])";
                else if (sourceTransform) return $"DistanceTo({sourceTransform.name})";
                return $"DistanceTo([UNDEFINED])";
            case ValueType.Angle:
                if (tag != null) return $"AngleBetween(Tag[{tag.name}], {axis})";
                else if (sourceTransform) return $"AngleBetween({sourceTransform.name}, {axis})";
                return $"AngleBetween([UNDEFINED], {axis})";
            case ValueType.Probe:
                if (probe != null) return $"ProbeIntersect({probe.name},{probe.GetTags()})";
                return $"ProbeIntersect([UNDEFINED])";
            case ValueType.ProbeDistance:
                if (probe != null) return $"ProbeIntersectionDistance({probe.name}, {probe.GetTags()})";
                return $"ProbeIntersectionDistance([UNDEFINED])";
            case ValueType.IsGrounded:
                if (movementPlatformer != null) return $"IsGrounded({movementPlatformer.name})";
                return $"IsGrounded([UNDEFINED])";
            case ValueType.IsGliding:
                if (movementPlatformer != null) return $"IsGliding({movementPlatformer.name})";
                return $"IsGliding([UNDEFINED])";
        }

        return "[Unknown]";
    }

    public string GetRawDescription(GameObject gameObject)
    {
        string desc = "(";
        if (negate) desc += "not ";
        desc += $"({GetVariableName(gameObject)}";
        if (GetDataType() == DataType.Number)
        {
            switch (comparison)
            {
                case Comparison.Equal: desc += " == "; break;
                case Comparison.Less: desc += " < "; break;
                case Comparison.LessEqual: desc += " <= "; break;
                case Comparison.Greater: desc += " > "; break;
                case Comparison.GreaterEqual: desc += " >= "; break;
                case Comparison.Different: desc += " <> "; break;
                default:
                    break;
            }
            desc += value;
            if (percentageCompare) desc += "%";
            desc += ")";
        }

        return desc;
    }

    public bool Evaluate(GameObject gameObject)
    {
        bool b = false;

        if (GetDataType() == DataType.Boolean)
        {
            switch (valueType)
            {
                case ValueType.Probe:
                    if (probe)
                    {
                        b = probe.GetIntersectionState();
                    }
                    break;
                case Condition.ValueType.IsGrounded:
                    if (movementPlatformer)
                    {
                        b = movementPlatformer.isGrounded;
                    }
                    break;
                case Condition.ValueType.IsGliding:
                    if (movementPlatformer)
                    {
                        b = movementPlatformer.isGliding;
                    }
                    break;
                default:
                    break;
            }
        }
        else
        {
            var currentVar = GetVariable();

            float currentValue = 0.0f;
            float minValue = 0;
            float maxValue = 0;
            Transform t;
            Rigidbody2D rb;
            if (currentVar == null)
            {
                switch (valueType)
                {
                    case Condition.ValueType.TagCount:
                        currentValue = HypertaggedObject.FindGameObjectsWithHypertag(tag).Count;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.WorldPositionX:
                        t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                        currentValue = t.position.x;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.WorldPositionY:
                        t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                        currentValue = t.position.y;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.RelativePositionX:
                        t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                        currentValue = t.localPosition.x;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.RelativePositionY:
                        t = (sourceTransform) ? (sourceTransform) : (gameObject.transform);
                        currentValue = t.localPosition.y;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.AbsoluteVelocityX:
                        rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                        if (rb) currentValue = rb.velocity.x;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.AbsoluteVelocityY:
                        rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                        if (rb) currentValue = rb.velocity.y;
                        minValue = 0;
                        maxValue = float.MaxValue;
                        break;
                    case Condition.ValueType.Distance:
                    case Condition.ValueType.Angle:
                        {
                            Transform target = null;

                            currentValue = float.MaxValue;

                            if (sourceTransform) target = sourceTransform;
                            else if (tag)
                            {
                                var potentialObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(tag);
                                foreach (var obj in potentialObjects)
                                {
                                    var d = Vector3.Distance(obj.position, gameObject.transform.position);
                                    if (d < currentValue)
                                    {
                                        currentValue = d;
                                        target = obj;
                                    }
                                }
                            }
                            if (valueType == ValueType.Distance)
                            {
                                if (target)
                                {
                                    currentValue = Vector3.Distance(gameObject.transform.position, target.position);
                                }
                            }
                            else if (valueType == ValueType.Angle)
                            {
                                if (target)
                                {
                                    Vector3 toObject = target.position - gameObject.transform.position;
                                    if (toObject.sqrMagnitude > 1e-6)
                                    {
                                        toObject.Normalize();

                                        Vector3 mainAxis;
                                        if (axis == Axis.UpAxis) mainAxis = gameObject.transform.up;
                                        else if (axis == Axis.RightAxis) mainAxis = gameObject.transform.right;
                                        else mainAxis = gameObject.transform.up;

                                        float dp = Mathf.Clamp(Vector3.Dot(toObject, mainAxis), -1.0f, 1.0f);
                                        currentValue = Mathf.Acos(dp);
                                        currentValue *= Mathf.Rad2Deg;
                                    }
                                }
                            }
                            minValue = 0;
                            maxValue = float.MaxValue;
                        }
                        break;
                    case Condition.ValueType.ProbeDistance:
                        if (probe == null)
                        {
                            currentValue = float.MaxValue;
                            minValue = 0;
                            maxValue = float.MaxValue;
                        }
                        else
                        {
                            currentValue = probe.GetIntersectionDistance();
                            minValue = probe.GetMinDistance();
                            maxValue = probe.GetMaxDistance();
                        }
                        break;
                    default:
                        return false;
                }
            }
            else
            {
                currentValue = currentVar.currentValue;
                minValue = currentVar.minValue;
                maxValue = currentVar.maxValue;
            }

            if (percentageCompare)
            {
                currentValue = 100 * (currentValue - minValue) / (maxValue - minValue);
            }

            b = false;
            switch (comparison)
            {
                case Condition.Comparison.Equal:
                    b = (currentValue == value);
                    break;
                case Condition.Comparison.Less:
                    b = (currentValue < value);
                    break;
                case Condition.Comparison.LessEqual:
                    b = (currentValue <= value);
                    break;
                case Condition.Comparison.Greater:
                    b = (currentValue > value);
                    break;
                case Condition.Comparison.GreaterEqual:
                    b = (currentValue >= value);
                    break;
                case Condition.Comparison.Different:
                    b = (currentValue != value);
                    break;
                default:
                    break;
            }
        }

        if (negate) b = !b;

        return b;
    }
}