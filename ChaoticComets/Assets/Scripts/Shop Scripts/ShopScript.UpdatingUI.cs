using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI code
     * ------------------------------------------------------------------------------------------------------------------ */
    private void PrepareP1UI()
    {
        ShopRefs.p1Events.gameObject.SetActive(true);
        ShopRefs.p1ShieldBar.fillAmount = data.player1health / 80;
        BetweenScenesScript.player1TempCredits = data.player1credits;
        BetweenScenesScript.player1TempLives = data.player1lives;
        for (int i = 0; i < 5; i++)
        {
            if (data.player1powerups[i] == 1)
                ShopRefs.listOfP1Powerups[i].SetActive(true);
        }
        ShopRefs.readyPromptText.text = $"Press 'Ready' to\nContinue to Level {data.level + 1}...";
    }

    private void PrepareP2UI()
    {
        ShopRefs.p2Events.gameObject.SetActive(true);
        ShopRefs.player2GUI.SetActive(true);
        ShopRefs.p2ShieldBar.fillAmount = data.player2health / 80;
        BetweenScenesScript.player2TempCredits = data.player2credits;
        BetweenScenesScript.player2TempLives = data.player2lives;
        for (int i = 0; i < 5; i++)
        {
            if (data.player2powerups[i] == 1)
                ShopRefs.listOfP2Powerups[i].SetActive(true);
        }
        ShopRefs.readyPromptText.text = $"Both players 'Ready' to\nContinue to Level {data.level + 1}...";
    }


    // If One Player mode is selected, edit where the buttons and text are in the shop.
    private void Player1OnlyGUI()
    {
        Button[] listOfButtons = FindObjectsOfType<Button>();
        foreach (Button gameObj in listOfButtons)
        {
            if (gameObj.transform.name.StartsWith("P1"))
            {
                float tempYPosition = gameObj.transform.localPosition.y;
                gameObj.transform.localPosition = new Vector3(430, tempYPosition);
            }
            if (gameObj.transform.name.EndsWith("Transfer"))
            {
                gameObj.gameObject.SetActive(false);
            }
            float tempXPosition = gameObj.transform.localPosition.x;
            float tempYPosition2 = gameObj.transform.localPosition.y + 30;
            gameObj.transform.localPosition = new Vector3(tempXPosition, tempYPosition2);
        }
        TextMeshProUGUI[] listOfTextBoxes = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI gameObj in listOfTextBoxes)
        {
            if (gameObj.transform.name == "P1UpgradesTitle")
                gameObj.transform.localPosition = new Vector3(430, 7);
            if (gameObj.transform.name == "UpgLifeTransfer")
                gameObj.gameObject.SetActive(false);

            else if (gameObj.transform.name.StartsWith("Upg"))
            {
                float tempYPosition = gameObj.transform.localPosition.y;
                gameObj.transform.localPosition = new Vector3(-144, tempYPosition);
                float tempXPosition = gameObj.transform.localPosition.x;
                float tempYPosition2 = gameObj.transform.localPosition.y + 30;
                gameObj.transform.localPosition = new Vector3(tempXPosition, tempYPosition2);
            }
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Button Updating code
     * ------------------------------------------------------------------------------------------------------------------ */

    // If a button pressed begins with 'P1' or 'P2', and does not end with 'Ready' or 'Transfer', then update the button's text based on BetweenScenesScript variables
    // If button ends with 'Transfer', then change the life transfer button text
    private void UpdateButtonText()
    {
        Button[] listOfButtons = GameObject.FindObjectsOfType<Button>();
        foreach (Button gameObj in listOfButtons)
        {
            string tempName = gameObj.transform.name;
            tempName = tempName.Substring(tempName.Length - 1, 1);
            if (int.TryParse(tempName, out int i))
            {
                i = int.Parse(tempName);
            }
            int priceP1 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP1[i] - 10);
            int priceP2 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenesScript.UpgradesP2[i] - 10);
            string upgradeTier; int tempUpgradeNumLength;

            if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready"))
            {
                if (!gameObj.transform.name.EndsWith("Transfer"))
                {
                    tempUpgradeNumLength = BetweenScenesScript.UpgradesP1[i].ToString().Length - 1;
                    upgradeTier = BetweenScenesScript.UpgradesP1[i].ToString().Insert(tempUpgradeNumLength, ".");
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Upgrade: {priceP1}c)";
                    if (BetweenScenesScript.UpgradesP1[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Maximum upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenesScript.PlayerCount == 2)
                    {
                        if (BetweenScenesScript.player1TempLives > 1) { gameObj.GetComponentInChildren<Text>().text = "Multiple Lives\n(Transfer 1 life to P2)"; }
                        else if (BetweenScenesScript.player1TempLives == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                        else
                        { /*(BetweenScenesScript.player1TempLives < 1)*/
                            gameObj.GetComponentInChildren<Text>().text = "No Lives";
                        }
                    }
                }
            }
            else if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready"))
            {
                if (!gameObj.transform.name.EndsWith("Transfer"))
                {
                    tempUpgradeNumLength = BetweenScenesScript.UpgradesP2[i].ToString().Length - 1;
                    upgradeTier = BetweenScenesScript.UpgradesP2[i].ToString().Insert(tempUpgradeNumLength, ".");
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Upgrade: {priceP2}c)";
                    if (BetweenScenesScript.UpgradesP2[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Maximum upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenesScript.PlayerCount == 2)
                    {
                        if (BetweenScenesScript.player2TempLives > 1) { gameObj.GetComponentInChildren<Text>().text = "Multiple Lives\n(Transfer 1 life to P1)"; }
                        else if (BetweenScenesScript.player2TempLives == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                        else { /*(BetweenScenesScript.player2TempLives < 1)*/ gameObj.GetComponentInChildren<Text>().text = "No Lives"; }
                    }
                }
            }
            ShopRefs.p1ScoreText.text = "Credits:\n" + BetweenScenesScript.player1TempCredits;
            ShopRefs.p1LivesText.text = "Lives: " + BetweenScenesScript.player1TempLives;
            if (data.playerCount == 2)
            {
                ShopRefs.p2ScoreText.text = "Credits:\n" + BetweenScenesScript.player2TempCredits;
                ShopRefs.p2LivesText.text = "Lives: " + BetweenScenesScript.player2TempLives;
            }
        }
    }
}
