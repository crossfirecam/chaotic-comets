using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] private int plrIndex, purchaseIndex;
    private TextMeshProUGUI titleText, descText, subDescText, upgradeBtnText, cancelBtnText;
    private Button upgradeBtn, cancelBtn;
    private int selectedUpgradeTier, selectedPurchasePrice;
    private bool selectedPurchaseMaxed;

    private readonly int baseUpgradePrice = 500, priceIncreasePerLevel = 750, upgradeCap = 5;
    private readonly int majorUpgradePrice = 10000, majorUpgradeCap = 2;
    // Total cost of 10,000c per maxed normal upgrade. Total cost of 30,000c for maxed major upgrades. All upgrades: 140,000c

    private const int BtnIndReady = 10, BtnIndexLife = 9, BtnIndShieldCell = 8, BtnIndShotLimit = 5, BtnIndShieldStr = 0;

    private void Start()
    {
        titleText = transform.Find("Title").GetComponent<TextMeshProUGUI>();
        descText = transform.Find("Desc").GetComponent<TextMeshProUGUI>();
        subDescText = transform.Find("SubDesc").GetComponent<TextMeshProUGUI>();

        upgradeBtn = transform.Find("BuyButton").GetComponent<Button>();
        cancelBtn = transform.Find("CancelButton").GetComponent<Button>();
        upgradeBtnText = upgradeBtn.GetComponentInChildren<TextMeshProUGUI>();
        cancelBtnText = cancelBtn.GetComponentInChildren<TextMeshProUGUI>();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Purchase Panel opening and closing.
     * ------------------------------------------------------------------------------------------------------------------ */
    public void PurchasePanelOpened(EventSystemShop eventSystem)
    {
        upgradeBtn.interactable = true;
        cancelBtn.interactable = true;
        eventSystem.SetSelectedGameObject(cancelBtn.gameObject);
    }
    public void PurchasePanelClosed()
    {
        upgradeBtn.interactable = false;
        cancelBtn.interactable = false;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Updating UI elements in the Purchase Panel
     * ------------------------------------------------------------------------------------------------------------------ */
    public void OnMainAreaButtonHover(int buttonHovered)
    {
        purchaseIndex = buttonHovered;
        titleText.text = purchasePanelTitleStrings[purchaseIndex];
        descText.text = purchasePanelDescStrings[purchaseIndex];
        UpdateTextElements();

        // On Insane Difficulty, shield upgrade shows a different string.
        if (BetweenScenes.Difficulty == 3 && buttonHovered == 0)
            descText.text = shieldUpgradeDescAltString;
    }

    private void UpdateTextElements()
    {
        if (purchaseIndex <= 7) // Tiered upgrades are between 0 and 7.
            selectedUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex];
        CheckPurchaseValidity();
        SetSubDescText();
        SetUpgradeButtonText();
    }

    private void CheckPurchaseValidity()
    {
        selectedPurchasePrice = 0; // Default value, won't be overwritten if the purchase isn't valid.
        switch (purchaseIndex)
        {
            case BtnIndexLife: // Extra Ship always valid to buy.
                selectedPurchasePrice = 5000;
                break;
            case BtnIndShieldCell: // Charge Shields requires less than 8 Cells to buy.
                if (BetweenScenes.PlayerShopShields[plrIndex] / 10 != 8)
                    selectedPurchasePrice = 200;
                break;
            case BtnIndShotLimit: // Shot Limit Upgrade is limited to 2.
                if (selectedUpgradeTier != majorUpgradeCap)
                    selectedPurchasePrice = majorUpgradePrice + (majorUpgradePrice * selectedUpgradeTier);
                break;
            case BtnIndShieldStr: // Shield Strength Upgrade is limited to 2.
                if (selectedUpgradeTier != majorUpgradeCap)
                    selectedPurchasePrice = majorUpgradePrice + (majorUpgradePrice * selectedUpgradeTier);
                break;
            default: // All other upgradable stats are limited to 5.
                if (selectedUpgradeTier != upgradeCap)
                    selectedPurchasePrice = baseUpgradePrice + (priceIncreasePerLevel * selectedUpgradeTier);
                break;
        }
        if (selectedPurchasePrice == 0)
            selectedPurchaseMaxed = true;
        else
            selectedPurchaseMaxed = false;
    }

    private void SetSubDescText()
    {
        switch (purchaseIndex)
        {
            case BtnIndReady: // Ready Button
                subDescText.text = "";
                break;
            case BtnIndexLife: // Buy Extra Ship
                string pluralOrNot = BetweenScenes.PlayerShopLives == 1 ? "" : "s";
                subDescText.text = "Currently: " + BetweenScenes.PlayerShopLives + " Ship" + pluralOrNot;
                break;
            case BtnIndShieldCell: // Charge Shields
                subDescText.text = "Currently: " + (BetweenScenes.PlayerShopShields[plrIndex] / 10) + "/8 Cells";
                break;
            case BtnIndShotLimit: // Shot Limit Upgrade uses +1 Shot modifiers
                subDescText.text = "Currently: Max " + (2 + selectedUpgradeTier) + " Shots";
                break;
            case BtnIndShieldStr: // Shield Strength Upgrade uses a +50% modifier.
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 50) + "%";
                break;
            default: // All other upgradable stats use a +20% modifier.
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 20) + "%";
                break;
        }
    }

    private void SetUpgradeButtonText()
    {
        cancelBtnText.text = "Cancel";
        upgradeBtnText.text = "Upgrade maxed"; // Default value, overwritten if upgrade isn't maxed.
        switch (purchaseIndex)
        {
            case BtnIndReady: // Ready Button
                upgradeBtnText.text = "";
                cancelBtnText.text = "";
                break;
            case BtnIndexLife: // Buy Extra Ship
                upgradeBtnText.text = "+1 Ship\n(5000c)";
                break;
            case BtnIndShieldCell: // Charge Shields
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = "+1 Cell\n(200c)";
                else
                    upgradeBtnText.text = "Shield Cells maxed";
                break;
            case BtnIndShotLimit: // Shot Limit Upgrade
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+1 Shot\n({selectedPurchasePrice}c)";
                break;
            case BtnIndShieldStr: // Shield Strength Upgrade
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+50%\n({selectedPurchasePrice}c)";
                break;
            default: // All other upgradable stats use a +20% modifier.
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+20%\n({selectedPurchasePrice}c)";
                break;
        }
    }

    /// <summary>
    /// Attempts a purchase of upgrade or item for a player in the shop.<br></br>
    /// 1. Check purchase is not already maxed. (Max upgrade, max shields)<br></br>
    /// 2. Check if purchase can be afforded by player.<br></br>
    /// - Success: Check which upgrade it is, perform relevant functions.<br></br>
    /// - Failure: Flash the player's credit counter red.
    /// </summary>
    public void AttemptPurchase()
    {
        if (!selectedPurchaseMaxed)
        {
            if (BetweenScenes.PlayerShopCredits[plrIndex] >= selectedPurchasePrice)
            {
                if (purchaseIndex == BtnIndShieldCell)
                {
                    BetweenScenes.PlayerShopShields[plrIndex] += 10f;
                    if (BetweenScenes.PlayerShopShields[plrIndex] > 80f)
                        BetweenScenes.PlayerShopShields[plrIndex] = 80f;
                }
                else if (purchaseIndex == BtnIndexLife)
                    BetweenScenes.PlayerShopLives += 1;
                else // All other upgrades
                    BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex] += 1;

                BetweenScenes.PlayerShopCredits[plrIndex] -= selectedPurchasePrice;
                upgradeBtn.GetComponent<AudioSource>().Play();
                ShopScript.i.PurchaseSucceeded(plrIndex);
                UpdateTextElements();
            }
            else
            {
                ShopScript.i.PurchaseFailed(plrIndex);
            }
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Strings for the Purchase Panel to explain upgrades.
     * ------------------------------------------------------------------------------------------------------------------ */
    private readonly string[] purchasePanelTitleStrings = { "Shield Strength", "Teleport Rate",
                                                          "Ship Speed", "Brake Power",
                                                          "Auto Firerate", "Shot Limit",
                                                          "Shot Speed", "Shot Range",
                                                          "Charge Shields", "Buy Extra Ship",
                                                          "Ready To Leave" };

    private readonly string[] purchasePanelDescStrings = { "Take more hits. \n+0% = 3 impacts\n+50% = 4 impacts\n+100% = 6 impacts",
                                                         "Teleportation becomes usable more often.",
                                                         "Accelerate faster and a gain a higher top speed.",
                                                         "Stop faster when holding the manual brake.",
                                                         "Holding down 'Fire' for automatic fire will shoot bullets faster.",
                                                         "Major increase of firepower. More bullets can be fired in one go.",
                                                         "All bullets will travel faster. Rate of fire increases.",
                                                         "All bullets will travel further. Rate of fire decreases.",
                                                         "Repair the shield of your ship.",
                                                         "Buy a spare Ship.\n(Note: Every 10,000 total points gives a free Ship)",
                                                         "" };

    private readonly string shieldUpgradeDescAltString = "Take more hits. \n+0% = 2 impacts\n+50% = 3 impacts\n+100% = 4 impacts";
}
