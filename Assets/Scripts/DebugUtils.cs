using UnityEngine;
using System.Collections.Generic;

public class DebugUtils
{
    public static void DrawCircle(Vector3 center, float radius)
    {
        var segments = 32;
        Gizmos.color = Color.red;
        Vector3 prevPoint = center + new Vector3(radius, 1, 0);

        for (int i = 1; i <= segments; i++)
        {
            float angle = i * Mathf.PI * 2 / segments;
            Vector3 newPoint = center + new Vector3(
                    Mathf.Cos(angle) * radius, 1, Mathf.Sin(angle) * radius);
            Gizmos.DrawLine(prevPoint, newPoint);
            prevPoint = newPoint;
        }
    }

    public static void DrawPath(List<Vector3> path)
    {
        //Gizmos.color = Color.magenta;

        if (path.Count < 2)
        {
            return;
        }

        var off = new Vector3(0, 3, 0);
        var prev = path[0];
        for (var i = 1; i < path.Count; ++i)
        {
            //Gizmos.DrawLine(prev, next);
            Debug.DrawLine(prev + off, path[i] + off, Color.magenta, 2000);
            prev = path[i];
        }
    }
}

