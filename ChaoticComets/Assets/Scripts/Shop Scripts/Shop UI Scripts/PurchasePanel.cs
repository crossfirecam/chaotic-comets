using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PurchasePanel : MonoBehaviour
{
    [SerializeField] private int plrIndex, purchaseIndex;
    [SerializeField] private TextMeshProUGUI titleText, descText, subDescText, upgradeBtnText;
    [SerializeField] private Button upgradeBtn, cancelBtn;

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

    public void SetTextElements(int buttonHovered)
    {
        purchaseIndex = buttonHovered;
        titleText.text = purchasePanelTitleStrings[purchaseIndex];
        descText.text = purchasePanelDescStrings[purchaseIndex];
        SetUpgradeButtonText();
    }

    private void SetUpgradeButtonText()
    {
        //// Buttons are passed to this function are upgrade buttons, their final char will be int-compatible
        //int currentUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][upgrade];

        //// Determine price for the upgrade
        //int tierComparedToBase = currentUpgradeTier - 10;
        //int priceForUpgrade = baseUpgradePrice + (priceIncreasePerLevel * tierComparedToBase);

        //// Set text for button
        //string tierInDecimalForm = currentUpgradeTier.ToString().Insert(currentUpgradeTier.ToString().Length - 1, ".");
        //button.GetComponentInChildren<Text>().text = $"Current: x{tierInDecimalForm}\n({priceForUpgrade}c to Upgrade)";
        //if (currentUpgradeTier == upgradeCap)
        //{
        //    button.GetComponentInChildren<Text>().text = $"Current: x{tierInDecimalForm}\n(Max upgrade)";
        //}
        //UpdatePlayerCounters(plrIndex);
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
