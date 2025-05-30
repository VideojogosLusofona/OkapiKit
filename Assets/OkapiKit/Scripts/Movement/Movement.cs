using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

namespace OkapiKit
{
    abstract public class Movement : OkapiElement
    {
        [SerializeField]
        protected bool          hasConditions = false;
        [SerializeField]
        protected Condition[]   conditions;
        
        protected Rigidbody2D   rb;
        protected AI            aiModule;
        protected Vector3       lastDelta;
        protected Vector3       _lastNonNullDelta;

        abstract public Vector2 GetSpeed();
        abstract public void SetSpeed(Vector2 speed);
        public virtual void SetMoveVector(Vector2 moveVector)
        {
            throw new System.NotImplementedException("SetMoveVector not implemented on this class!");
        }

        virtual public bool IsLinear() { return true; }

        abstract public string GetTitle();

        public Vector3 lastNonNullDelta => _lastNonNullDelta;

        public bool hasAI
        {
            get
            {
                if (aiModule == null) 
                    aiModule = GetComponent<AI>();

                return (aiModule != null);
            }
        }

        protected virtual void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            aiModule = GetComponent<AI>();
            UpdateExplanation();
        }

        protected void MoveDelta(Vector3 delta)
        {
            if (rb != null)
            {
                rb.linearVelocity = delta / Time.deltaTime;
            }
            else
            {
                transform.position = transform.position + delta;
            }
            lastDelta = delta;
            if (delta.magnitude > 0.0f) _lastNonNullDelta = delta;
        }

        protected void StopMovement()
        {
            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
            }
        }

        protected void RotateZ(float angle)
        {
            if ((rb != null) && (!rb.freezeRotation))
            {
                rb.MoveRotation(rb.rotation + angle);
            }
            else
            {
                transform.rotation = transform.rotation * Quaternion.Euler(0.0f, 0.0f, angle);
            }
        }

        protected void RotateTo(Vector2 upDir, float maxAngle)
        {
            if ((rb != null) && (!rb.freezeRotation))
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                Quaternion newRotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);

                rb.MoveRotation(newRotation);
            }
            else
            {
                Quaternion target = Quaternion.LookRotation(Vector3.forward, upDir);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, target, maxAngle);
            }
        }

        protected virtual bool isMovementActive()
        {
            if (hasConditions)
            {
                foreach (var condition in conditions)
                {
                    if (!condition.Evaluate(gameObject)) return false;
                }
            }

            return true;
        }

        override protected string Internal_UpdateExplanation()
        {
            _explanation = "";
            if (description != "") _explanation += description + "\n----------------\n";

            string ident = "";
            if (hasConditions)
            {
                if ((conditions != null) && (conditions.Length > 0))
                {
                    _explanation += "This movement will only affect the object if ";
                    for (int i = 0; i < conditions.Length; i++)
                    {
                        if (i > 0) _explanation += " and ";
                        _explanation += conditions[i].GetRawDescription(gameObject);
                    }
                    _explanation += ".\n\n";

                    ident = "  ";
                }
            }

            _explanation += GetRawDescription(ident, gameObject);

            return _explanation;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (hasConditions)
            {
                if ((conditions == null) || (conditions.Length == 0))
                {
                    _logs.Add(new LogEntry(LogEntry.Type.Error, "Conditions active, but no conditions defined!", "If there's no conditions defined, it's the same as having the use conditions flag turned off."));
                }
                else
                {
                    var condLogs = new List<LogEntry>();
                    int index = 0;
                    foreach (var condition in conditions)
                    {
                        condLogs.Clear();
                        condition.CheckErrors(gameObject, condLogs);
                        foreach (var l in condLogs)
                        {
                            _logs.Add(new LogEntry(l.type, $"Condition {index}: {l.text}", l.tooltip));
                        }
                        index++;
                    }
                }
            }
        }
    }
}
