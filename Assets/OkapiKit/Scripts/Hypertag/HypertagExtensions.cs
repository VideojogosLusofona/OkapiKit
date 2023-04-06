using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace OkapiKit
{
    public static class HypertaggedExtension
    {
        public static HypertaggedObject FindObjectWithHypertag(this Object go, Hypertag tag)
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            foreach (var obj in objects)
            {
                if (obj.Has(tag)) return obj;
            }

            return null;
        }
        public static T FindObjectOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            foreach (var obj in objects)
            {
                if (obj.Has(tag))
                {
                    var ret = obj.GetComponent<T>();
                    if (ret)
                    {
                        return ret;
                    }
                }
            }

            return null;
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            var ret = new List<T>();
            foreach (var obj in objects)
            {
                if (obj.Has(tag))
                {
                    var comp = obj.GetComponent<T>();
                    if (comp)
                    {
                        ret.Add(comp);
                    }
                }
            }

            return ret.ToArray();
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag[] tags) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            var ret = new List<T>();
            foreach (var obj in objects)
            {
                if (obj.Has(tags))
                {
                    var comp = obj.GetComponent<T>();
                    if (comp)
                    {
                        ret.Add(comp);
                    }
                }
            }

            return ret.ToArray();
        }

        public static T FindObjectOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag[] tags) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            foreach (var obj in objects)
            {
                if (obj.Has(tags))
                {
                    var ret = obj.GetComponent<T>();
                    if (ret)
                    {
                        return ret;
                    }
                }
            }

            return null;
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag tag, bool sortByDistance = false) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            var ret = new List<T>();
            foreach (var obj in objects)
            {
                if (obj.Has(tag))
                {
                    var comp = obj.GetComponent<T>();
                    if (comp)
                    {
                        ret.Add(comp);
                    }
                }
            }
            if (sortByDistance)
            {
                SortObjects(go.transform.position, ret);
            }

            return ret.ToArray();
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag[] tags, bool sortByDistance = false) where T : Component
        {
            var objects = Object.FindObjectsOfType<HypertaggedObject>();
            var ret = new List<T>();
            foreach (var obj in objects)
            {
                if (obj.Has(tags))
                {
                    var comp = obj.GetComponent<T>();
                    if (comp)
                    {
                        ret.Add(comp);
                    }
                }
            }
            if (sortByDistance)
            {
                SortObjects(go.transform.position, ret);
            }

            return ret.ToArray();
        }

        public static void SortObjects<T>(Vector3 position, List<T> objects) where T : Component
        {
            objects.Sort(delegate (T o1, T o2)
            {
                float d1 = Vector3.Distance(o1.transform.position, position);
                float d2 = Vector3.Distance(o2.transform.position, position);
                if (d1 < d2) return -1;
                else if (d1 > d2) return 1;
                return 0;
            });
        }

        public static bool HasHypertags(this GameObject go, Hypertag[] tags)
        {
            if (tags == null) return false;
            if (tags.Length == 0) return false;

            foreach (var tag in tags)
            {
                if (go.HasHypertag(tag)) return true;
            }

            return false;
        }

        public static bool HasHypertag(this GameObject go, Hypertag tag)
        {
            var hos = go.GetComponentsInChildren<HypertaggedObject>();
            if (hos == null)
            {
                return false;
            }

            foreach (var ho in hos)
            {
                if (ho.Has(tag)) return true;
            }

            return false;
        }

        public static bool HasHypertags(this Component go, Hypertag[] tags, bool includeChildren = true)
        {
            if (tags == null) return false;
            if (tags.Length == 0) return false;

            foreach (var tag in tags)
            {
                if (go.HasHypertag(tag, includeChildren)) return true;
            }

            return false;
        }

        public static bool HasHypertag(this Component go, Hypertag tag, bool includeChildren = true)
        {
            HypertaggedObject[] hos;
            if (includeChildren)
                hos = go.GetComponentsInChildren<HypertaggedObject>();
            else
                hos = go.GetComponents<HypertaggedObject>();

            if (hos == null)
            {
                return false;
            }

            foreach (var ho in hos)
            {
                if (ho.Has(tag)) return true;
            }

            return false;
        }

        public static Hypertag[] GetTags(this GameObject go)
        {
            var hos = go.GetComponent<HypertaggedObject>();

            if (hos == null)
            {
                return null;
            }

            return hos.GetTags();
        }

        public static string GetTagsString(this GameObject go)
        {
            var ret = new List<Hypertag>();
            var hos = go.GetComponent<HypertaggedObject>();
            if (hos == null)
            {
                return "";
            }

            return hos.GetTagString();
        }

        public static void AddHypertag(this GameObject go, Hypertag tag)
        {
            var ho = go.GetComponent<HypertaggedObject>();
            if (ho == null) ho = go.AddComponent<HypertaggedObject>();
            if (ho)
            {
                ho.AddTag(tag);
            }
        }
    }
}