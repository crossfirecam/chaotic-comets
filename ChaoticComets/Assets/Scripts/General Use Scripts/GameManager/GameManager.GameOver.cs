using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine;
using TMPro;

public partial class GameManager : MonoBehaviour
{
    private int totalScore;
    private string mode;

    /* ------------------------------------------------------------------------------------------------------------------
     * Game Over Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // Show game over panel and pause the game when the game is over
    public void GameOver()
    {
        // Bring cursor back, tell game not to attempt resuming from save if 'Play Again' is picked, and open panel
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false;
        Refs.gameOverPanel.SetActive(true);
        Refs.buttonWhenGameOver.Select();

        CalculateTotalScore();

        // Shrink layout if a new high score is not accomplished
        if (!HighScoreHandling.IsThisAHighScore(totalScore, mode))
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
        CalculateTotalScore();
        if (HighScoreHandling.IsThisAHighScore(totalScore, mode))
        {
            Refs.gamePausePanel.SetActive(false);
            Refs.gameOverPanelAlt.SetActive(true);
            Refs.buttonWhenGameOverAlt.Select();
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
        if (Refs.gameOverPanel.activeInHierarchy)
        {
            nameFromField = Refs.gameOverPanel.transform.Find("NewScoreParts").Find("NameField").GetComponent<TMP_InputField>().text;
        }
        else //(Refs.gameOverPanelAlt.activeInHierarchy)
        {
            nameFromField = Refs.gameOverPanelAlt.transform.Find("NewScoreParts").Find("NameField").GetComponent<TMP_InputField>().text;
        }

        // Renames blank names to Anonymous
        if (string.IsNullOrWhiteSpace(nameFromField))
        {
            nameFromField = "Anonymous";
        }

        // Submit high score
        if (HighScoreHandling.IsThisAHighScore(totalScore, mode))
        {
            HighScoreHandling.AddHighscoreEntry(nameFromField, levelNo, totalScore, mode, BetweenScenesScript.PlayerCount);
        }
    }

    // Calculate total score, using Total Credits count to include upgrade spending. Update some text if there's two players.
    private void CalculateTotalScore()
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
            mode = $"2P({difficulty})";
            Text changeCongratsTextIf2P;
            if (Refs.gameOverPanel.activeInHierarchy)
            {
                changeCongratsTextIf2P = Refs.gameOverPanel.transform.Find("NewScoreParts").Find("EnterNameText").GetComponent<Text>();
                changeCongratsTextIf2P.text = "New highscore!\nEnter your names.";
            }
            else if (Refs.gameOverPanelAlt.activeInHierarchy)
            {
                changeCongratsTextIf2P = Refs.gameOverPanelAlt.transform.Find("NewScoreParts").Find("EnterNameText").GetComponent<Text>();
                changeCongratsTextIf2P.text = "...but you got a new highscore!\nEnter your names.";
            }
        }
    }
}
