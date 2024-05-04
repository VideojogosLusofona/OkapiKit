using UnityEngine;


namespace OkapiKitV2
{
    public class OkapiObject : MonoBehaviour
    {
        [SerializeField] private OkapiTag[]    tags;
        [SerializeField] private OkapiScript[] objectScripts;

        public int              scriptCount => (objectScripts != null) ? (objectScripts.Length) : (0);
        public OkapiScript[]    scripts => objectScripts;
    }
}