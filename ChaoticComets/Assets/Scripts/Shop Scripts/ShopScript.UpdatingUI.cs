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
        ShopRefs.listOfPlrTotalScoreText[plrToPrep].text = "T: " + data.playerList[plrToPrep].totalCredits;
        ShopRefs.plrShipsText.text = "Ships: " + data.lives;
        BetweenScenes.PlayerShopCredits[plrToPrep] = data.playerList[plrToPrep].credits;
        BetweenScenes.PlayerShopLives = data.lives;
        BetweenScenes.PlayerShopShields[plrToPrep] = data.playerList[plrToPrep].health;
        for (int i = 0; i < 5; i++)
        {
            if (data.playerList[plrToPrep].powerups[i] == 1)
            {
                ShopRefs.listOfPlrPowerups[plrToPrep][i].SetActive(true);
            }
        }
        ShopRefs.readyPromptText.text = $"Press 'Ready' to continue to Level {data.level + 1}...";

        if (plrToPrep == 1)
        {
            //ShopRefs.player2GUI.SetActive(true);
            ShopRefs.readyPromptText.text = $"Both players 'Ready' to continue to Level {data.level + 1}...";
        }
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player 1 Only GUI - If One Player mode is selected, edit where the buttons and text are in the shop.
     * ------------------------------------------------------------------------------------------------------------------ */
    private void Player1OnlyGUI()
    {
        // TODO Remove divider and shift Shop UI over if only in 1 Player Mode
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * MainButtonHovered - Change what's shown on the purchasing panel when a button is hovered over.
     * ------------------------------------------------------------------------------------------------------------------ */
    public void MainButtonHovered(int plrIndex)
    {
        // Find button pressed, turn the last character in the name into an integer
        Button buttonHovered = ShopRefs.plrEventSystems[plrIndex].currentSelectedGameObject.GetComponent<Button>();
        int whichHovered = int.Parse(buttonHovered.name.Last().ToString());

        ShopRefs.plrPurchasePanels[plrIndex].SetTextElements(whichHovered);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * UpdatePlayerCounters - After any button edit, update text of player's credits, and both player's lives
     * ------------------------------------------------------------------------------------------------------------------ */
    private void UpdatePlayerCounters(int plrIndex)
    {
        ShopRefs.listOfPlrScoreText[plrIndex].text = BetweenScenes.PlayerShopCredits[plrIndex] + "c";
    }
}
