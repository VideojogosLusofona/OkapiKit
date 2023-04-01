using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionModifySystem : Action
    {
        public enum ChangeType { MouseCursorVisibility = 0 };
        public enum StateChange { Enable = 0, Disable = 1, Toggle = 2 };

        [SerializeField]
        private ChangeType changeType;
        [SerializeField]
        private StateChange state;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            switch (changeType)
            {
                case ChangeType.MouseCursorVisibility:
                    if (state == StateChange.Enable) Cursor.visible = true;
                    else if (state == StateChange.Disable) Cursor.visible = false;
                    else Cursor.visible = !Cursor.visible;
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Modify System";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            var desc = GetPreconditionsString(gameObject);

            switch (changeType)
            {
                case ChangeType.MouseCursorVisibility:
                    switch (state)
                    {
                        case StateChange.Enable:
                            desc += $"enables mouse cursor";
                            break;
                        case StateChange.Disable:
                            desc += $"disables mouse cursor";
                            break;
                        case StateChange.Toggle:
                            desc += $"toggles mouse cursor";
                            break;
                    }
                    break;
            }

            return desc;
        }
    }
}