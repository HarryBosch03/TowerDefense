using System;
using UnityEngine;

namespace TowerDefense.Runtime.Level
{
    public class Path : MonoBehaviour
    {
        public Corner[] path;
        public float cornerRadius = 0.1f;

        private void Awake()
        {
            GetPath();
        }

        private void GetPath()
        {
            if (path != null) return;
            
            path = new Corner[transform.childCount];
            for (var i = 0; i < transform.childCount; i++)
            {
                path[i].position = transform.GetChild(i).position;
            }

            for (var i = 0; i < transform.childCount - 1; i++)
            {
                path[i].rotation = Quaternion.LookRotation(path[i + 1].position - path[i].position, Vector3.up);
                transform.GetChild(i).rotation = path[i].rotation;
            }

            path[^1].rotation = path[^2].rotation;
        }

        public Corner Sample(float distance, out bool pastEnd)
        {
            GetPath();
            pastEnd = false;
            for (var i = 0; i < path.Length - 1; i++)
            {
                var a = path[i];
                var b = path[i + 1];
                var segmentDistance = (b.position - a.position).magnitude;
                if (distance <= segmentDistance)
                {
                    return new Corner()
                    {
                        position = Vector3.Lerp(a.position, b.position, distance / segmentDistance),
                        rotation = a.rotation,
                    };
                }

                distance -= segmentDistance;
            }

            pastEnd = true;
            return path[^1];
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.yellow;
            var last = Vector3.zero;
            var totalLength = 0f;

            for (var i = 0; i < transform.childCount; i++)
            {
                var a = transform.GetChild(i).position;
                if (i > 0) totalLength += (a - last).magnitude;
                last = a;
                Gizmos.DrawSphere(a, 0.5f);
            }

            var step = 1f / 1024f;
            for (var p = 0f; p < 1f - step; p += step)
            {
                var a = Sample((p) * totalLength, out _);
                var b = Sample((p + step) * totalLength, out _);
                Gizmos.DrawLine(a.position, b.position);
            }
        }

        public struct Corner
        {
            public Vector3 position;
            public Quaternion rotation;

            public static Corner Lerp(Corner a, Corner b, float t) => new()
            {
                position = Vector3.Lerp(a.position, b.position, t),
                rotation = Quaternion.Slerp(a.rotation, b.rotation, t),
            };
        }
    }
}