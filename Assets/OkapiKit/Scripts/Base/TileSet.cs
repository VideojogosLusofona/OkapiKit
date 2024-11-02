using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OkapiKit
{
    [CreateAssetMenu(menuName = "Okapi Kit/Tile Set")]
    public class TileSet : OkapiScriptableObject
    {
        [SerializeField] private List<TileBase> tiles;

        public override string GetRawDescription(string ident, ScriptableObject refObject)
        {
            string desc = "";

            desc += $"Defines a tile set for use in conditions.";

            return desc;
        }

        protected override string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            _explanation += GetRawDescription("", this);

            return _explanation;
        }

        internal bool IsOnSet(TileBase tileBase)
        {
            if (tiles == null) return false;

            return tiles.Contains(tileBase);
        }
    }
}
