using UnityEngine;
using UnityEngine.SceneManagement;

public partial class MainMenu : MonoBehaviour
{
    public void StartGame()
    {
        StartCoroutine(FadeBlack("to"));
        Invoke("LoadScene", 1f);
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
        BetweenScenes.TutorialMode = true;
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

    public void ChangeMusicPassToManager(float musVolume)
    {
        musicManager.ChangeMusic(musVolume);
    }
    public void ChangeSFXPassToManager(float sfxVolume)
    {
        musicManager.ChangeSFX(sfxVolume);
    }
}
