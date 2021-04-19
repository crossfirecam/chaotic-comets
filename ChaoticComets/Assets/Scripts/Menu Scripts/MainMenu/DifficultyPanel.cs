using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class DifficultyPanel : MonoBehaviour
{
    [Header("Difficulty Screen")]
    public Transform playerDecorDiffDialog;
    public Button diffBackButton;
    public Button diffEasyButton, diffNormalButton, diffHardButton;
    public TextMeshProUGUI difficultyTitleText, difficultyText;
    private readonly string diffTitle0 = "Select difficulty", diffTitle1 = "Select difficulty\n(both players)";
    private readonly string diffText0 = "Go back to main menu.";
    private readonly string diffText1 = "- Ship comes to a complete stop.\n- Asteroids move slowly. \n- UFO's are just here to play.";
    private readonly string diffText2 = "- Ship requires manual braking.\n- Asteroids are a threat.\n- UFO's are aggressive.";
    private readonly string diffText3 = "- Ship has a weaker manual brake.\n- Asteroids are fast.\n- UFO's will attack without prejudice.";
    private readonly string diffText4 = "<u>Same as Hard except:</u>\n- Ship is 50% more vulnerable.\n- Asteroids are even faster.\n- Canisters are very rare.";

    [SerializeField] private MainMenu mainMenu;

    // Display a prompt to select difficulty
    public void ShowDifficultyPanel()
    {
        // Dismiss save dialog if open
        mainMenu.savePanel.gameObject.SetActive(false);

        // Open difficulty dialog, select Normal button by default
        diffNormalButton.Select();

        // Depending on player count, change aesthetics
        if (BetweenScenes.PlayerCount == 1)
        {
            difficultyTitleText.text = diffTitle0;
            playerDecorDiffDialog.Find("1P").gameObject.SetActive(true);
            playerDecorDiffDialog.Find("2P").gameObject.SetActive(false);
        }
        else if (BetweenScenes.PlayerCount == 2)
        {
            difficultyTitleText.text = diffTitle1;
            playerDecorDiffDialog.Find("1P").gameObject.SetActive(false);
            playerDecorDiffDialog.Find("2P").gameObject.SetActive(true);
        }
    }

    public void SetDifficultyAndStart(int i)
    {
        BetweenScenes.Difficulty = i;
        mainMenu.StartGame();
    }

    public void ChangeDifficultyText()
    {
        if (EventSystem.current.currentSelectedGameObject.name == "Diff-BackButton") { difficultyText.text = diffText0; }
        else if (EventSystem.current.currentSelectedGameObject.name == "Diff-EasyButton") { difficultyText.text = diffText1; }
        else if (EventSystem.current.currentSelectedGameObject.name == "Diff-NormalButton") { difficultyText.text = diffText2; }
        else if (EventSystem.current.currentSelectedGameObject.name == "Diff-HardButton") { difficultyText.text = diffText3; }
        else if (EventSystem.current.currentSelectedGameObject.name == "Diff-InsaneButton") { difficultyText.text = diffText4; }
    }
}
