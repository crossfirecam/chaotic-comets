using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System.Collections.Generic;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Prepare UI - Fill shield bar, credit/lives counters
     * ------------------------------------------------------------------------------------------------------------------ */
    private void PrepareUI(int plrToPrep)
    {
        ShopRefs.listOfPlrShieldBars[plrToPrep].fillAmount = data.playerList[plrToPrep].health / 80;
        ShopRefs.listOfPlrTotalScoreText[plrToPrep].text = "Total Score:\n" + data.playerList[plrToPrep].totalCredits;
        BetweenScenes.PlayerShopCredits[plrToPrep] = data.playerList[plrToPrep].credits;
        BetweenScenes.PlayerShopLives[plrToPrep] = data.playerList[plrToPrep].lives;
        for (int i = 0; i < 5; i++)
        {
            if (data.playerList[plrToPrep].powerups[i] == 1)
            {
                ShopRefs.listOfPlrPowerups[plrToPrep][i].SetActive(true);
            }
        }
        ShopRefs.readyPromptText.text = $"Press 'Ready' to\nContinue to Level {data.level + 1}...";

        if (plrToPrep == 1)
        {
            ShopRefs.player2GUI.SetActive(true);
            ShopRefs.readyPromptText.text = $"Both players 'Ready' to\nContinue to Level {data.level + 1}...";
        }
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player 1 Only GUI - If One Player mode is selected, edit where the buttons and text are in the shop.
     * ------------------------------------------------------------------------------------------------------------------ */
    private void Player1OnlyGUI()
    {
        Button[] listOfButtons = FindObjectsOfType<Button>();
        foreach (Button button in listOfButtons)
        {
            Transform btnTra = button.transform;
            if (btnTra.name.StartsWith("P1"))
            {
                float tempY = btnTra.localPosition.y + 30;
                btnTra.localPosition = new Vector3(430, tempY);
            }
            if (btnTra.name.EndsWith("Transfer"))
            {
                button.gameObject.SetActive(false);
            }
        }
        TextMeshProUGUI[] listOfTextBoxes = FindObjectsOfType<TextMeshProUGUI>();
        foreach (TextMeshProUGUI text in listOfTextBoxes)
        {
            Transform txtTra = text.transform;
            if (txtTra.name == "P1UpgradesTitle")
            {
                txtTra.localPosition = new Vector3(430, 7);
                continue;
            }
            if (txtTra.name == "UpgLifeTransfer")
                txtTra.gameObject.SetActive(false);

            else if (txtTra.name.StartsWith("Upg"))
            {
                float tempY = txtTra.localPosition.y + 30;
                txtTra.localPosition = new Vector3(-144, tempY);
            }
        }
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Button Updating code
     * ------------------------------------------------------------------------------------------------------------------ */
    private List<Button> listOfAllButtons = new List<Button>();
    private List<Button> listOfFilteredButtons = new List<Button>();
    private void UpdateButtonText()
    {
        // If filtered button list is empty, fill it with updatable buttons
        if (!listOfFilteredButtons.Any()) {
            listOfAllButtons = FindObjectsOfType<Button>().ToList();
            foreach (Button button in listOfAllButtons)
            {
                // The list-populating is exclusion-based because most of the desired buttons end in different single digits
                // These unwanted buttons are for a UI selection bug, checking for mouse input, and readying the players respectively
                if (!button.name.EndsWith("Bug") && !button.name.EndsWith("Check") && !button.name.EndsWith("Ready"))
                {
                    listOfFilteredButtons.Add(button);
                }
            }
        }
        foreach (Button button in listOfFilteredButtons)
        {
            // If button name does not end with 'Transfer'... It is an upgrade button.
            // Update the button's text based on BetweenScenes variables.
            if (!button.name.EndsWith("Transfer"))
            {
                // Buttons that don't match any EndsWith criteria above are upgrade buttons
                int upgrade = int.Parse(button.name.Last().ToString());
                int player = int.Parse(button.name[1].ToString());
                int plrIndex = player - 1;
                int currentUpgradeTier = BetweenScenes.PlayerShopUpgrades[plrIndex][upgrade];

                // Determine price for the upgrade
                int tierComparedToBase = currentUpgradeTier - 10;
                int priceForUpgrade = baseUpgradePrice + (priceIncreasePerLevel * tierComparedToBase);

                // Set text for button
                string tierInDecimalForm = currentUpgradeTier.ToString().Insert(currentUpgradeTier.ToString().Length - 1, ".");
                button.GetComponentInChildren<Text>().text = $"Current: x{tierInDecimalForm}\n({priceForUpgrade}c to Upgrade)";
                if (currentUpgradeTier == upgradeCap)
                {
                    button.GetComponentInChildren<Text>().text = $"Current: x{tierInDecimalForm}\n(Max upgrade)";
                }
            }
            // If button ends with 'Transfer', then change the life transfer button text.
            else if (button.name.EndsWith("Transfer"))
            {
                if (BetweenScenes.PlayerCount == 2)
                {
                    int player = int.Parse(button.name[1].ToString());
                    int plrIndex = player - 1;
                    int otherPlayer = 0;
                    switch (player)
                    {
                        case 1: otherPlayer = 2; break;
                        case 2: otherPlayer = 1; break;
                    }
                    if (BetweenScenes.PlayerShopLives[plrIndex] > 1) { button.GetComponentInChildren<Text>().text = $"Transfer 1 life to P{otherPlayer}\n(Cost: 500c)"; }
                    else if (BetweenScenes.PlayerShopLives[plrIndex] == 1) { button.GetComponentInChildren<Text>().text = "One Life\n(Cannot transfer)"; }
                    else { button.GetComponentInChildren<Text>().text = "No Lives"; }
                }
            }
        }
        // After any button edit, change player's life and credits counter
        for (int j = 0; j < BetweenScenes.PlayerCount; j++)
        {
            ShopRefs.listOfPlrScoreText[j].text = BetweenScenes.PlayerShopCredits[j] + "c";
            ShopRefs.listOfPlrLivesText[j].text = "Lives: " + BetweenScenes.PlayerShopLives[j];
        }
    }
}
