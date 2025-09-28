using UnityEngine;

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
}

