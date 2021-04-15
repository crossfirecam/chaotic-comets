using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour {

    [SerializeField] private Button menuButton;
    private Button buttonToReturnTo;

    private void Start()
    {
        StartCoroutine(UsefulFunctions.CheckController());
    }

    public void VisitMain() {
        SceneManager.LoadScene("StartMenu");
    }

    public void ChangeUpSelectionForMenuButton()
    {
        buttonToReturnTo = EventSystem.current.currentSelectedGameObject.GetComponent<Button>();
        Navigation customNav = new Navigation
        {
            mode = Navigation.Mode.Explicit,
            selectOnUp = buttonToReturnTo
        };
        menuButton.navigation = customNav;
    }
}
