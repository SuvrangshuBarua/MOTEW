using UnityEngine;
using System.Collections.Generic;

namespace God {

public class ToolManager : MonoBehaviour
{
    private Dictionary<string, BaseTool> _tools = new ();
    private BaseTool _selected;

    private bool _mDown;
    private bool _mUp;

    void Awake()
    {
        _tools.Add("hammer", Hammer.CreateInstance<Hammer>());
        _tools.Add("pickup", Pickup.CreateInstance<Pickup>());
        _selected = _tools["pickup"];
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mDown = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            _mUp = true;
        }
    }

    void FixedUpdate()
    {
        if (_mDown)
        {
            _selected?.MouseDown();
            _mDown = false;
            return;
        }

        if (_mUp)
        {
            _selected?.MouseUp();
            _mUp = false;
            return;
        }

        _selected?.FixedUpdate();
    }
}

} // namespace God

