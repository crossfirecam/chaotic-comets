using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainMenu : MonoBehaviour
{
    public void StartGame(int i)
    {
        if (BetweenScenesScript.PlayerCount == 1)
        {
            if (i == 0) { BetweenScenesScript.ControlTypeP1 = 1; }
            else { BetweenScenesScript.ControlTypeP1 = 0; }
        }
        if (BetweenScenesScript.PlayerCount == 2)
        {
            if (i == 0)
            {
                BetweenScenesScript.ControlTypeP1 = 1;
                BetweenScenesScript.ControlTypeP2 = 1;
            }
            else if (i == 1)
            {
                BetweenScenesScript.ControlTypeP1 = 0;
                BetweenScenesScript.ControlTypeP2 = 1;
            }
            else
            {
                BetweenScenesScript.ControlTypeP1 = 0;
                BetweenScenesScript.ControlTypeP2 = 0;
            }
        }
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadScene", 1f);
    }

    private void LoadScene()
    {
        // If control panel is called while resuming a save, then load store before next level
        // Else, load as if a new game was started
        if (BetweenScenesScript.ResumingFromSave == true)
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
        BetweenScenesScript.TutorialMode = true;
        SceneManager.LoadScene("MainScene");
    }
    public void VisitHelp()
    {
        SceneManager.LoadScene("HelpMenu");
    }

    public void VisitAbout()
    {
        SceneManager.LoadScene("AboutMenu");
    }

    public void EndGame()
    {
        Application.Quit();
    }
}
