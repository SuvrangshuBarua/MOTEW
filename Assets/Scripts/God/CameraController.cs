using System;
using UnityEngine;

namespace God
{

public class CameraController : MonoBehaviour
{
    public float MoveSpeed = 15.0f;
    public float ZoomSpeed = 20f;

    public float MinZoom = 10f;
    public float MaxZoom = 60f;

    public Vector2 BorderX = new Vector2(-30, 30);
    public Vector2 BorderZ = new Vector2(-50, 0);

    private LayerMask _layer;
    private Camera _cam;
    private Vector3 _lastMousePos;
    private Draggable _draggable = null;

    void Start()
    {
        _layer = LayerMask.NameToLayer("NPC");
        _layer = 1 << _layer;
        _cam = Camera.main;
    }

    void FixedUpdate()
    {
        if (_draggable && _draggable.IsDragged)
        {
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Ray ray = _cam.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, _layer))
            {
                _draggable = hit.collider.GetComponent<Draggable>();
                if (_draggable != null)
                {
                    return;
                }
            }
        }
    }
    
    private void LateUpdate()
    {
        if (_draggable && _draggable.IsDragged)
        {
            return;
        }

        if (!Input.GetMouseButton(0))
        {
            return;
        }

        HandleCameraMovement();
        HandleZoom();
        ClampCamera();
        
        Debug.DrawRay(_cam.transform.position, _cam.transform.forward * 100, Color.red);
    }

    void HandleCameraMovement()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _lastMousePos = Input.mousePosition;
            return;
        }

        if (Input.GetMouseButton(0))
        {
            Vector3 delta = Input.mousePosition - _lastMousePos;
            Vector3 move = new Vector3(-delta.x, 0, -delta.y); 
            move *= MoveSpeed * Time.deltaTime;

            // move in camera's plane
            Vector3 forward = _cam.transform.forward;
            forward.y = 0;
            forward.Normalize();

            Vector3 right = _cam.transform.right;
            right.y = 0;
            right.Normalize();

            //Debug.Log(move);
            Vector3 movement = right * move.x + forward * move.z;
            _cam.transform.position += movement;

            _lastMousePos = Input.mousePosition;
        }
    }

    void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (scroll != 0f)
        {
            float newFOV = _cam.fieldOfView - scroll * ZoomSpeed;
            _cam.fieldOfView = Mathf.Clamp(newFOV, MinZoom, MaxZoom);
        }
    }

    void ClampCamera()
    {
        Vector3 pos = _cam.transform.position;
        pos.x = Mathf.Clamp(pos.x, BorderX.x, BorderX.y);
        pos.z = Mathf.Clamp(pos.z, BorderZ.x, BorderZ.y);
        _cam.transform.position = pos;
    }
}

} // namespace God

