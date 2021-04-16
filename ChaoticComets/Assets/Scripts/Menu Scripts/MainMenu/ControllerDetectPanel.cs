using UnityEngine;
using UnityEngine.UI;
using Rewired;

public class ControllerDetectPanel : MonoBehaviour
{
    [SerializeField] private bool checksForPlayerCount = false;
    private bool onlyShowPl2 = false;
    [SerializeField] private Image statusImgPl1, statusImgPl2;
    [SerializeField] private Sprite sprKeyboard, sprController;
    [SerializeField] private GameObject p1title, p2title;
    private Player player1, player2;

    private void Awake()
    {

        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);
        CheckControllers();

        // If Tutorial is set to P2 controls, then hide P1's control display
        if (onlyShowPl2)
            InTutorialSetP2ToLearner();
        // If only one player is in MainScene or ShopMenu, then hide P2's control display
        else if (checksForPlayerCount && BetweenScenes.PlayerCount == 1)
            DisableP2Display();

        if (PlayerPrefs.GetInt("ContDialog") == 1)
            gameObject.SetActive(false);
    }

    public void CheckControllers()
    {
        if (player1.controllers.Joysticks.Count == 0)
            statusImgPl1.sprite = sprKeyboard;
        else
            statusImgPl1.sprite = sprController;

        if (player2.controllers.Joysticks.Count == 0)
            statusImgPl2.sprite = sprKeyboard;
        else
            statusImgPl2.sprite = sprController;
    }

    // Used in 1P mode or Tutorial when P1 is set to be the learner.
    private void DisableP2Display()
    {
        p2title.SetActive(false);
        statusImgPl2.gameObject.SetActive(false);
    }
    
    public void SetTutorialP2Only()
    {
        onlyShowPl2 = true;
    }
    // Only used in Tutorial when P2 is set to be the learner.
    private void InTutorialSetP2ToLearner()
    {
        p1title.SetActive(false);
        statusImgPl1.gameObject.SetActive(false);
        p2title.SetActive(true);
        statusImgPl2.gameObject.SetActive(true);
    }

    void OnControllerConnected(ControllerStatusChangedEventArgs args)
    {
        CheckControllers();
    }

    void OnControllerDisconnected(ControllerStatusChangedEventArgs args)
    {
        CheckControllers();
    }

    private void OnDestroy()
    {
        ReInput.ControllerConnectedEvent -= OnControllerConnected;
        ReInput.ControllerDisconnectedEvent -= OnControllerDisconnected;
    }
}
