using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI code
     * ------------------------------------------------------------------------------------------------------------------ */
    private void PrepareUI(int playerToPrep)
    {
        if (playerToPrep == 1)
        {
            ShopRefs.p1Events.gameObject.SetActive(true);
            ShopRefs.p1ShieldBar.fillAmount = data.playerList[0].health / 80;
            BetweenScenes.player1ShopCredits = data.playerList[0].credits;
            BetweenScenes.player1ShopLives = data.playerList[0].lives;
            for (int i = 0; i < 5; i++)
            {
                if (data.playerList[0].powerups[i] == 1)
                    ShopRefs.listOfP1Powerups[i].SetActive(true);
            }
            ShopRefs.readyPromptText.text = $"Press 'Ready' to\nContinue to Level {data.level + 1}...";
        }
        else if (playerToPrep == 2)
        {
            ShopRefs.p2Events.gameObject.SetActive(true);
            ShopRefs.player2GUI.SetActive(true);
            ShopRefs.p2ShieldBar.fillAmount = data.playerList[0].health / 80;
            BetweenScenes.player2ShopCredits = data.playerList[0].credits;
            BetweenScenes.player2ShopLives = data.playerList[0].lives;
            for (int i = 0; i < 5; i++)
            {
                if (data.playerList[0].powerups[i] == 1)
                    ShopRefs.listOfP2Powerups[i].SetActive(true);
            }
            ShopRefs.readyPromptText.text = $"Both players 'Ready' to\nContinue to Level {data.level + 1}...";
        }
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

    // If a button pressed begins with 'P1' or 'P2', and does not end with 'Ready' or 'Transfer', then update the button's text based on BetweenScenes variables
    // If button ends with 'Transfer', then change the life transfer button text
    private void UpdateButtonText()
    {
        Button[] listOfButtons = FindObjectsOfType<Button>();
        foreach (Button gameObj in listOfButtons)
        {
            string tempName = gameObj.transform.name;
            tempName = tempName.Substring(tempName.Length - 1, 1);
            if (int.TryParse(tempName, out int i))
            {
                i = int.Parse(tempName);
            }
            int priceP1 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP1[i] - 10);
            int priceP2 = baseUpgradePrice + priceIncreasePerLevel * (BetweenScenes.UpgradesP2[i] - 10);
            string upgradeTier; int tempUpgradeNumLength;

            if (gameObj.transform.name.StartsWith("P1") && !gameObj.transform.name.EndsWith("Ready"))
            {
                if (!gameObj.transform.name.EndsWith("Transfer"))
                {
                    tempUpgradeNumLength = BetweenScenes.UpgradesP1[i].ToString().Length - 1;
                    upgradeTier = BetweenScenes.UpgradesP1[i].ToString().Insert(tempUpgradeNumLength, ".");
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n{priceP1}c to Upgrade";
                    if (BetweenScenes.UpgradesP1[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Max upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenes.PlayerCount == 2)
                    {
                        if (BetweenScenes.player1ShopLives > 1) { gameObj.GetComponentInChildren<Text>().text = "Multiple Lives\n(Transfer 1 life to P2)"; }
                        else if (BetweenScenes.player1ShopLives == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                        else { /*(BetweenScenes.player1TempLives < 1)*/ gameObj.GetComponentInChildren<Text>().text = "No Lives"; }
                    }
                }
            }
            else if (gameObj.transform.name.StartsWith("P2") && !gameObj.transform.name.EndsWith("Ready"))
            {
                if (!gameObj.transform.name.EndsWith("Transfer"))
                {
                    tempUpgradeNumLength = BetweenScenes.UpgradesP2[i].ToString().Length - 1;
                    upgradeTier = BetweenScenes.UpgradesP2[i].ToString().Insert(tempUpgradeNumLength, ".");
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n{priceP2}c to Upgrade";
                    if (BetweenScenes.UpgradesP2[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Max upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenes.PlayerCount == 2)
                    {
                        if (BetweenScenes.player2ShopLives > 1) { gameObj.GetComponentInChildren<Text>().text = "Multiple Lives\n(Transfer 1 life to P1)"; }
                        else if (BetweenScenes.player2ShopLives == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                        else { /*(BetweenScenesScript.player2TempLives < 1)*/ gameObj.GetComponentInChildren<Text>().text = "No Lives"; }
                    }
                }
            }
            ShopRefs.p1ScoreText.text = BetweenScenes.player1ShopCredits + "c";
            ShopRefs.p1LivesText.text = "Lives: " + BetweenScenes.player1ShopLives;
            ShopRefs.p1TotalScoreText.text = "Total Score:\n" + data.playerList[0].totalCredits;
            if (data.playerCount == 2)
            {
                ShopRefs.p2ScoreText.text = BetweenScenes.player2ShopCredits + "c";
                ShopRefs.p2LivesText.text = "Lives: " + BetweenScenes.player2ShopLives;
                ShopRefs.p2TotalScoreText.text = "Total Score:\n" + data.playerList[1].totalCredits;
            }
        }
    }
}
