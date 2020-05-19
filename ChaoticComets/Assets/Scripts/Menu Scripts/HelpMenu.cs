using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpMenu : MonoBehaviour {

    void Update() {
        if (Input.GetButtonDown("MenuNavCancel") || Input.GetButtonDown("MenuNavSubmit")) {
             VisitMain();
        }
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }
}
