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
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Selective GUI Placement - Depending on player count selected, edit layout and text in the shop.
     * ------------------------------------------------------------------------------------------------------------------ */
    private void Player1OnlyGUI()
    {
        Vector3 plr1UiStartPos = ShopRefs.plrShopUis[0].transform.localPosition;
        ShopRefs.plrShopUis[0].transform.localPosition = new Vector3(0f, plr1UiStartPos.y, plr1UiStartPos.z);
        ShopRefs.shopDivider.SetActive(false);
        ShopRefs.readyPromptText.text = $"Press 'Ready' to continue to Area {data.level + 1}...";
    }

    private void Player1And2GUI()
    {
        ShopRefs.plr2GameUi.SetActive(true);
        ShopRefs.plrShopUis[1].gameObject.SetActive(true);
        ShopRefs.readyPromptText.text = $"Both players 'Ready' to continue to Area {data.level + 1}...";
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * 
     * ------------------------------------------------------------------------------------------------------------------ */

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
