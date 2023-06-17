using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public partial class UiManager : MonoBehaviour
{
    private static UiManager _i;
    public static UiManager i { get { if (_i == null) _i = FindObjectOfType<UiManager>(); return _i; } }

    [Header("Fade Screen Black")]
    public Image fadeBlackOverlay;

    private void Awake()
    {
        plrUiPowerups = new GameObject[][] { plr1UiPowerups, plr2UiPowerups };

        // Find text objects that are children of player's Respawn Overlays
        plrRespawnOverlayTexts = new TextMeshProUGUI[]
        {
            plrRespawnOverlayObjects[0].GetComponentInChildren<TextMeshProUGUI>(),
            plrRespawnOverlayObjects[1].GetComponentInChildren<TextMeshProUGUI>()
        };
    }
}
