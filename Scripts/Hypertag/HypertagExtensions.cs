using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.YamlDotNet.Core.Tokens;
using UnityEngine;

namespace OkapiKit
{
    public static class HypertaggedExtension
    {
        public static HypertaggedObject FindObjectWithHypertag(this Object go, Hypertag tag)
        {
            return HypertaggedObject.GetObjectByTag(tag);
        }
        public static T FindObjectOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
        {
            return HypertaggedObject.FindObjectByHypertag<T>(tag);
        }

        public static void FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag tag, List<T> ret) where T : Component
        {
            HypertaggedObject.FindObjectsByHypertag(tag, ret);
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag tag) where T : Component
        {
            return HypertaggedObject.FindObjectsByHypertag<T>(tag).ToArray();
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this Object go, Hypertag[] tags) where T : Component
        {
            return HypertaggedObject.FindObjectsByHypertag<T>(tags).ToArray();
        }

        public static T FindObjectOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag[] tags) where T : Component
        {
            return HypertaggedObject.FindObjectByHypertag<T>(tags);
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag tag, bool sortByDistance = false) where T : Component
        {
            var objects = HypertaggedObject.FindObjectsByHypertag<T>(tag);
            if (sortByDistance)
            {
                SortObjects(go.transform.position, objects);
            }

            return objects.ToArray();
        }

        public static T[] FindObjectsOfTypeWithHypertag<T>(this MonoBehaviour go, Hypertag[] tags, bool sortByDistance = false) where T : Component
        {
            var objects = HypertaggedObject.FindObjectsByHypertag<T>(tags);
            if (sortByDistance)
            {
                SortObjects(go.transform.position, objects);
            }

            return objects.ToArray();
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

        public static bool HasHypertags(this GameObject go, Hypertag[] tags, bool includeChildren = true)
        {
            if (tags == null) return false;
            if (tags.Length == 0) return false;

            foreach (var tag in tags)
            {
                if (go.HasHypertag(tag, includeChildren)) return true;
            }

            return false;
        }

        public static bool HasHypertag(this GameObject go, Hypertag tag, bool includeChildren = true)
        {
            if (includeChildren)
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
            }
            else
            {
                var hypertag = go.GetComponent<HypertaggedObject>();
                if (hypertag == null) return false;

                return hypertag.Has(tag);                
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