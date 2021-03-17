using UnityEngine;
using TMPro;

public class UiManager : MonoBehaviour
{
    private static UiManager _i;
    public static UiManager i { get { if (_i == null) _i = FindObjectOfType<UiManager>(); return _i; } }

    private void Awake()
    {
        plrUiPowerups = new GameObject[][] { plr1UiPowerups, plr2UiPowerups};
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * Game State UI - Wave, Ships, Bonus counters in lower middle of screen.
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private TextMeshProUGUI midUiWave, midUiShips, midUiBonus;

    public void SetWaveText(int wave)
    {
        midUiWave.text = "Wave: " + wave;
        if (wave == -1) // Tutorial Mode
            midUiWave.text = "(Training)";
    }

    public void SetShipsText(int ships)
    {
        midUiShips.text = "Ships: " + ships;
        if (ships <= -1) // Tutorial Mode
            midUiShips.text = "Ships: Inf.";
    }
    public void SetBonusText(int bonus)
    {
        midUiBonus.text = "Bonus: " + bonus;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Current Credits, Total Credits
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private TextMeshProUGUI[] plrUiCurrentCredits, plrUiTotalCredits;
    public void SetPlayerCredits(int playerNum, int current, int total)
    {
        plrUiCurrentCredits[playerNum].text = current + "¢";
        plrUiTotalCredits[playerNum].text = "T: " + total;
    }


    /* ------------------------------------------------------------------------------------------------------------------
     * Player UI - Powerup Icons
     * ------------------------------------------------------------------------------------------------------------------ */
    [SerializeField] private GameObject[] plr1UiPowerups, plr2UiPowerups;
    [SerializeField] private GameObject[][] plrUiPowerups = new GameObject[2][];

    public void SetPlayerPowerup(int playerNum, int powerupId, bool activeState)
    {
        // Set a [Certain Player's] [Chosen Powerup] to (This State)
        plrUiPowerups[playerNum][powerupId].SetActive(activeState);
    }
}
