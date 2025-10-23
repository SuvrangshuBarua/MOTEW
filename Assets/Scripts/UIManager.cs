using System;
using System.Collections.Generic;
using God;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using StringBuilder = System.Text.StringBuilder;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _cashText;
    [SerializeField] private Text RemianingHumonText;
    [SerializeField] private Text KillCountText;
    [SerializeField] private Text SoulText;
    [SerializeField] private Button _toolButton;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Sprite _pickupSprite;
    [SerializeField] private Sprite _hammerSprite;
    [SerializeField] private Sprite _igniteSprite;

    [SerializeField] private GameObject _unlockMenu;
    [SerializeField] private Button _openUnlockMenu;
    [SerializeField] private Button _closeUnlockMenu;
    [SerializeField] private Button _unlockHammer;
    [SerializeField] private Button _unlockIgnite;
    [SerializeField] private Text _unlockHammerPrice;
    [SerializeField] private Text _unlockIgnitePrice;

    private Sprite _active = null;

    private Dictionary<string, Sprite> _toolMap = new();

    private void OnEnable()
    {
        _toolButton.onClick.AddListener(Selection);
        GameManager.Instance.OnCashChanged += DisplayCash;
        GameManager.Instance.OnCashChanged += UpdateUpgradeButton;
        _upgradeButton.onClick.AddListener(Upgrade);

        _openUnlockMenu.onClick.AddListener(OnOpenUnlockMenu);
        _closeUnlockMenu.onClick.AddListener(OnCloseUnlockMenu);
        _unlockHammer.onClick.AddListener(OnUnlockHammer);
        _unlockIgnite.onClick.AddListener(OnUnlockIgnite);
    }

    private StringBuilder _stringBuilder;

    private void Awake()
    {
        _toolMap.Add(Pickup.Name, _pickupSprite);
        _toolMap.Add(Hammer.Name, _hammerSprite);
        _toolMap.Add(Ignite.Name, _igniteSprite);

        _active = _toolMap[Pickup.Name];

        _stringBuilder = new StringBuilder();
        
        DisplayCash(0);

        //UpdateUpgradeButton();
    }

    private void Start()
    {
        SetUnlockPrices();
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
        KillCountText.text =  "KillCount: " + GameManager.Instance.HumonDeathCount.ToString();
        RemianingHumonText.text = "Remaining Humons: " + GameManager.Instance.Population.ToString();
        _stringBuilder.Append("Cash: ").Append(cash);
        _cashText.text = _stringBuilder.ToString();

        UpdateSouls();
    }

    private void UpdateSouls()
    {
        SoulText.text = "Souls: " + GameManager.Instance.Souls.ToString();

        // update color of prices in tool unlock menu
        _unlockHammerPrice.color = (GameManager.Instance.Souls < Hammer.Price) ? Color.red : Color.green;
        _unlockIgnitePrice.color = (GameManager.Instance.Souls < Ignite.Price) ? Color.red : Color.green;
    }

    private void SetUnlockPrices()
    {
        _unlockHammerPrice.text = $"Price: {Hammer.Price} Souls";
        _unlockIgnitePrice.text = $"Price: {Ignite.Price} Souls";
        UpdateSouls();
    }

    private void OnOpenUnlockMenu()
    {
        _unlockMenu.SetActive(true);
    }

    private void OnCloseUnlockMenu()
    {
        _unlockMenu.SetActive(false);
    }

    // I miss real templates

    private void OnUnlockHammer()
    {
        if (GameManager.Instance.Souls < Hammer.Price)
        {
            return;
        }

        GameManager.Instance.Souls -= Hammer.Price;
        UpdateSouls();

        ToolManager.Instance.UnlockHammer();
        _unlockHammer.gameObject.SetActive(false);
    }

    private void OnUnlockIgnite()
    {
        if (GameManager.Instance.Souls < Ignite.Price)
        {
            return;
        }

        GameManager.Instance.Souls -= Ignite.Price;
        UpdateSouls();

        ToolManager.Instance.UnlockIgnite();
        _unlockIgnite.gameObject.SetActive(false);
    }
}

