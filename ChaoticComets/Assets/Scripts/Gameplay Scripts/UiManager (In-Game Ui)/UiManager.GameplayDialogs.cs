using UnityEngine;
using TMPro;
using UnityEngine.UI;

public partial class UiManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Congratulations Dialog - Methods to do with players passing a level
     * ------------------------------------------------------------------------------------------------------------------ */
    [Header("Congrats Dialog UI")]
    [SerializeField] private GameObject panelCongrats;
    [SerializeField] private GameObject gameLevelShieldRechargeText;
    public void LevelCompleted()
    {
        panelCongrats.SetActive(true);
        StartCoroutine(FadeScreenBlack("to"));
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
