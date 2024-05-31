using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using UnityEngine.Events;

namespace OkapiKit
{
    [DisallowMultipleComponent, AddComponentMenu("Okapi/Hypertag")]
    public class HypertaggedObject : OkapiElement
    {
        static private Dictionary<Hypertag, List<HypertaggedObject>> hypertaggedObjects = new Dictionary<Hypertag, List<HypertaggedObject>>();

        static void AddObject(HypertaggedObject obj)
        {
            foreach (var t in obj.hypertags)
            {
                hypertaggedObjects.TryGetValue(t, out var list);
                if (list == null)
                {
                    list = new List<HypertaggedObject>();
                    hypertaggedObjects.Add(t, list);
                }

                list.Add(obj);
            }
        }

        static void RemoveObject(HypertaggedObject obj)
        {
            foreach (var t in obj.hypertags)
            {
                hypertaggedObjects[t].Remove(obj);
            }
        }

        private void OnEnable()
        {
            AddObject(this);
        }

        private void OnDisable()
        {
            RemoveObject(this);
        }

        void OnDestroy()
        {
            RemoveObject(this);
        }

        [SerializeField]
        private List<Hypertag> hypertags;

        public Hypertag[] GetTags() { return hypertags.ToArray(); }

        public string GetTagString()
        {
            if ((hypertags == null) || (hypertags.Count == 0)) return "";

            string ret = "";
            foreach (var tag in hypertags)
            {
                if (tag)
                {
                    if (ret != "") ret += ", ";
                    ret += tag.name;
                }
            }

            return ret;
        }

        public void AddTag(Hypertag tag)
        {
            if (hypertags == null) hypertags = new List<Hypertag>();

            if (!hypertags.Contains(tag)) hypertags.Add(tag);
        }

        public void AddTag(Hypertag[] tags)
        {
            foreach (var t in tags)
            {
                AddTag(t);
            }
        }

        public void AddTag(List<Hypertag> tags)
        {
            foreach (var t in tags)
            {
                AddTag(t);
            }
        }

        public bool Has(Hypertag tag)
        {
            if (hypertags == null) return false;
            foreach (var t in hypertags)
            {
                if (t == tag) return true;
            }

            return false;
        }

        public bool Has(Hypertag[] tags)
        {
            if (hypertags == null) return false;
            if (tags == null) return false;

            foreach (var t1 in tags)
            {
                foreach (var t2 in hypertags)
                {
                    if (t1 == t2) return true;
                }
            }

            return false;
        }

        public static List<GameObject> FindGameObjectsWithHypertag(Hypertag[] tags)
        {
            List<GameObject> ret = new List<GameObject>();

            FindGameObjectsWithHypertag(tags, ret);

            return ret;
        }

        public static List<GameObject> FindGameObjectsWithHypertag(Hypertag[] tags, List<GameObject> ret)
        {
            foreach (var t in tags)
            {
                hypertaggedObjects.TryGetValue(t, out var list);
                if (list != null)
                {
                    foreach (var ho in list) ret.Add(ho.gameObject);
                }
            }

            return ret;
        }

        public static List<GameObject> FindGameObjectsWithHypertag(Hypertag tag)
        {
            List<GameObject> ret = new List<GameObject>();

            FindGameObjectsWithHypertag(tag, ret);

            return ret;
        }

        public static List<GameObject> FindGameObjectsWithHypertag(Hypertag tag, List<GameObject> ret)
        {
            hypertaggedObjects.TryGetValue(tag, out var list);
            if (list != null)
            {
                foreach (var ho in list) ret.Add(ho.gameObject);
            }

            return ret;
        }

        public static GameObject FindGameObjectWithHypertag(Hypertag tag)
        {
            hypertaggedObjects.TryGetValue(tag, out var list);
            if (list != null)
            {
                foreach (var ho in list) return ho.gameObject;
            }

            return null;
        }

        public static T FindObjectByHypertag<T>(Hypertag tag) where T : Component
        {
            List<HypertaggedObject> hos;
            if (hypertaggedObjects.TryGetValue(tag, out hos))
            {
                foreach (HypertaggedObject ho in hos)
                {
                    var c = ho.GetComponent<T>();
                    if (c) return c;
                }
            }

            return default(T);
        }

        public static T FindObjectByHypertag<T>(Hypertag[] tags) where T : Component
        {
            foreach (var tag in tags)
            {
                if (hypertaggedObjects.TryGetValue(tag, out var hos))
                {
                    foreach (HypertaggedObject ho in hos)
                    {
                        var c = ho.GetComponent<T>();
                        if (c) return c;
                    }
                }
            }

            return default(T);
        }

        public static List<T> FindObjectsByHypertag<T>(Hypertag tag) where T : Component
        {
            List<T> ret = new List<T>();

            return FindObjectsByHypertag<T>(tag, ret);
        }

        public static List<T> FindObjectsByHypertag<T>(Hypertag[] tags) where T : Component
        {
            List<T> ret = new List<T>();

            return FindObjectsByHypertag<T>(tags, ret);
        }

        public static List<T> FindObjectsByHypertag<T>(Hypertag tag, List<T> ret) where T : Component
        {
            hypertaggedObjects.TryGetValue(tag, out var list);
            if (list != null)
            {
                foreach (var ho in list)
                {
                    var comp = ho.GetComponent<T>();
                    if (comp) ret.Add(comp);
                }
            }
#if UNITY_EDITOR
            else
            {
                if (!UnityEditor.EditorApplication.isPlaying)
                {
                    var hos = FindObjectsOfType<HypertaggedObject>(tag);
                    foreach (var ho in hos)
                    {
                        if (ho.Has(tag))
                        {
                            var comp = ho.GetComponent<T>();
                            if (comp) ret.Add(comp);
                        }
                    }
                }
            }
#endif

            return ret;
        }

        public static List<T> FindObjectsByHypertag<T>(Hypertag[] tags, List<T> ret) where T : Component
        {
            foreach (var tag in tags)
            {
                hypertaggedObjects.TryGetValue(tag, out var list);
                if (list != null)
                {
                    foreach (var ho in list)
                    {
                        var comp = ho.GetComponent<T>();
                        if (comp) ret.Add(comp);
                    }
                }
            }

            return ret;
        }

        public static List<HypertaggedObject> GetAll()
        {
            var ret = new List<HypertaggedObject>();
            
            foreach (var hol in hypertaggedObjects)
            {
                ret.AddRange(hol.Value);
            }

            return ret;
        }

        public static HypertaggedObject GetObjectByTag(Hypertag tag)
        {
            if (hypertaggedObjects.TryGetValue(tag, out var hos))
            {
                if (hos.Count > 0) return hos[0];
            }

            return null;
        }

        public static List<HypertaggedObject> GetObjectsByTag(Hypertag tag)
        {
            if (hypertaggedObjects.TryGetValue(tag, out var hos))
            {
                return new List<HypertaggedObject>(hos);
            }

            return null;
        }

        public static HypertaggedObject GetObjectByTag(Hypertag[] tags)
        {
            foreach (var tag in tags)
            {
                if (hypertaggedObjects.TryGetValue(tag, out var hos))
                {
                    if (hos.Count > 0) return hos[0];
                }
            }

            return null;
        }

        public static List<HypertaggedObject> GetObjectsByTag(Hypertag[] tags)
        {
            var ret = new List<HypertaggedObject>();

            foreach (var tag in tags)
            {
                if (hypertaggedObjects.TryGetValue(tag, out var hos))
                {
                    ret.AddRange(hos);
                }
            }

            return ret;
        }

        public override string GetRawDescription(string ident, GameObject refObject)
        {
            if (description != "") return description;

            return "";
        }

        protected override string Internal_UpdateExplanation()
        {
            string e = GetRawDescription("", gameObject);

            _explanation = "";
            if (e != null)
            {
                for (int i = 0; i < e.Length; i++)
                {
                    if (i != ' ')
                    {
                        _explanation += char.ToUpper(e[i]) + e.Substring(i + 1);
                        break;
                    }
                    else
                    {
                        _explanation += e[i];
                    }
                }
            }
            return _explanation;
        }

        protected override void CheckErrors()
        {
            base.CheckErrors();

            if ((hypertags == null) || (hypertags.Count == 0))
            {
                _logs.Add(new LogEntry(LogEntry.Type.Error, "Tags not defined!", "Having a hypertag component without hypertags defined is pointless!\nHypertags allow OkapiKit (and other custom scripts) identify an object in the scene.\nAn object can have multiple hypertags."));
            }
            else
            {
                int index = 0;
                foreach (var tag in hypertags)
                {
                    if (tag == null)
                    {
                        _logs.Add(new LogEntry(LogEntry.Type.Error, $"Tag slot {index} is empty in tag list!", "Empty tags are useless, remove it or fill it in."));
                    }
                    index++;
                }
            }
        }
    }
}