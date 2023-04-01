using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

namespace OkapiKit
{
    public class CameraFollow2d : OkapiElement
    {
        public enum Mode { SimpleFeedbackLoop = 0, Box = 1 };

        [SerializeField] Mode mode = Mode.SimpleFeedbackLoop;
        [SerializeField] Hypertag targetTag;
        [SerializeField] Transform targetObject;
        [SerializeField] float followSpeed = 0.9f;
        [SerializeField] Rect rect = new Rect(-100.0f, -100.0f, 200.0f, 200.0f);
        [SerializeField] BoxCollider2D cameraLimits;

        new Camera camera;

        public override string UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            if (targetTag == null)
            {
                if (targetObject == null)
                {
                    _explanation += "Camera follows [UNDEFINED] target.\n";
                }
                else
                {
                    _explanation += $"Camera follows [{targetObject.name}] target.\n";
                }
            }
            else
            {
                _explanation += $"Camera follows the closest object tagged with [{targetTag.name}].\n";
            }
            if (mode == Mode.SimpleFeedbackLoop)
            {
                _explanation += $"It will trail the target, closing {100 * followSpeed}% of the distance each frame.\n";
            }
            else
            {
                _explanation += $"It will box the target within the given rect.\n";
            }

            if (cameraLimits)
            {
                _explanation += $"The camera won't leave the area defined by {cameraLimits.name}.";
            }

            return _explanation;
        }

        void Start()
        {
            camera = GetComponent<Camera>();

            if (mode == Mode.Box)
            {
                float currentZ = transform.position.z;
                Vector3 targetPos = GetTargetPos();
                transform.position = new Vector3(targetPos.x, targetPos.y, currentZ);

                CheckBounds();
            }
        }

        void FixedUpdate()
        {
            switch (mode)
            {
                case Mode.SimpleFeedbackLoop:
                    FixedUpdate_SimpleFeedbackLoop();
                    break;
                case Mode.Box:
                    FixedUpdate_Box();
                    break;
            }
        }

        void FixedUpdate_SimpleFeedbackLoop()
        {
            float currentZ = transform.position.z;

            Vector3 err = GetTargetPos() - transform.position;

            Vector3 newPos = transform.position + err * followSpeed;
            newPos.z = currentZ;

            transform.position = newPos;
        }

        void FixedUpdate_Box()
        {
            float currentZ = transform.position.z;
            Vector3 targetPos = GetTargetPos();
            Vector2 delta = transform.position;
            Rect r = rect;
            r.position += delta;

            if (targetPos.x > r.xMax) r.position += new Vector2(targetPos.x - r.xMax, 0);
            if (targetPos.x < r.xMin) r.position += new Vector2(targetPos.x - r.xMin, 0);
            if (targetPos.y < r.yMin) r.position += new Vector2(0, targetPos.y - r.yMin);
            if (targetPos.y > r.yMax) r.position += new Vector2(0, targetPos.y - r.yMax);

            transform.position = new Vector3(r.center.x, r.center.y, currentZ);

            CheckBounds();
        }

        public void CheckBounds()
        {
            if (cameraLimits == null) return;

            Bounds r = cameraLimits.bounds;

            float halfHeight = camera.orthographicSize;
            float halfWidth = camera.aspect * halfHeight;

            float xMin = transform.position.x - halfWidth;
            float xMax = transform.position.x + halfWidth;
            float yMin = transform.position.y - halfHeight;
            float yMax = transform.position.y + halfHeight;

            Vector3 position = transform.position;

            if (xMin <= r.min.x) position.x = r.min.x + halfWidth;
            else if (xMax >= r.max.x) position.x = r.max.x - halfWidth;
            if (yMin <= r.min.y) position.y = r.min.y + halfHeight;
            else if (yMax >= r.max.y) position.y = r.max.y - halfHeight;

            transform.position = position;
        }

        public Vector3 GetTargetPos()
        {
            if (targetObject != null) return targetObject.transform.position;
            else if (targetTag)
            {
                Transform closest = null;
                var potentialObjects = gameObject.FindObjectsOfTypeWithHypertag<Transform>(targetTag);
                var minDist = float.MaxValue;
                foreach (var obj in potentialObjects)
                {
                    var d = Vector3.Distance(obj.position, transform.position);
                    if (d < minDist)
                    {
                        minDist = d;
                        closest = obj;
                    }
                }

                if (closest)
                {
                    return closest.position;
                }
            }

            return transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetTargetPos(), 0.5f);

            if (mode == Mode.Box)
            {
                Vector2 delta = transform.position;
                Rect r = rect;
                r.position += delta;

                Gizmos.color = Color.magenta;
                Gizmos.DrawLine(new Vector2(r.xMin, r.yMin), new Vector2(r.xMax, r.yMin));
                Gizmos.DrawLine(new Vector2(r.xMax, r.yMin), new Vector2(r.xMax, r.yMax));
                Gizmos.DrawLine(new Vector2(r.xMax, r.yMax), new Vector2(r.xMin, r.yMax));
                Gizmos.DrawLine(new Vector2(r.xMin, r.yMax), new Vector2(r.xMin, r.yMin));
            }

            if (cameraLimits)
            {
                Bounds r = cameraLimits.bounds;

                Gizmos.color = Color.green;
                Gizmos.DrawLine(new Vector2(r.min.x, r.min.y), new Vector2(r.max.x, r.min.y));
                Gizmos.DrawLine(new Vector2(r.max.x, r.min.y), new Vector2(r.max.x, r.max.y));
                Gizmos.DrawLine(new Vector2(r.max.x, r.max.y), new Vector2(r.min.x, r.max.y));
                Gizmos.DrawLine(new Vector2(r.min.x, r.max.y), new Vector2(r.min.x, r.min.y));
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) CameraFollow2d.GetRawDescription";
        }

    }
}