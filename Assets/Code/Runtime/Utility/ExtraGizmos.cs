using System.Collections.Generic;
using UnityEngine;

namespace TowerDefense.Runtime.Utility
{
    public class ExtraGizmos
    {
        public class Line
        {
            public List<Vector3> points = new();

            public void Add(float x, float y, float z = 0f) => Add(new Vector3(x, y, z));
            public void Add(Vector3 point) => points.Add(point);

            public void DrawLoop(Color? color = null)
            {
                DrawSegment(color);
                Gizmos.DrawLine(points[^1], points[0]);
            }

            public void DrawSegment(Color? color = null)
            {
                if (color.HasValue) Gizmos.color = color.Value;
                for (var i = 0; i < points.Count - 1; i++)
                {
                    Gizmos.DrawLine(points[i], points[i + 1]);
                }
            }
        }
    }
}