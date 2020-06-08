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
            if (BetweenScenesScript.UpgradesP1[whichUpgrade] < upgradeCap)
            {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP1[whichUpgrade] - 10);
                if (BetweenScenesScript.player1TempCredits >= price)
                {
                    BetweenScenesScript.UpgradesP1[whichUpgrade] += 1;
                    BetweenScenesScript.player1TempCredits -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenesScript.UpgradesP1)} Credits left: {BetweenScenesScript.player1TempCredits}");
                }
                else
                {
                    print("Upgrade failed, not enough credits");
                }
            }
        }
        UpdateButtonText();
    }

    public void PerformUpgradeP2(int whichUpgrade)
    {
        if (ShopRefs.p2Events.currentSelectedGameObject.name.StartsWith("P2"))
        {
            if (BetweenScenesScript.UpgradesP2[whichUpgrade] < upgradeCap)
            {
                int price = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP2[whichUpgrade] - 10);
                if (BetweenScenesScript.player2TempCredits >= price)
                {
                    BetweenScenesScript.UpgradesP2[whichUpgrade] += 1;
                    BetweenScenesScript.player2TempCredits -= price;
                    print($"Upgrades: {string.Join(",", BetweenScenesScript.UpgradesP2)} Credits left: {BetweenScenesScript.player2TempCredits}");
                }
                else
                {
                    print("Upgrade failed, not enough credits");
                }
            }
        }
        UpdateButtonText();
    }

    // Give a life to player number 'i'
    public void GiveLife(int playerSendingLife)
    {
        if (playerSendingLife == 1 && BetweenScenesScript.player1TempLives > 1)
        {
            BetweenScenesScript.player1TempLives -= 1;
            BetweenScenesScript.player2TempLives += 1;
            if (BetweenScenesScript.player2TempLives == 1)
            {
                ShopRefs.p2ShieldBar.fillAmount = 1f;
            }
        }
        if (playerSendingLife == 2 && BetweenScenesScript.player2TempLives > 1)
        {
            BetweenScenesScript.player2TempLives -= 1;
            BetweenScenesScript.player1TempLives += 1;
            if (BetweenScenesScript.player1TempLives == 1)
            {
                ShopRefs.p1ShieldBar.fillAmount = 1f;
            }
        }
        UpdateButtonText();
    }
}
