using System;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{

    // Code for shop's Ready system. If both players are ready, shop closes.
    public void ReadyUp(int plrReady)
    {
        plrReady -= 1; // Make compatible with array
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
            print(btn.transform.name);
            if (btn.transform.name.StartsWith(btnStartToCheck) && !btn.transform.name.EndsWith("Ready"))
            {
                btn.GetComponent<Button>().interactable = !plrIsReady[plrReady];
            }
        }

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
    * Upgrades code.
    * They must be separate functions due to how Unity handles button press scripting. Only one variable can be passed per button.
    * ------------------------------------------------------------------------------------------------------------------ */

    public void PerformUpgradeP1(int whichUpgrade)
    {
        Button buttonUpgrading = ShopRefs.plrEventSystems[0].currentSelectedGameObject.GetComponent<Button>();
        if (buttonUpgrading.name.StartsWith("P1"))
        {
            if (BetweenScenes.UpgradesP1[whichUpgrade] < upgradeCap)
            {

                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP1[whichUpgrade] - 10);
                if (BetweenScenes.playerShopCredits[0] >= price)
                {
                    buttonUpgrading.GetComponent<AudioSource>().Play();
                    BetweenScenes.UpgradesP1[whichUpgrade] += 1;
                    BetweenScenes.playerShopCredits[0] -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenes.UpgradesP1)} Credits left: {BetweenScenes.playerShopCredits[0]}");
                }
                else
                {
                    StartCoroutine(FlashCreditsRed(0));
                }
            }
        }
        UpdateButtonText();
    }

    public void PerformUpgradeP2(int whichUpgrade)
    {
        Button buttonUpgrading = ShopRefs.plrEventSystems[1].currentSelectedGameObject.GetComponent<Button>();
        if (buttonUpgrading.name.StartsWith("P2"))
        {
            if (BetweenScenes.UpgradesP2[whichUpgrade] < upgradeCap)
            {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP2[whichUpgrade] - 10);
                if (BetweenScenes.playerShopCredits[1] >= price)
                {
                    buttonUpgrading.GetComponent<AudioSource>().Play();
                    BetweenScenes.UpgradesP2[whichUpgrade] += 1;
                    BetweenScenes.playerShopCredits[1] -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenes.UpgradesP2)} Credits left: {BetweenScenes.playerShopCredits[1] }");
                }
                else
                {
                    StartCoroutine(FlashCreditsRed(1));
                }
            }
        }
        UpdateButtonText();
    }

    // Give a life to player number 'i'
    public void GiveLife(int plrSendingLife)
    {
        plrSendingLife -= 1; // Make compatible with array
        int plrReceivingLife;

        // TODO support more than two players. Probably automatically determine the player with the least lives.
        if (plrSendingLife == 0) { plrReceivingLife = 1; }
        else /*(plrSendingLife == 1)*/ { plrReceivingLife = 0; }

        // If player can send a life, then give a life
        if (BetweenScenes.playerShopLives[plrSendingLife] > 1 && BetweenScenes.playerShopCredits[plrSendingLife] >= 500)
        {
            Button buttonGivingLife = ShopRefs.plrEventSystems[0].currentSelectedGameObject.GetComponent<Button>();
            buttonGivingLife.GetComponent<AudioSource>().Play();

            BetweenScenes.playerShopCredits[plrSendingLife] -= 500;
            BetweenScenes.playerShopLives[plrSendingLife] -= 1;
            BetweenScenes.playerShopLives[plrReceivingLife] += 1;
            // Reset dead player's shields to full when receiving their first new life
            if (BetweenScenes.playerShopLives[plrReceivingLife] == 1)
            {
                ShopRefs.listOfPlrShieldBars[plrReceivingLife].fillAmount = 1f;
            }
        }
        else
        {
            // If life transfer fails, flash one or both of the offending statistics
            if (BetweenScenes.playerShopCredits[plrSendingLife] < 500)
            {
                StartCoroutine(FlashCreditsRed(plrSendingLife));
            }
            if (BetweenScenes.playerShopLives[plrSendingLife] == 1)
            {
                StartCoroutine(FlashLivesRed(plrSendingLife));
            }
        }
        UpdateButtonText();
    }

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
    private bool[] isAlreadyFlashingLives = { false, false };
    private IEnumerator FlashLivesRed(int playerFlashing)
    {
        if (!isAlreadyFlashingLives[playerFlashing])
        {
            GetComponent<AudioSource>().Play(); // ShopManager contains a 'UiError'-playing AudioSource
            isAlreadyFlashingLives[playerFlashing] = true;
            ShopRefs.listOfPlrLivesText[playerFlashing].color = Color.red;
            yield return new WaitForSeconds(.5f);
            ShopRefs.listOfPlrLivesText[playerFlashing].color = Color.white;
            isAlreadyFlashingLives[playerFlashing] = false;
        }
    }
}
