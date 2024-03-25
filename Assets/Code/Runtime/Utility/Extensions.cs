using UnityEngine;

namespace TowerDefense.Runtime.Utility
{
    public static class Extensions
    {
        public static GameObject Find(this GameObject gameObject, string path)
        {
            var find = gameObject.transform.Find(path);
            return find ? find.gameObject : null;
        }

        public static T Find<T>(this Transform transform, string path)
        {
            var find = transform.Find(path);
            return find ? find.GetComponent<T>() : default;
        }

        public static Transform Search(this Transform transform, string name)
        {
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                if (child.name == name) return child;
            }
            
            for (var i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i);
                var res = child.Search(name);
                if (res) return res;
            }

            return null;
        }
    }
}