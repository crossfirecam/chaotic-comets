using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public partial class GameManager : MonoBehaviour
{
    // When a level starts, perform starting operations

    private IEnumerator StartNewLevel()
    {
        levelNo++;
        // Asteroid number depends on level number. Iterated in SpawnProp()
        asteroidCount = 0;
        for (int i = 0; i < levelNo + 1; i++) { SpawnProp(PropType.Asteroid); }
        yield return new WaitForSeconds(0.1f);
        // Player Respawn
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.RespawnShip(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.RespawnShip(); }

        // Set a cap on how many UFOs can spawn. Double this in two-player. An aside... always one canteen per player per level.
        if (levelNo == 1) { ufoCap = 0; }
        else if (levelNo <= 3) { ufoCap = 1; }
        else if (levelNo <= 7) { ufoCap = 2; }
        else { ufoCap = 3; }

        // Double the cap if both players are alive
        if (!player1dead)
        {
            if (!player2dead)
            {
                canisterCap *= 2;
                ufoCap *= 2;
            }
        }

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
            foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<Ufo>().TeleportStart(); }
            Invoke("EndLevelFanFare2", 2.5f);
        }
    }
    private void EndLevelFanFare2()
    {
        Refs.gameLevelPanel.SetActive(true);
        // Player Shield Recovery
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.ShipIsRecovering(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.ShipIsRecovering(); }
        StartCoroutine(FadeBlack("to"));
        Invoke("BringUpShop", 3f);
    }

    // Leaves Main scene and brings up the shop
    private void BringUpShop()
    {
        BetweenScenesScript.ResumingFromSave = true;
        Saving_SaveManager.SaveData(this, Refs.playerShip1.gameObject, Refs.playerShip2.gameObject);
        SceneManager.LoadScene("ShopMenu");
    }

    // Alien UFO that spawns during the end-level transition will check this, and despawn if level transition is happening
    public bool CheckIfEndOfLevel()
    {
        if (asteroidCount <= 0 && !tutorialMode) { return true; }
        return false;
    }

    // If a ship has less than full shields, show the text say shields are being recharged
    public void ShowRechargeText() { Refs.gameLevelShieldRechargeText.SetActive(true); }

    // Show game over panel and pause the game when the game is over
    public void GameOver()
    {
        Cursor.visible = true;
        BetweenScenesScript.ResumingFromSave = false;
        Saving_SaveManager.EraseData();
        Refs.gameOverPanel.SetActive(true);
        Refs.buttonWhenGameOver.Select();

        musicLoop.Pause();
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<Ufo>().CheckAlienSounds(1); }

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
        Image tempFade = Refs.fadeBlack.GetComponent<Image>();
        Color origColor = tempFade.color;
        float speedOfFade = 0.4f;
        Refs.fadeBlack.SetActive(true);
        if (ToOrFrom == "from")
        {
            fadingAlpha = 1f;
            while (fadingAlpha > 0f)
            {
                fadingAlpha -= speedOfFade * Time.deltaTime;
                tempFade.color = new Color(origColor.r, origColor.g, origColor.b, fadingAlpha);
                yield return null;
            }
            Refs.fadeBlack.SetActive(false);
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
