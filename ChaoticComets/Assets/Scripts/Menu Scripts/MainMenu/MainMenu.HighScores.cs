using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static HighScoreHandling;

public partial class MainMenu : MonoBehaviour
{
    // Basis of this code from Code Monkey https://www.youtube.com/watch?v=iAbaqGYdnyI

    public TextMeshProUGUI txtScoresHeader;
    public Transform scoreContainer, scoreTemplate;
    public GameObject resetScoresDialog;
    public Button btnResetNo;
    private List<Transform> highscoreEntryTransformList;

    /* ------------------------------------------------------------------------------------------------------------------
     * High Score Table Populating Methods
     * ------------------------------------------------------------------------------------------------------------------ */
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

        int numOfScoresOnCurrentList = 0;
        // If ScorePreference is set to 0 (All Scores), sort the full list into the best 10, and create the table
        if (PlayerPrefs.GetInt("ScorePreference") == 0)
        {
            List<HighscoreEntry> sortedList = highscores.highscoreEntryList.OrderByDescending(hs => hs.score).ToList();
            if(sortedList.Count > 10)
            {
                sortedList.RemoveRange(10, sortedList.Count - 10);
            }

            foreach (HighscoreEntry sortedEntry in sortedList)
            {
                CreateHighScoreEntryTransform(sortedEntry, scoreContainer, highscoreEntryTransformList);
                numOfScoresOnCurrentList++;
            }

        }
        // If ScorePreference is set to 1 or 2 (P1 Only or P2 Only), only create a table using scores from that mode
        else if (PlayerPrefs.GetInt("ScorePreference") == 1 || PlayerPrefs.GetInt("ScorePreference") == 2)
        {
            foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
            {
                if (PlayerPrefs.GetInt("ScorePreference") == 1 && highscoreEntry.mode.StartsWith("1P"))
                {
                    CreateHighScoreEntryTransform(highscoreEntry, scoreContainer, highscoreEntryTransformList);
                    numOfScoresOnCurrentList++;
                }
                else if (PlayerPrefs.GetInt("ScorePreference") == 2 && highscoreEntry.mode.StartsWith("2P"))
                {
                    CreateHighScoreEntryTransform(highscoreEntry, scoreContainer, highscoreEntryTransformList);
                    numOfScoresOnCurrentList++;
                }
            }
        }

        // If no entries are added, then notify user the list is empty
        if (numOfScoresOnCurrentList == 0)
        {
            HighscoreEntry thisListIsEmpty = new HighscoreEntry { name = "<i>This mode's scores are empty.</i>" };
            CreateHighScoreEntryTransform(thisListIsEmpty, scoreContainer, highscoreEntryTransformList);
            thisListIsEmpty = new HighscoreEntry { name = "<i>Play a round and come back!</i>" };
            CreateHighScoreEntryTransform(thisListIsEmpty, scoreContainer, highscoreEntryTransformList);
        }
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

        // If level is 0, then the entry is telling user the list is empty. Remove 0's for level and credits columns.
        if (highscoreEntry.level == 0)
        {
            scoreEntry.Find("Level").GetComponent<TextMeshProUGUI>().text = "";
            scoreEntry.Find("Credits").GetComponent<TextMeshProUGUI>().text = "";
        }

        transformList.Add(scoreEntry);
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Reset Panel Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    Navigation navSelectUp = new Navigation { mode = Navigation.Mode.Explicit };
    Navigation navSelectDown = new Navigation { mode = Navigation.Mode.Explicit };
    public void ShowResetPanel()
    {
        resetScoresDialog.SetActive(true);
        btnResetNo.Select();

        // Initialise navigation change variables
        Transform resetDialogItself = resetScoresDialog.transform.Find("ResetDialog");
        Button resetNoButton = resetDialogItself.Find("ResetNoButton").GetComponent<Button>();
        Button resetCPUButton = resetDialogItself.Find("ResetRemoveCPUButton").GetComponent<Button>();
        Button resetYesButton = resetDialogItself.Find("ResetYesButton").GetComponent<Button>();

        // Change attributes of button navigation depending on if the CPU removal button has been pressed (can also be reset)
        bool cpuButtonPressable = PlayerPrefs.GetInt("RemovedCPUs") == 0;
        resetCPUButton.interactable = cpuButtonPressable;
        navSelectDown.selectOnDown = cpuButtonPressable ? resetCPUButton : resetYesButton;
        navSelectUp.selectOnUp = cpuButtonPressable ? resetCPUButton : resetNoButton;
        resetNoButton.navigation = navSelectDown;
        resetYesButton.navigation = navSelectUp;
    }

    // When Reset Scores button is pressed, reset scores to default values, and repopulate the table
    public void ResetPanelYes()
    {
        ResetHighScoreEntries();
        ChangeScoreTypeAndPopulate(0);
        BackToMenu();
    }

    // When Remove CPU Scores button is pressed, remove the default values, and repopulate the table
    public void ResetPanelRemoveCPUs()
    {
        RemoveDefaultsFromScoreList();
        ChangeScoreTypeAndPopulate(0);
        BackToMenu();
    }
}

