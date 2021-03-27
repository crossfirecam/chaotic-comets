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
    private readonly int shieldUpgradePrice = 10000, shieldUpgradeCap = 2;
    // Total cost of 10,000c per maxed upgrade. Total cost of 30,000c for maxed shield upgrade. All upgrades = 110,000c.

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
    }

    private void UpdateTextElements()
    {
        if (purchaseIndex <= 6)
            selectedUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex];
        CheckPurchaseValidity();
        SetSubDescText();
        SetUpgradeButtonText();
    }

    private void CheckPurchaseValidity()
    {
        switch (purchaseIndex)
        {
            case 8: // Buy Extra Ship
                selectedPurchasePrice = 2500;
                break;
            case 7: // Charge Shields
                if (BetweenScenes.PlayerShopShields[plrIndex] / 10 == 8)
                    selectedPurchasePrice = 0;
                else
                    selectedPurchasePrice = 200;
                break;
            case 0: // Shield Strength Upgrade. Uses +50% modifiers, limited to 2.
                if (selectedUpgradeTier == shieldUpgradeCap)
                    selectedPurchasePrice = 0;
                else
                    selectedPurchasePrice = shieldUpgradePrice + (shieldUpgradePrice * selectedUpgradeTier);
                break;
            default: // All other upgradable stats use a +20% modifier, limited to 10.
                if (selectedUpgradeTier == upgradeCap)
                    selectedPurchasePrice = 0;
                else
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
            case 9: // Ready Button
                subDescText.text = "";
                break;
            case 8: // Buy Extra Ship
                string pluralOrNot = BetweenScenes.PlayerShopLives == 1 ? "" : "s";
                subDescText.text = "Currently: " + BetweenScenes.PlayerShopLives + " Ship" + pluralOrNot;
                break;
            case 7: // Charge Shields
                subDescText.text = "Currently: " + (BetweenScenes.PlayerShopShields[plrIndex] / 10) + "/8 Cells";
                break;
            case 0: // Shield Strength Upgrade uses a +50% modifier.
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
        switch (purchaseIndex)
        {
            case 9: // Ready Button
                upgradeBtnText.text = "";
                cancelBtnText.text = "";
                break;
            case 8: // Buy Extra Ship
                upgradeBtnText.text = "+1 Ship\n(2500c)";
                break;
            case 7: // Charge Shields
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = "+1 Shield Cell\n(200c)";
                else
                    upgradeBtnText.text = "Shields Cells maxed";
                break;
            case 0: // Shield Strength Upgrade
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+50% Modifier\n({selectedPurchasePrice}c)";
                else
                    upgradeBtnText.text = "Upgrade maxed";
                break;
            default: // All other upgradable stats use a +20% modifier.
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+20% Modifier\n({selectedPurchasePrice}c)";
                else
                    upgradeBtnText.text = "Upgrade maxed";
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
                if (purchaseIndex <= 6)
                    BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex] += 1;
                else if (purchaseIndex == 7)
                    BetweenScenes.PlayerShopShields[plrIndex] += 10f;
                else if (purchaseIndex == 8)
                    BetweenScenes.PlayerShopLives += 1;

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
    private readonly string[] purchasePanelTitleStrings = { "Shield Strength",
                                                          "Brake Power", "Teleport Rate",
                                                          "Auto Firerate", "Munitions Firerate",
                                                          "Shot Speed", "Shot Range",
                                                          "Charge Shields", "Buy Extra Ship",
                                                          "Ready To Leave" };

    private readonly string[] purchasePanelDescStrings = { "Take more hits. \n+0% = 3 impacts\n+50% = 4 impacts\n+100% = 6 impacts",
                                                         "Stop faster when holding the manual brake.",
                                                         "Hyperspace becomes usable more often.",
                                                         "Holding down 'Fire' for automatic fire will shoot bullets faster.",
                                                         "The cooldown between shots with Triple or Rapid Shot can be reduced.",
                                                         "All bullets will travel faster.",
                                                         "All bullets will travel further.",
                                                         "Pay to repair the shield of your ship.",
                                                         "Buy a spare Ship.\n(Note: Every 5000 total points gives a free Ship)",
                                                         "" };
}
