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

    private void ResetBetweenScenesScript()
    {
        BetweenScenesScript.ResumingFromSave = false; // Set to false first, in case game is closed while save is being loaded
        BetweenScenesScript.TutorialMode = false;
        BetweenScenesScript.MusicVolume = PlayerPrefs.GetFloat("Music");
        BetweenScenesScript.SFXVolume = PlayerPrefs.GetFloat("SFX");
        BetweenScenesScript.player1TempCredits = 0; // Reset temporary credit & lives count to 0. These will be set if a store is loaded and progressed past
        BetweenScenesScript.player2TempCredits = 0;
        BetweenScenesScript.player1TempLives = 0;
        BetweenScenesScript.player2TempLives = 0;
        BetweenScenesScript.UpgradesP1 = new int[] { 10, 10, 10, 10 };
        BetweenScenesScript.UpgradesP2 = new int[] { 10, 10, 10, 10 };
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
