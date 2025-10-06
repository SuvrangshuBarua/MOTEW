using UnityEngine;
using System.Collections.Generic;

namespace God {

public class ToolManager : MonoBehaviour
{
    public GameObject HammerPrefab;

    private Dictionary<string, BaseTool> _tools = new ();
    private BaseTool _selected;

    private bool _mDown;
    private bool _mUp;

    void Awake()
    {
        var hammer = Hammer.CreateInstance<Hammer>();
        hammer.Prefab = HammerPrefab;
        _tools.Add("hammer", hammer);

        _tools.Add("pickup", Pickup.CreateInstance<Pickup>());

        _selected = _tools["hammer"];
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

