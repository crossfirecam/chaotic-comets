using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour {

    [SerializeField] private Button leaveHelpButton = default;

    private void Start()
    {
        StartCoroutine(UsefulFunctions.CheckController());
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }
}
