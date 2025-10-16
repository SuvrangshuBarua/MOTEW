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
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Sprite _pickupSprite;
    [SerializeField] private Sprite _hammerSprite;
    [SerializeField] private Sprite _igniteSprite;
    private Sprite _active = null;

    private Dictionary<string, Sprite> _toolMap = new();

    private void OnEnable()
    {
        _toolButton.onClick.AddListener(Selection);
        GameManager.Instance.OnCashChanged += DisplayCash;
        GameManager.Instance.OnCashChanged += UpdateUpgradeButton;
        _upgradeButton.onClick.AddListener(Upgrade);
    }

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _toolMap.Add("pickup", _pickupSprite);
        _toolMap.Add("hammer", _hammerSprite);
        _toolMap.Add("ignite", _igniteSprite);

        _active = _toolMap["pickup"];

        _stringBuilder = new StringBuilder();

        UpdateUpgradeButton();
    }

    private void OnDisable()
    {
        _toolButton.onClick.RemoveAllListeners();
        GameManager.Instance.OnCashChanged -= DisplayCash;
        GameManager.Instance.OnCashChanged -= UpdateUpgradeButton;
        _upgradeButton.onClick.RemoveAllListeners();
    }

    private void Selection()
    {
        var selectedTool = ToolManager.Instance.SelectNextTool();
        _active = _toolMap[selectedTool];

        
        UpdateSelectedToolButton();
        UpdateUpgradeButton();
    }

    private void UpdateSelectedToolButton()
    {
        _toolButton.transform.GetChild(0)
                .GetComponent<Image>().sprite = _active;
    }

    private void UpdateUpgradeButton(int unused = 0)
    {
        _upgradeButton.transform.GetChild(0)
                .GetComponent<Image>().sprite = _active;

        var text = _upgradeButton.transform.GetChild(0)
                .GetChild(1).GetComponent<TextMeshProUGUI>();
        var cost = ToolManager.Instance.UpgradeCost();

        _upgradeButton.gameObject.SetActive(
                cost != int.MaxValue);

        text.text = $"${cost}";
        text.color = GameManager.Instance.CanDeductCash(cost) ?
                Color.green : Color.red;
    }

    private void Upgrade()
    {
        ToolManager.Instance.UpgradeTool();

        UpdateUpgradeButton();
    }

    private void DisplayCash(int cash)
    {
        _stringBuilder.Clear();
        _stringBuilder.Append("Cash: ").Append(cash);
        _cashText.text = _stringBuilder.ToString();
    }
}

