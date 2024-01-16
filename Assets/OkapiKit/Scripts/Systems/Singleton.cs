using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    [AddComponentMenu("Okapi/Other/Singleton")]
    public class Singleton : OkapiElement
    {
        [SerializeField] private Hypertag singletonTag;

        static Dictionary<Hypertag, Singleton> singletons = new Dictionary<Hypertag, Singleton>();

        // Start is called before the first frame update
        void Start()
        {
            if (singletonTag == null)
            {
                Destroy(gameObject);
                return;
            }
            Singleton singleton;
            if (singletons.TryGetValue(singletonTag, out singleton))
            {
                if ((singleton != null) && (singleton != this))
                {
                    Destroy(gameObject);
                    return;
                }
            }

            singletons.Add(singletonTag, this);
            DontDestroyOnLoad(this);
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            return "(UNUSED) Singleton.GetRawDescription";
        }

        protected override string Internal_UpdateExplanation()
        {
            if (description != "") _explanation = description;
            else _explanation = "";

            if (singletonTag != null)
            {
                _explanation += $"There can only be one singleton with tag {singletonTag.name} at any time.";
            }
            else
            {
                _explanation += $"There can only be one singleton with tag [UNDEFINED] at any time.";
            }

            return _explanation;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if (singletonTag == null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "A tag has to be defined for the singleton to work!", "When the scene with this object loads, there will be a check to see if a singleton with this tag already exists. If it does, this object self-destructs. If not, this object is flagged so that it can't be deleted when a scene unloads."));
            }
            if (transform.parent != null)
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "A singleton can't have a parent object.", "Objects that survive scene unload need to be at the top level of the scene, so they can't have a parent."));

            }
        }
    }
}
