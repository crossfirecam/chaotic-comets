using System.Collections.Generic;
using System.Linq;
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
            if (newMode.StartsWith(scoreToCompare.mode.Substring(0,2)) && scoreToCompare.score < newScore)
            {
                return true;
            }
        }
        return false;
    }
    public static void AddHighscoreEntry(string name, int level, int score, string mode, int modeToFilter)
    {
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Create & add new entry to 'highscores'
        HighscoreEntry highscoreEntry = new HighscoreEntry { name = name, level = level, score = score, mode = mode };
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Filter list to specific gamemode, sort, remove lowest entry
        List<HighscoreEntry> listOfMatchingMode = highscores.highscoreEntryList.Where(hs => hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        listOfMatchingMode = listOfMatchingMode.OrderByDescending(hs => hs.score).ToList();
        if (listOfMatchingMode.Count > 10)
        {
            listOfMatchingMode.RemoveRange(10, listOfMatchingMode.Count - 10);
        }

        // ... and create full list including other gamemode
        List<HighscoreEntry> fullListOfScores = highscores.highscoreEntryList.Where(hs => hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        fullListOfScores.AddRange(listOfMatchingMode);

        // Save updated Highscores
        Highscores newEntries = new Highscores { highscoreEntryList = fullListOfScores };
        string json = JsonUtility.ToJson(newEntries);
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

        string[] defaultModes = { "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)",
                                  "2P (CPU Score)", "2P (CPU Score)", "2P (CPU Score)" };
        string[] defaultNames = { "Rubén", "Alex", "Bozza", "Elly", "Nick", "Sam", "Literal Bot",
                                  "Josh & Shane", "Ann & Matt", "Renee & Bubba" };
        int[] defaultLevels =   { 12, 10, 8, 7, 6, 3, 2,
                                  9, 5, 4 };
        int[] defaultScores =   { 10000, 9000, 7000, 6000, 5000, 2000, 1000,
                                  8000, 4000, 3000 };

        for (int i = 0; i < 10; i++)
        {
            HighscoreEntry defaultEntry = new HighscoreEntry { name = defaultNames[i], level = defaultLevels[i], score = defaultScores[i], mode = defaultModes[i] };
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
