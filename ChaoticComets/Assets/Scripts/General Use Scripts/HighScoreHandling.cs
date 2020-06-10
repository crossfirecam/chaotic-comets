using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class HighScoreHandling
{
    // Basis of this code from Code Monkey https://www.youtube.com/watch?v=iAbaqGYdnyI
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
            if (scoreToCompare.score < newScore)
            {
                return true;
            }
        }
        return false;
    }
    public static void AddHighscoreEntry(string name, int level, int score, string mode)
    {
        int modeToFilter = BetweenScenes.PlayerCount;
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
        List<HighscoreEntry> fullListOfScores = highscores.highscoreEntryList.Where(hs => !hs.mode.StartsWith(modeToFilter.ToString())).ToList();
        fullListOfScores.AddRange(listOfMatchingMode);

        // Save updated Highscores
        Highscores newEntries = new Highscores { highscoreEntryList = fullListOfScores };
        string json = JsonUtility.ToJson(newEntries);
        PlayerPrefs.SetString("highscoreTable", json);
        PlayerPrefs.Save();
    }

    public static void ResetHighScoreEntries()
    {
        // Reset relevant PlayerPrefs to nil
        PlayerPrefs.SetString("highscoreTable", null);
        PlayerPrefs.SetString("SavedNameFor1P", "");
        PlayerPrefs.SetString("SavedNameFor2P", "");
        PlayerPrefs.SetInt("RemovedCPUs", 0);

        Highscores defaultEntries = new Highscores
        {
            highscoreEntryList = new List<HighscoreEntry>()
        };

        // Default values
        string[] defaultModes = { "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)", "1P (CPU Score)",
                                  "2P (CPU Score)", "2P (CPU Score)", "2P (CPU Score)", "2P (CPU Score)" };
        string[] defaultNames = { "Rubén", "Bozza", "Eric", "Acorn", "It's Joe!", "A Literal Bot",
                                  "Justin & Alex", "The Twins", "Josh & Shane", "Rene & Bubba" };
        int[] defaultLevels =   { 12, 9, 8, 6, 4, 2,
                                  10, 7, 5, 3 };
        int[] defaultScores =   { 10000, 8000, 7000, 5000, 3000, 1000,
                                  9000, 6000, 4000, 2000 };

        // For all ten entries, add an entry
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

    public static void RemoveDefaultsFromScoreList()
    {
        // Reset relevant PlayerPref
        PlayerPrefs.SetInt("RemovedCPUs", 1);
        PlayerPrefs.Save();

        // Load saved Highscores
        string jsonString = PlayerPrefs.GetString("highscoreTable");
        Highscores highscores = JsonUtility.FromJson<Highscores>(jsonString);

        Highscores trimmedScores = new Highscores
        {
            highscoreEntryList = new List<HighscoreEntry>()
        };

        // Check all scores for the CPU tag
        foreach (HighscoreEntry entryToCheck in highscores.highscoreEntryList)
        {
            if (!entryToCheck.mode.Contains("CPU"))
            {
                trimmedScores.highscoreEntryList.Add(entryToCheck);
            }
        }

        // Save updated Highscores
        string json = JsonUtility.ToJson(trimmedScores);
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
