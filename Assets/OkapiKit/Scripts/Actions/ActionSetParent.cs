using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class ActionSetParent : Action
    {
        public enum Target { None = 0, Object = 1, Tag = 2 };

        [SerializeField]
        private Target target = Target.Object;

        [SerializeField, ShowIf("needsTarget")]
        private GameObject targetObject;
        [SerializeField, ShowIf("needsTags")]
        new private Hypertag tag;

        private bool needsTarget => target == Target.Object;
        private bool needsTags => target == Target.Tag;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;

            switch (target)
            {
                case Target.None:
                    transform.SetParent(null);
                    break;
                case Target.Object:
                    if (targetObject) transform.SetParent(targetObject.transform);
                    else transform.SetParent(null);
                    break;
                case Target.Tag:
                    var obj = HypertaggedObject.FindGameObjectWithHypertag(tag);
                    if (obj) transform.SetParent(obj.transform);
                    else transform.SetParent(null);
                    break;
                default:
                    break;
            }
        }

        public override string GetActionTitle() => "Set Parent";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            switch (target)
            {
                case Target.None:
                    desc += "remove this object from parent";
                    break;
                case Target.Object:
                    if (targetObject) desc += $"sets this object as a child of {targetObject.name}";
                    else desc += "remove this object from parent";
                    break;
                case Target.Tag:
                    if (tag) desc += $"sets this object as a child of object with tag {tag.name}";
                    else desc += $"sets this object as a child of object with tag [Undefined]";
                    break;
                default:
                    break;
            }
            return desc;
        }
    }
}