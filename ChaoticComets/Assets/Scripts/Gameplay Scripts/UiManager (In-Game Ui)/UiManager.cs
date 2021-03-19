using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public partial class UiManager : MonoBehaviour
{
    private static UiManager _i;
    public static UiManager i { get { if (_i == null) _i = FindObjectOfType<UiManager>(); return _i; } }

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

    /* ------------------------------------------------------------------------------------------------------------------
     * Fade screen to black
     * ------------------------------------------------------------------------------------------------------------------ */
    [Header("Fade Screen To Black UI")]
    [SerializeField] private GameObject fadeBlack;
    private float fadingAlpha = 0f;

    // FadeBlack is used to gradually fade the screen to or from black
    public IEnumerator FadeScreenBlack(string ToOrFrom)
    {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.6f;
        fadeBlack.SetActive(true);
        if (ToOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            fadeBlack.SetActive(false);
        }
        else if (ToOrFrom == "to")
        {
            fadingAlpha = 0f;
            speedOfFade = 1.2f;
            yield return new WaitForSeconds(2f);
            while (fadingAlpha < 1f)
            {
                fadingAlpha += speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
        }
    }

}
