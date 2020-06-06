using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public static class HighScoreHandling
{
    public static bool IsThisAHighScore(int newScore, string newMode)
    {
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        foreach (HighscoreEntry scoreToCompare in highscores.highscoreEntryList)
        {
            if (scoreToCompare.mode == newMode && scoreToCompare.score < newScore)
            {
                return true;
            }
        }
        return false;
    }
    public static void AddHighscoreEntry(string name, int level, int score, string mode)
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

    public static void ResetHighScoreEntries()
    {
        PlayerPrefs.SetString("highscoreTable", null);
        Highscores defaultEntries = new Highscores
        {
            highscoreEntryList = new List<HighscoreEntry>()
        };

        string[] defaultNames = { "John", "John", "John", "John", "John", "John", "John", "John", "John", "John",  // 1P Default Names
                                  "John", "John", "John", "John", "John", "John", "John", "John", "John", "John"}; // 2P Default Names
        int[] defaultLevels =   { 9, 8, 7, 6, 5, 4, 3, 2, 1, 0,   // 1P Default Levels
                                  9, 8, 7, 6, 5, 4, 3, 2, 1, 0 }; // 2P Default Levels
        int[] defaultScores =   { 10000, 9000, 8000, 7000, 6000, 5000, 4000, 3000, 2000, 1000,   // 1P Default Scores
                                  10000, 9000, 8000, 7000, 6000, 5000, 4000, 3000, 2000, 1000 }; // 2P Default Scores

        for (int i = 0; i < 20; i++)
        {
            HighscoreEntry defaultEntry = new HighscoreEntry { name = defaultNames[i], level = defaultLevels[i], score = defaultScores[i], mode = i < 10 ? "1P" : "2P" };
            defaultEntries.highscoreEntryList.Add(defaultEntry);
        }

        // Save updated Highscores
        string json = JsonUtility.ToJson(defaultEntries);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public class Highscores
    {
        public List<HighscoreEntry> highscoreEntryList;
    }

    // Represents a single Highscore Entry
    [System.Serializable]
    public class HighscoreEntry
    {
        public string name;
        public int level;
        public int score;
        public string mode;
    }
}
