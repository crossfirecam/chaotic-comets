using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Rewired;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI code
     * ------------------------------------------------------------------------------------------------------------------ */
    private void PrepareUI(int plrToPrep)
    {
        plrToPrep -= 1; // Make compatible with array

        ShopRefs.listOfPlrShieldBars[plrToPrep].fillAmount = data.playerList[plrToPrep].health / 80;
        BetweenScenes.playerShopCredits[plrToPrep] = data.playerList[plrToPrep].credits;
        BetweenScenes.playerShopLives[plrToPrep] = data.playerList[plrToPrep].lives;
        for (int i = 0; i < 5; i++)
        {
            if (data.playerList[plrToPrep].powerups[i] == 1)
            {
                ShopRefs.listOfPlrPowerups[plrToPrep][i].SetActive(true);
            }
        }
        ShopRefs.plrEventSystems[plrToPrep].gameObject.SetActive(true);
        ShopRefs.readyPromptText.text = $"Press 'Ready' to\nContinue to Level {data.level + 1}...";

        if (plrToPrep == 1)
        {
            ShopRefs.player2GUI.SetActive(true);
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
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n({priceP1}c to Upgrade)";
                    if (BetweenScenes.UpgradesP1[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Max upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenes.PlayerCount == 2)
                    {
                        if (BetweenScenes.playerShopLives[0] > 1) { gameObj.GetComponentInChildren<Text>().text = "Transfer 1 life to P2\n(Cost: 500c)"; }
                        else if (BetweenScenes.playerShopLives[0] == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
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
                    gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n({priceP2}c to Upgrade)";
                    if (BetweenScenes.UpgradesP2[i] == upgradeCap)
                    {
                        gameObj.GetComponentInChildren<Text>().text = $"Current: x{upgradeTier}\n(Max upgrade)";
                    }
                }
                else
                {
                    if (BetweenScenes.PlayerCount == 2)
                    {
                        if (BetweenScenes.playerShopLives[1] > 1) { gameObj.GetComponentInChildren<Text>().text = "Transfer 1 life to P1\n(Cost: 500c)"; }
                        else if (BetweenScenes.playerShopLives[1] == 1) { gameObj.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                        else { /*(BetweenScenesScript.player2TempLives < 1)*/ gameObj.GetComponentInChildren<Text>().text = "No Lives"; }
                    }
                }
            }

            for (int j = 0; j < BetweenScenes.PlayerCount; j++)
            {
                ShopRefs.listOfPlrScoreText[j].text = BetweenScenes.playerShopCredits[j] + "c";
                ShopRefs.listOfPlrLivesText[j].text = "Lives: " + BetweenScenes.playerShopLives[j];
            }
        }
    }
}
