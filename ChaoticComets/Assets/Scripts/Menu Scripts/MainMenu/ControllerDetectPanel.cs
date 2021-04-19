using UnityEngine;
using UnityEngine.UI;
using Rewired;
using TMPro;

public class ControllerDetectPanel : MonoBehaviour
{
    [SerializeField] private bool checksForPlayerCount = false;
    private bool onlyShowPl2 = false;
    [SerializeField] private Image statusImgPl1, statusImgPl2;
    [SerializeField] private Sprite sprKeyboard, sprController;
    [SerializeField] private GameObject p1title, p2title;
    [SerializeField] private TextMeshProUGUI p1Desc, p2Desc;
    private Player player1, player2;
    private RectTransform rt;

    private void Awake()
    {
        rt = GetComponent<RectTransform>();
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        ReInput.ControllerDisconnectedEvent += OnControllerDisconnected;
        player1 = ReInput.players.GetPlayer(0);
        player2 = ReInput.players.GetPlayer(1);

        // If Tutorial is set to P2 controls, then hide P1's control display
        if (onlyShowPl2)
            DisableP1Display();
        // If only one player is in MainScene or ShopMenu, then hide P2's control display
        else if (checksForPlayerCount && BetweenScenes.PlayerCount == 1)
            DisableP2Display();

        CheckControllers();
        if (PlayerPrefs.GetInt("ContDialog") == 1)
            gameObject.SetActive(false);
    }

    public void CheckControllers()
    {
        if (player1.controllers.Joysticks.Count == 0)
        {
            statusImgPl1.sprite = sprKeyboard;
            p1Desc.text = "";
        }
        else
        {
            statusImgPl1.sprite = sprController;
            p1Desc.text = player1.controllers.Joysticks[0].name;
        }

        if (player2.controllers.Joysticks.Count == 0)
        {
            statusImgPl2.sprite = sprKeyboard;
            p2Desc.text = "";
        }
        else
        {
            statusImgPl2.sprite = sprController;
            p2Desc.text = player2.controllers.Joysticks[0].name;
        }

        // If a player's icon is hidden, set text to nothing so dialog won't expand unnecessarily in next section
        if (!p1title.activeInHierarchy)
            p1Desc.text = "";
        if (!p2title.activeInHierarchy)
            p2Desc.text = "";

        // If there's text showing the current controller, then expand dialog slightly
        if (p1Desc.text != "" || p2Desc.text != "")
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 70);
        else
            rt.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, 60);
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
    private void DisableP1Display()
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
