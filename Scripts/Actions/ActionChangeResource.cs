using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Resource")]
    public class ActionChangeResource : Action
    {
        [SerializeField] private TargetResource resource;
        [SerializeField] private OperationType  operation;
        [SerializeField] private OkapiValue     changeValue;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            var resToChange = resource.GetTarget(gameObject);
            if (resToChange == null)
            {
                Debug.LogWarning($"Can't find resource {resource.GetRawDescription("resource", gameObject)}!");
                return;
            }

            var value = changeValue.GetFloat(gameObject);

            switch (operation)
            {
                case OperationType.Reset:
                    resToChange.ResetResource();
                    break;
                case OperationType.Set:
                    resToChange.SetResource(value);
                    break;
                case OperationType.Add:
                    resToChange.Change(Resource.ChangeType.Burst, value, transform.position, transform.right, gameObject, true);
                    break;
                case OperationType.Subtract:
                    resToChange.Change(Resource.ChangeType.Burst, -value, transform.position, transform.right, gameObject, true);
                    break;
                case OperationType.RevSubtract:
                    resToChange.SetResource(value - resToChange.resource);
                    break;
                case OperationType.Multiply:
                    resToChange.SetResource(resToChange.resource * value);
                    break;
                case OperationType.Divide:
                    resToChange.SetResource(resToChange.resource / value);
                    break;
                case OperationType.RevDivide:
                    resToChange.SetResource(value / resToChange.resource);
                    break;
                default:
                    break;
            }
        }

        string GetTargetName()
        {
            return $"{resource.GetShortDescription(gameObject)}";
        }

        public override string GetActionTitle()
        {
            string title = "Change Value";
            switch (operation)
            {
                case OperationType.Reset:
                    return $"Reset {GetTargetName()}";
                case OperationType.Set:
                    return $"{GetTargetName()} = {changeValue.GetDescription()}";
                case OperationType.Add:
                    return $"{GetTargetName()} = {GetTargetName()} + {changeValue.GetDescription()}";
                case OperationType.Subtract:
                    return $"{GetTargetName()} = {GetTargetName()} - {changeValue.GetDescription()}";
                case OperationType.RevSubtract:
                    return $"{GetTargetName()} = {changeValue.GetDescription()} - {GetTargetName()}";
                case OperationType.Multiply:
                    return $"{GetTargetName()} = {GetTargetName()} * {changeValue.GetDescription()}";
                case OperationType.Divide:
                    return $"{GetTargetName()} = {GetTargetName()} / {changeValue.GetDescription()}";
                case OperationType.RevDivide:
                    return $"{GetTargetName()} = {changeValue.GetDescription()} / {GetTargetName()}";
                default:
                    break;
            }
            return title;
        }

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string n = GetTargetName();

            string desc = GetPreconditionsString(gameObject);

            switch (operation)
            {
                case OperationType.Reset:
                    desc += $"resets resource {n}";
                    break;
                case OperationType.Set:
                    desc += $"sets resource {n} to {changeValue.GetDescription()}";
                    break;
                case OperationType.Add:
                    desc += $"adds {changeValue.GetDescription()} to resource {n}";
                    break;
                case OperationType.Subtract:
                    desc += $"subtracts {changeValue.GetDescription()} from resource {n}";
                    break;
                case OperationType.RevSubtract:
                    desc += $"subtracts resource {n} from {changeValue.GetDescription()}";
                    break;
                case OperationType.Multiply:
                    desc += $"multiplies resource {n} by {changeValue.GetDescription()}";
                    break;
                case OperationType.Divide:
                    desc += $"divides resource {n} by {changeValue.GetDescription()}";
                    break;
                case OperationType.RevDivide:
                    desc += $"divides resource {changeValue.GetDescription()} by {n}";
                    break;
                default:
                    break;
            }

            return desc;
        }

        protected override void CheckErrors(int level)
        {
            base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            resource.CheckErrors(_logs, "resource", gameObject);
            changeValue.CheckErrors(_logs, "value", gameObject);
        }
    }
}
