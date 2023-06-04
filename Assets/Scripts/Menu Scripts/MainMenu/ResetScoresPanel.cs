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

        // Initialise navigation change variables
        Transform resetDialogItself = transform.Find("ResetScoresDialog");
        Button resetNoButton = resetDialogItself.Find("ResetNoButton").GetComponent<Button>();
        Button resetCPUButton = resetDialogItself.Find("ResetRemoveCPUButton").GetComponent<Button>();
        Button resetYesButton = resetDialogItself.Find("ResetYesButton").GetComponent<Button>();
        resetNoButton.Select();

        // Change attributes of button navigation depending on if the CPU removal button has been pressed (can also be reset)
        bool cpuButtonPressable = PlayerPrefs.GetInt("RemovedCPUs") == 0;
        resetCPUButton.interactable = cpuButtonPressable;
        navSelectDown.selectOnDown = cpuButtonPressable ? resetCPUButton : resetYesButton;
        navSelectUp.selectOnUp = cpuButtonPressable ? resetCPUButton : resetNoButton;
        resetNoButton.navigation = navSelectDown;
        resetYesButton.navigation = navSelectUp;
    }
}
