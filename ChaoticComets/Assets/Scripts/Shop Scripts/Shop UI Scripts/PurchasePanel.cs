using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] private int plrIndex, purchaseIndex;
    [SerializeField] private TextMeshProUGUI titleText, descText, subDescText, upgradeBtnText, cancelBtnText;
    [SerializeField] private Button upgradeBtn, cancelBtn;
    private int selectedUpgradeTier, selectedPurchasePrice;

    private readonly int baseUpgradePrice = 500, priceIncreasePerLevel = 750, upgradeCap = 15;


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
    public void SetTextElements(int buttonHovered)
    {
        purchaseIndex = buttonHovered;
        if (purchaseIndex <= 5)
            selectedUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][purchaseIndex];
        titleText.text = purchasePanelTitleStrings[purchaseIndex];
        descText.text = purchasePanelDescStrings[purchaseIndex];
        SetSubDescText();
        SetUpgradeButtonText();
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
                selectedPurchasePrice = 2500;
                upgradeBtnText.text = "+1 Ship\n(2500c)";
                break;
            case 6: // Charge Shields
                selectedPurchasePrice = 200;
                upgradeBtnText.text = "+1 Shield Cell\n(200c)";
                break;
            default: // All upgradable stats use a +10% modifier.
                selectedPurchasePrice = baseUpgradePrice + (priceIncreasePerLevel * selectedUpgradeTier);
                upgradeBtnText.text = $"+10% Modifier\n({selectedPurchasePrice}c)";
                break;
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Purchasing something.
     * ------------------------------------------------------------------------------------------------------------------ */
    
    public void AttemptPurchase()
    {

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
