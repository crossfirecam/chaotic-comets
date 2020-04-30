using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour
{
    // When a level starts, perform starting operations
    void StartNewLevel()
    {
        levelNo++;
        // Asteroid number depends on level number. Iterated in SpawnProp()
        asteroidCount = 0;
        for (int i = 0; i < levelNo + 1; i++) { SpawnProp("asteroid"); }
        // Player Respawn
        if (!player1dead) { playerShip1.playerSpawnDeath.RespawnShip(); }
        if (!player2dead) { playerShip2.playerSpawnDeath.RespawnShip(); }

        // Set a cap on how many UFOs or canisters can spawn
        if (levelNo == 1) { propCap = 2; }
        else if (levelNo < 3) { propCap = 3; }
        else if (levelNo < 9) { propCap = 4; }
        else { propCap = 5; }
        // Set when the first UFO and canister will spawn
        AlienAndPowerupLogic(PropSpawnReason.AlienFirst);
        AlienAndPowerupLogic(PropSpawnReason.CanisterFirst);
    }

    // At the end of a level activate level transition dialog, set the UFO to disappear, and if a player is still alive, respawn them
    public void EndLevelFanFare()
    {
        if (!player1dead || !player2dead)
        {
            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
            foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<UfoAllTypes>().TeleportStart(); }
            Invoke("EndLevelFanFare2", 2.5f);
        }
    }
    private void EndLevelFanFare2()
    {
        gameLevelPanel.SetActive(true);
        // Player Shield Recovery
        if (!player1dead) { playerShip1.playerSpawnDeath.ShipIsRecovering(); }
        if (!player2dead) { playerShip2.playerSpawnDeath.ShipIsRecovering(); }
        StartCoroutine(FadeBlack("to"));
        Invoke("BringUpShop", 3f);
    }

    // Leaves Main scene and brings up the shop
    private void BringUpShop()
    {
        BetweenScenesScript.ResumingFromSave = true;
        Saving_SaveManager.SaveData(this, playerShip1.gameObject, playerShip2.gameObject);
        SceneManager.LoadScene("ShopMenu");
    }

    // Alien UFO that spawns during the end-level transition will check this, and despawn if level transition is happening
    public bool CheckIfEndOfLevel()
    {
        if (asteroidCount <= 0) { return true; }
        return false;
    }

    // If a ship has less than full shields, show the text say shields are being recharged
    public void ShowRechargeText() { gameLevelShieldRechargeText.SetActive(true); }

    // Show game over panel and pause the game when the game is over
    public void GameOver()
    {
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false;
        Saving_SaveManager.EraseData();
        gameOverPanel.SetActive(true);
        buttonWhenGameOver.Select();

        musicLoop.Pause();
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<UfoAllTypes>().CheckAlienSounds(1); }

        Time.timeScale = 0;
    }

    // Reload the scene and restart playback if user decides to play again
    public void PlayAgain()
    {
        SceneManager.LoadScene("MainScene");
        Time.timeScale = 1;
    }

    private IEnumerator FadeBlack(string ToOrFrom)
    {
        Image tempFade = fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.4f;
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
