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

    void Awake()
    {
        _tools.Add(Pickup.Name, Pickup.CreateInstance<Pickup>());

        /*
        var hammer = Hammer.CreateInstance<Hammer>();
        hammer.Prefab = HammerPrefab;
        _tools.Add("hammer", hammer);

        var ignite = Hammer.CreateInstance<Ignite>();
        ignite.Prefab = FirePrefab;
        _tools.Add("ignite", ignite);
        */


        _selected = _tools[Pickup.Name];
    }

    public void SelectTool(string tool)
    {
        _selected = _tools[tool];
    }

    public BaseTool GetTool(string name)
    {
        return _tools[name];
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

    public void UnlockHammer()
    {
        var hammer = Hammer.CreateInstance<Hammer>();
        hammer.Prefab = HammerPrefab;
        _tools.Add(Hammer.Name, hammer);
    }

    public void UnlockIgnite()
    {
        var ignite = Ignite.CreateInstance<Ignite>();
        ignite.Prefab = FirePrefab;
        _tools.Add(Ignite.Name, ignite);
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

        _selected.Update();
        ToolIndicator.Instance.Draw(_selected.Range());
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

