using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public partial class ShopScript : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Prepare UI - Fill shield bar, credit/lives counters
     * ------------------------------------------------------------------------------------------------------------------ */
    private void PrepareUI(int plrToPrep)
    {
        BetweenScenes.PlayerShopCredits[plrToPrep] = data.playerList[plrToPrep].credits;
        BetweenScenes.PlayerShopLives = data.lives;
        BetweenScenes.PlayerShopShields[plrToPrep] = data.playerList[plrToPrep].health;

        UpdatePlayerUI(plrToPrep);
        ShopRefs.listOfPlrTotalScoreText[plrToPrep].text = "T: " + data.playerList[plrToPrep].totalCredits;
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
            ShopRefs.plrMainPanels[1].gameObject.SetActive(true);
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
    public void PurchaseSucceeded(int plrIndex)
    {
        UpdatePlayerUI(plrIndex);
    }

    public void PurchaseFailed(int plrIndex)
    {
        StartCoroutine(FlashCreditsRed(plrIndex));
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * UpdatePlayerCounters - After any button edit, update text of player's credits
     * ------------------------------------------------------------------------------------------------------------------ */
    private void UpdatePlayerUI(int plrIndex)
    {
        ShopRefs.listOfPlrScoreText[plrIndex].text = BetweenScenes.PlayerShopCredits[plrIndex] + "¢";
        ShopRefs.plrShipsText.text = "Ships: " + BetweenScenes.PlayerShopLives;
        ShopRefs.listOfPlrShieldBars[plrIndex].fillAmount = BetweenScenes.PlayerShopShields[plrIndex] / 80;
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
}
