using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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
            case 0: txtScoresHeader.text = "Top 10 Scores (All)"; break;
            case 1: txtScoresHeader.text = "Top 10 Scores (1 Player Only)"; break;
            case 2: txtScoresHeader.text = "Top 10 Scores (2 Player Only)"; break;
        }
        PopulateHighScoreTable();
    }

    private void PopulateHighScoreTable()
    {
        RemovePreviousScoreList();

        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        highscoreEntryTransformList = new List<Transform>();
        foreach (HighscoreEntry highscoreEntry in highscores.highscoreEntryList)
        {
            CreateHighScoreEntryTransform(highscoreEntry, scoreContainer, highscoreEntryTransformList);
        }
    }

    public void ShowResetPanel()
    {
        resetScoresDialog.SetActive(true);
        btnResetNo.Select();
    }
    public void ResetPanelYes()
    {
        //TODO Functionality
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

    private void AddHighscoreEntry(string name, int level, int score, string mode)
    {
        // Create HighscoreEntry
        HighscoreEntry highscoreEntry = new HighscoreEntry { name = name, level = level, score = score, mode = mode };

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Add new entry to Highscores
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Save updated Highscores
        string json = JsonUtility.ToJson(highscores);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    private void RemovePreviousScoreList()
    {
        GameObject[] previousScoreEntries = GameObject.FindGameObjectsWithTag("ScoreEntry");
        foreach (GameObject entry in previousScoreEntries)
            Destroy(entry);
    }

    private class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    // Represents a single Highscore Entry
    [System.Serializable]
    private class HighscoreEntry
    {
        public string name;
        public int level;
        public int score;
        public string mode;
    }
}

