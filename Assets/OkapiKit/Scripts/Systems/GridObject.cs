using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Grid Object")]
    public class GridObject : OkapiElement
    {
        [SerializeField] private Pivot      _pivot = Pivot.Center;
        [SerializeField] private bool       _canPush;
        [SerializeField] private float      _mass = 1.0f;
        [SerializeField] private Vector2    _size = Vector2.zero;
        [SerializeField]
        protected AnimationCurve stepAnimationCurve;

        private GridSystem  _grid;
        private Vector2     originalPos;
        private Vector2?    targetPos = null;
        protected float     moveElapsedTime;
        protected float     moveTotalDistance;
        protected float     moveTotalTime;
        protected Vector2   moveTotalDelta;
        protected Vector2   moveDirection;
        protected Vector2   moveSpeed;

        protected Rigidbody2D   rb;
        public Vector2 lastDelta { get; private set; }

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
        public Vector2 size => _size;

        List<GridObject> exclusionList;

        private void Start()
        {
            _grid = GetComponentInParent<GridSystem>();
            rb = GetComponent<Rigidbody2D>();
            exclusionList = new()
            {
                this
            };

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
                    ThrowEvent(TriggerOnGridEvent.GridEvent.Step, null);
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
            // Check if object can move to this position
            var targetWorldPos = grid.GridToWorld(gridPos, pivot);
            var originalGridPos = grid.WorldToGrid(transform.position);
            var obstacleQuery = grid.IsObstacle(targetWorldPos, _size, gameObject.layer, exclusionList);
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

        void ThrowEvent(TriggerOnGridEvent.GridEvent evt, GridObject obj)
        {
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
        }

        private void OnDrawGizmosSelected()
        {
            if ((_size.x > 0) && (_size.y > 0))
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireCube(transform.position, _size);
            }
        }

        public Vector3 Snap(Vector3 worldPos)
        {
            return grid.Snap(worldPos, pivot);
        }

        internal Vector3 GridToWorld(Vector2Int gridPos)
        {
            return grid.GridToWorld(gridPos, pivot);
        }
    }
}
