using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HelpMenu : MonoBehaviour {

    private void OnMenuGoBack()
    {
        VisitMain();
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }
}
