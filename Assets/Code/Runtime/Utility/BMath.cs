using UnityEngine;

namespace TowerDefense.Runtime.Utility
{
    public class BMath
    {
        public static Vector3 Tangent(Vector3 v)
        {
            var t0 = Vector3.Cross(v, Vector3.up);
            var t1 = Vector3.Cross(v, Vector3.forward);

            return t0.sqrMagnitude > t1.sqrMagnitude ? t0 : t1;
        }
    }
}