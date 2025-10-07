using System;
using System.Collections.Generic;
using God;
using UnityEngine;
using UnityEngine.UI;

public enum Tool
{
    pickup = 0,
    hammer
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private Button _toolButton;
    [SerializeField] private Sprite _pickupSprite;
    [SerializeField] private Sprite _hammerSprite;
    private Dictionary<string, Sprite> _toolMap = new();

    private void OnEnable()
    {
        _toolButton.onClick.AddListener(Selection);
    }

    private void Awake()
    {
        _toolMap.Add("pickup", _pickupSprite);
        _toolMap.Add("hammer", _hammerSprite);
    }

    private void OnDisable()
    {
        _toolButton.onClick.RemoveAllListeners();
    }

    private void Selection()
    {
        var selectedTool = ToolManager.Instance.SelectNextTool();
        _toolButton.transform.GetChild(0).GetComponent<Image>().sprite = _toolMap[selectedTool];
        
    }
}
