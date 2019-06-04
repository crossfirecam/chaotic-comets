using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class AboutMenuScript : MonoBehaviour {

    // General purpose variables
    public int stage = 0;

    // UI variables
    public Sprite dev0, dev1, dev2, dev3, dev4, dev5, devLast;
    public Image devImage;
    public TextMeshProUGUI devText;
    public Text devNextButtonText;
    private readonly string devText1 = "This project began in Oct 2018 with a simple experiment of game physics. Much of the material was placeholders.";

    // ----------

    void Start() {
        devText.text = devText1;
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }

    public void NextStageOfDev() {
        if (stage < 6) { stage++; }
        else { stage = 0; }
        if (stage == 0) {
            devImage.sprite = dev0;
            devText.text = devText1;
            devNextButtonText.text = "Learn about development >";
        }
        else if (stage == 1) {
            devImage.sprite = dev1;
            devText.text = "By the end of Oct 2018, the project was in a playable state. Asteroid physics were finished, a health/power system worked on, and a basic menu system was added.";
            devNextButtonText.text = "More development info >";
        }
        else if (stage == 2) {
            devImage.sprite = dev2;
            devText.text = "Early Nov 2018 development led to a multiplayer mode, UFO enemies, and powerups.";
        }
        else if (stage == 3) {
            devImage.sprite = dev3;
            devText.text = "All placeholder textures were removed, replaced with 3D assets made from scratch. This was the build used for a public design show in late Nov 2018. It was received well.";
        }
        else if (stage == 4) {
            devImage.sprite = dev4;
            devText.text = "In Mar 2019, Chaotic Comets was published online with UI enhancements, added difficulty options, and selecting control type for each player is now much easier.";
        }
        else if (stage == 5) {
            devImage.sprite = dev5;
            devText.text = "In May 2019, Chaotic Comets was updated to 1.1 - Ship handling & visual effects were improved, sound effects / music were added, and plenty of bugs were fixed.";
            devNextButtonText.text = "Upcoming features >";
        }
        else if (stage == 6) {
            devImage.sprite = devLast;
            devText.text = "Upcoming patches will include more features to enhance gameplay. Ideas include a shop between levels, a save system, and new enemies/bonus items. Thank you for trying out the game!";
            devNextButtonText.text = "< To start of development";
        }
    }
}
