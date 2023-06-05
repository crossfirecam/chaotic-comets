using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HighScoreHandling
{
    // Basis of this code from Code Monkey https://www.youtube.com/watch?v=iAbaqGYdnyI

    /* ------------------------------------------------------------------------------------------------------------------
     * IsThisAHighScore - Checks against a set of criteria, to see if newScore is eligible to replace any existing score
     * ------------------------------------------------------------------------------------------------------------------ */
    public static bool IsThisAHighScore(int newScore)
    {
        // If in cheater mode, then return false
        if (BetweenScenes.CheaterMode)
        {
            return false;
        }

        int modeToFilter = BetweenScenes.PlayerCount;
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // If score = 0, then return false
        if (newScore == 0)
        {
            return false;
        }

        // If less than ten entries in the mode's list, then return true
        List<HighscoreEntry> listOfMatchingMode = highscores.highscoreEntryList.Where(hs => hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        if (listOfMatchingMode.Count < 10)
        {
            return true;
        }

        // If any score 
        foreach (HighscoreEntry scoreToCompare in listOfMatchingMode)
        {
            if (scoreToCompare.credits < newScore)
            {
                return true;
            }
        }
        return false;
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * AddHighscoreEntry - Add a highscore with 4 attributes to the JSON highscore table
     * ------------------------------------------------------------------------------------------------------------------ */
    public static void AddHighscoreEntry(string name, int level, int score, string mode)
    {
        int modeToFilter = BetweenScenes.PlayerCount;
        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        // Create & add new entry to 'highscores'
        HighscoreEntry highscoreEntry = new HighscoreEntry { name = name, level = level, credits = score, mode = mode };
        highscores.highscoreEntryList.Add(highscoreEntry);

        // Filter list to specific gamemode, sort, remove lowest entry
        List<HighscoreEntry> listOfMatchingMode = highscores.highscoreEntryList.Where(hs => hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        listOfMatchingMode = listOfMatchingMode.OrderByDescending(hs => hs.credits).ToList();
        if (listOfMatchingMode.Count > 10)
        {
            listOfMatchingMode.RemoveRange(10, listOfMatchingMode.Count - 10);
        }

        // ... and create full list including other gamemode
        List<HighscoreEntry> fullListOfScores = highscores.highscoreEntryList.Where(hs => !hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        fullListOfScores.AddRange(listOfMatchingMode);

        // Save updated Highscores
        Highscores newEntries = new Highscores { highscoreEntryList = fullListOfScores };
        string json = JsonUtility.ToJson(newEntries);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * ResetHighScoreEntries - Player can choose to erase the high score board.
     * ------------------------------------------------------------------------------------------------------------------ */
    public static void ResetHighScoreEntries()
    {
        // Reset relevant PlayerPrefs to nil
        PlayerPrefs.SetString("highscoreTable", null);
        PlayerPrefs.SetString("SavedNameFor1P", "");
        PlayerPrefs.SetString("SavedNameFor2P", "");

        // Placeholder scores
        Highscores defaultEntries = new Highscores
        {
            highscoreEntryList = new List<HighscoreEntry>()
        };
        for (int i = 0; i < 10; i++)
            defaultEntries.highscoreEntryList.Add(new HighscoreEntry { name = "", level = 0, credits = 0, mode = "1P" });
        for (int i = 0; i < 10; i++)
            defaultEntries.highscoreEntryList.Add(new HighscoreEntry { name = "", level = 0, credits = -1, mode = "2P" });

        // Save blank scores
        string json = JsonUtility.ToJson(defaultEntries);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Highscores & HighscoreEntry - Public classes for an entire list of highscores, and a single highscore entry
     * ------------------------------------------------------------------------------------------------------------------ */
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
        public int credits;
        public string mode;
    }
}
