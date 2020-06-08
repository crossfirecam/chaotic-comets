using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HighScoreHandling;

public partial class MainMenu : MonoBehaviour
{
    // Guide for High Score table from Code Monkey https://www.youtube.com/watch?v=iAbaqGYdnyI

    public TextMeshProUGUI txtScoresHeader;
    public Transform scoreContainer, scoreTemplate;
    public GameObject resetScoresDialog;
    public Button btnResetNo;
    private List<Transform> highscoreEntryTransformList;

    public void ChangeScoreTypeAndPopulate(int mode)
    {
        PlayerPrefs.SetInt("ScorePreference", mode);
        PlayerPrefs.Save();
        switch (mode)
        {
            case 0: txtScoresHeader.text = "Top 10 Scores (Both Modes)"; break;
            case 1: txtScoresHeader.text = "Top 10 Scores (1 Player Mode)"; break;
            case 2: txtScoresHeader.text = "Top 10 Scores (2 Player Mode)"; break;
        }
        PopulateHighScoreTable();
    }

    private void PopulateHighScoreTable()
    {
        // Remove all previously shown high score objects
        GameObject[] previousScoreEntries = GameObject.FindGameObjectsWithTag("ScoreEntry");
        foreach (GameObject entry in previousScoreEntries)
            Destroy(entry);

        // Fetch current high score list
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Initialise a table of high score entries
        highscoreEntryTransformList = new List<Transform>();

        // If ScorePreference is set to 0 (All Scores), sort the full list into the best 10, and create the table
        if (PlayerPrefs.GetInt("ScorePreference") == 0)
        {
            List<HighscoreEntry> sortedList = highscores.highscoreEntryList.OrderByDescending(hs => hs.score).ToList();
            if(sortedList.Count > 10)
            {
                sortedList.RemoveRange(10, sortedList.Count - 10);
            }

            foreach (HighscoreEntry sortedEntry in sortedList)
                CreateHighScoreEntryTransform(sortedEntry, scoreContainer, highscoreEntryTransformList);

        }
        // If ScorePreference is set to 1 or 2 (P1 Only or P2 Only), only create a table using scores from that mode
        else if (PlayerPrefs.GetInt("ScorePreference") == 1 || PlayerPrefs.GetInt("ScorePreference") == 2)
        {
            foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
            {
                if (PlayerPrefs.GetInt("ScorePreference") == 1 && highscoreEntry.mode.StartsWith("1P"))
                    CreateHighScoreEntryTransform(highscoreEntry, scoreContainer, highscoreEntryTransformList);
                else if (PlayerPrefs.GetInt("ScorePreference") == 2 && highscoreEntry.mode.StartsWith("2P"))
                    CreateHighScoreEntryTransform(highscoreEntry, scoreContainer, highscoreEntryTransformList);
            }
        }
    }

    public void ShowResetPanel()
    {
        resetScoresDialog.SetActive(true);
        btnResetNo.Select();
    }

    // When Reset button is pressed on main menu, reset scores to default values, and repopulate the table
    public void ResetPanelYes()
    {
        ResetHighScoreEntries();
        ChangeScoreTypeAndPopulate(0);
        BackToMenu();
    }
    private void CreateHighScoreEntryTransform(HighscoreEntry highscoreEntry, Transform container, List<Transform> transformList)
    {
        // Instantiate entry, relocate it
        float baseEntryY = 80f, offsetEntryY = 20f;
        Transform scoreEntry = Instantiate(scoreTemplate, container);
        RectTransform scoreEntryRect = scoreEntry.GetComponent<RectTransform>();
        scoreEntryRect.anchoredPosition = new Vector2(0, baseEntryY - offsetEntryY * transformList.Count);
        scoreEntry.gameObject.SetActive(true);

        // Populate entry with details
        scoreEntry.Find("Name").GetComponent<TextMeshProUGUI>().text = highscoreEntry.name;
        scoreEntry.Find("Level").GetComponent<TextMeshProUGUI>().text = highscoreEntry.level.ToString();
        scoreEntry.Find("Credits").GetComponent<TextMeshProUGUI>().text = highscoreEntry.score.ToString();
        scoreEntry.Find("Mode").GetComponent<TextMeshProUGUI>().text = highscoreEntry.mode;

        transformList.Add(scoreEntry);
    }
}

