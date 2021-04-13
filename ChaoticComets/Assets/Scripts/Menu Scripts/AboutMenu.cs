using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AboutMenu : MonoBehaviour {

    // General purpose variables
    public int stage = 0;

    // UI variables
    public Sprite devIntro, devPre1, devPre2, devPreFinal, dev1_0, dev1_1, dev1_2, dev1_3, dev1_4, devWhatNext;
    public Image devImage;
    public TextMeshProUGUI devText, devNextButtonText;
    private readonly string devTextIntro = "Chaotic Comets was in development from late 2018 to early 2021. Press the button below to learn more about the game's history.",
        devTextPre1 = "<u>Late October 2018</u>: The first build of the game that's worth showing. Asteroid physics, a health/teleport system, and a basic menu system were present.",
        devTextPre2 = "<u>Early November 2018</u>: Further development led to adding UFO enemies and powerups.",
        devTextPreFinal = "<u>Late November 2018</u>: Placeholder sprites were replaced with 3D assets made from scratch. This build was shown at a public design show. It was received well.",
        devText1_0 = "<u>March 2019</u>: Chaotic Comets was revisited to be published online. It came with UI enhancements, difficulty options, and easier controller selection.",
        devText1_1 = "<u>May 2019</u>: Chaotic Comets was updated to 1.1. Ship handling & visual effects were improved, sound effects / music were added, and plenty of bugs were fixed.",
        devText1_2 = "<u>June 2019</u>: Chaotic Comets was updated to 1.2. UFO's have more interesting mechanics. Auto-saving and a shop between levels was introduced.",
        devText1_3 = "<u>June 2020</u>: After a year of on-and-off dev, 1.3 was done. Includes a new UFO, interactive tutorial, highscores, and a <i>ton</i> of other features, code review & bug fixes.",
        devText1_4 = "<u>April 2021</u>: A month was spent finishing the game with 1.4. Major improvements are the new UI and Shop, along with many tweaks to make the game more fun to play.",
        devTextWhatNext = "After 3 years of this project being active, I consider Chaotic Comets as completed. I've done all I want to do with it, but am open to bug reports or minor changes. I hope you enjoy the game!";
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
        Application.OpenURL("https:/crossfirecam.itch.io/");
    }

    public void NextStageOfDev() {
        if (stage < 9) { stage++; }
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
            devNextButtonText.text = "What's Next? >";
        }
        else if (stage == 9) {
            devImage.sprite = devWhatNext;
            devText.text = devTextWhatNext;
            devNextButtonText.text = "< Back to Beginning";
        }
    }
}
