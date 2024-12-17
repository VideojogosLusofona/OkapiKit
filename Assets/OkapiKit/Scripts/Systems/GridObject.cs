using Codice.CM.Common.Tree.Partial;
using System;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Grid Object")]
    public class GridObject : OkapiElement
    {
        [SerializeField] private Pivot          _pivot = Pivot.Center;
        [SerializeField] private bool           _canPush;
        [SerializeField] private float          _mass = 1.0f;
        [SerializeField] private GridObject[]   ignoreCollisionObjects;
        [SerializeField] private Hypertag[]     ignoreCollisionTags; 
        [SerializeField] private Vector2        _size = Vector2.zero;
        [SerializeField]
        protected AnimationCurve stepAnimationCurve;

        private GridSystem _grid;
        private Vector2 originalPos;
        private Vector2? targetPos = null;
        protected float moveElapsedTime;
        protected float moveTotalDistance;
        protected float moveTotalTime;
        protected Vector2 moveTotalDelta;
        protected Vector2 moveDirection;
        protected Vector2 moveSpeed;

        protected float originalAngle;
        protected float? targetAngle;
        protected float rotateElapsedTime = 0.0f;
        protected float rotateTotalAngle;
        protected float rotateTotalTime;

        protected Rigidbody2D rb;
        public Vector2 lastDelta { get; private set; }
        public float lastAngularDelta { get; private set; }

        public GridSystem grid
        {
            get
            {
                if (_grid == null) _grid = GetComponentInParent<GridSystem>();

                return _grid;
            }
        }

        public Pivot pivot => _pivot;
        public bool canPush => _canPush;
        public float mass => _mass;
        public bool isMoving => targetPos.HasValue;
        public bool isRotating => targetAngle.HasValue;
        public Vector2 size => _size;

        List<GridObject> exclusionList;

        private void Start()
        {
            _grid = GetComponentInParent<GridSystem>();
            if (_grid == null) _grid = FindAnyObjectByType<GridSystem>();
            rb = GetComponent<Rigidbody2D>();
            exclusionList = new()
            {
                this
            };
            if (ignoreCollisionObjects != null)
            {
                foreach (var obj in ignoreCollisionObjects)
                {
                    if (obj != null)
                    {
                        exclusionList.Add(obj);
                        var children = obj.GetComponentsInChildren<GridObject>();
                        foreach (var c in children)
                        {
                            exclusionList.Add(c);
                        }
                    }
                }
            }

            transform.position = grid.Snap(transform.position, _pivot);

            grid?.Register(this);
        }

        protected virtual void FixedUpdate()
        {
            if (targetPos.HasValue)
            {
                moveElapsedTime += Time.fixedDeltaTime;

                float currentT = moveElapsedTime / moveTotalTime;
                float animT = currentT;
                if ((stepAnimationCurve != null) && (stepAnimationCurve.length > 0))
                {
                    animT = stepAnimationCurve.Evaluate(animT);
                }

                Vector3 prevPos = transform.position;
                Vector3 newPos;
                if (currentT >= 1.0f)
                {
                    newPos = targetPos.Value;
                    targetPos = null;
                    ThrowEvent(TriggerOnGridEvent.GridEvent.StepEnd, null);
                }
                else
                {
                    newPos = originalPos + moveTotalDelta * animT;
                }

                if (rb != null)
                {
                    rb.MovePosition(newPos);
                }
                else
                {
                    transform.position = newPos;
                }
                lastDelta = newPos - prevPos;
            }

            if (targetAngle.HasValue)
            {
                rotateElapsedTime += Time.fixedDeltaTime;

                float currentT = rotateElapsedTime / rotateTotalTime;
                float animT = currentT;
                if ((stepAnimationCurve != null) && (stepAnimationCurve.length > 0))
                {
                    animT = stepAnimationCurve.Evaluate(animT);
                }

                float prevAngle = transform.rotation.eulerAngles.z;
                float newAngle;
                if (currentT >= 1.0f)
                {
                    newAngle = targetAngle.Value;
                    targetAngle = null;
                    ThrowEvent(TriggerOnGridEvent.GridEvent.RotateEnd, null);
                }
                else
                {
                    newAngle = originalAngle + rotateTotalAngle * animT;
                }

                if (rb != null)
                {
                    rb.MoveRotation(newAngle);
                }
                else
                {
                    transform.rotation = Quaternion.Euler(0.0f, 0.0f, newAngle);
                }
                lastAngularDelta = newAngle - prevAngle;
            }
        }

        private void OnDestroy()
        {
            grid?.Unregister(this);
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = ident;

            desc += $"Object is centered around the {pivot} of the visual representation.\n";
            if (canPush)
            {
                desc += $"This object can be pushed by other objects, as long as their strength is larger or equal to {mass}.\n";
                if ((_size.x > 0.0f) && (_size.y > 0.0f))
                {
                    desc += $"This object will be treated for pushing purposes as a box of size {_size}.\n";
                }
                else
                {
                    desc += $"This object will be treated for pushing purposes as a point.\n";
                }
            }
            else
            {
                desc += $"This object can not be pushed by other objects.\n";
            }
            if ((ignoreCollisionObjects != null) && (ignoreCollisionObjects.Length > 0))
            {
                desc += "This object will ignore collision with objects ";
                for (int i = 0; i < ignoreCollisionObjects.Length; i++)
                {
                    if (ignoreCollisionObjects[i] == null) desc += "NULL";
                    else desc += ignoreCollisionObjects[i].name;
                    if (i < ignoreCollisionObjects.Length - 1) desc += ",";
                }
                desc += ".\n";
            }
            if ((ignoreCollisionTags != null) && (ignoreCollisionTags.Length > 0))
            {
                desc += "This object will ignore collision with tags [";

                for (int i = 0; i < ignoreCollisionTags.Length; i++)
                {
                    if (ignoreCollisionTags[i] == null) desc += "NULL";
                    else desc += ignoreCollisionTags[i].name;
                    if (i < ignoreCollisionTags.Length - 1) desc += ",";
                }
                desc += "].\n";
            }
            if ((stepAnimationCurve != null) && (stepAnimationCurve.length > 0))
            {
                desc += "The character will use an animation curve while moving.\n";
            }


            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            GridSystem grid = GetComponentInParent<GridSystem>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid system in parent", "Grid objects need to be a child of an object with a grid system component!"));
            }
        }

        public bool MoveToGrid(Vector2Int gridPos, Vector2 speed, float pushStrength)
        {
            var currentExclusionList = exclusionList;
            if ((ignoreCollisionTags != null) && (ignoreCollisionTags.Length > 0))
            {
                currentExclusionList = new(exclusionList);

                var objs = HypertaggedObject.FindObjectsByHypertag<GridObject>(ignoreCollisionTags);
                foreach (var obj in objs)
                {
                    currentExclusionList.Add(obj);
                    var children = obj.GetComponentsInChildren<GridObject>();
                    foreach (var c in children)
                    {
                        exclusionList.Add(c);
                    }
                }
            }

            // Check if object can move to this position
            var targetWorldPos = grid.GridToWorld(gridPos, pivot);
            var originalGridPos = grid.WorldToGrid(transform.position);
            var obstacleQuery = grid.IsObstacle(targetWorldPos, _size, gameObject.layer, currentExclusionList);
            if (obstacleQuery.hasObstacle)
            {
                // Is it a wall? If it is, just give up
                if (obstacleQuery.obj == null)
                {
                    ThrowEvent(TriggerOnGridEvent.GridEvent.HitWall, null);
                    return false;
                }

                // It's an object - Can we push it?
                if (pushStrength <= 0.0f)
                {
                    ThrowEvent(TriggerOnGridEvent.GridEvent.HitObject, obstacleQuery.obj);
                    return false;
                }

                // Is it pushable? 
                if (!obstacleQuery.obj.canPush)
                {
                    ThrowEvent(TriggerOnGridEvent.GridEvent.HitObject, obstacleQuery.obj);
                    return false;
                }

                // Do we have enough strength to push it?
                if (pushStrength < obstacleQuery.obj.mass)
                {
                    ThrowEvent(TriggerOnGridEvent.GridEvent.HitObject, obstacleQuery.obj);
                    return false;
                }

                // We can move the object, so now we move it by using MoveToGrid on him, but
                // we have to receive the result (for example, there might be a wall stopping the
                // movement, or not enough strength to push two objects)
                Vector2Int delta = gridPos - originalGridPos;
                if (!obstacleQuery.obj.MoveToGrid(gridPos + delta, speed, pushStrength - obstacleQuery.obj.mass))
                {
                    ThrowEvent(TriggerOnGridEvent.GridEvent.HitObject, obstacleQuery.obj);
                    return false;
                }

                ThrowEvent(TriggerOnGridEvent.GridEvent.PushObject, obstacleQuery.obj);
            }

            originalPos = grid.GridToWorld(originalGridPos, pivot);
            targetPos = targetWorldPos;
            moveElapsedTime = 0.0f;
            moveTotalDelta = (targetPos.Value - originalPos);
            moveTotalDistance = moveTotalDelta.magnitude;
            moveDirection = moveTotalDelta / moveTotalDistance;
            moveSpeed = speed;
            moveTotalTime = moveTotalDistance / moveSpeed.magnitude;

            return true;
        }

        internal void TeleportTo(Vector2 target)
        {
            originalPos = transform.position;
            transform.position = grid.Snap(target, pivot);
            targetPos = null;
            moveElapsedTime = 0.0f;
            moveTotalDelta = new Vector2(transform.position.x, transform.position.y) - originalPos;
            moveTotalDistance = moveTotalDelta.magnitude;
            moveTotalTime = 0.0f;
            moveDirection = moveTotalDelta / moveTotalDistance;
        }

        bool EventNeedsCollider(TriggerOnGridEvent.GridEvent evt)
        {
            switch (evt)
            {
                case TriggerOnGridEvent.GridEvent.PushObject:
                case TriggerOnGridEvent.GridEvent.HitObject:
                case TriggerOnGridEvent.GridEvent.WasPushed:
                case TriggerOnGridEvent.GridEvent.WasHit:
                    return true;
            }

            return false;
        }

        void ThrowEvent(TriggerOnGridEvent.GridEvent evt, GridObject obj)
        {
            if (EventNeedsCollider(evt))
            {
                TriggerOnCollision.PushLastCollider(obj.gameObject);
            }

            var allEventHandlers = GetComponentsInChildren<TriggerOnGridEvent>();
            foreach (var handler in allEventHandlers)
            {
                handler.ThrowEvent(evt, obj);
            }
            if (obj)
            {
                allEventHandlers = obj.GetComponentsInChildren<TriggerOnGridEvent>();
                foreach (var handler in allEventHandlers)
                {
                    handler.ThrowEvent(evt, this);
                }
            }

            if (EventNeedsCollider(evt))
            {
                TriggerOnCollision.PopLastCollider();

                if (evt == TriggerOnGridEvent.GridEvent.HitObject)
                {
                    obj.ThrowEvent(TriggerOnGridEvent.GridEvent.WasHit, this);
                }
                else if (evt == TriggerOnGridEvent.GridEvent.PushObject)
                {
                    obj.ThrowEvent(TriggerOnGridEvent.GridEvent.WasPushed, this);
                }
            }
        }

        private void OnDrawGizmosSelected()
        {
            if ((_size.x > 0) && (_size.y > 0))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, _size);
            }
        }

        public void Snap()
        {
            transform.position = grid.Snap(transform.position, _pivot);
        }

        public Vector3 Snap(Vector3 worldPos)
        {
            return grid.Snap(worldPos, pivot);
        }

        internal Vector3 GridToWorld(Vector2Int gridPos)
        {
            return grid.GridToWorld(gridPos, pivot);
        }

        internal Vector2Int WorldToGrid(Vector3 worldPos)
        {
            return grid.WorldToGrid(worldPos);
        }

        internal bool RotateOnGrid(float rotationSpeed, float angularStepSize)
        {
            if (Mathf.Abs(rotationSpeed) < 1e-3) return false;

            originalAngle = SnapAngle(transform.rotation.eulerAngles.z, angularStepSize);
            targetAngle = SnapAngle(originalAngle + Mathf.Sign(rotationSpeed) * angularStepSize, angularStepSize);
            rotateElapsedTime = 0.0f;
            rotateTotalAngle = Mathf.DeltaAngle(originalAngle, targetAngle.Value);
            rotateTotalTime = Mathf.Abs(rotateTotalAngle) / Mathf.Abs(rotationSpeed);

            return true;
        }

        float SnapAngle(float angle, float angularStepSize)
        {
            while (angle < 0) { angle += 360.0f; }
            while (angle > 360) { angle -= 360.0f; }

            angle = (Mathf.RoundToInt(angle / angularStepSize)) * angularStepSize;

            return angle;
        }

        internal bool IsOnTile(Vector3 worldPos, TileBase tile)
        {
            return grid.IsOnTile(worldPos, tile);
        }
        internal bool IsOnTile(Vector3 worldPos, TileSet tileSet)
        {
            return grid.IsOnTile(worldPos, tileSet);
        }

        internal void RunRules(Vector3 worldPos, TileConverterRule[] rules)
        {
            grid.RunRules(worldPos, rules);
        }
        internal void RunRules(Vector2Int gridPos, TileConverterRule[] rules)
        {
            var worldPos = grid.GridToWorld(gridPos, pivot);

            grid.RunRules(worldPos, rules);
        }
    }
}
