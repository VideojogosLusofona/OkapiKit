using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OkapiKit
{
    public enum Pivot { Center, TopLeft, TopRight, TopCenter, BottomLeft, BottomCenter, BottomRight };

    [AddComponentMenu("Okapi/Other/Grid System")]
    [RequireComponent(typeof(Grid))]
    public class GridSystem : OkapiElement
    {
        [SerializeField] private Collider2D[] gridcolliders;

        private Grid                    _grid;
        private Bounds                  bounds;
        private int                     mapSizeX, mapSizeY;
        private int                     stride;
        private Dictionary<int, byte[]> mapsByLayer = new();
        private List<GridObject>        gridObjects = new();
        private List<Tilemap>           tilemaps = new(); 
        private bool                    collidersDirty = false;

        private Grid grid
        {
            get
            {
                if (_grid == null) _grid = GetComponent<Grid>();

                return _grid;
            }
        }

        public Vector2 cellSize => grid?.cellSize ?? Vector2.one;

        public Vector2Int WorldToGrid(Vector2 pos)
        {
            Vector2Int ret = Vector2Int.zero;

            ret.x = Mathf.FloorToInt(pos.x / cellSize.x);
            ret.y = Mathf.FloorToInt(pos.y / cellSize.y);

            return ret;
        }

        public Vector2 GridToWorld(Vector2Int gridPos, Pivot pivot = Pivot.Center)
        {
            var offset = GetOffset(pivot);
            Vector2 ret = Vector2.zero;

            ret.x = (gridPos.x + offset.x) * cellSize.x;
            ret.y = (gridPos.y + offset.y) * cellSize.y;

            return ret;

        }

        public Vector3 Snap(Vector3 worldPos, Pivot pivot = Pivot.Center)
        {
            Vector2 offset = GetOffset(pivot);

            Vector3 ret = worldPos;
            ret.x = (Mathf.Floor(ret.x / cellSize.x) + offset.x) * cellSize.x;
            ret.y = (Mathf.Floor(ret.y / cellSize.y) + offset.y) * cellSize.y;
            return ret;
        }

        public Vector2 GetOffset(Pivot pivot)
        {
            Vector2 offset = Vector2.zero;
            switch (pivot)
            {
                case Pivot.Center: offset = Vector2.one * 0.5f; break;
                case Pivot.TopLeft: offset = new Vector2(0.0f, 1.0f); break;
                case Pivot.TopCenter: offset = new Vector2(0.5f, 1.0f); break;
                case Pivot.TopRight: offset = new Vector2(1.0f, 1.0f); break;
                case Pivot.BottomLeft: offset = new Vector2(0.0f, 0.0f); break;
                case Pivot.BottomCenter: offset = new Vector2(0.5f, 0.0f); break;
                case Pivot.BottomRight: offset = new Vector2(1.0f, 0.0f); break;
            }

            return offset;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            string desc = ident;

            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", gameObject);

            return _explanation;
        }

        protected override void CheckErrors(int level)
        {
              base.CheckErrors(level); if (level > Action.CheckErrorsMaxLevel) return;

            Grid grid = GetComponentInParent<Grid>();
            if (grid == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "No grid in object", "Grid system need to be on an object with a Grid component!"));
            }

            if ((gridcolliders != null) && (gridcolliders.Length > 0))
            {
                for (int i = 0; i < gridcolliders.Length; i++)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, $"Invalid grid collider at index {i}", "There's a collider entry, but the object is invalid!"));
                }
            }
        }

        protected void Start()
        {
            collidersDirty = true;

            tilemaps = new(GetComponentsInChildren<Tilemap>());
        }

        void UpdateColliders()
        {
            // Get total bounds
            bounds = new Bounds(Vector3.zero, Vector3.zero);

            if ((gridcolliders != null) && (gridcolliders.Length > 0))
            {
                bool init = false;

                for (int i = 0; i < gridcolliders.Length; i++)
                {
                    if (gridcolliders[i] == null) continue;

                    if (!init) { bounds = gridcolliders[i].bounds; init = true; }
                    else bounds.Encapsulate(gridcolliders[i].bounds);
                }
            }

            // Compute map size
            mapSizeX = Mathf.CeilToInt((bounds.size.x / grid.cellSize.x) / 8) * 8;
            stride = mapSizeX / 8;
            mapSizeY = Mathf.CeilToInt(bounds.size.y / grid.cellSize.y);

            mapsByLayer.Clear();

            // Compute collision masks
            if ((gridcolliders != null) && (gridcolliders.Length > 0))
            {
                foreach (var collider in gridcolliders)
                {
                    if (collider == null) continue;

                    var map = GetMapByLayer(collider.gameObject.layer);
                    int idx;

                    Vector2 worldPos = new Vector2(bounds.min.x + grid.cellSize.x * 0.5f, bounds.min.y + grid.cellSize.y * 0.5f);

                    for (int y = 0; y < mapSizeY; y++)
                    {
                        for (int x = 0; x < mapSizeX; x++)
                        {
                            if (collider.OverlapPoint(worldPos))
                            {
                                idx = y * stride + x / 8;
                                map[idx] |= (byte)(1 << (x % 8));

                                Debug.DrawLine(worldPos, worldPos + new Vector2(5.0f, 5.0f), Color.red, 5.0f);
                            }
                            worldPos.x += grid.cellSize.x;
                        }
                        worldPos.x = bounds.min.x + grid.cellSize.x * 0.5f;
                        worldPos.y += grid.cellSize.y;
                    }
                }
            }

            collidersDirty = false;
        }

        byte[] GetMapByLayer(int layer)
        {
            if (mapsByLayer.TryGetValue(layer, out byte[] ret))
            {
                return ret;
            }

            ret = new byte[stride * mapSizeY];
            mapsByLayer[layer] = ret;

            return ret;
        }

        public (bool hasObstacle, GridObject obj) IsObstacle(Vector2 worldPos, Vector2 size, int layer, List<GridObject> exclusionList)
        {
            if (collidersDirty)
            {
                UpdateColliders();
            }

            Vector2Int maskTilePos = GetGridPos(worldPos);
            int idx = maskTilePos.y * stride + maskTilePos.x / 8;

            var compatibleMasks = GetCollisionCompatibleMasks(layer).ToList();
            foreach (var mask in compatibleMasks)
            {
                if ((mask[idx] & (byte)(1 << (maskTilePos.x % 8))) != 0) return (true, null);
            }

            foreach (var obj in gridObjects)
            {
                if (exclusionList.Contains(obj)) continue;
                if (!Physics2D.GetIgnoreLayerCollision(layer, obj.gameObject.layer))
                {
                    if ((size.x > 0.0f) && (size.y > 0.0f) && (obj.size.x > 0.0f) && (obj.size.y > 0.0f))
                    {
                        Bounds b1 = new Bounds(worldPos, size);
                        Bounds b2 = new Bounds(obj.transform.position, obj.size);

                        if (!b1.Intersects(b2)) continue;
                    }
                    else
                    {
                        var obstaclePos = GetGridPos(obj.transform.position);
                        if (obstaclePos != maskTilePos) continue;
                    }

                    return (true, obj);
                }
            }

            return (false, null);
        }

        Vector2Int GetGridPos(Vector2 worldPos) => new Vector2Int(Mathf.FloorToInt((worldPos.x - bounds.min.x) / cellSize.x), Mathf.FloorToInt((worldPos.y - bounds.min.y) / cellSize.y));

        private IEnumerable<byte[]> GetCollisionCompatibleMasks(int targetLayer)
        {
            return mapsByLayer
                .Where(kvp => !Physics2D.GetIgnoreLayerCollision(kvp.Key, targetLayer))
                .Select(kvp => kvp.Value);
        }

        internal void Register(GridObject gridObject)
        {
            gridObjects.Add(gridObject);
        }

        internal void Unregister(GridObject gridObject)
        {
            gridObjects.Remove(gridObject);
        }

        internal bool IsOnTile(Vector3 worldPos, TileBase tile)
        {
            foreach (var tilemap in tilemaps)
            {
                var cellPos = tilemap.WorldToCell(worldPos);
                if (tile == tilemap.GetTile(cellPos)) return true;
            }

            return false;
        }

        internal bool IsOnTile(Vector3 worldPos, TileSet tileSet)
        {
            foreach (var tilemap in tilemaps)
            {
                var cellPos = tilemap.WorldToCell(worldPos);
                if (tileSet.IsOnSet(tilemap.GetTile(cellPos))) return true;
            }

            return false;
        }

        internal void RunRules(Vector3 worldPos, TileConverterRule[] rules)
        {            
            foreach (var tilemap in tilemaps)
            {
                var test = tilemap.WorldToCell(new Vector3(-1000.0f, 1000.0f, 0));

                var cellPos = tilemap.WorldToCell(worldPos);
                if (tilemap.cellBounds.Contains(cellPos))
                {
                    var tile = tilemap.GetTile(cellPos);
                    if (MatchRule(tile, rules, out var newTile))
                    {
                        tilemap.SetTile(cellPos, newTile);
                        collidersDirty = true;
                    }
                }
            }
        }

        bool MatchRule(TileBase tile, TileConverterRule[] rules, out TileBase newTile)
        {
            foreach (var rule in rules)
            {
                if (rule.source == tile)
                {
                    newTile = rule.dest;
                    return true;
                }
            }

            newTile = null;
            return false;
        }
    }
}
