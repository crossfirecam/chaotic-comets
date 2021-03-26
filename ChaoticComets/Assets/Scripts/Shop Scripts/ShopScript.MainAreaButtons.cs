using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
    * Button Lists - Many functions in ShopScript tell a variety of buttons to toggle interactivity. To do this, initilise the buttons that belong to certain players.
    * ------------------------------------------------------------------------------------------------------------------ */
    private Button[][] mainPlayerButtons = new Button[2][];
    private void LoadInButtonArrays()
    {
        Button[] listOfButtons = FindObjectsOfType<Button>();
        List<Button> p1ButtonsTemp = new List<Button>();
        List<Button> p2ButtonsTemp = new List<Button>();

        foreach (Button btn in listOfButtons)
        {
            if (btn.transform.name.StartsWith("P1"))
                p1ButtonsTemp.Add(btn);
            else if (btn.transform.name.StartsWith("P2"))
                p2ButtonsTemp.Add(btn);
        }
        mainPlayerButtons[0] = p1ButtonsTemp.ToArray();
        mainPlayerButtons[1] = p2ButtonsTemp.ToArray();
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Ready System - If both players are ready, shop closes.
    * ------------------------------------------------------------------------------------------------------------------ */
    public void ReadyUp(int plrReady)
    {
        Navigation customNav = new Navigation { mode = Navigation.Mode.Explicit };
        Button[] listOfButtons = FindObjectsOfType<Button>();

        // Set text and nav for Ready button (disable moving up if player is readied)
        plrIsReady[plrReady] = !plrIsReady[plrReady];
        ShopRefs.plrReadyBtns[plrReady].GetComponentInChildren<Text>().text = plrIsReady[plrReady] ? "Unready" : "Ready";
        customNav.selectOnUp = plrIsReady[plrReady] ? null : ShopRefs.plrAboveReadyBtns[plrReady];
        ShopRefs.plrReadyBtns[plrReady].GetComponent<Button>().navigation = customNav;

        // Find all of a player's buttons that aren't 'Ready' button, and disable them
        foreach (Button btn in mainPlayerButtons[plrReady])
        {
            if (!btn.transform.name.EndsWith("Ready8"))
            {
                btn.interactable = !plrIsReady[plrReady];
            }
        }

        // If all players are ready, start the next level
        if (plrIsReady.All(x => x))
        {
            for (int i = 0; i < plrIsReady.Length; i++)
            {
                ShopRefs.plrReadyBtns[i].GetComponentInChildren<Text>().text = "";
                ShopRefs.plrReadyBtns[i].GetComponent<Button>().interactable = false;
            }
            GoBackToGame();
        }
    }

    private GameObject btnLastPressedInMainArea;
    public void EnterPurchasePanel(int plrIndex)
    {
        btnLastPressedInMainArea = ShopRefs.plrEventSystems[plrIndex].currentSelectedGameObject;
        foreach (Button btn in mainPlayerButtons[plrIndex])
        {
            btn.interactable = false;
        }
        EventSystemShop plrEvents = ShopRefs.plrEventSystems[plrIndex];
        ShopRefs.plrPurchasePanels[plrIndex].PurchasePanelOpened(plrEvents);
    }
    public void LeavePurchasePanel(int plrIndex)
    {
        foreach (Button btn in mainPlayerButtons[plrIndex])
        {
            btn.interactable = true;
        }
        ShopRefs.plrPurchasePanels[plrIndex].PurchasePanelClosed();
        ShopRefs.plrEventSystems[plrIndex].SetSelectedGameObject(btnLastPressedInMainArea);
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Upgrade Button - A button can only pass one variable. Passes which player pressed it, and figures out which one was pressed in code.
    * ------------------------------------------------------------------------------------------------------------------ */
    //public void PerformUpgrade(int plrIndex)
    //{
    //    // Find button pressed, turn the last character in the name into an integer
    //    Button buttonUpgrading = ShopRefs.plrEventSystems[plrIndex].currentSelectedGameObject.GetComponent<Button>();
    //    int whichUpgrade = int.Parse(buttonUpgrading.name.Last().ToString());

    //    // If the upgrade hasn't reached the upgrade cap, then attempt the upgrade
    //    // If it can be afforded, perform it. If not, flash the credits text red
    //    if (BetweenScenes.PlayerShopUpgrades[plrIndex][whichUpgrade] < upgradeCap)
    //    {
    //        int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.PlayerShopUpgrades[plrIndex][whichUpgrade] - 10);
    //        if (BetweenScenes.PlayerShopCredits[plrIndex] >= price)
    //        {
    //            buttonUpgrading.GetComponent<AudioSource>().Play();
    //            BetweenScenes.PlayerShopUpgrades[plrIndex][whichUpgrade] += 1;
    //            BetweenScenes.PlayerShopCredits[plrIndex] -= price;
    //            print($"Upgrades: {string.Join(",", BetweenScenes.PlayerShopUpgrades[plrIndex])} Credits left: {BetweenScenes.PlayerShopCredits[plrIndex]}");
    //            //UpdateButtonUpgrade(plrIndex, buttonUpgrading);
    //        }
    //        else
    //        {
    //            StartCoroutine(FlashCreditsRed(plrIndex));
    //        }
    //    }
    //}

    /* ------------------------------------------------------------------------------------------------------------------
    * Stat Flashing Methods - Gives player feedback if not enough credits or lives to perform an action.
    * ------------------------------------------------------------------------------------------------------------------ */
    private bool[] isAlreadyFlashingCredits = { false, false };
    private IEnumerator FlashCreditsRed(int playerFlashing)
    {
        if (!isAlreadyFlashingCredits[playerFlashing])
        {
            GetComponent<AudioSource>().Play(); // ShopManager contains a 'UiError'-playing AudioSource
            isAlreadyFlashingCredits[playerFlashing] = true;
            ShopRefs.listOfPlrScoreText[playerFlashing].color = Color.red;
            yield return new WaitForSeconds(.5f);
            ShopRefs.listOfPlrScoreText[playerFlashing].color = Color.white;
            isAlreadyFlashingCredits[playerFlashing] = false;
        }
    }
}
