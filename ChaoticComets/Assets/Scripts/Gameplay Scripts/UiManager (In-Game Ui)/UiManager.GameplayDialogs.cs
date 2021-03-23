using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public partial class UiManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Congratulations Dialog - Methods to do with players passing a level
     * ------------------------------------------------------------------------------------------------------------------ */
    [Header("Congrats Dialog UI")]
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject gameLevelShieldRechargeText, congratsNoBonusText;
    [SerializeField] private TextMeshProUGUI congratsBonusText;
    [SerializeField] private AudioSource congratsBonusAudSrc;
    public void LevelCompleted(int bonusAmount)
    {
        panelCongrats.SetActive(true);
        StartCoroutine(CountdownBonus(bonusAmount));
    }

    private IEnumerator CountdownBonus(int bonusAmount)
    {
        yield return new WaitForSeconds(2);
        if (bonusAmount == 0)
        {
            congratsNoBonusText.SetActive(true);
        }
        else
        {
            congratsBonusText.gameObject.SetActive(true);
            for (int i = bonusAmount; i > 0; i -= 10)
            {
                GameManager.i.Refs.playerShip1.credits += 10;
                GameManager.i.Refs.playerShip1.totalCredits += 10;
                GameManager.i.Refs.playerShip2.credits += 10;
                GameManager.i.Refs.playerShip2.totalCredits += 10;

                int[] p1Cred = { GameManager.i.Refs.playerShip1.credits, GameManager.i.Refs.playerShip1.totalCredits };
                int[] p2Cred = { GameManager.i.Refs.playerShip2.credits, GameManager.i.Refs.playerShip2.totalCredits };
                UiManager.i.SetPlayerCredits(0, p1Cred[0], p1Cred[1]);
                UiManager.i.SetPlayerCredits(1, p2Cred[0], p2Cred[1]);

                congratsBonusText.text = "Time Bonus: " + i;
                congratsBonusAudSrc.Play();
                yield return new WaitForSeconds(0.04f);
            }
            congratsBonusText.text = "Time Bonus: 0";
        }
        StartCoroutine(FadeScreenBlack("to"));
        GameManager.i.Invoke("BringUpShop", 2f);
    }

    // If a ship has less than full shields, show the text say shields are being recharged
    public void ShowRechargeText()
    {
        gameLevelShieldRechargeText.SetActive(true);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Tutorial Dialog - Methods to do with players beginning the tutorial
     * ------------------------------------------------------------------------------------------------------------------ */
    [Header("Tutorial Start Dialog UI")]
    [SerializeField] private GameObject panelTutorialChoice;
    [SerializeField] private Button buttonWhenTutorialChoice;
    public void DisplayTutorialChoiceDialog()
    {
        Time.timeScale = 0;
        panelTutorialChoice.SetActive(true);
        buttonWhenTutorialChoice.Select();
    }
    public void DismissTutorialChoiceDialog()
    {
        Time.timeScale = 1;
        panelTutorialChoice.SetActive(false);
        buttonWhenTutorialChoice.Select();
    }

    public bool GameIsOnTutorialScreen()
    {
        return panelTutorialChoice.activeInHierarchy;
    }
}
