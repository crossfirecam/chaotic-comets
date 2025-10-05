using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AboutMenu : MonoBehaviour {

    // General purpose variables
    public int stage = 0;

    // UI variables
    public Sprite devIntro, devPre1, devPre2, devPreFinal, dev1_0, dev1_1, dev1_2, dev1_3, dev1_4, dev1_5, devWhatNext;
    public Image devImage;
    public TextMeshProUGUI devText, devNextButtonText;
    private readonly string devTextIntro = "Chaotic Comets was in development from late 2018 to mid 2023. Press the button below to learn about the game's history.",
        devTextPre1 = "<u>Late October 2018</u>: The earliest playable build featured asteroid physics, a health/teleport system, and a basic menu system.",
        devTextPre2 = "<u>Early November 2018</u>: Further development added a UFO enemy and powerups.",
        devTextPreFinal = "<u>Late November 2018</u>: The first public build was well-received at a university design show.",
        devText1_0 = "<u>March 2019, v1.0</u>: The game was published online, coming with UI enhancements and difficulty options.",
        devText1_1 = "<u>May 2019, v1.1</u>: Ship handling & visual effects were improved, and SFX/music were added.",
        devText1_2 = "<u>June 2019, v1.2</u>: UFOs have more interesting mechanics. Auto-saving and a shop between levels was introduced.",
        devText1_3 = "<u>June 2020, v1.3</u>: Additions included a new UFO, interactive tutorial, highscores, and many small features & bug fixes.",
        devText1_4 = "<u>April 2021, v1.4</u>: A month was spent polishing the game with 1.4. Major improvements are the new UI and Shop.",
        devText1_5 = "<u>June 2023, v1.5</u>: The game is now playable on the web! While the project was revisited, some final tweaks to the gameplay and presentation were made.",
        devTextWhatNext = "After 5 years of occasional development progress, Chaotic Comets is complete. I hope you enjoy the game!";
    public Button returnToMenuButton;

    // ----------

    void Start() {
        devText.text = devTextIntro;
        StartCoroutine(UsefulFunctions.CheckController());
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }
    public void VisitWebsite()
    {
        Application.OpenURL("https://crossfirecam.itch.io/");
    }

    public void NextStageOfDev() {
        if (stage < 10) { stage++; }
        else { stage = 0; }
        if (stage == 0) {
            devImage.sprite = devIntro;
            devText.text = devTextIntro;
            devNextButtonText.text = "Learn About Development >";
        }
        else if (stage == 1) {
            devImage.sprite = devPre1;
            devText.text = devTextPre1;
            devNextButtonText.text = "More Development Info >";
        }
        else if (stage == 2) {
            devImage.sprite = devPre2;
            devText.text = devTextPre2;
        }
        else if (stage == 3) {
            devImage.sprite = devPreFinal;
            devText.text = devTextPreFinal;
        }
        else if (stage == 4) {
            devImage.sprite = dev1_0;
            devText.text = devText1_0;
        }
        else if (stage == 5) {
            devImage.sprite = dev1_1;
            devText.text = devText1_1;
        }
        else if (stage == 6)
        {
            devImage.sprite = dev1_2;
            devText.text = devText1_2;
        }
        else if (stage == 7)
        {
            devImage.sprite = dev1_3;
            devText.text = devText1_3;
        }
        else if (stage == 8)
        {
            devImage.sprite = dev1_4;
            devText.text = devText1_4;
        }
        else if (stage == 9)
        {
            devImage.sprite = dev1_5;
            devText.text = devText1_5;
            devNextButtonText.text = "What's Next? >";
        }
        else if (stage == 10) {
            devImage.sprite = devWhatNext;
            devText.text = devTextWhatNext;
            devNextButtonText.text = "< Back to Beginning";
        }
    }
}
