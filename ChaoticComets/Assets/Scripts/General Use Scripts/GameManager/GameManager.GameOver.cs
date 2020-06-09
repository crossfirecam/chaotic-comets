using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public partial class GameManager : MonoBehaviour
{
    private int totalScore;
    private string mode;
    private TMP_InputField currentNameField;

    /* ------------------------------------------------------------------------------------------------------------------
     * Game Over Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // Show game over panel and pause the game when the game is over
    public void GameOver()
    {
        // Bring cursor back, tell game not to attempt resuming from save if 'Play Again' is picked, and open panel
        BetweenScenesScript.ResumingFromSave = false;
        Refs.gameOverPanel.SetActive(true);
        Refs.buttonWhenGameOver.Select();
        FindFieldAndLoadLastName();

        CalculateTotalScore("GameOver");

        // Shrink layout if a new high score is not accomplished
        if (!HighScoreHandling.IsThisAHighScore(totalScore))
        {
            RectTransform gameOverRt = Refs.gameOverPanel.GetComponent<RectTransform>();
            gameOverRt.sizeDelta = new Vector2(gameOverRt.sizeDelta.x, 150);
            gameOverRt.Find("NewScoreParts").gameObject.SetActive(false);
        }


        // Halt all sounds and game speed
        musicManager.PauseMusic();
        musicManager.FindAllSfxAndPlayPause(0);
        Time.timeScale = 0;
    }

    // Reload the scene and restart playback if user decides to play again
    public void PlayAgain()
    {
        CheckAndSaveHighscore();
        Time.timeScale = 1;
        SceneManager.LoadScene("MainScene");
    }

    // Saves highscore if it's accomplished, and returns to main menu.
    public void ExitGameFromGameOver()
    {
        CheckAndSaveHighscore();
        Time.timeScale = 1;
        SceneManager.LoadScene("StartMenu");
    }

    // Return to main menu if user decides to leave from pause. Prompts user before leaving if a high score is accomplished.
    public void ExitGameFromPause()
    {
        CalculateTotalScore("MissionCancel");
        if (HighScoreHandling.IsThisAHighScore(totalScore) && !tutorialMode)
        {
            Refs.gamePausePanel.SetActive(false);
            Refs.gameOverPanelAlt.SetActive(true);
            Refs.fadeBlack.SetActive(false);
            Refs.buttonWhenGameOverAlt.Select();
            FindFieldAndLoadLastName();
        }
        else
        {
            Time.timeScale = 1;
            SceneManager.LoadScene("StartMenu");
        }
    }

    private void CheckAndSaveHighscore()
    {
        // Set highscore's name depending on which panel's InputField is being used.
        string nameFromField;
        nameFromField = currentNameField.text;

        // Renames blank names to Anonymous
        if (string.IsNullOrWhiteSpace(nameFromField))
        {
            nameFromField = "Anonymous";
        }

        // Submit high score
        if (HighScoreHandling.IsThisAHighScore(totalScore))
        {
            HighScoreHandling.AddHighscoreEntry(nameFromField, levelNo, totalScore, mode);

            // If name is not the default name, then set the preset name for next game in that mode. Reduces annoyance to controller users in particular.
            if (nameFromField == "Anonymous")
            {
                nameFromField = "";
            }
            if (BetweenScenesScript.PlayerCount == 1) { PlayerPrefs.SetString("SavedNameFor1P", nameFromField); }
            else if (BetweenScenesScript.PlayerCount == 2) { PlayerPrefs.SetString("SavedNameFor2P", nameFromField); }
        }
    }

    // Calculate total score, using Total Credits count to include upgrade spending. Update some text if there's two players.
    private void CalculateTotalScore(string originOfRequest)
    {
        string difficulty = "";
        switch (BetweenScenesScript.Difficulty)
        {
            case 0: difficulty = "Easy"; break;
            case 1: difficulty = "Normal"; break;
            case 2: difficulty = "Hard"; break;
        }
        totalScore = Refs.playerShip1.totalCredits;
        mode = $"1P ({difficulty})";
        if (BetweenScenesScript.PlayerCount == 2)
        {
            totalScore += Refs.playerShip2.totalCredits;
            mode = $"2P ({difficulty})";
            Text changeCongratsTextIf2P;
            if (originOfRequest == "GameOver")
            {
                changeCongratsTextIf2P = Refs.gameOverPanel.transform.Find("NewScoreParts").Find("EnterNameText").GetComponent<Text>();
                changeCongratsTextIf2P.text = "New highscore!\nEnter your names.";
            }
            else if (originOfRequest == "MissionCancel")
            {
                changeCongratsTextIf2P = Refs.gameOverPanelAlt.transform.Find("NewScoreParts").Find("EnterNameText").GetComponent<Text>();
                changeCongratsTextIf2P.text = "...but you got a new highscore!\nEnter your names.";
            }
        }
    }

    private void FindFieldAndLoadLastName()
    {
        if (Refs.gameOverPanel.activeInHierarchy)
        {
            currentNameField = Refs.gameOverPanel.transform.Find("NewScoreParts").Find("NameField").GetComponent<TMP_InputField>();
        }
        else //(Refs.gameOverPanelAlt.activeInHierarchy)
        {
            currentNameField = Refs.gameOverPanelAlt.transform.Find("NewScoreParts").Find("NameField").GetComponent<TMP_InputField>();
        }
        if (BetweenScenesScript.PlayerCount == 1) { currentNameField.text = PlayerPrefs.GetString("SavedNameFor1P"); }
        else if (BetweenScenesScript.PlayerCount == 2) { currentNameField.text = PlayerPrefs.GetString("SavedNameFor2P"); }
    }
}
