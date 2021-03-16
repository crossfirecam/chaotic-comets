using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{


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
        string btnStartToCheck = "P" + (plrReady + 1);
        foreach (Button btn in listOfButtons)
        {
            if (btn.transform.name.StartsWith(btnStartToCheck) && !btn.transform.name.EndsWith("Ready"))
            {
                btn.GetComponent<Button>().interactable = !plrIsReady[plrReady];
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

    /* ------------------------------------------------------------------------------------------------------------------
    * Upgrade Button - A button can only pass one variable. Passes which player pressed it, and figures out which one was pressed in code.
    * ------------------------------------------------------------------------------------------------------------------ */
    public void PerformUpgrade(int player)
    {
        // Find button pressed, turn the last character in the name into an integer
        Button buttonUpgrading = ShopRefs.plrEventSystems[player].currentSelectedGameObject.GetComponent<Button>();
        int whichUpgrade = int.Parse(buttonUpgrading.name.Last().ToString());

        // If the upgrade hasn't reached the upgrade cap, then attempt the upgrade
        // If it can be afforded, perform it. If not, flash the credits text red
        if (BetweenScenes.PlayerShopUpgrades[player][whichUpgrade] < upgradeCap)
        {
            int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.PlayerShopUpgrades[player][whichUpgrade] - 10);
            if (BetweenScenes.PlayerShopCredits[player] >= price)
            {
                buttonUpgrading.GetComponent<AudioSource>().Play();
                BetweenScenes.PlayerShopUpgrades[player][whichUpgrade] += 1;
                BetweenScenes.PlayerShopCredits[player] -= price;
                print($"Upgrades: {string.Join(",", BetweenScenes.PlayerShopUpgrades[player])} Credits left: {BetweenScenes.PlayerShopCredits[player]}");
                UpdateButtonUpgrade(player, buttonUpgrading);
            }
            else
            {
                StartCoroutine(FlashCreditsRed(player));
            }
        }
    }

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
