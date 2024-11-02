using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Action/Change Tile")]
    public class ActionChangeTile : Action
    {
        public enum VicinityType { Single, FourWay, EightWay, Circle };

        [SerializeField] private VicinityType       vicinityType = VicinityType.Single;
        [SerializeField] private float              radius = 1.0f;
        [SerializeField] private Transform          target;
        [SerializeField] private Hypertag[]         tags;
        [SerializeField] private TileConverterRule[]    rules;

        private GridObject gridObject;

        public override void Execute()
        {
            if (!enableAction) return;
            if (!EvaluatePreconditions()) return;
            if (gridObject == null) return;

            var positions = GetTargetPositions();
            foreach (var position in positions)
            {
                Fill(position);
            }
        }

        public override string GetActionTitle() => "Change Tile";

        public override string GetRawDescription(string ident, GameObject gameObject)
        {
            string desc = GetPreconditionsString(gameObject);

            string targetPos = "";
            if ((tags != null) && (tags.Length > 0))
            {
                targetPos = "all positions tagged with {";
                for (int i = 0; i < tags.Length; i++) 
                {
                    if (i != 0) desc += ",";
                    desc += $"[{tags[i]}]";
                }
                targetPos += "}";
            }
            else if (target != null)
            {
                targetPos = $"at {target.name}";
            }
            else
            {
                targetPos = $"at this positon";
            }

            switch (vicinityType)
            {
                case VicinityType.Single:
                    desc += $"changes tiles {targetPos} according to the given rules:\n";
                    break;
                case VicinityType.FourWay:
                    desc += $"changes tiles {targetPos}, and positions up, down, left and right, up to {radius} cells away, according to the given rules:\n";
                    break;
                case VicinityType.EightWay:
                    desc += $"changes tiles {targetPos}, and positions all around, up to {radius} cells away, according to the given rules:\n";
                    break;
                case VicinityType.Circle:
                    desc += $"changes tiles {targetPos}, in circle of {radius} cells away, according to the given rules:\n";
                    break;
                default:
                    break;
            }

            if ((rules != null) && (rules.Length > 0))
            {
                for (int i = 0; i < rules.Length; i++)
                {
                    var rule = rules[i];
                    if (rule.source == null)
                    {
                        if (rule.dest == null)
                        {
                            desc += $"  {i + 1}. Invalid rule";
                        }
                        else
                        {
                            desc += $"  {i + 1}. None => {rule.dest.name}";
                        }
                    }
                    else
                    {
                        if (rule.dest == null)
                        {
                            desc += $"  {i + 1}. {rule.source.name} => Deleted";
                        }
                        else
                        {
                            desc += $"  {i + 1}. {rule.source.name} => {rule.dest.name}";
                        }
                    }
                    if (i < rules.Length - 1) desc += "\n";
                }
            }
            else
            {
                desc += "  Rules are not defined";
            }
            return desc;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (gridObject == null)
            {
                gridObject = GetComponent<GridObject>();
                if (gridObject == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No Grid Object found!", "Change Tile action requires a Grid Object to be executed!"));
                }
            }

            if ((tags == null) || (tags.Length == 0))
            {
                if (transform == null)
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Tags not defined!", "Tags (or a target object) is necessary to indicate the target of the operation."));
                }
            }
            else
            {
                int index = 0;
                foreach (var tag in tags)
                {
                    if (tag == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Tag slot is empty in tag list (index={index})!", "Empty tags are useless, fill it in, or delete it"));
                    }
                    index++;
                }
            }

            if ((rules == null) || (rules.Length == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Warning, $"Missing rules for operation!", "Need to specify what kind of transformations will be done"));
            }
            else
            {
                int index = 0;
                foreach (var rule in rules)
                {
                    if (rule == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Undefined rule at slot {index}!", "Empty tags are useless, fill it in, or delete it"));
                    }
                    else if ((rule.source == null) && (rule.dest == null))
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Invalid rule at slot {index}!", "Rule is [NONE] to [NONE], which is invalid!"));
                    }
                    index++;
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();

            gridObject = GetComponent<GridObject>();
        }

        List<Vector3> GetTargetPositions()
        {
            List<Vector3> ret = new();
            if ((tags == null) || (tags.Length == 0))
            {
                if (target != null)
                {
                    ret.Add(target.position);
                    return ret;
                }
                else
                {
                    ret.Add(transform.position);
                    return ret;
                }                
            }

            var objs = gameObject.FindObjectsOfTypeWithHypertag<Transform>(tags);

            foreach (var obj in objs)
            {
                ret.Add(obj.position);
            }
            return ret;
        }

        public void Fill(Vector3 worldPos)
        {
            Vector2Int gridPos = gridObject.WorldToGrid(worldPos);
            int        r = Mathf.FloorToInt(radius);
            switch (vicinityType)
            {
                case VicinityType.Single:
                    gridObject.RunRules(worldPos, rules);
                    break;
                case VicinityType.FourWay:
                    gridObject.RunRules(gridPos, rules);
                    for (int i = 1; i <= r; i++)
                    {
                        gridObject.RunRules(new Vector2Int(gridPos.x + i, gridPos.y), rules);
                        gridObject.RunRules(new Vector2Int(gridPos.x - i, gridPos.y), rules);
                        gridObject.RunRules(new Vector2Int(gridPos.x, gridPos.y + i), rules);
                        gridObject.RunRules(new Vector2Int(gridPos.x, gridPos.y - i), rules);
                    }
                    break;
                case VicinityType.EightWay:
                    for (int y = gridPos.y - r; y <= gridPos.y + r; y++)
                    {
                        for (int x = gridPos.x - r; x <= gridPos.x + r; x++)
                        {
                            gridObject.RunRules(new Vector2Int(x, y), rules);
                        }
                    }
                    break;
                case VicinityType.Circle:
                    for (int y = gridPos.y - r; y <= gridPos.y + r; y++)
                    {
                        for (int x = gridPos.x - r; x <= gridPos.x + r; x++)
                        {
                            if (Vector2.Distance(new Vector2(x, y), new Vector2(gridPos.x, gridPos.y)) < radius)
                            {
                                gridObject.RunRules(new Vector2Int(x, y), rules);
                            }
                        }
                    }
                    break;
                default:
                    break;
            }
        }
    }
};