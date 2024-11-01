using Codice.CM.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;

namespace OkapiKit
{
    [RequireComponent(typeof(GridObject))]
    public abstract class MovementGrid : Movement
    {
        [SerializeField]
        protected float cooldown = 0.0f;
        [SerializeField]
        protected float pushStrength = 0.0f;

        private GridSystem _grid;
        private GridObject _gridObject;
        
        protected float moveCooldownTimer = 0.0f;
        protected Vector3 currentVelocity = Vector3.zero;

        protected GridSystem grid
        {
            get
            {
                if (_gridObject == null) _gridObject = gridObject;                

                return _gridObject?.grid ?? null;
            }
        }

        protected GridObject gridObject
        {
            get
            {
                if (_gridObject == null) _gridObject = GetComponent<GridObject>();

                return _gridObject;
            }
        }

        protected Vector2 gridSize => grid?.cellSize ?? Vector2.one;


        protected override void Awake()
        {
            base.Awake();

            _grid = GetComponentInParent<GridSystem>();
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            GridSystem grid = GetComponentInParent<GridSystem>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid system object in parent", "Grid components need to be a child of an object with a grid system object!"));
            }
            GridObject gridObject = GetComponentInParent<GridObject>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid object object", "Grid components need to be also part of an object with a Grid Object component!"));
            }
        }

        protected virtual void Update()
        {
            if ((cooldown > 0.0f) || (moveCooldownTimer > 0.0f))
            {
                moveCooldownTimer -= Time.deltaTime;
                if (moveCooldownTimer < 0.0f) moveCooldownTimer = 0.0f;
            }
        }

        protected void MoveInDirection(Vector2 moveVector, bool relativeAxis)
        {
            if (moveVector != Vector2.zero)
            {
                Vector2Int gridPos = grid.WorldToGrid(transform.position);

                Vector2 posInc = Vector2.zero;
                if (moveVector.x < 0) posInc.x -= 1.0f;
                if (moveVector.x > 0) posInc.x += 1.0f;
                if (moveVector.y < 0) posInc.y -= 1.0f;
                if (moveVector.y > 0) posInc.y += 1.0f;

                if (posInc.x != 0.0f)
                {
                    posInc.y = 0.0f;
                }

                if (relativeAxis)
                {
                    posInc = transform.TransformDirection(posInc);

                    if (posInc.x < -1e-3) posInc.x = -1.0f;
                    else if (posInc.x > 1e-3) posInc.x = 1.0f;
                    else posInc.x = 0.0f;

                    if (posInc.x == 0.0f)
                    {
                        if (posInc.y < -1e-3) posInc.y = -1.0f;
                        else if (posInc.y > 1e-3) posInc.y = 1.0f;
                        else posInc.y = 0.0f;
                    }
                    else posInc.y = 0.0f;
                }

                Vector2Int nextGridPos = gridPos;
                nextGridPos.x = (int)(nextGridPos.x + posInc.x);
                nextGridPos.y = (int)(nextGridPos.y + posInc.y);

                currentVelocity = posInc * GetSpeed();

                if (gridObject.MoveToGrid(nextGridPos, GetSpeed(), pushStrength))
                {
                    moveCooldownTimer = cooldown;
                }
            }
            else
            {
                // This code forces a direction after the movement
                if (currentVelocity.magnitude > 1e-3f)
                {
                    currentVelocity = currentVelocity.normalized * 0.5f;
                }
                else currentVelocity = Vector2.zero;
            }
        }
    }
}
