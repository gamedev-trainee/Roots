using UnityEngine;

namespace Roots
{
    public static class GameObjectExtensions
    {
        public static GameObject Clone(this GameObject self, Transform parent = null)
        {
            GameObject go = GameObject.Instantiate(self);
            go.transform.SetParent(parent);
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
            return go;
        }
    }
}
