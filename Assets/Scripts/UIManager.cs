using System;
using System.Collections.Generic;
using God;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Text = TMPro.TextMeshProUGUI;
using StringBuilder = System.Text.StringBuilder;

internal class Upgrades
{
    internal class Upgrade
    {
        public Button Button;
        public Text Data;
        public List<Transform> Bars = new ();
        private int _bar = 0;

        internal Upgrade(Transform upgrade)
        {
            Button = upgrade.Find("Button")
                    .GetComponent<Button>();
            Data = upgrade.Find("Cost").GetComponent<Text>();

            foreach (Transform child in upgrade.Find("Bars"))
            {
                Bars.Add(child);
            }
        }

        internal void FillBar()
        {
            if (_bar == Bars.Count)
            {
                return;
            }

            Bars[_bar].Find("Fill").gameObject.SetActive(true);
            ++_bar;
        }
    }

    public Upgrade Cooldown;
    public Upgrade Damage;
    public Upgrade Range;

    private GameObject _upgrades;
    private BaseTool _tool;

    internal Upgrades(GameObject upgrades, BaseTool tool)
    {
        _upgrades = upgrades;
        _tool = tool;

        Cooldown = new Upgrade(
                _upgrades.transform.Find("Cooldown"));
        Damage = new Upgrade(
                _upgrades.transform.Find("Damage"));
        Range = new Upgrade(_upgrades.transform.Find("Range"));

        Cooldown.Button.onClick.AddListener(OnCooldown);
        Damage.Button.onClick.AddListener(OnDamage);
        Range.Button.onClick.AddListener(OnRange);

        var stats = _tool.GetCooldownStats();
        Cooldown.Data.text =
                $"{stats[0].Value}s -> {stats[1].Value}s"
                + $" ({stats[1].Cost} Cash)";

        stats = _tool.GetDamageStats();
        Damage.Data.text =
                $"{stats[0].Value} -> {stats[1].Value}"
                + $" ({stats[1].Cost} Cash)";

        stats = _tool.GetDamageStats();
        Range.Data.text =
                $"{stats[0].Value} -> {stats[1].Value}"
                + $" ({stats[1].Cost} Cash)";
    }

    internal void SetActive(bool b)
    {
        _upgrades.SetActive(b);
    }

    public void OnCooldown()
    {
        bool upgraded = _tool.TryUpgradeCooldown();
        if (!upgraded)
        {
            return;
        }

        var stats = _tool.GetCooldownStats();
        if (stats.Length == 1)
        {
            Cooldown.Data.text = $"{stats[0].Value}s";
        }
        else
        {
            Cooldown.Data.text =
                    $"{stats[0].Value}s -> {stats[1].Value}s"
                    + $" ({stats[1].Cost} Cash)";
        }

        Cooldown.FillBar();
    }

    public void OnDamage()
    {
        bool upgraded = _tool.TryUpgradeDamage();
        if (!upgraded)
        {
            return;
        }

        var stats = _tool.GetDamageStats();
        if (stats.Length == 1)
        {
            Damage.Data.text = $"{stats[0].Value}";
        }
        else
        {
            Damage.Data.text =
                    $"{stats[0].Value} -> {stats[1].Value}"
                    + $" ({stats[1].Cost} Cash)";
        }

        Damage.FillBar();
    }

    public void OnRange()
    {
        bool upgraded = _tool.TryUpgradeRange();
        if (!upgraded)
        {
            return;
        }

        var stats = _tool.GetRangeStats();
        if (stats.Length == 1)
        {
            Range.Data.text = $"{stats[0].Value}";
        }
        else
        {
            Range.Data.text =
                    $"{stats[0].Value} -> {stats[1].Value}"
                    + $" ({stats[1].Cost} Cash)";
        }

        Range.FillBar();
    }
}

public class UIManager : MonoBehaviour
{
    [SerializeField] private Text _cashText;
    [SerializeField] private Text RemianingHumonText;
    [SerializeField] private Text SoulText;
    [SerializeField] private Button _upgradeButton;
    [SerializeField] private Sprite _pickupSprite;
    [SerializeField] private Sprite _hammerSprite;
    [SerializeField] private Sprite _igniteSprite;

    [SerializeField] private GameObject _unlockMenu;
    [SerializeField] private Button _openUnlockMenu0;
    [SerializeField] private Button _openUnlockMenu1;
    [SerializeField] private Button _closeUnlockMenu;
    [SerializeField] private Button _unlockHammer;
    [SerializeField] private Button _unlockIgnite;
    [SerializeField] private Text _unlockHammerPrice;
    [SerializeField] private Text _unlockIgnitePrice;

    [SerializeField] private GameObject _toolbar;
    [SerializeField] private GameObject _pickupTooltip;
    [SerializeField] private GameObject _cooldown;
    [SerializeField] private GameObject _damage;
    [SerializeField] private GameObject _range;
    [SerializeField] private GameObject _selectedTool;
    [SerializeField] private Button _selectPickup;
    [SerializeField] private Button _selectHammer;
    [SerializeField] private Button _selectIgnite;

    private Upgrades _hammerUpgrades;
    private Upgrades _igniteUpgrades;

    private Sprite _active = null;

    private Dictionary<string, Sprite> _toolMap = new();

    private void OnEnable()
    {
        GameManager.Instance.OnCashChanged += DisplayCash;
        GameManager.Instance.OnCashChanged += UpdateUpgradeButton;
        _upgradeButton.onClick.AddListener(Upgrade);

        _openUnlockMenu0.onClick.AddListener(OnOpenUnlockMenu);
        _openUnlockMenu1.onClick.AddListener(OnOpenUnlockMenu);
        _closeUnlockMenu.onClick.AddListener(OnCloseUnlockMenu);
        _unlockHammer.onClick.AddListener(OnUnlockHammer);
        _unlockIgnite.onClick.AddListener(OnUnlockIgnite);

        _selectPickup.onClick.AddListener(OnSelectPickup);
        _selectHammer.onClick.AddListener(OnSelectHammer);
        _selectIgnite.onClick.AddListener(OnSelectIgnite);
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
    }

    private void Start()
    {
        SetUnlockPrices();
        UpdateUpgradeButton();
    }

    private void OnDisable()
    {
        GameManager.Instance.OnCashChanged -= DisplayCash;
        GameManager.Instance.OnCashChanged -= UpdateUpgradeButton;
        _upgradeButton.onClick.RemoveAllListeners();
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
        RemianingHumonText.text = "Humons Remaining: " + GameManager.Instance.Population.ToString();
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
        _toolbar.SetActive(false);
    }

    private void OnCloseUnlockMenu()
    {
        _unlockMenu.SetActive(false);
        _toolbar.SetActive(true);
    }

    // I miss real templates

    private void OnUnlockHammer()
    {
        if (GameManager.Instance.Souls < Hammer.Price)
        {
            return;
        }

        ToolManager.Instance.UnlockHammer();

        _hammerUpgrades = new Upgrades(_toolbar.transform
                .Find("HammerUpgrades").gameObject,
                ToolManager.Instance.GetTool(Hammer.Name));

        GameManager.Instance.Souls -= Hammer.Price;
        UpdateSouls();

        _openUnlockMenu0.gameObject.SetActive(false);
        _unlockHammer.gameObject.SetActive(false);
        _selectHammer.gameObject.SetActive(true);
    }

    private void OnUnlockIgnite()
    {
        if (GameManager.Instance.Souls < Ignite.Price)
        {
            return;
        }

        ToolManager.Instance.UnlockIgnite();

        _igniteUpgrades = new Upgrades(_toolbar.transform
                .Find("IgniteUpgrades").gameObject,
                ToolManager.Instance.GetTool(Ignite.Name));

        GameManager.Instance.Souls -= Ignite.Price;
        UpdateSouls();

        _openUnlockMenu1.gameObject.SetActive(false);
        _unlockIgnite.gameObject.SetActive(false);
        _selectIgnite.gameObject.SetActive(true);
    }

    private void OnSelectPickup()
    {
        _selectedTool.GetComponent<Image>()
                .sprite = _pickupSprite;
        _hammerUpgrades?.SetActive(false);
        _igniteUpgrades?.SetActive(false);
        _pickupTooltip.gameObject.SetActive(true);
        ToolManager.Instance.SelectTool(Pickup.Name);
    }

    private void OnSelectHammer()
    {
        _selectedTool.GetComponent<Image>()
                .sprite = _hammerSprite;
        _hammerUpgrades?.SetActive(true);
        _igniteUpgrades?.SetActive(false);
        _pickupTooltip.gameObject.SetActive(false);
        ToolManager.Instance.SelectTool(Hammer.Name);
    }

    private void OnSelectIgnite()
    {
        _selectedTool.GetComponent<Image>()
                .sprite = _igniteSprite;
        _hammerUpgrades?.SetActive(false);
        _igniteUpgrades?.SetActive(true);
        _pickupTooltip.gameObject.SetActive(false);
        ToolManager.Instance.SelectTool(Ignite.Name);
    }
}

