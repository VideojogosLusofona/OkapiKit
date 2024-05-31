using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [System.Serializable]
    public struct Condition
    {
        [System.Serializable]
        public enum ValueType
        {
            None = 0, TagCount = 1,
            WorldPositionX = 2, WorldPositionY = 3, RelativePositionX = 4, RelativePositionY = 5,
            VelocityX = 14, VelocityY = 15,
            AbsoluteVelocityX = 6, AbsoluteVelocityY = 7,
            Distance = 8, Angle = 9,
            Probe = 10, ProbeDistance = 11,
            IsGrounded = 12, IsGliding = 13,
        };
        [System.Serializable] public enum Comparison { Equal = 0, Less = 1, LessEqual = 2, Greater = 3, GreaterEqual = 4, Different = 5 };
        [System.Serializable] public enum Axis { UpAxis = 0, RightAxis = 1 };

        [System.Serializable] public enum DataType { Number = 0, Boolean = 1 };

        public bool                 negate;
        public VariableInstance     valueHandler;
        public Variable             variable;
        public ValueType            valueType;
        public Hypertag             tag;
        public bool                 tagCountRangeEnabled;
        public float                tagCountRange;
        public Transform            sourceTransform;
        public Rigidbody2D          rigidBody;
        public Axis                 axis;
        public Probe                probe;
        public MovementPlatformer   movementPlatformer;
        public Comparison           comparison;
        public float                value;
        public VariableInstance     comparisonValueHandler;
        public Variable             comparisonVariable;
        public bool                 percentageCompare;

        // Objects to minimize GC allocs
        static List<Transform> potentialTransforms = new();
        static List<GameObject> gameObjects = new();

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
                    if (tagCountRangeEnabled)
                    {
                        if (tag) return $"ObjectWithTagInRange({tag.name},{tagCountRange})";
                        return "ObjectsWithTagsInRange([Unknown],{tagCountRange})";
                    }
                    else
                    {
                        if (tag) return $"ObjectsWithTag({tag.name})";
                        return "ObjectsWithTag([Unknown])";
                    }
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
                case ValueType.VelocityX:
                    if (rigidBody) return $"{rigidBody.name}.velocity.x";
                    return $"{gameObject.name}.velocity.x";
                case ValueType.VelocityY:
                    if (rigidBody) return $"{rigidBody.name}.velocity.y";
                    return $"{gameObject.name}.velocity.y";
                case ValueType.AbsoluteVelocityX:
                    if (rigidBody) return $"Abs({rigidBody.name}.velocity.x)";
                    return $"Abs({gameObject.name}.velocity.x)";
                case ValueType.AbsoluteVelocityY:
                    if (rigidBody) return $"Abs({rigidBody.name}.velocity.y)";
                    return $"Abs({gameObject.name}.velocity.y)";
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
                    else if (tag != null) return $"ProbeIntersect(Tag({tag.name}))";
                    return $"ProbeIntersect([UNDEFINED])";
                case ValueType.ProbeDistance:
                    if (probe != null) return $"ProbeIntersectionDistance({probe.name}, {probe.GetTags()})";
                    else if (tag != null) return $"ProbeIntersect(Tag({tag.name}))";
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

        string GetComparisonValueString()
        {
            if (comparisonVariable) return comparisonVariable.name;
            if (comparisonValueHandler) return comparisonValueHandler.name;

            return $"{value}";
        }

        public string GetRawDescription(GameObject gameObject)
        {
            string desc = "";
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
                desc += GetComparisonValueString();
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
                        else
                        {
                            if (tag)
                            {
                                var probe = gameObject.FindObjectOfTypeWithHypertag<Probe>(tag);
                                if (probe)
                                {
                                    b = probe.GetIntersectionState();
                                }
                            }
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
                            {
                                gameObjects.Clear();
                                HypertaggedObject.FindGameObjectsWithHypertag(tag, gameObjects);
                                if (tagCountRangeEnabled)
                                {
                                    currentValue = 0;
                                    foreach (var obj in gameObjects)
                                    {
                                        if (Vector3.Distance(obj.transform.position, gameObject.transform.position) < tagCountRange)
                                        {
                                            currentValue++;
                                        }
                                    }
                                }
                                else
                                {
                                    currentValue = gameObjects.Count;
                                }
                                minValue = 0;
                                maxValue = gameObjects.Count;
                            }
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
                        case Condition.ValueType.VelocityX:
                            rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                            if (rb) currentValue = rb.velocity.x;
                            minValue = 0;
                            maxValue = float.MaxValue;
                            break;
                        case Condition.ValueType.VelocityY:
                            rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                            if (rb) currentValue = rb.velocity.y;
                            minValue = 0;
                            maxValue = float.MaxValue;
                            break;
                        case Condition.ValueType.AbsoluteVelocityX:
                            rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                            if (rb) currentValue = Mathf.Abs(rb.velocity.x);
                            minValue = 0;
                            maxValue = float.MaxValue;
                            break;
                        case Condition.ValueType.AbsoluteVelocityY:
                            rb = (rigidBody) ? (rigidBody) : (gameObject.GetComponent<Rigidbody2D>());
                            if (rb) currentValue = Mathf.Abs(rb.velocity.y);
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
                                    potentialTransforms.Clear();
                                    gameObject.FindObjectsOfTypeWithHypertag<Transform>(tag, potentialTransforms);
                                    foreach (var obj in potentialTransforms)
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
                            currentValue = float.MaxValue;
                            minValue = 0;
                            maxValue = float.MaxValue;

                            if (probe == null)
                            {
                                if (tag)
                                {
                                    var probe = gameObject.FindObjectOfTypeWithHypertag<Probe>(tag);
                                    if (probe)
                                    {
                                        currentValue = probe.GetIntersectionDistance();
                                        minValue = probe.GetMinDistance();
                                        maxValue = probe.GetMaxDistance();
                                    }
                                }
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

                float comparisonValue = value;

                if (comparisonValueHandler) comparisonValue = comparisonValueHandler.GetValue();
                else if (comparisonVariable) comparisonValue = comparisonVariable.currentValue;

                b = false;
                switch (comparison)
                {
                    case Condition.Comparison.Equal:
                        b = (currentValue == comparisonValue);
                        break;
                    case Condition.Comparison.Less:
                        b = (currentValue < comparisonValue);
                        break;
                    case Condition.Comparison.LessEqual:
                        b = (currentValue <= comparisonValue);
                        break;
                    case Condition.Comparison.Greater:
                        b = (currentValue > comparisonValue);
                        break;
                    case Condition.Comparison.GreaterEqual:
                        b = (currentValue >= comparisonValue);
                        break;
                    case Condition.Comparison.Different:
                        b = (currentValue != comparisonValue);
                        break;
                    default:
                        break;
                }
            }

            if (negate) b = !b;

            return b;
        }

        public void CheckErrors(GameObject go, List<LogEntry> errors)
        {
            if ((variable == null) && (valueHandler == null) && (valueType == ValueType.None))
            {
                errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined condition!", "The condition is empty, define the criteria."));
            }
            else if (valueType == ValueType.TagCount)
            {
                if (tag == null) errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined tag to count!", "We need to count tags for this condition to work, so we need to know what tags to count."));
            }
            else if ((valueType == ValueType.AbsoluteVelocityX) && (go != null))
            {
                if (rigidBody == null)
                {
                    if (go.GetComponent<Rigidbody2D>() == null)
                    {
                        errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined rigid body for absolute velocity X!", "To have a velocity, we need to have a rigid body, since it's there the velocity will be defined at."));
                    }
                    else
                    {
                        errors.Add(new LogEntry(LogEntry.Type.Warning, "Rigid body for absolute velocity X exists, but should be explicitly set!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                    }
                }
            }
            else if ((valueType == ValueType.AbsoluteVelocityY) && (go != null))
            {
                if (rigidBody == null)
                {
                    if (go.GetComponent<Rigidbody2D>() == null)
                    {
                        errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined rigid body for absolute velocity Y!", "To have a velocity, we need to have a rigid body, since it's there the velocity will be defined at."));
                    }
                    else
                    {
                        errors.Add(new LogEntry(LogEntry.Type.Warning, "Rigid body for absolute velocity Y exists, but should be explicitly set!", "Setting options explicitly is always better than letting the system find them, since it might have to guess our intentions."));
                    }
                }
            }
            else if (valueType == ValueType.Distance)
            {
                if ((tag == null) && (sourceTransform == null))
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined target for Distance value!", "We need to check the distance to an object, so we need to define which object"));
                }
            }
            else if (valueType == ValueType.Angle)
            {
                if ((tag == null) && (sourceTransform == null))
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined target for Angle value", "We need to check the rotation angle of an object, so we need to define which object"));
                }
            }
            else if (valueType == ValueType.Probe)
            {
                if ((probe == null) && (tag == null))
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined probe for line of sight condition", "We need to check the line of sight of a specific probe, so we need to define which one, either by direct reference or by tag!"));
                }
            }
            else if (valueType == ValueType.ProbeDistance)
            {
                if ((probe == null) && (tag == null))
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined probe for intersection distance condition", "We need to check the distance to the intersection of a specific probe, so we need to define which one, either by referencing the probe, or by using its tag!"));
                }
            }
            else if (valueType == ValueType.IsGrounded)
            {
                if (movementPlatformer == null)
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined platform movement target to check for ground", "We need to check if a specific character is jumping, so we need to know which character"));
                }
            }
            else if (valueType == ValueType.IsGliding)
            {
                if (movementPlatformer == null)
                {
                    errors.Add(new LogEntry(LogEntry.Type.Error, "Undefined platform movement target to check for gliding", "We need to check if a specific character is glidding, so we need to know which character"));
                }
            }
        }
    }
}