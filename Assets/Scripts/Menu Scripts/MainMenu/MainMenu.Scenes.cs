using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(UsefulFunctions.FadeScreenBlack("to", fadeBlackOverlay));
        Invoke(nameof(LoadScene), 1f);
    }

    private void LoadScene()
    {
        // If control panel is called while resuming a save, then load store before next level
        // Else, load as if a new game was started
        if (BetweenScenes.ResumingFromSave == true)
        {
            SceneManager.LoadScene("ShopMenu");
        }
        else
        { // If a new game is started, then erase old data
            Saving_SaveManager.EraseData();
            SceneManager.LoadScene("MainScene");
        }
    }

    public void VisitTutorial()
    {
        BetweenScenes.BackToMainMenuButton = "Tutorial";
        BetweenScenes.TutorialMode = true;
        SceneManager.LoadScene("MainScene");
    }
    public void VisitHelp()
    {
        BetweenScenes.BackToMainMenuButton = "Help";
        SceneManager.LoadScene("HelpMenu");
    }

    public void VisitAbout()
    {
        BetweenScenes.BackToMainMenuButton = "About";
        SceneManager.LoadScene("AboutMenu");
    }

    public void ExitGame()
    {
        Application.Quit();
    }
}
