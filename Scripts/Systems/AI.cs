using NaughtyAttributes;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace OkapiKit
{
    public class AI : OkapiElement
    {
        [Flags]
        public enum Flags 
        { 
            WanderEnable = 1, 
            FleeEnable = 2,
            ChaseEnable = 4,
            SearchEnable = 8,
            PatrolEnable = 16,
        };
        private enum State { Idle, Wander, Flee, Chase, Search, Patrol };
        
        private enum Mode { Random, Sequence };

        [SerializeField]
        private float       agentRadius = 8.0f;
        [SerializeField]
        private Vector2     agentOffset = Vector2.zero;
        [SerializeField] 
        private Flags       flags;
        [SerializeField] 
        private LayerMask   obstacleMask;
        [SerializeField] 
        private float       wanderRadius = 50.0f;
        [SerializeField, MinMaxSlider(0.1f, 15.0f)] 
        private Vector2     timeBetweenWanders = new Vector2(2.0f, 2.0f);
        [SerializeField] 
        private float       wanderSpeedModifier = 1.0f;
        [SerializeField]
        private float       fleeProximityRadius = 20.0f;
        [SerializeField]
        private float       fleeSpeedModifier = 1.0f;
        [SerializeField, MinMaxSlider(0.1f, 15.0f)]
        private Vector2     stopFleeingInterval = new Vector2(2.0f, 2.0f);
        [SerializeField]
        private Hypertag[]  fleeTags;
        [SerializeField]
        private float       chaseStopDistance = 20.0f;
        [SerializeField]
        private float       chaseViewDistance = 100.0f;
        [SerializeField]
        private bool        chaseUseFOV = false;
        [SerializeField]
        private float       chaseFieldOfView = 45.0f;
        [SerializeField]
        private float       chaseSpeedModifier = 1.0f;
        [SerializeField]
        private bool        chaseUseMaxRangeFromSpawn = false;
        [SerializeField]
        private float       chaseMaxRangeFromSpawn = 100.0f;
        [SerializeField]
        private bool        chaseRequireLOS = false;
        [SerializeField]
        private float       searchRadius = 50.0f;
        [SerializeField]
        private float       searchSpeedModifier = 1.0f;
        [SerializeField]
        private float       searchMaxTime = 10.0f;
        [SerializeField]
        private Hypertag[]  chaseTags;
        [SerializeField]
        private float       patrolSpeedModifier = 1.0f;
        [SerializeField]
        private Mode        patrolMode;
        [SerializeField]
        private Path        patrolPath;
        [SerializeField]
        private Transform[] patrolPoints;

        State       state;
        State       prevState;
        Movement    movement;
        float       timeToNextState;
        State       nextState;
        Vector3     spawnPos;
        Vector3     wanderTarget;
        Transform   chaseTarget;
        Vector3     lastChasePos;
        float       searchStartTime;
        Vector3     searchBasePos;
        Vector3     searchTarget;
        Vector3[]   lastPos;
        int         lastPosIndex;
        Vector3[]   patrolPointsCache;
        Vector3     patrolTarget;
        int         patrolIndex;

        public bool canWander => (flags & Flags.WanderEnable) != 0;
        public bool canFlee => (flags & Flags.FleeEnable) != 0;
        public bool canChase => (flags & Flags.ChaseEnable) != 0;
        public bool canSearch => (flags & Flags.SearchEnable) != 0;
        public bool canPatrol => (flags & Flags.PatrolEnable) != 0;

        public float GetLastMoveDistance()
        {
            float distance = 0.0f;
            for (int i = 1; i < lastPos.Length; i++)
            {
                distance += Vector3.Distance(lastPos[i - 1], lastPos[i]);
            }
            return distance;
        }

        // Start is called once before the first execution of Update after the MonoBehaviour is created
        void Start()
        {
            movement = GetComponent<Movement>();
            spawnPos = transform.position.xy0();

            lastPos = new Vector3[20];
            for (int i = 0; i < lastPos.Length; i++) lastPos[i] = spawnPos;

            if (patrolPath != null)
            {
                var pts = patrolPath.GetPoints();
                patrolPointsCache = new Vector3[pts.Count];
                for (int i = 0; i < pts.Count; i++) patrolPointsCache[i] = pts[i];
                patrolMode = Mode.Sequence;
            }
            else if ((patrolPoints != null) && (patrolPoints.Length > 0))
            {
                patrolPointsCache = new Vector3[patrolPoints.Length];
                for (int i = 0; i < patrolPoints.Length; i++) patrolPointsCache[i] = patrolPoints[i].position;
            }
        }

        private void OnEnable()
        {
            movement?.SetMoveVector(Vector2.zero);
        }

        private void OnDisable()
        {
            movement?.SetMoveVector(Vector2.zero);
        }

        // Update is called once per frame
        void Update()
        {
            movement?.SetMoveVector(Vector2.zero);
            if (timeToNextState > 0.0f)
            {
                timeToNextState -= Time.deltaTime;
                if (timeToNextState <= 0.0f)
                {
                    ChangeState(nextState);
                }
            }
            switch (state)
            {
                case State.Idle: UpdateIdle(); break;
                case State.Wander: UpdateWander(); break;
                case State.Flee: UpdateFlee(); break;
                case State.Chase: UpdateChase(); break;
                case State.Search: UpdateSearch(); break;
                case State.Patrol: UpdatePatrol(); break;
            }

            lastPos[lastPosIndex] = transform.position;
            lastPosIndex = (lastPosIndex + 1) % lastPos.Length;
        }

        void ChangeState(State s, float transitionTime = 0.0f)
        {
            if (transitionTime == 0.0f)
            {
                prevState = state;
                state = s;
                timeToNextState = 0.0f;
                BeginState();
            }
            else
            {
                timeToNextState = transitionTime;
                nextState = s;
            }
        }

        void BeginState()
        {
            switch (state)
            {
                case State.Idle: break;
                case State.Wander: BeginWander(); break;
                case State.Search: BeginSearch(); break;
                case State.Patrol: BeginPatrol(); break;
                default:
                    break;
            }
        }

        bool MoveToTarget(Vector3 targetPos, float speedModifier)
        {
            Vector3 direction = (targetPos - transform.position).xy0();
            float distance = direction.magnitude;
            if (distance < 1e-3)
            {
                return false;
            }
            else
            {
                if (distance > 1.0f)
                {
                    direction.Normalize();
                }
                movement.SetMoveVector(speedModifier * direction * movement.GetSpeed());
            }

            return true;
        }

        bool HasLOS(Vector3 targetPos)
        {
            // Check if there is LOS
            var direction = targetPos - transform.position;
            var distance = direction.magnitude;
            if (distance > 0.0f) direction.Normalize();
            var hit = Physics2D.CircleCast(transform.position + agentOffset.xy0(), agentRadius, direction, distance, obstacleMask);

            return (hit.collider == null);
        }

        Vector3 GetRandomPoint(Vector3 centerPos, float radius)
        {
            // Choose a target position
            int nTries = 0;
            while (nTries < 20)
            {
                Vector3 pos = centerPos + UnityEngine.Random.insideUnitCircle.xy0() * radius;

                // Check if there is LOS
                if (HasLOS(pos))
                {
                    return pos;
                }

                nTries++;
            };

            return transform.position;
        }

        #region Idle State
        void UpdateIdle()
        {
            if (timeToNextState <= 0.0f)
            {
                if (canChase)
                {
                    CheckForChaseTarget();
                    if (chaseTarget)
                    {
                        ChangeState(State.Chase);
                        return;
                    }
                }
                if (canPatrol)
                {
                    ChangeState(State.Patrol, 0.0f);
                }
                else if (canWander)
                {
                    ChangeState(State.Wander, 0.0f);
                }
            }
            if (FleeInterrupt())
            {

            }
        }
        #endregion

        #region Wander State
        void BeginWander()
        {
            wanderTarget = GetRandomPoint(spawnPos, wanderRadius);
        }

        void UpdateWander()
        {
            if (timeToNextState <= 0.0f)
            {
                if (canChase)
                {
                    CheckForChaseTarget();
                    if (chaseTarget)
                    {
                        ChangeState(State.Chase);
                        return;
                    }
                }
                if (!MoveToTarget(wanderTarget, wanderSpeedModifier))
                {
                    ChangeState(State.Wander, timeBetweenWanders.Random());
                }
            }
            if (FleeInterrupt())
            {

            }
        }
        #endregion

        #region Flee State
        bool FleeInterrupt()
        {
            if (!canFlee) return false;

            // This is not an explicit state, this is something that can interrupt any state
            // It just changes the current movement orders if we need to run from someone
            var scaryObjects = this.FindObjectsOfTypeWithHypertag<Transform>(fleeTags, false);
            var invProximityRadius = 1.0f / fleeProximityRadius;
            var direction = Vector3.zero;
            foreach (var scaryObject in scaryObjects)
            {
                float distance = Vector3.Distance(scaryObject.position, transform.position);
                if (distance < fleeProximityRadius)
                {
                    direction += (transform.position - scaryObject.position) / invProximityRadius;
                }
            }

            if (direction.magnitude > 0.0f)
            {
                direction.Normalize();

                movement.SetMoveVector(fleeSpeedModifier * direction * movement.GetSpeed());

                ChangeState(State.Flee, 0.0f);

                return true;
            }

            return false;
        }

        void UpdateFlee()
        {
            if (!FleeInterrupt())
            {
                if (timeToNextState <= 0.0f)
                {
                    ChangeState(State.Idle, stopFleeingInterval.Random());
                }
            }
        }
        #endregion

        #region Chase State

        void CheckForChaseTarget()
        {
            chaseTarget = null;            

            // This is not an explicit state, this is something that can interrupt any state
            // It just changes the current movement orders if we need to run from someone
            var potentialChaseTargets = this.FindObjectsOfTypeWithHypertag<Transform>(chaseTags, false);
            var minDist = float.MaxValue;            

            foreach (var potentialChaseTarget in potentialChaseTargets)
            {
                if (chaseUseMaxRangeFromSpawn)
                {
                    if (Vector3.Distance(potentialChaseTarget.position.xy0(), spawnPos) > chaseMaxRangeFromSpawn)
                    {
                        continue;
                    }
                }
                float distance = Vector3.Distance(potentialChaseTarget.position, transform.position);
                if ((distance < chaseViewDistance) && (minDist > distance))
                {
                    if (chaseUseFOV)
                    {
                        Vector3 toTarget = (potentialChaseTarget.position - transform.position).SafeNormalized();
                        if (movement.lastNonNullDelta.x < 0.0f) toTarget.x = -toTarget.x;
                        float angle = Mathf.Rad2Deg * Mathf.Acos(Vector3.Dot(toTarget.xy0(), Vector3.right));
                        if (Mathf.Abs(angle) > chaseFieldOfView) continue;
                    }

                    if (chaseRequireLOS)
                    {
                        if (!HasLOS(potentialChaseTarget.position)) continue;
                    }

                    minDist = distance;
                    chaseTarget = potentialChaseTarget;
                    lastChasePos = chaseTarget.position;
                }
            }
        }

        void UpdateChase()
        {
            if (timeToNextState <= 0.0f)
            {
                if (chaseTarget == null)
                {
                    ChangeState(State.Idle);
                }
                else
                {
                    if (chaseUseMaxRangeFromSpawn)
                    {
                        float distanceToSpawn = Vector3.Distance(spawnPos.xy0(), transform.position.xy0());
                        if (distanceToSpawn > chaseMaxRangeFromSpawn)
                        {
                            ChangeState(State.Idle);
                            return;
                        }
                    }

                    float distance = Vector3.Distance(chaseTarget.position.xy0(), transform.position.xy0());
                    if (distance > chaseStopDistance)
                    {
                        if (HasLOS(chaseTarget.position))
                        {
                            lastChasePos = chaseTarget.position;
                            MoveToTarget(lastChasePos, chaseSpeedModifier);
                        }
                        else 
                        {
                            if (HasLOS(chaseTarget.position))
                            {
                                lastChasePos = chaseTarget.position;
                            }
                            if ((!MoveToTarget(lastChasePos, chaseSpeedModifier)) || (GetLastMoveDistance() < 1.0f))
                            {
                                CheckForChaseTarget();
                                if (chaseTarget == null)
                                {
                                    if (canSearch)
                                    {
                                        searchStartTime = Time.time;
                                        searchBasePos = lastChasePos;

                                        ChangeState(State.Search);
                                    }
                                    else
                                    {
                                        ChangeState(State.Idle);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        #endregion

        #region Search State
        void BeginSearch()
        {
            searchTarget = GetRandomPoint(searchBasePos, searchRadius);
        }

        void UpdateSearch()
        {
            if (timeToNextState <= 0.0f)
            {
                if (canChase)
                {
                    CheckForChaseTarget();
                    if (chaseTarget)
                    {
                        ChangeState(State.Chase);
                        return;
                    }
                }
                if ((Time.time - searchStartTime) > searchMaxTime)
                {
                    ChangeState(State.Idle, 0.0f);
                }
                else if (!MoveToTarget(searchTarget, searchSpeedModifier))
                {
                    ChangeState(State.Search, 0.0f);
                }
            }
            if (FleeInterrupt())
            {

            }
        }
        #endregion

        #region Patrol State

        void BeginPatrol()
        {
            if (patrolMode == Mode.Random) patrolIndex = UnityEngine.Random.Range(0, patrolPointsCache.Length);
            patrolTarget = patrolPointsCache[patrolIndex];
            patrolIndex = (patrolIndex + 1) % patrolPointsCache.Length;
        }

        void UpdatePatrol()
        {
            if (timeToNextState <= 0.0f)
            {
                if (canChase)
                {
                    CheckForChaseTarget();
                    if (chaseTarget)
                    {
                        ChangeState(State.Chase);
                        return;
                    }
                }
                if ((!MoveToTarget(patrolTarget, patrolSpeedModifier)) || (GetLastMoveDistance() < 1.0f))
                {
                    BeginPatrol();
                }
            }
            if (FleeInterrupt())
            {

            }
        }

        #endregion

        protected override void CheckErrors()
        {
            base.CheckErrors();

            var movement = GetComponent<Movement>();
            if (movement == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "AI module requires a XY Movement element.", "If you want to use the AI module, you need to add a movement module."));
            }
            else if (movement is not MovementXY)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "AI module requires a XY Movement element.", "Currently, only MovementXY module is supported for movement."));
            }

            if (canFlee)
            {
                if ((fleeTags == null) || (fleeTags.Length == 0))
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No flee tags defined!", "Flee is enabled, but no flee tags are defined! These tags define what the AI runs away from."));
                else
                {
                    for (int i = 0; i < fleeTags.Length; i++)
                    {
                        if (fleeTags[i] == null)
                            _logs.Add(new LogEntry(LogEntry.Type.Error, $"Flee tag #{i} is null.", "You must assign a valid Hypertag for all flee targets."));
                    }
                }
            }

            if (canChase)
            {
                if ((chaseTags == null) || (chaseTags.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Warning, "No chase tags defined!", "Chase is enabled, but no chase tags are defined. The AI won't have any targets to chase."));
                }
            }

            if (canPatrol)
            {
                if ((patrolPath == null) && ((patrolPoints == null) || (patrolPoints.Length == 0)))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "No patrol path or points defined!", "Patrol is enabled but neither a patrol path nor patrol points are assigned."));
                }
            }
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            List<string> parts = new List<string>
            {
                "This module enables AI behavior for this object, including stateful transitions between behaviors such as idle, patrol, wandering, fleeing, chasing, and searching."
            };

            if (canPatrol)
            {
                string patrolInfo = patrolPath != null ? "a predefined path" : "a list of patrol points";
                parts.Add($"- Patrols using {patrolInfo}, in {patrolMode} mode, at {patrolSpeedModifier:0.##}× movement speed.");
            }

            if (canWander)
            {
                parts.Add($"- Wanders within a radius of {wanderRadius:0.##} units from its spawn point every {timeBetweenWanders.x:0.##}–{timeBetweenWanders.y:0.##}s at {wanderSpeedModifier:0.##}× speed.");
            }

            if (canFlee)
            {
                parts.Add($"- Flees from objects tagged with: {string.Join(", ", fleeTags.Select(t => t != null ? t.name : "<null>"))}, within {fleeProximityRadius:0.##} units, at {fleeSpeedModifier:0.##}× speed.");
            }

            if (canChase)
            {
                var chaseFlags = new List<string>();
                if (chaseUseFOV) chaseFlags.Add($"FOV {chaseFieldOfView}°");
                if (chaseRequireLOS) chaseFlags.Add("LOS required");
                if (chaseUseMaxRangeFromSpawn) chaseFlags.Add($"max range {chaseMaxRangeFromSpawn} units");

                string modifiers = chaseFlags.Count > 0 ? $" ({string.Join(", ", chaseFlags)})" : "";
                parts.Add($"- Chases objects tagged with: {string.Join(", ", chaseTags.Select(t => t != null ? t.name : "<null>"))}, up to {chaseViewDistance:0.##} units{modifiers}, at {chaseSpeedModifier:0.##}× speed.");
            }

            if (canSearch)
            {
                parts.Add($"- Searches for lost targets within {searchRadius:0.##} units for up to {searchMaxTime:0.##}s at {searchSpeedModifier:0.##}× speed.");
            }

            return string.Join("\n", parts.Select(p => $"{ident}{p}"));
        }



#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (!EditorApplication.isPlaying) spawnPos = transform.position;

            if (EditorApplication.isPlaying)
            {
                Gizmos.color = Color.grey;
                DebugHelpers.DrawTextAt(transform.position, Vector3.zero, 10, Gizmos.color, $"State = {state}", false, true);
                Gizmos.DrawWireSphere(spawnPos + agentOffset.xy0(), agentRadius * 0.25f);
            }
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + agentOffset.xy0(), agentRadius);
            DebugHelpers.DrawTextAt(transform.position + agentOffset.xy0() + Vector3.right * agentRadius, Vector3.up * 20.0f, 10, Gizmos.color, "Agent Radius");

            if (canPatrol)
            {
                Handles.color = Gizmos.color = Color.green;
                if (patrolPath != null)
                {
                    patrolPath.DrawPath();
                }
                else if ((patrolPoints != null) && (patrolPoints.Length > 0.0f))
                {
                    if (patrolMode == Mode.Sequence)
                    {
                        for (int i = 0; i < patrolPoints.Length; i++)
                        {
                            Handles.DrawDottedLine(patrolPoints[i].position, patrolPoints[(i + 1) % patrolPoints.Length].position, 5.0f);
                        }
                    }
                    else
                    {
                        for (int i = 0; i < patrolPoints.Length; i++)
                        {
                            for (int j = 0; j < patrolPoints.Length; j++)
                            {
                                Handles.DrawDottedLine(patrolPoints[i].position, patrolPoints[j].position, 5.0f);
                            }
                        }
                    }
                }

                if ((EditorApplication.isPlaying) && (state == State.Patrol))
                {
                    Handles.color = Gizmos.color;
                    Handles.DrawDottedLine(transform.position, patrolTarget, 2.0f);
                }
            }
            else if (canWander)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(transform.position, wanderRadius);
                DebugHelpers.DrawTextAt(transform.position + Vector3.right * wanderRadius, Vector3.up * 20.0f, 10, Gizmos.color, "Wander Radius");

                if ((EditorApplication.isPlaying) && (state == State.Wander))
                {
                    Handles.color = Gizmos.color;
                    Handles.DrawDottedLine(transform.position, wanderTarget, 2.0f);
                }
            }
            if (canFlee)
            {
                Gizmos.color = new Color(1.0f, 1.0f, 0.0f, 0.5f);
                Gizmos.DrawWireSphere(transform.position, fleeProximityRadius);
                DebugHelpers.DrawTextAt(transform.position + Vector3.right * fleeProximityRadius, Vector3.down * 20.0f, 10, Gizmos.color, "Flee Radius");
            }
            if (canChase)
            {
                if (chaseUseMaxRangeFromSpawn)
                {
                    Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.5f);
                    Gizmos.DrawWireSphere(spawnPos, chaseMaxRangeFromSpawn);
                    DebugHelpers.DrawTextAt(spawnPos + Vector3.right * chaseMaxRangeFromSpawn, Vector3.down * 20.0f, 10, Gizmos.color, "Max. Chase Distance");
                }
                if (chaseUseFOV)
                {
                    Handles.color = Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 0.15f);
                    Handles.DrawSolidArc(transform.position, Vector3.forward, Vector3.right.RotateZ(-chaseFieldOfView), chaseFieldOfView * 2.0f, chaseViewDistance);
                }
                else
                {
                    Gizmos.color = new Color(1.0f, 0.0f, 0.0f, 1.0f);
                    Gizmos.DrawWireSphere(transform.position, chaseViewDistance);
                }
                DebugHelpers.DrawTextAt(transform.position + Vector3.right * chaseViewDistance, Vector3.down * 20.0f, 10, Gizmos.color, "View Distance");

                Gizmos.color = new Color(1.0f, 0.5f, 0.0f, 0.5f);
                Gizmos.DrawWireSphere(transform.position, chaseStopDistance);
                DebugHelpers.DrawTextAt(transform.position + Vector3.right * chaseStopDistance, Vector3.down * 20.0f, 10, Gizmos.color, "Stop Distance");

                if ((EditorApplication.isPlaying) && (state == State.Chase) && (chaseTarget != null))
                {
                    if (HasLOS(chaseTarget.position))
                    {
                        Handles.color = Gizmos.color = Color.red;
                        Handles.DrawDottedLine(transform.position, lastChasePos, 2.0f);
                    }
                    else
                    {
                        Handles.color = Gizmos.color = new Color(1.0f, 0.25f, 0.25f, 0.5f);
                        Handles.DrawDottedLine(transform.position, lastChasePos, 2.0f);
                    }
                }
            }
            if (canSearch)
            {
                if ((EditorApplication.isPlaying) && (state == State.Search))
                {
                    Handles.color = Gizmos.color = Color.green;
                    Handles.DrawDottedLine(transform.position, searchTarget, 2.0f);
                }
            }
        }
#endif
    }
}
