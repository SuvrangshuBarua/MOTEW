using UnityEngine;

namespace God
{

public class Pickup : BaseTool
{
    private Draggable _draggable = null;

    private float _fixedY;
    private float _pickupOffset = 4f;
    private Plane _dragPlane;

    public override float Range()
    {
        return 0f;
    }

    public override void MouseDown()
    {
        // NOTE: AS OF RIGHT NOW, ONLY OBJECTS IN THE "NPC"
        // LAYER CAN BE DRAGGED.  TO ADD MORE, ADD THE LAYER
        // TO THE RAYCAST MASK.

        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (!Physics.Raycast(ray, out RaycastHit hit,
                Mathf.Infinity, LayerMask.GetMask("NPC")))
        {
            return;
        }

        if (!hit.collider.gameObject.TryGetComponent(
                    out Draggable draggable))
        {
            _draggable = null;
            return;
        }
        _draggable = draggable;
        _draggable.IsDragged = true;

        // disable gravity
        if (draggable.TryGetComponent(out Rigidbody body))
        {
            body.useGravity = false;
        }

        // elevate the object
        Vector3 dir = Camera.main.transform.position
                - _draggable.transform.position;
        dir = dir.normalized * -_pickupOffset;
        _draggable.transform.position -= dir;

        // dragging only moves in x, z direction
        _fixedY = _draggable.transform.position.y;

        _dragPlane = new Plane(
                Vector3.up, new Vector3(0, _fixedY, 0));

        _draggable.OnStartDrag?.Invoke();
    }

    public override void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _fixedY += 1f;
        }
    }

    public override void MouseUp()
    {
        if (_draggable == null)
        {
            return;
        }

        if (_draggable.TryGetComponent(out Rigidbody body))
        {
            body.useGravity = true;
        }

        _draggable.IsDragged = false;
        _draggable.OnDragEnd?.Invoke();

        _draggable = null;
    }

    public override void FixedUpdate()
    {
        if (_draggable == null)
        {
            return;
        }

        // TODO: can miss inputs
        

        Ray ray = Camera.main.ScreenPointToRay(
                Input.mousePosition);

        if (_dragPlane.Raycast(ray, out float enter))
        {
            Vector3 hit = ray.GetPoint(enter);
            hit.y = _fixedY;
            _draggable.transform.position = hit;

            _draggable.OnDragging?.Invoke();
        }
    }
    
}

} // namespace God

