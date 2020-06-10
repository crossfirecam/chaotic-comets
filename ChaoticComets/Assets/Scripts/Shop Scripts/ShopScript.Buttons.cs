using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public partial class ShopScript : MonoBehaviour
{

    // Code for shop's Ready system. If both players are ready, shop closes.
    public void ReadyUp(int plrReady)
    {
        // 
        Navigation customNav = new Navigation { mode = Navigation.Mode.Explicit };
        Button[] listOfButtons = FindObjectsOfType<Button>();

        if (plrReady == 1)
        {
            p1IsReady = !p1IsReady;
            ShopRefs.p1ReadyButton.GetComponentInChildren<Text>().text = p1IsReady ? "Unready" : "Ready";
            customNav.selectOnUp = p1IsReady ? null : ShopRefs.p1UpgBtnAboveReady;
            ShopRefs.p1ReadyButton.GetComponent<Button>().navigation = customNav;
        }
        else
        {
            p2IsReady = !p2IsReady;
            ShopRefs.p2ReadyButton.GetComponentInChildren<Text>().text = p2IsReady ? "Unready" : "Ready";
            customNav.selectOnUp = p2IsReady ? null : ShopRefs.p2UpgBtnAboveReady;
            ShopRefs.p2ReadyButton.GetComponent<Button>().navigation = customNav;
        }

        foreach (Button gameObj in listOfButtons)
        { // Find all P1 buttons that aren't 'Ready' button, and disable them
            if (gameObj.transform.name.StartsWith(plrReady == 1 ? "P1" : "P2") && !gameObj.transform.name.EndsWith("Ready"))
            {
                gameObj.GetComponent<Button>().interactable = plrReady == 1 ? !p1IsReady : !p2IsReady;
            }
        }

        if (p1IsReady && p2IsReady)
        {
            ShopRefs.p1ReadyButton.GetComponentInChildren<Text>().text = "";
            ShopRefs.p2ReadyButton.GetComponentInChildren<Text>().text = "";
            ShopRefs.p1ReadyButton.GetComponent<Button>().interactable = false;
            ShopRefs.p2ReadyButton.GetComponent<Button>().interactable = false;
            GoBackToGame();
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Upgrades code.
    * They must be separate functions due to how Unity handles button press scripting. Only one variable can be passed per button.
    * ------------------------------------------------------------------------------------------------------------------ */

    public void PerformUpgradeP1(int whichUpgrade)
    {
        if (ShopRefs.p1Events.currentSelectedGameObject.name.StartsWith("P1"))
        {
            if (BetweenScenes.UpgradesP1[whichUpgrade] < upgradeCap)
            {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP1[whichUpgrade] - 10);
                if (BetweenScenes.player1ShopCredits >= price)
                {
                    BetweenScenes.UpgradesP1[whichUpgrade] += 1;
                    BetweenScenes.player1ShopCredits -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenes.UpgradesP1)} Credits left: {BetweenScenes.player1ShopCredits}");
                }
                else
                {
                    StartCoroutine(FlashCreditsRed(1));
                }
            }
        }
        UpdateButtonText();
    }

    public void PerformUpgradeP2(int whichUpgrade)
    {
        if (ShopRefs.p2Events.currentSelectedGameObject.name.StartsWith("P2"))
        {
            if (BetweenScenes.UpgradesP2[whichUpgrade] < upgradeCap)
            {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP2[whichUpgrade] - 10);
                if (BetweenScenes.player2ShopCredits >= price)
                {
                    BetweenScenes.UpgradesP2[whichUpgrade] += 1;
                    BetweenScenes.player2ShopCredits -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenes.UpgradesP2)} Credits left: {BetweenScenes.player2ShopCredits}");
                }
                else
                {
                    StartCoroutine(FlashCreditsRed(2));
                }
            }
        }
        UpdateButtonText();
    }

    // Give a life to player number 'i'
    public void GiveLife(int playerSendingLife)
    {
        if (playerSendingLife == 1 && BetweenScenes.player1ShopLives > 1)
        {
            BetweenScenes.player1ShopLives -= 1;
            BetweenScenes.player2ShopLives += 1;
            if (BetweenScenes.player2ShopLives == 1)
            {
                ShopRefs.p2ShieldBar.fillAmount = 1f;
            }
        }
        if (playerSendingLife == 2 && BetweenScenes.player2ShopLives > 1)
        {
            BetweenScenes.player2ShopLives -= 1;
            BetweenScenes.player1ShopLives += 1;
            if (BetweenScenes.player1ShopLives == 1)
            {
                ShopRefs.p1ShieldBar.fillAmount = 1f;
            }
        }
        UpdateButtonText();
    }

    private bool isAlreadyFlashingP1 = false, isAlreadyFlashingP2 = false;
    private IEnumerator FlashCreditsRed(int playerFlashing)
    {
        if (playerFlashing == 1 && !isAlreadyFlashingP1)
        {
            isAlreadyFlashingP1 = true;
            ShopRefs.p1ScoreText.color = Color.red;
            yield return new WaitForSeconds(.5f);
            ShopRefs.p1ScoreText.color = Color.white;
            isAlreadyFlashingP1 = false;
        }
        else if (playerFlashing == 2 && !isAlreadyFlashingP2)
        {
            isAlreadyFlashingP2 = true;
            ShopRefs.p2ScoreText.color = Color.red;
            yield return new WaitForSeconds(.5f);
            ShopRefs.p2ScoreText.color = Color.white;
            isAlreadyFlashingP2 = false;
        }
    }
}
