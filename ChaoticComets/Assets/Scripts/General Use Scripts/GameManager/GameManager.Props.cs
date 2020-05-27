using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("Prop Variables")]
    public bool instantkillAsteroids = false;
    private readonly int lastLevelWithoutEnemies = 1;
    private float ufoAmountSpawned, canisterAmountSpawned, ufoCap, canisterCap = 1; // Variables used to track how many props have, and can spawn.

    [Header("Player Status Variables")]
    public bool player1dead = false;
    public bool player2dead = true; // Only one player by default
    [HideInInspector] public bool player1TEMPDEAD = false, player2TEMPDEAD = false; // Only used to alert UFO that player is temporarily inactive

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
        float[] canisterSubsequentArray = { 12f, 24f };
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

        // If in the tutorial, canisters and aliens will respawn infinitely until dealt with
        if (tutorialMode)
        {
            if (reason == PropSpawnReason.CanisterRespawn)
            {
                Invoke("RespawnCanister", 1.0f);
            }
            else if (reason == PropSpawnReason.AlienRespawn)
            {
                if (Refs.tutorialManager.GetComponent<TutorialManager>().popUpIndex == 12)
                {
                    SpawnProp(PropType.UfoPasser);
                }
                else if (Refs.tutorialManager.GetComponent<TutorialManager>().popUpIndex == 15)
                {
                    SpawnProp(PropType.UfoFollower);
                }
            }
        }
        else if ((reason == PropSpawnReason.AlienFirst || reason == PropSpawnReason.AlienRespawn) && ufoAmountSpawned < ufoCap)
        {
            if (levelNo > lastLevelWithoutEnemies)
            { // Alien will not appear until a certain level
                ufoAmountSpawned += 1;
                print($"Next UFO will spawn in: {randomTime}. Only {ufoCap - ufoAmountSpawned} more can spawn.");
                Invoke("RespawnAlien", randomTime);
            }
        }
        else if ((reason == PropSpawnReason.CanisterFirst || reason == PropSpawnReason.CanisterRespawn) && canisterAmountSpawned < canisterCap)
        {
            canisterAmountSpawned += 1;
            print($"Next canister will spawn in: {randomTime}. Only {canisterCap - canisterAmountSpawned} more can spawn.");
            Invoke("RespawnCanister", randomTime);
        }
    }

    // When alien or powerup is required, call SpawnProp
    public void RespawnAlien() {
        float randomiser = Random.Range(0f, 2f);
        if (randomiser < 1f)
        {
            SpawnProp(PropType.UfoFollower);
        }
        else if (randomiser < 2f)
        {
            SpawnProp(PropType.UfoPasser);
        }
    }

    public void RespawnCanister() { SpawnProp(PropType.Canister); }

    public enum PropType { Asteroid, Canister, UfoFollower, UfoPasser };
    public void SpawnProp(PropType type, Vector2 chosenLocation = default, bool safeVersion = false)
    {
        Vector2 spawnPosition = new Vector2();
        if (chosenLocation == default)
        {
            float originChoice = Random.Range(0f, 4f);
            if (type == PropType.UfoPasser)
            {
                originChoice = 0.5f;
            }
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
        }
        else
        {
            spawnPosition = chosenLocation;
        }
        if (type == PropType.UfoFollower)
        {
            GameObject newFollower = Instantiate(Refs.ufoFollowerProp);
            newFollower.transform.position = spawnPosition;
        }
        if (type == PropType.UfoPasser)
        {
            GameObject newPasser = Instantiate(Refs.ufoPasserProp);
            newPasser.transform.position = spawnPosition;
        }
        else if (type == PropType.Canister)
        {
            GameObject newCanister = Instantiate(Refs.canisterProp);
            newCanister.transform.position = spawnPosition;
        }
        else if (type == PropType.Asteroid)
        {
            GameObject newAsteroid;
            if (!safeVersion)
            {
                newAsteroid = Instantiate(Refs.largeAsteroidProp, spawnPosition, Quaternion.identity);
            }
            else {
                newAsteroid = Instantiate(Refs.largeAsteroidSafeProp, spawnPosition, Quaternion.identity);
            }
            asteroidCount += 1;
            if (instantkillAsteroids)
            {
                GameObject childAsteroid = newAsteroid.transform.GetChild(0).gameObject;
                childAsteroid.GetComponent<AsteroidBehaviour>().DebugMode();
            }
        }
    }

    // Update asteroids for each one destroyed. If in normal gameplay, end the level at 0 asteroids.
    public void UpdateNumberAsteroids(int change)
    {
        asteroidCount += change;
        if (asteroidCount == 0 && !tutorialMode)
        {
            Invoke("EndLevelFanFare", 2f);
        }
    }


    // If a player dies, set the value for their death. If both are dead, game is over
    public void PlayerDied(int playerThatDied)
    {
        print($"Player {playerThatDied} has died.");
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
            ufo.GetComponent<Ufo>().PlayerDied();
        }
    }

    public void SpawnPropFromCheat(string prop)
    {
        if (prop == "asteroid")
        {
            SpawnProp(PropType.Asteroid);
        }
        else if (prop == "canister")
        {
            SpawnProp(PropType.Canister);
        }
        else if (prop == "ufo-follower")
        {
            SpawnProp(PropType.UfoFollower);
        }
        else if (prop == "ufo-passer")
        {
            SpawnProp(PropType.UfoPasser);
        }
    }
}
