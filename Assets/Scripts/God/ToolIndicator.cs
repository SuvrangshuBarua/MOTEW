using UnityEngine;
using GrimTools.Runtime.Core;

namespace God {

[RequireComponent(typeof(LineRenderer))]
public class ToolIndicator : MonoSingleton<ToolIndicator>
{
    private int _segments = 128;
    private LineRenderer line;

    void Start()
    {
        line = GetComponent<LineRenderer>();
        line.positionCount = _segments + 1;
        line.useWorldSpace = true;
    }

    public void Draw(float radius)
    {
        var center = GetMouseWorldPosition();
        float angle = 0f;
        for (int i = 0; i <= _segments; i++)
        {
            float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
            float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            Vector3 point = new Vector3(x, 0.01f, z) + center;
            line.SetPosition(i, point);
            angle += 360f / _segments;
        }
    }

    Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            return hit.point;
        }
        return Vector3.zero;
    }
}

} // namespace God

