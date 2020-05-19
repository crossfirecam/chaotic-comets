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
    public Sprite dev0, dev1, dev2, dev3, dev4, dev5, dev6, devLast;
    public Image devImage;
    public TextMeshProUGUI devText;
    public Text devNextButtonText;
    private readonly string devText0 = "This project began in Oct 2018 with a simple experiment of game physics. Much of the material was placeholders.",
        devText1 = "By the end of Oct 2018, the project was in a playable state. Asteroid physics were finished, a health/power system worked on, and a basic menu system was added.",
        devText2 = "Early Nov 2018 development led to a multiplayer mode, UFO enemies, and powerups.",
        devText3 = "All placeholder textures were removed, replaced with 3D assets made from scratch. This was the build used for a public design show in late Nov 2018. It was received well.",
        devText4 = "In Mar 2019, Chaotic Comets was published online with UI enhancements, added difficulty options, and selecting control type for each player is now much easier.",
        devText5 = "In May 2019, Chaotic Comets was updated to 1.1 - Ship handling & visual effects were improved, sound effects / music were added, and plenty of bugs were fixed.",
        devText6 = "In June 2019, Chaotic Comets was updated to 1.2 - UFO's have more interesting mechanics, saving and a shop between levels was introduced, and a lot of testing with how the UI is handled.",
        devText7 = "Upcoming patches will include more features to enhance gameplay. Thank you for trying out the game!";
    public Button returnToMenuButton;

    // ----------

    void Start() {
        devText.text = devText0;
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
        if (stage < 7) { stage++; }
        else { stage = 0; }
        if (stage == 0) {
            devImage.sprite = dev0;
            devText.text = devText0;
            devNextButtonText.text = "Learn about development >";
        }
        else if (stage == 1) {
            devImage.sprite = dev1;
            devText.text = devText1;
            devNextButtonText.text = "More development info >";
        }
        else if (stage == 2) {
            devImage.sprite = dev2;
            devText.text = devText2;
        }
        else if (stage == 3) {
            devImage.sprite = dev3;
            devText.text = devText3;
        }
        else if (stage == 4) {
            devImage.sprite = dev4;
            devText.text = devText4;
        }
        else if (stage == 5) {
            devImage.sprite = dev5;
            devText.text = devText5;
        }
        else if (stage == 6) {
            devImage.sprite = dev6;
            devText.text = devText6;
            devNextButtonText.text = "Upcoming features >";
        }
        else if (stage == 7) {
            devImage.sprite = devLast;
            devText.text = devText7;
            devNextButtonText.text = "< To start of development";
        }
    }
}
