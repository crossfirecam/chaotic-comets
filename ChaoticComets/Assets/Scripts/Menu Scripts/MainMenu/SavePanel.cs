using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SavePanel : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;
    [SerializeField] private Button saveFirstButton;
    [SerializeField] private TextMeshProUGUI saveDialogHeader, saveDialogInfo, saveDialogNewGame;

    public void ShowSavePanel()
    {
        Saving_PlayerManager data = Saving_SaveManager.LoadData();

        string savePlayerCount = $"{(data.playerCount == 1 ? "One" : "Two")}";
        saveDialogHeader.text = $"{savePlayerCount} Player Mode\nAuto-Save Found";
        saveDialogNewGame.text = $"New {savePlayerCount} Player Game";

        string saveDifficulty;
        switch (data.difficulty)
        {
            case 0: saveDifficulty = "Easy"; break;
            case 1: saveDifficulty = "Normal"; break;
            case 2: saveDifficulty = "Hard"; break;
            default: saveDifficulty = "Error"; break;
        }
        string saveLevel = $"Level {data.level + 1}";
        saveDialogInfo.text = $"{saveDifficulty}, {saveLevel}";

        saveFirstButton.Select();
    }

    public void SaidYesToResuming()
    {
        BetweenScenes.ResumingFromSave = true;
        BetweenScenes.CheaterMode = false;
        Saving_PlayerManager data = Saving_SaveManager.LoadData();
        BetweenScenes.Difficulty = data.difficulty;
        BetweenScenes.PlayerCount = data.playerCount;
        mainMenu.StartGame();
    }

    public void SaidNoToResuming()
    {
        BetweenScenes.ResumingFromSave = false;
        mainMenu.difficultyPanel.gameObject.SetActive(true);
        mainMenu.difficultyPanel.ShowDifficultyPanel();
    }
}
