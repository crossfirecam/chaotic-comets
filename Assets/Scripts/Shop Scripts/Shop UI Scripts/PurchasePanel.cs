﻿using System.Collections;
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

    private readonly int baseUpgradePrice = 500, priceIncreasePerLevel = 500, upgradeCap = 5;
    private readonly int majorUpgradePrice = 10000, majorUpgradeCap = 2;

    private const int BtnIndReady = 8, BtnIndShotLimit = 5, BtnIndAutoFire = 4, BtnIndTeleport = 1, BtnIndShieldStr = 0;

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

        // On Hard Difficulty, shield upgrade shows a different string.
        if (BetweenScenes.Difficulty == 2 && buttonHovered == 0)
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
            case BtnIndShotLimit: // Shot Limit: +1 Shot modifiers
                subDescText.text = "Currently: Max " + (2 + selectedUpgradeTier) + " Shots";
                break;
            case BtnIndShieldStr: // Shield Strength: +50% modifier.
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 50) + "%";
                break;
            case BtnIndAutoFire: // Auto-Fire: +20% modifier
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 20) + "%";
                break;
            case BtnIndTeleport: // Auto-Fire: +20% modifier
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 20) + "%";
                break;
            default: // All other upgradable stats use a +10% modifier.
                subDescText.text = "Currently: +" + (selectedUpgradeTier * 10) + "%";
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
            case BtnIndShotLimit:
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+1 Shot\n({selectedPurchasePrice}c)";
                break;
            case BtnIndShieldStr:
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+50%\n({selectedPurchasePrice}c)";
                break;
            case BtnIndAutoFire:
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+20%\n({selectedPurchasePrice}c)";
                break;
            case BtnIndTeleport:
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+20%\n({selectedPurchasePrice}c)";
                break;
            default: // All other upgradable stats use a +20% modifier.
                if (!selectedPurchaseMaxed)
                    upgradeBtnText.text = $"+10%\n({selectedPurchasePrice}c)";
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
                                                          "Auto-Fire", "Shot Limit",
                                                          "Shot Speed", "Shot Range",
                                                          "Ready To Leave" };

    private readonly string[] purchasePanelDescStrings = { "Take more hits. \n+0% = 3 impacts\n+50% = 4 impacts\n+100% = 6 impacts",
                                                         "Reduce teleport cooldown.",
                                                         "Accelerate faster, higher top speed.",
                                                         "Stop faster with the manual brake.",
                                                         "Holding down 'Fire' will shoot faster.",
                                                         "Major increase of firepower. More bullets can be fired in one go.",
                                                         "Bullets travel faster. Rate of fire increases.",
                                                         "Bullets travel further. Rate of fire decreases.",
                                                         "" };

    private readonly string shieldUpgradeDescAltString = "Take more hits. \n+0% = 2 impacts\n+50% = 3 impacts\n+100% = 4 impacts";
}
