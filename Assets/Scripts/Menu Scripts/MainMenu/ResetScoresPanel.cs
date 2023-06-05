using UnityEngine;
using UnityEngine.UI;

public class ResetScoresPanel : MonoBehaviour
{
    [SerializeField] private MainMenu mainMenu;


    Navigation navSelectUp = new Navigation { mode = Navigation.Mode.Explicit };
    Navigation navSelectDown = new Navigation { mode = Navigation.Mode.Explicit };

    public void ShowResetPanel()
    {
        mainMenu.NotifyMainMenuWhichButtonToReturnTo();

        Transform resetDialogItself = transform.Find("ResetScoresDialog");
        Button resetNoButton = resetDialogItself.Find("ResetNoButton").GetComponent<Button>();
        resetNoButton.Select();
    }
}
