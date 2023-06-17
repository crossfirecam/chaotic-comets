using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;
using System;

public class MainPanel : MonoBehaviour
{
    [SerializeField] private int plrIndex;
    private EventSystemShop plrEvents;
    private PurchasePanel purchasePanel;

    [HideInInspector] public Button readyBtn;
    private Button btnToBlockWhenReadied;
    private TextMeshProUGUI readyBtnText;

    public bool plrReady = false;

    private void Start()
    {
        readyBtn = transform.Find("ButtonReady").GetComponent<Button>();
        btnToBlockWhenReadied = transform.Find("MaintenanceSection/Button8").GetComponent<Button>();
        readyBtnText = readyBtn.GetComponentInChildren<TextMeshProUGUI>();
        purchasePanel = transform.parent.Find("PurchasePanel").GetComponent<PurchasePanel>();

        LoadInButtonArrays();
        plrEvents = ShopScript.i.ShopRefs.plrEventSystems[plrIndex];
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * MainButtonHovered - Change what's shown on the purchasing panel when a button is hovered over.
     * ------------------------------------------------------------------------------------------------------------------ */
    public void MainButtonHovered()
    {
        // Find button pressed, turn the last character in the name into an integer
        Button buttonHovered = plrEvents.currentSelectedGameObject.GetComponent<Button>();
        bool success = int.TryParse(buttonHovered.name.Last().ToString(), out int whichHovered);
        if (!success) { whichHovered = 10; } // If last char can't be parsed as int, it is the Ready button.
        plrEvents.firstSelectedGameObject = buttonHovered.gameObject;

        purchasePanel.OnMainAreaButtonHover(whichHovered);
    }

    public void PurchaseButtonHovered()
    {
        Button buttonHovered = plrEvents.currentSelectedGameObject.GetComponent<Button>();
        plrEvents.firstSelectedGameObject = buttonHovered.gameObject;
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Ready System - If both players are ready, shop closes.
    * ------------------------------------------------------------------------------------------------------------------ */
    public void ReadyUp()
    {
        Navigation customNav = new Navigation { mode = Navigation.Mode.Explicit };

        // Set text and nav for Ready button (disable moving up if player is readied)
        plrReady = !plrReady;
        readyBtnText.text = plrReady ? "Unready" : "Ready";
        customNav.selectOnRight = plrReady ? null : btnToBlockWhenReadied;
        readyBtn.navigation = customNav;

        // Find all of a player's buttons that aren't 'Ready' button, and disable them
        foreach (Button btn in mainPlayerButtons)
        {
            if (btn != readyBtn)
            {
                btn.interactable = !plrReady;
            }
        }

        ShopScript.i.CheckReadyStatus();
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Swap between Purchase Panel and Main Panel.
    * ------------------------------------------------------------------------------------------------------------------ */
    private GameObject btnLastPressedInMainArea;
    public void EnterPurchasePanel()
    {
        btnLastPressedInMainArea = plrEvents.currentSelectedGameObject;
        foreach (Button btn in mainPlayerButtons)
            btn.interactable = false;
        purchasePanel.PurchasePanelOpened(plrEvents);
    }
    public void LeavePurchasePanel()
    {
        foreach (Button btn in mainPlayerButtons)
            btn.interactable = true;
        purchasePanel.PurchasePanelClosed();
        plrEvents.SetSelectedGameObject(btnLastPressedInMainArea);
    }

    /* ------------------------------------------------------------------------------------------------------------------
    * Button Lists - Many functions in ShopScript tell a variety of buttons to toggle interactivity. To do this, initilise the buttons that belong to this player.
    * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private Button[] mainPlayerButtons;
    private void LoadInButtonArrays()
    {
        Button[] listOfButtons = FindObjectsOfType<Button>();
        List<Button> buttonsTemp = new List<Button>();

        foreach (Button btn in listOfButtons)
        {
            if (btn.transform.parent == transform || btn.transform.parent.parent == transform)
                buttonsTemp.Add(btn);
        }
        mainPlayerButtons = buttonsTemp.ToArray();
    }
}
