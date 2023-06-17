using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class HelpMenu : MonoBehaviour {

    [SerializeField] private Button menuButton;
    private Button buttonToReturnTo;
    [SerializeField] private GameObject desktopControls, webControls;

    private void Start()
    {
        StartCoroutine(UsefulFunctions.CheckController());

        #if UNITY_WEBGL
            desktopControls.SetActive(false);
            webControls.SetActive(true);
        #else
            desktopControls.SetActive(true);
            webControls.SetActive(false);
        #endif
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
