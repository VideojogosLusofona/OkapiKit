using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Helpers/Order 2d")]
    public class Order2d : OkapiElement
    {
        [SerializeField]
        private float       offsetZ = 0.0f;

        SpriteRenderer  spriteRenderer;
        OkapiConfig     okapiConfig;

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "";
        }

        protected override string Internal_UpdateExplanation()
        {
            return "";
        }

        protected override void Awake()
        {
            base.Awake();

            spriteRenderer = GetComponent<SpriteRenderer>();
            okapiConfig = OkapiConfig.instance;
        }

        void LateUpdate()
        {
            if (okapiConfig.orderMode == OrderMode.Z)
            {
                var pos = transform.position;
                pos.z = Mathf.Clamp(okapiConfig.orderScaleY * pos.y + offsetZ, okapiConfig.orderMinZ, okapiConfig.orderMaxZ);
                transform.position = pos;
            }
            else if ((okapiConfig.orderMode == OrderMode.Order) && (spriteRenderer))
            {
                spriteRenderer.sortingOrder = (int)Mathf.Clamp(okapiConfig.orderScaleY * transform.position.y, okapiConfig.orderMin, okapiConfig.orderMax);
            }
        }
    }
}
