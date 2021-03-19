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
    public void LevelCompleted()
    {
        panelCongrats.SetActive(true);
        StartCoroutine(FadeScreenBlack("to"));
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
