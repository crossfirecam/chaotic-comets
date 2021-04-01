using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("Prop Variables")]
    private readonly int lastLevelWithoutEnemies = 1;
    private float ufoAmountSpawned, canisterAmountSpawned, ufoCap, canisterCap = 1; // Variables used to track how many props have, and can spawn.

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
                Invoke(nameof(RespawnCanister), 1.0f);
            }
            else if (reason == PropSpawnReason.AlienRespawn)
            {
                if (TutorialManager.i.popUpIndex == 12)
                {
                    SpawnProp(PropType.UfoPasser);
                }
                else if (TutorialManager.i.popUpIndex == 15)
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
                Invoke(nameof(RespawnAlien), randomTime);
            }
        }
        else if ((reason == PropSpawnReason.CanisterFirst || reason == PropSpawnReason.CanisterRespawn) && canisterAmountSpawned < canisterCap)
        {
            canisterAmountSpawned += 1;
            print($"Next canister will spawn in: {randomTime}. Only {canisterCap - canisterAmountSpawned} more can spawn.");
            Invoke(nameof(RespawnCanister), randomTime);
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
                originChoice = Random.Range(0f, 2f);
            }
            if (originChoice < 1f)
            { // Spawn on the left
                spawnPosition = new Vector2(screenLeft - 1.5f, Random.Range(-6f, 8f));
            }
            else if (originChoice < 2f)
            { // Spawn on the right
                spawnPosition = new Vector2(screenRight + 1.5f, Random.Range(-6f, 8f));
            }
            else if (originChoice < 3f)
            { // Spawn on the top
                spawnPosition = new Vector2(Random.Range(-13f, 13f), screenTop + 1.5f);
            }
            else if (originChoice < 4f)
            { // Spawn on the bottom
                spawnPosition = new Vector2(Random.Range(-13f, 13f), screenBottom - 1.5f);
            }
        }
        else
        {
            spawnPosition = chosenLocation;
        }
        if (type == PropType.UfoFollower)
        {
            GameObject newFollower = Instantiate(Refs.ufoFollowerProp, Refs.propParent);
            newFollower.transform.position = spawnPosition;
        }
        if (type == PropType.UfoPasser)
        {
            GameObject newPasser = Instantiate(Refs.ufoPasserProp, Refs.propParent);
            newPasser.transform.position = spawnPosition;
        }
        else if (type == PropType.Canister)
        {
            GameObject newCanister = Instantiate(Refs.canisterProp, Refs.propParent);
            newCanister.transform.position = spawnPosition;
        }
        else if (type == PropType.Asteroid)
        {
            GameObject newAsteroid;
            if (!safeVersion)
            {
                newAsteroid = Instantiate(Refs.largeAsteroidProp, spawnPosition, Quaternion.identity, Refs.propParent);
            }
            else {
                newAsteroid = Instantiate(Refs.largeAsteroidSafeProp, spawnPosition, Quaternion.identity, Refs.propParent);
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
            Invoke(nameof(EndLevelFanFare), 1f);
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
