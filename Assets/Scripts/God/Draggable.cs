using UnityEngine;

namespace God
{

public class Draggable : MonoBehaviour
{
    public System.Action OnStartDrag;
    public System.Action OnDragging;
    public System.Action OnDragEnd;

    public bool IsDragged => _isDragged;

    private bool _isDragged = false;

    private float _fixedY;                 // The Y position we lock to
    private float _pickupOffset = 2f;      // How far to bring object toward camera
    private Plane _dragPlane;              // The plane to drag along (XZ)

    void Start()
    {
        _isDragged = false;
    }

    void OnMouseDown()
    {
        _isDragged = true;

        // disable gravity
        var body = GetComponent<Rigidbody>();
        if (body)
        {
            body.useGravity = false;
        }

        // "pick up" the object
        Vector3 dir = Camera.main.transform.position - gameObject.transform.position;
        dir = dir.normalized * -_pickupOffset;
        transform.position -= dir;

        // dragging only moves in x, z direction
        _fixedY = transform.position.y;

        _dragPlane = new Plane(Vector3.up, new Vector3(0, _fixedY, 0));

        OnStartDrag?.Invoke();
    }

    void OnMouseDrag()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            hit.y = _fixedY;
            transform.position = hit;

            OnDragging?.Invoke();
        }
    }

    void OnMouseUp()
    {
        _isDragged = false;

        // re-enable gravity
        var body = GetComponent<Rigidbody>();
        if (body)
        {
            body.useGravity = true;
        }

        OnDragEnd?.Invoke();
    }
}

} // namespace God

