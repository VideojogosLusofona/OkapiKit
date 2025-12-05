using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Camera Follow")]
    public class CameraFollow2d : OkapiElement
    {
        public enum Mode { SimpleFeedbackLoop = 0, CameraTrap = 1, ExponentialDecay = 2 };
        public enum TagMode { Closest = 0, Furthest = 1, Average = 2 };

        [SerializeField] Mode mode = Mode.SimpleFeedbackLoop;
        [SerializeField] Hypertag targetTag;
        [SerializeField] TagMode tagMode = TagMode.Closest;
        [SerializeField] bool allowZoom;
        [SerializeField] float zoomMargin = 1.1f;
        [SerializeField] Vector2 minMaxSize = new Vector2(180.0f, 360.0f);
        [SerializeField] Transform targetObject;
        [SerializeField] float followSpeed = 0.9f;
        [SerializeField] Rect rect = new Rect(-100.0f, -100.0f, 200.0f, 200.0f);
        [SerializeField] BoxCollider2D cameraLimits;

        private new Camera camera;
        private Bounds allObjectsBound;
        private List<Transform> potentialTransforms = new();

        protected override string Internal_UpdateExplanation()
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
                switch (tagMode)
                {
                    case TagMode.Closest:
                        _explanation += $"Camera follows the closest object tagged with [{targetTag.name}].\n";
                        break;
                    case TagMode.Furthest:
                        _explanation += $"Camera follows the furthest object tagged with [{targetTag.name}].\n";
                        break;
                    case TagMode.Average:
                        _explanation += $"Camera follows the average position of all objects tagged with [{targetTag.name}].\n";
                        if (allowZoom)
                        {
                            _explanation += $"Camera can zoom in to a minimum height of {minMaxSize.x} pixels, and a maximum of {minMaxSize.y},\nwith a margin of {(zoomMargin - 1.0f) * 100}% around it.\n";
                        }
                        break;
                    default:
                        break;
                }
            }
            if (mode == Mode.SimpleFeedbackLoop)
            {
                _explanation += $"It will trail the target, closing {100 * followSpeed}% of the distance each frame.\n";
            }
            else if (mode == Mode.CameraTrap)
            {
                _explanation += $"It will box the target within the given rect.\n";
            }
            else if (mode == Mode.ExponentialDecay)
            {
                _explanation += $"It will trail the target, closing {100 * followSpeed}% of the distance each second.\n";
            }

            if (cameraLimits)
            {
                _explanation += $"The camera won't leave the area defined by {cameraLimits.name}.";
            }

            return _explanation;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            var camera = GetComponent<Camera>();
            if ((camera == null) || (!camera.orthographic))
            {
                if ((tag != null) && (tagMode == TagMode.Average) && (allowZoom))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "A ortographic camera component needs to be on the same object as this system!", "For the follow to work properly, we need to reference an ortographic cameras."));
                }
            }
            if ((targetTag == null) && (targetObject == null))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No follow target is defined!", "If we're following an object, we need to set either a reference to the object, or the tags of objects to find and follow."));
            }
            if (cameraLimits == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, "Camera limits are not set!", "This is not mandatory, but it's common to have some sort of limits on the camera, so that it can't fall outside of the scope of the scene.\nCreate a box collider and reference it under camera limits."));
            }
        }

        void Start()
        {
            camera = GetComponent<Camera>();

            if (mode == Mode.CameraTrap)
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
                case Mode.CameraTrap:
                    FixedUpdate_Box();
                    break;
                case Mode.ExponentialDecay:
                    FixedUpdate_ExponentialDecay();
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

            RunZoom();
            CheckBounds();
        }
        void FixedUpdate_ExponentialDecay()
        {
            // Nice explanation of this: https://www.youtube.com/watch?v=LSNQuFEDOyQ&ab_channel=FreyaHolm%C3%A9r
            Vector3 targetPos = GetTargetPos();

            Vector3 newPos = targetPos + (transform.position - targetPos) * Mathf.Pow((1.0f - followSpeed), Time.fixedDeltaTime);
            newPos.z = transform.position.z;

            transform.position = newPos;

            RunZoom();
            CheckBounds();
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

            RunZoom();
            CheckBounds();
        }

        void RunZoom()
        {
            if ((targetTag != null) && (tagMode == TagMode.Average) && (allowZoom))
            {
                float height1 = Mathf.Clamp(allObjectsBound.extents.y * zoomMargin, minMaxSize.x, minMaxSize.y);
                float height2 = Mathf.Clamp(allObjectsBound.extents.x * zoomMargin, camera.aspect * minMaxSize.x, camera.aspect * minMaxSize.y) / camera.aspect;

                float height = Mathf.Max(height1, height2);
                camera.orthographicSize = height;
            }
        }

        void CheckBounds()
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
                Vector3 selectedPosition = transform.position;

                potentialTransforms.Clear();
                gameObject.FindObjectsOfTypeWithHypertag(targetTag, potentialTransforms);
                if (tagMode == TagMode.Closest)
                {
                    var minDist = float.MaxValue;
                    foreach (var obj in potentialTransforms)
                    {
                        var d = Vector3.Distance(obj.position, transform.position);
                        if (d < minDist)
                        {
                            minDist = d;
                            selectedPosition = obj.position;
                        }
                    }
                }
                else if (tagMode == TagMode.Furthest)
                {
                    var maxDist = 0.0f;
                    foreach (var obj in potentialTransforms)
                    {
                        var d = Vector3.Distance(obj.position, transform.position);
                        if (d > maxDist)
                        {
                            maxDist = d;
                            selectedPosition = obj.position;
                        }
                    }
                }
                else if (tagMode == TagMode.Average)
                {
                    if (potentialTransforms.Count > 0)
                    {
                        allObjectsBound = new Bounds(potentialTransforms[0].position, Vector3.zero);
                        selectedPosition = Vector3.zero;
                        foreach (var obj in potentialTransforms)
                        {
                            var d = Vector3.Distance(obj.position, transform.position);
                            selectedPosition += obj.position;
                            allObjectsBound.Encapsulate(obj.position);
                        }
                        selectedPosition /= potentialTransforms.Count;
                    }
                }

                return selectedPosition;
            }

            return transform.position;
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(GetTargetPos(), 0.5f);

            if (mode == Mode.CameraTrap)
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