using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Helpers/Order 2d")]
    public class Order2d : OkapiElement
    {
        [SerializeField]
        private float       offsetZ = 0.0f;

        SpriteRenderer  spriteRenderer;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = "Sorts the objects using the Y coordinate.\n";
            desc += "This is useful for games that are midway between side-view and top-down.\n";
            desc += "Most is configured on the Okapi Config object, because it has to match between objects.\n";
            if ((OkapiConfig.orderMode == OrderMode.Z) && (offsetZ != 0.0f))
            {
                if (offsetZ > 0.0f) desc += $"The object will be pushed away from the camera {offsetZ} units.";
                else desc += $"The object will be pushed towards the camera {offsetZ} units.";
            }
            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponent<SpriteRenderer>();
        }

        void LateUpdate()
        {
            if (OkapiConfig.orderMode == OrderMode.Z)
            {
                var pos = transform.position;
                pos.z = Mathf.Clamp(OkapiConfig.orderScaleY * pos.y + offsetZ, OkapiConfig.orderMinZ, OkapiConfig.orderMaxZ);
                transform.position = pos;
            }
            else if ((OkapiConfig.orderMode == OrderMode.Order) && (spriteRenderer))
            {
                spriteRenderer.sortingOrder = (int)Mathf.Clamp(OkapiConfig.orderScaleY * transform.position.y, OkapiConfig.orderMin, OkapiConfig.orderMax);
            }
        }
    }
}
