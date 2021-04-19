using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using static Constants;

public partial class GameManager : MonoBehaviour
{
    /* ------------------------------------------------------------------------------------------------------------------
     * Start of Level Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // When a level starts, perform starting operations
    private IEnumerator StartNewLevel()
    {
        yield return new WaitForSeconds(0.05f);

        // Increase level count, and erase autosave data
        levelNo++;
        UiManager.i.SetWaveText(levelNo);
        Saving_SaveManager.EraseData();

        // Tell ships to disable model & colliders if they are dead
        if (player1dead)
            Refs.playerShip1.plrSpawnDeath.PretendShipDoesntExist(true);
        if (player2dead && BetweenScenes.PlayerCount == 2)
            Refs.playerShip2.plrSpawnDeath.PretendShipDoesntExist(true);

        // Asteroid number depends on level number. Iterated in SpawnProp(). Hard cap of 15 asteroids.
        asteroidCount = 0;
        int asteroidCap = (levelNo + 3) / 2;
        if (asteroidCap >= 15)
            asteroidCap = 15;

        for (int i = 0; i < asteroidCap; i++)
            SpawnProp(PropType.Asteroid);

        yield return new WaitForSeconds(0.01f);

        // Player Respawn
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.RespawnShip(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.RespawnShip(); }

        // Set a cap on how many UFOs can spawn.
        if (levelNo == 1)
            ufoCap = 0;
        else
            ufoCap = (levelNo / 3) + 1;

        // Double the Canister and UFO cap if both players are alive
        if (!(player1dead || player2dead))
        {
            canisterCap *= 2; ufoCap *= 2;
        }

        // Set when the first UFO and canister will spawn
        StartCoroutine(nameof(UfoSpawning));
        StartCoroutine(nameof(CanisterSpawning));

        StartCoroutine(nameof(TimeBonusCounter));
    }

    /* ------------------------------------------------------------------------------------------------------------------
     * End of Level Methods
     * ------------------------------------------------------------------------------------------------------------------ */

    // At the end of a level activate level transition dialog, set the UFO to disappear, and if a player is still alive, refill their shields
    public void EndLevelFanFare()
    {
        if (!player1dead || !player2dead)
        {
            float waitTime = 1f;
            GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag(Tag_Ufo);
            if (listOfUfos.Length != 0)
            {
                foreach (GameObject ufo in listOfUfos) { ufo.GetComponent<Ufo>().TeleportStart(true); }
                waitTime = 2f;
            }
            Invoke(nameof(EndLevelFanFare2), waitTime);
        }
    }
    private void EndLevelFanFare2()
    {
        // Player Shield Recovery
        if (!player1dead) { Refs.playerShip1.plrSpawnDeath.ShipIsRecovering(); }
        if (!player2dead) { Refs.playerShip2.plrSpawnDeath.ShipIsRecovering(); }
        UiManager.i.LevelCompleted(bonusValue);
        // Shop scene will open after 2 second wait, bonus counter, another 2 second wait
    }

    // Leaves Main scene and brings up the shop
    private void BringUpShop()
    {
        BetweenScenes.ResumingFromSave = true;
        Saving_SaveManager.SaveData(this, Refs.playerShip1, Refs.playerShip2);
        SceneManager.LoadScene("ShopMenu");
    }

    // Alien UFO that spawns during the end-level transition will check this, and despawn if level transition is happening
    public bool CheckIfEndOfLevel()
    {
        if (asteroidCount <= 0 && !tutorialMode) { return true; }
        return false;
    }


}
