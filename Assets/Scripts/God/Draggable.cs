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

    private float _fixedY;
    private float _pickupOffset = 2f;
    private Plane _dragPlane;

    private bool _mDown = false;
    private bool _mUp = false;

    void Start()
    {
        _isDragged = false;
    }

    void FixedUpdate()
    {
        if (_mDown)
        {
            MouseDown();
            _mDown = false;
            return;
        }

        if (_mUp)
        {
            MouseUp();
            _mUp = false;
            return;
        }

        MouseDrag();
    }

    void Update()
    {
        // inputs are frame based, checking them
        // in FixedUpdate() is lossy (tested)

        if (Input.GetMouseButtonDown(0))
        {
            _mDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mDown = false;
            _mUp = true;
        }
    }

    bool IsSelected()
    {
        // check if we were selected
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, (1 << gameObject.layer)))
        {
            return hit.collider.gameObject == this.gameObject;
        }

        return false;
    }

    void MouseDown()
    {
        if (!IsSelected())
        {
            return;
        }

        _isDragged = true;

        // disable gravity
        if (TryGetComponent<Rigidbody>(out Rigidbody body))
        {
            body.useGravity = false;
        }

        // elevate the object
        Vector3 dir = Camera.main.transform.position - gameObject.transform.position;
        dir = dir.normalized * -_pickupOffset;
        transform.position -= dir;

        // dragging only moves in x, z direction
        _fixedY = transform.position.y;

        _dragPlane = new Plane(Vector3.up, new Vector3(0, _fixedY, 0));

        OnStartDrag?.Invoke();
    }

    void MouseDrag()
    {
        if (!_isDragged)
        {
            return;
        }

        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            hit.y = _fixedY;
            transform.position = hit;

            OnDragging?.Invoke();
        }
    }

    void MouseUp()
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

