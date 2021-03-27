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
        if (purchaseIndex <= 5)
            selectedUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex];
        CheckPurchaseValidity();
        SetSubDescText();
        SetUpgradeButtonText();
    }

    private void CheckPurchaseValidity()
    {
        switch (purchaseIndex)
        {
            case 7: // Buy Extra Ship
                selectedPurchasePrice = 2500;
                break;
            case 6: // Charge Shields
                if (BetweenScenes.PlayerShopShields[plrIndex] / 10 == 8)
                    selectedPurchasePrice = 0;
                else
                    selectedPurchasePrice = 200;
                break;
            default: // All upgradable stats use a +10% modifier.
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
            case 8: // Ready Button
                subDescText.text = "";
                break;
            case 7: // Buy Extra Ship
                string pluralOrNot = BetweenScenes.PlayerShopLives == 1 ? "" : "s";
                subDescText.text = "Currently: " + BetweenScenes.PlayerShopLives + " Ship" + pluralOrNot;
                break;
            case 6: // Charge Shields
                subDescText.text = "Currently: " + (BetweenScenes.PlayerShopShields[plrIndex] / 10) + "/8 Cells";
                break;
            default: // All upgradable stats use a +10% modifier.
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 10) + "%";
                break;
        }
    }

    private void SetUpgradeButtonText()
    {
        cancelBtnText.text = "Cancel";
        switch (purchaseIndex)
        {
            case 8: // Ready Button
                upgradeBtnText.text = "";
                cancelBtnText.text = "";
                break;
            case 7: // Buy Extra Ship
                upgradeBtnText.text = "+1 Ship\n(2500c)";
                break;
            case 6: // Charge Shields
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = "+1 Shield Cell\n(200c)";
                else
                    upgradeBtnText.text = "Shields Cells maxed";
                break;
            default: // All upgradable stats use a +10% modifier.
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+10% Modifier\n({selectedPurchasePrice}c)";
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
                if (purchaseIndex <= 5)
                    BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex] += 1;
                else if (purchaseIndex == 6)
                    BetweenScenes.PlayerShopShields[plrIndex] += 10f;
                else if (purchaseIndex == 7)
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
    private readonly string[] purchasePanelTitleStrings = { "Brake Power", "Teleport Rate",
                                                          "Auto Firerate", "Munitions Firerate",
                                                          "Shot Speed", "Shot Range",
                                                          "Charge Shields", "Buy Extra Ship",
                                                          "Ready To Leave" };

    private readonly string[] purchasePanelDescStrings = { "Stop faster when holding the manual brake.",
                                                         "Hyperspace becomes usable more often.",
                                                         "Holding down 'Fire' for automatic fire will shoot bullets faster.",
                                                         "The cooldown between shots with Triple or Rapid Shot can be reduced.",
                                                         "All bullets will travel faster.",
                                                         "All bullets will travel further.",
                                                         "Pay to repair the shield of your ship.",
                                                         "Buy a spare Ship.\n(Note: Every 5000 total points gives a free Ship)",
                                                         "" };
}
