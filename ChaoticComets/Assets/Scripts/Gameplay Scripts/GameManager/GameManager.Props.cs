using System.Collections;
using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("Prop Variables")]
    private readonly int lastLevelWithoutEnemies = 1;


    private float chanceOfSpawningUfo, baseChanceOfSpawningUfo = 0.1f;
    private const float SpawningUfoInterval = 8f;
    private int ufoAmountSpawned, ufoCap;
    /// <summary>
    /// Each round except the first, UFO's are guaranteed to appear occasionally.<br/>
    /// Attempt to spawn a UFO every 8 seconds, 20% chance of success.<br/>
    /// Each time it fails, increase the chance by 5%. When it succeeds, reset chance to 20%.
    /// </summary>
    private IEnumerator UfoSpawning()
    {
        if (levelNo <= lastLevelWithoutEnemies)
            yield return null;

        // Difficulty increases UFO spawn chances
        if (BetweenScenes.Difficulty == 1)
            baseChanceOfSpawningUfo = 0.2f;
        else if (BetweenScenes.Difficulty == 2)
            baseChanceOfSpawningUfo = 0.3f;
        chanceOfSpawningUfo = baseChanceOfSpawningUfo;

        yield return new WaitForSeconds(8); // Wait before starting.

        while (ufoAmountSpawned < ufoCap)
        {
            Debug.Log("Attempting UFO spawn");
            float chanceCheck = Random.Range(0f, 1f);
            if (chanceCheck < chanceOfSpawningUfo)
            {
                ufoAmountSpawned += 1;
                ChooseUfoAndSpawn();
                chanceOfSpawningUfo = baseChanceOfSpawningUfo;
            }
            else
                chanceOfSpawningUfo += 0.05f;
            yield return new WaitForSeconds(SpawningUfoInterval);
        }
    }

    private readonly float chanceOfAppearing = 0.5f, chanceOfTwoAppearingIn2P = 0.25f;
    private readonly float chanceOfSpawningCanister = 0.1f, spawningCanisterInterval = 5f;
    private int canisterAmountSpawned = 0, canisterCap = 1;
    /// <summary>
    /// Each round, canisters only appear 50% of the entire time. In 25% of those cases and only in 2P mode, spawn another one.<br/>
    /// If a canister is set to appear, every 5 seconds it has a 10% chance to spawn.
    /// </summary>
    private IEnumerator CanisterSpawning()
    {
        // Does canister appear this wave?
        float appearCheck = Random.Range(0f, 1f);
        if (appearCheck < chanceOfAppearing)
            yield return null;

        // In 2P, two canisters appearing is a 25% chance of successful spawn attempts.
        if (canisterCap == 2)
        {
            float doubleSpawnCheck = Random.Range(0f, 1f);
            if (doubleSpawnCheck > chanceOfTwoAppearingIn2P)
                canisterCap = 1;
        }

        yield return new WaitForSeconds(5); // Wait before starting.

        while (canisterAmountSpawned < canisterCap)
        {
            float chanceCheck = Random.Range(0f, 1f);
            if (chanceCheck < chanceOfSpawningCanister)
            {
                canisterAmountSpawned += 1;
                SpawnProp(PropType.Canister);
            }
            yield return new WaitForSeconds(spawningCanisterInterval);
        }
    }

    /// <summary>
    /// When spawning a UFO, choose UfoFollower or UfoPasser with an equal chance.
    /// </summary>
    public void ChooseUfoAndSpawn() {
        float randomiser = Random.Range(0f, 2f);
        if (randomiser < 1f)
        {
            SpawnProp(PropType.UfoFollower);
        }
        else
        {
            SpawnProp(PropType.UfoPasser);
        }
    }

    public enum PropType { Asteroid, Canister, UfoFollower, UfoPasser };
    public void SpawnProp(PropType type, Vector2 chosenLocation = default, bool safeVersion = false)
    {
        // Determine spawn direction.
        Vector2 spawnPosition = new Vector2();
        if (chosenLocation == default)
        {
            float originChoice = Random.Range(0f, 4f);

            if (type == PropType.UfoPasser) // UFO Passer can only spawn on left or right.
                originChoice = Random.Range(0f, 2f);

            if (originChoice < 1f)
                spawnPosition = SideOfScreen("Left");
            else if (originChoice < 2f)
                spawnPosition = SideOfScreen("Right");
            else if (originChoice < 3f)
                spawnPosition = SideOfScreen("Top");
            else if (originChoice < 4f)
                spawnPosition = SideOfScreen("Bottom");
        }
        else
        {
            spawnPosition = chosenLocation;
        }

        // Determine object to be spawned.
        if (type == PropType.UfoFollower)
        {
            Instantiate(Refs.ufoFollowerProp, spawnPosition, Quaternion.identity, Refs.propParent);
        }
        if (type == PropType.UfoPasser)
        {
            Instantiate(Refs.ufoPasserProp, spawnPosition, Quaternion.identity, Refs.propParent);
        }
        else if (type == PropType.Canister)
        {
            Instantiate(Refs.canisterProp, spawnPosition, Quaternion.identity, Refs.propParent);
        }
        else if (type == PropType.Asteroid)
        {
            GameObject newAsteroid;
            if (!safeVersion)
                Instantiate(Refs.largeAsteroidProp, spawnPosition, Quaternion.identity, Refs.propParent);
            else
                Instantiate(Refs.largeAsteroidSafeProp, spawnPosition, Quaternion.identity, Refs.propParent);

            asteroidCount += 1;
        }
    }

    private Vector2 SideOfScreen(string side)
    {
        switch (side)
        {
            case "Left":
                return new Vector2(screenLeft - 1.5f, Random.Range(-6f, 8f));
            case "Right":
                return new Vector2(screenRight + 1.5f, Random.Range(-6f, 8f));
            case "Top":
                return new Vector2(Random.Range(-13f, 13f), screenTop + 1.5f);
            default: // Bottom
                return new Vector2(Random.Range(-13f, 13f), screenBottom - 1.5f);
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

    public void SpawnPropFromCheat(string propStr)
    {
        switch (propStr)
        {
            case "asteroid":
                SpawnProp(PropType.Asteroid); break;
            case "canister":
                SpawnProp(PropType.Canister); break;
            case "ufo-follower":
                SpawnProp(PropType.UfoFollower); break;
            case "ufo-passer":
                SpawnProp(PropType.UfoPasser); break;
        }
    }

    public void RespawnPropForTutorial(string propStr)
    {
        switch (propStr)
        {
            case "canister":
                StartCoroutine(RespawnPropForTutorial2(PropType.Canister));
                break;
            case "ufo-passer":
                StartCoroutine(RespawnPropForTutorial2(PropType.UfoPasser));
                break;
        }
    }

    private IEnumerator RespawnPropForTutorial2(PropType prop)
    {
        yield return new WaitForSeconds(1);
        SpawnProp(prop);
    }
}
