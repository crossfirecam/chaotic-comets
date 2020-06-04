using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class AboutMenu : MonoBehaviour {

    // General purpose variables
    public int stage = 0;

    // UI variables
    public Sprite devIntro, devPre1, devPre2, devPre3, devPreFinal, dev1_0, dev1_1, dev1_2, dev1_3, devUpcoming;
    public Image devImage;
    public TextMeshProUGUI devText;
    public Text devNextButtonText;
    private readonly string devTextIntro = "Chaotic Comets has been in development since late 2018. Press the button below to learn more about the game's history.",
        devTextPre1 = "<u>Early October 2018</u>: Chaotic Comet's development began with a simple experiment of game physics. Much of the material was placeholders.",
        devTextPre2 = "<u>Late October 2018</u>: The project was in a playable state. Asteroid physics, a health/power system, and a basic menu system was added.",
        devTextPre3 = "<u>Early November 2018</u>: Further development led to adding UFO enemies and powerups.",
        devTextPreFinal = "<u>Late November 2018</u>: All placeholder sprites were removed, replaced with 3D assets made from scratch. This was the build used for a public design show. It was received well.",
        devText1_0 = "<u>March 2019</u>: Chaotic Comets was revisited to be published online. It came with UI enhancements, difficulty options, and easier controller selection.",
        devText1_1 = "<u>May 2019</u>: Chaotic Comets was updated to 1.1. Ship handling & visual effects were improved, sound effects / music were added, and plenty of bugs were fixed.",
        devText1_2 = "<u>June 2019</u>: Chaotic Comets was updated to 1.2. UFO's have more interesting mechanics, saving and a shop between levels was introduced, and there was a lot of testing done.",
        devText1_3 = "<u>June 2020</u>: After a year of on-and-off dev, 1.3 was done. Includes improved UFO AI, an interactive tutorial, better controller support, and a <i>ton</i> of code review & bug fixes.",
        devTextUpcoming = "Upcoming patches will include more features to enhance gameplay. Thank you for playing the game!";
    public Button returnToMenuButton;

    // ----------

    void Start() {
        devText.text = devTextIntro;
    }

    void Update() {
        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.Equals(null)) {
            returnToMenuButton.Select();
        }
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }

    public void NextStageOfDev() {
        if (stage < 9) { stage++; }
        else { stage = 0; }
        if (stage == 0) {
            devImage.sprite = devIntro;
            devText.text = devTextIntro;
            devNextButtonText.text = "Learn about development >";
        }
        else if (stage == 1) {
            devImage.sprite = devPre1;
            devText.text = devTextPre1;
            devNextButtonText.text = "More development info >";
        }
        else if (stage == 2) {
            devImage.sprite = devPre2;
            devText.text = devTextPre2;
        }
        else if (stage == 3) {
            devImage.sprite = devPre3;
            devText.text = devTextPre3;
        }
        else if (stage == 4) {
            devImage.sprite = devPreFinal;
            devText.text = devTextPreFinal;
        }
        else if (stage == 5) {
            devImage.sprite = dev1_0;
            devText.text = devText1_0;
        }
        else if (stage == 6) {
            devImage.sprite = dev1_1;
            devText.text = devText1_1;
        }
        else if (stage == 7)
        {
            devImage.sprite = dev1_2;
            devText.text = devText1_2;
        }
        else if (stage == 8)
        {
            devImage.sprite = dev1_3;
            devText.text = devText1_3;
            devNextButtonText.text = "Upcoming features >";
        }
        else if (stage == 9) {
            devImage.sprite = devUpcoming;
            devText.text = devTextUpcoming;
            devNextButtonText.text = "< To start of development";
        }
    }
}
