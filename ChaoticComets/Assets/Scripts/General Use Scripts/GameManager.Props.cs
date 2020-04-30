using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    public bool instantkillAsteroids = false;
    private readonly int lastLevelWithoutEnemies = 1;

    public PlayerMain playerShip1, playerShip2;
    public bool player1dead = false, player2dead = true; // Only one player by default
    [HideInInspector] public bool player1TEMPDEAD = false, player2TEMPDEAD = false; // Only used to alert UFO that player is temporarily inactive

    public GameObject largeAsteroidProp, alienShipProp, canisterProp;
    private float ufoAmountSpawned, canisterAmountSpawned, propCap; // Variables used to track how many props have, and can spawn.

    public enum PropSpawnReason { AlienFirst, CanisterFirst, AlienRespawn, CanisterRespawn };
    public void AlienAndPowerupLogic(PropSpawnReason reason)
    {
        // Alien has 10-15sec to spawn the first time.
        // Alien has 15-30sec to spawn all following times.
        // Canister has 8-20sec to spawn the first time.
        // Canister has 20-40sec to spawn all following times.

        float[] alienFirstTimeArray = { 10f, 15f };
        float[] alienSubsequentArray = { 15f, 30f };
        float[] canisterFirstTimeArray = { 8f, 16f };
        float[] canisterSubsequentArray = { 20f, 40f };
        float[] chosenArray = { }; // Left blank to be filled by one of the above
        float minTime; float maxTime;

        switch (reason)
        {
            case PropSpawnReason.AlienFirst:
                chosenArray = alienFirstTimeArray; break;
            case PropSpawnReason.CanisterFirst:
                chosenArray = canisterFirstTimeArray; break;
            case PropSpawnReason.AlienRespawn:
                chosenArray = alienSubsequentArray; break;
            case PropSpawnReason.CanisterRespawn:
                chosenArray = canisterSubsequentArray; break;
        }

        minTime = chosenArray[0];
        maxTime = chosenArray[1];

        float randomTime = Random.Range(minTime, maxTime);

        if ((reason == PropSpawnReason.AlienFirst || reason == PropSpawnReason.AlienRespawn) && ufoAmountSpawned < propCap)
        {
            if (levelNo > lastLevelWithoutEnemies)
            { // Alien will not appear until a certain level
                ufoAmountSpawned += 1;
                Debug.Log($"Next UFO will spawn in: {randomTime}. Only {propCap - ufoAmountSpawned} more can spawn.");
                Invoke("RespawnAlien", randomTime);
            }
        }
        else if ((reason == PropSpawnReason.CanisterFirst || reason == PropSpawnReason.CanisterRespawn) && canisterAmountSpawned < propCap)
        {
            canisterAmountSpawned += 1;
            Debug.Log($"Next canister will spawn in: {randomTime}. Only {propCap - canisterAmountSpawned} more can spawn.");
            Invoke("RespawnCanister", randomTime);
        }
    }

    // When alien or powerup is required, call SpawnProp
    public void RespawnAlien() { SpawnProp("ufo"); }

    public void RespawnCanister() { SpawnProp("canister"); }

    public void SpawnProp(string type)
    {
        float originChoice = Random.Range(0f, 4f);
        Vector2 spawnPosition = new Vector2();
        if (originChoice < 1f)
        { // Spawn on the left
            spawnPosition = new Vector2(screenLeft - 1.5f, Random.Range(-9f, 9f));
        }
        else if (originChoice < 2f)
        { // Spawn on the right
            spawnPosition = new Vector2(screenRight + 1.5f, Random.Range(-9f, 9f));
        }
        else if (originChoice < 3f)
        { // Spawn on the top
            spawnPosition = new Vector2(Random.Range(-12f, 12f), screenTop + 1.5f);
        }
        else if (originChoice < 4f)
        { // Spawn on the bottom
            spawnPosition = new Vector2(Random.Range(-12f, 12f), screenBottom - 1.5f);
        }
        if (type == "ufo")
        {
            GameObject newAlien = Instantiate(alienShipProp);
            newAlien.transform.position = spawnPosition;
        }
        else if (type == "canister")
        {
            GameObject newCanister = Instantiate(canisterProp);
            newCanister.transform.position = spawnPosition;
        }
        else if (type == "asteroid")
        {
            GameObject newAsteroid = Instantiate(largeAsteroidProp, spawnPosition, Quaternion.identity);
            asteroidCount += 1;
            if (instantkillAsteroids)
            {
                GameObject childAsteroid = newAsteroid.transform.GetChild(0).gameObject;
                childAsteroid.GetComponent<AsteroidBehaviour>().DebugMode();
            }
        }
    }

    // If in normal gameplay, update asteroids for each one destroyed.
    public void UpdateNumberAsteroids(int change)
    {
        if (!helpMenuMode)
        {
            asteroidCount += change;
            if (asteroidCount == 0)
            {
                Invoke("EndLevelFanFare", 2f);
            }
        }
    }


    // If a player dies, set the value for their death. If both are dead, game is over
    public void PlayerDied(int playerThatDied)
    {
        Debug.Log($"Player {playerThatDied} has died.");
        if (playerThatDied == 1) { player1dead = true; }
        else if (playerThatDied == 2) { player2dead = true; }
        if (player1dead && player2dead)
        {
            Invoke("GameOver", 2f);
        }
    }

    // When either ship is destroyed, alien will change target
    public void PlayerLostLife(int playerNumber)
    {
        if (playerNumber == 1) { player1TEMPDEAD = true; }
        else if (playerNumber == 2) { player2TEMPDEAD = true; }
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject ufo in listOfUfos)
        {
            ufo.GetComponent<UfoAllTypes>().PlayerDied();
        }
    }
}
