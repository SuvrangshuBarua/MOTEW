using System;
using System.Collections.Generic;
using God;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using StringBuilder = System.Text.StringBuilder;

public enum Tool
{
    pickup = 0,
    hammer
}
public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _cashText;
    [SerializeField] private Button _toolButton;
    [SerializeField] private Button _upgradeHammerButton;
    [SerializeField] private Sprite _pickupSprite;
    [SerializeField] private Sprite _hammerSprite;
    private Dictionary<string, Sprite> _toolMap = new();

    private void OnEnable()
    {
        _toolButton.onClick.AddListener(Selection);
        GameManager.Instance.OnCashChanged += DisplayCash;
        _upgradeHammerButton.onClick.AddListener(()=> ToolManager.Instance.UpgradeTool("hammer"));
    }

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _toolMap.Add("pickup", _pickupSprite);
        _toolMap.Add("hammer", _hammerSprite);
        _stringBuilder = new StringBuilder();
    }

    private void OnDisable()
    {
        _toolButton.onClick.RemoveAllListeners();
        GameManager.Instance.OnCashChanged -= DisplayCash;
        _upgradeHammerButton.onClick.RemoveAllListeners();
    }

    private void Selection()
    {
        var selectedTool = ToolManager.Instance.SelectNextTool();
        _toolButton.transform.GetChild(0).GetComponent<Image>().sprite = _toolMap[selectedTool];
        
    }

    private void DisplayCash(int cash)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append("Cash: ").Append(cash);
        _cashText.text = _stringBuilder.ToString();
    }
}
