using UnityEngine;
using System.Collections.Generic;
using GrimTools.Runtime.Core;

namespace God {

public class ToolManager : MonoSingleton<ToolManager>
{
    public GameObject HammerPrefab;

    private Dictionary<string, BaseTool> _tools = new ();
    private BaseTool _selected;

    private bool _mDown;
    private bool _mUp;

    private string[] _toolKeys;
    private int _currentToolIndex;

    void Awake()
    {
        var hammer = Hammer.CreateInstance<Hammer>();
        hammer.Prefab = HammerPrefab;
        
        _tools.Add("pickup", Pickup.CreateInstance<Pickup>());
        _tools.Add("hammer", hammer);
        _toolKeys = new string[_tools.Count];
        _tools.Keys.CopyTo(_toolKeys, 0);

        _currentToolIndex = 0;
        _selected = _tools[_toolKeys[_currentToolIndex]];
    }

    public string SelectNextTool()
    {
        _currentToolIndex = (_currentToolIndex + 1) % _toolKeys.Length;
        _selected = _tools[_toolKeys[_currentToolIndex]];
        return _toolKeys[_currentToolIndex];
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

