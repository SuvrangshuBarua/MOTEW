using UnityEngine;
using System.Collections.Generic;
using GrimTools.Runtime.Core;

namespace God {

public class ToolManager : MonoSingleton<ToolManager>
{
    public GameObject HammerPrefab;
    public GameObject FirePrefab;

    private Dictionary<string, BaseTool> _tools = new ();
    private BaseTool _selected;

    private bool _mDown;
    private bool _mUp;

    private string[] _toolKeys;
    private int _currentToolIndex;

    void Awake()
    {
        _tools.Add("pickup", Pickup.CreateInstance<Pickup>());

        var hammer = Hammer.CreateInstance<Hammer>();
        hammer.Prefab = HammerPrefab;
        _tools.Add("hammer", hammer);

        var ignite = Hammer.CreateInstance<Ignite>();
        ignite.Prefab = FirePrefab;
        _tools.Add("ignite", ignite);


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

    public void UpgradeTool(string toolName = null)
    {
        var tool = (toolName == null) ?
                _selected : _tools[toolName];

        var cost = tool.UpgradeCost();
        if (GameManager.Instance.CanDeductCash(cost))
        {
            tool.Upgrade();
            GameManager.Instance.SetCash(
                    GameManager.Instance.GetCash() - cost);
        }
    }

    public int UpgradeCost(string toolName = null)
    {
        var tool = (toolName == null) ?
                _selected : _tools[toolName];

        return tool.UpgradeCost();
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
        _selected?.Update();
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

