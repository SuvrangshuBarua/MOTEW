using UnityEngine;
using System.Collections.Generic;

public class ToolManager : MonoBehaviour
{
    private List<BaseTool> _tools = new ();
    private BaseTool _selected;

    void Awake()
    {
        _tools.Add(new Hammer());
        _selected = _tools[0];
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                _selected?.Use(hit);
            }
        }
    }
}

