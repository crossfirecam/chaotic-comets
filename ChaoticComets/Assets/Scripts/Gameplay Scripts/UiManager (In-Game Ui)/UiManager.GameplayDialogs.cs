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
    [SerializeField] private TextMeshProUGUI congratsTitleText, congratsBonusText;
    [SerializeField] private AudioSource congratsBonusAudSrc;
    public void LevelCompleted(int bonusTime)
    {
        panelCongrats.SetActive(true);
        StartCoroutine(CountdownBonus(bonusTime));
    }

    /// <summary>
    /// Show 'Area Clear!' for two seconds.<br/>
    /// Show bonus congrats or failure text for one second.<br/>
    /// Start ticking down bonus if player has it, or skip.<br/>
    /// Wait two seconds, and fade screen to the shop.
    /// </summary>
    private IEnumerator CountdownBonus(int bonusTime)
    {
        int bonusToAward = FigureOutBonusAmount(bonusTime);
        yield return new WaitForSeconds(2);
        congratsTitleText.gameObject.SetActive(true);
        if (bonusToAward == 0)
        {
            yield return new WaitForSeconds(1);
            congratsNoBonusText.SetActive(true);
        }
        else
        {
            yield return new WaitForSeconds(1);
            congratsBonusText.gameObject.SetActive(true);
            congratsBonusText.text = "Time Bonus: " + bonusToAward;
            yield return new WaitForSeconds(1);

            for (int i = bonusToAward; i > 0; i -= 10)
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
        yield return new WaitForSeconds(2);
        StartCoroutine(UsefulFunctions.FadeScreenBlack("to", fadeBlackOverlay));
        GameManager.i.Invoke("BringUpShop", 2f);
    }

    private int FigureOutBonusAmount(int bonusTime)
    {
        int bonusToAward;
        int randResponse = Random.Range(0, 4);
        if (bonusTime == 0)
        {
            congratsTitleText.text = bonusResponses[0][randResponse];
            bonusToAward = 0;
        }
        else if (bonusTime > GameManager.i.savedMaxBonusLevel / 3f * 2)
        {
            congratsTitleText.text = bonusResponses[1][randResponse] + "\nHave a big bonus!";
            bonusToAward = 500;
        }
        else if (bonusTime > GameManager.i.savedMaxBonusLevel / 3f)
        {
            congratsTitleText.text = bonusResponses[2][randResponse] + "\nHere's a bonus.";
            bonusToAward = 350;
        }
        else
        {
            congratsTitleText.text = bonusResponses[3][randResponse] + "\nTake a small bonus.";
            bonusToAward = 200;
        }
        congratsTitleText.text = "\"" + congratsTitleText.text + "\""; // Quotation marks around response.
        return bonusToAward;
    }

    /// <summary>
    /// Stores responses for the congrats dialog. In order: 0 bonus, high bonus, medium bonus, low bonus.
    /// </summary>
    private readonly string[][] bonusResponses = new string[4][]
    {
        new string[] { "Time is money. And\nbud, you ain't got time.",
                       "Maybe an upgrade will help?",
                       "Better luck next time.",
                       "Little too slow." },
        new string[] { "Now that's some speed!",
                       "Real impressive work!",
                       "INCREDIBLE!",
                       "I love the enthusiasm!" },
        new string[] { "Cool performance.",
                       "Way past fast.",
                       "Great work.",
                       "Hey, you're doing well." },
        new string[] { "Nice job.",
                       "Awesome.",
                       "Just on time.",
                       "Pretty good." }
    };

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
    [SerializeField] private GameObject panelPlayerChoice;
    [SerializeField] private Button buttonWhenTutorialChoice, buttonWhenPlayerChoice;
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
    }

    public void DisplayTutorialPlayerDialog()
    {
        panelPlayerChoice.SetActive(true);
        buttonWhenPlayerChoice.Select();
    }
    public void DismissTutorialPlayerDialog()
    {
        panelPlayerChoice.SetActive(false);
        buttonWhenTutorialChoice.Select();
    }


    public bool GameIsOnTutorialScreen()
    {
        return panelTutorialChoice.activeInHierarchy;
    }
}
