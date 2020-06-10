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

    private void Update()
    {
        if (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.Equals(null))
        {
            leaveHelpButton.Select();
        }
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }
}
