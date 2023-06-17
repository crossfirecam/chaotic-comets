using System.Collections;
using UnityEngine;
using static Constants;

public partial class GameManager : MonoBehaviour
{
    [Header("Prop Variables")]
    private readonly int lastLevelWithoutEnemies = 1;


    private float chanceOfSpawningUfo, baseChanceOfSpawningUfo = 0.15f;
    private const float SpawningUfoInterval = 8f, StopSpawningNewUfosInterval = 10f;
    private int ufoAmountSpawned, ufoCap;
    /// <summary>
    /// Each round except the first, UFO's are guaranteed to appear occasionally.<br/>
    /// Attempt to spawn a UFO every 8 seconds, 10/20/30% chance of success.<br/>
    /// Each time it fails, increase the chance by 5%. When it succeeds, reset chance to base%.
    /// </summary>
    private IEnumerator UfoSpawning()
    {
        if (levelNo <= lastLevelWithoutEnemies)
            yield return null;

        // Difficulty increases UFO spawn chances
        if (BetweenScenes.Difficulty == 1)
            baseChanceOfSpawningUfo = 0.2f;
        else if (BetweenScenes.Difficulty == 2)
            baseChanceOfSpawningUfo = 0.25f;
        chanceOfSpawningUfo = baseChanceOfSpawningUfo;

        yield return new WaitForSeconds(7); // Wait before starting.

        while (ufoAmountSpawned < ufoCap)
        {
            float chanceCheck = Random.Range(0f, 1f);
            if (chanceCheck < chanceOfSpawningUfo)
            {
                ufoAmountSpawned += 1;
                ChooseUfoAndSpawn();
                chanceOfSpawningUfo = baseChanceOfSpawningUfo;
                yield return new WaitForSeconds(StopSpawningNewUfosInterval); // Guarantee a long gap between UFO's
            }
            else
                chanceOfSpawningUfo += 0.05f;
            yield return new WaitForSeconds(SpawningUfoInterval);
        }
    }

    private const float ChanceOfCanAppearing = 0.75f, ChanceOfCanAppearingHard = 0.375f, ChanceOfTwoCansAppearingIn2P = 0.5f, 
                        ChanceOfLargeCanisterGroup = 0.05f, SpawningCanisterInterval = 5f;
    private float chanceOfSpawningCanister = 0.1f;
    private int canisterAmountSpawned = 0, canisterCap = 1;
    /// <summary>
    /// Each round, canisters only appear 75% of the entire time. In 50% of those cases and only in 2P mode, spawn another one.<br/>
    /// On Hard Mode, canisters only appear 37.5% of the entire time.
    /// <br/>
    /// If a canister is set to appear, every 5 seconds it has a 10% chance to spawn.<br/>
    /// - If this chance fails, the chance is increased by 2.5%. Resets to 10% when the spawn succeeds.<br/>
    /// <br/>
    /// On the first canister spawned, it has a 5% chance to turn into 3 canisters instead.<br/>
    /// - This cancels the second canister spawn if there is one.
    /// </summary>
    private IEnumerator CanisterSpawning()
    {
        // Does canister appear this wave? Hard Mode has lower chance.
        float appearCheck = Random.Range(0f, 1f);
        if (BetweenScenes.Difficulty == 2 && appearCheck > ChanceOfCanAppearingHard)
            yield break;
        if (appearCheck > ChanceOfCanAppearing)
            yield break;

        // In 2P, two canisters appearing is a 50% chance of successful spawn attempts.
        if (canisterCap == 2)
        {
            float doubleSpawnCheck = Random.Range(0f, 1f);
            if (doubleSpawnCheck > ChanceOfTwoCansAppearingIn2P)
                canisterCap = 1;
        }

        Debug.Log("Canister will spawn during this Area. Amount: " + canisterCap);
        yield return new WaitForSeconds(10); // Wait before starting.

        while (canisterAmountSpawned < canisterCap)
        {
            float chanceCheck = Random.Range(0f, 1f);
            if (chanceCheck < chanceOfSpawningCanister)
            {
                chanceOfSpawningCanister = 0.1f;
                canisterAmountSpawned += 1;
                SpawnProp(PropType.Canister);

                // 5% chance of 2 more canisters spawning in place of just one. Only applies to the first canister spawn attempt.
                if (canisterAmountSpawned == 1)
                {
                    float largeGroupCheck = Random.Range(0f, 1f);
                    if (largeGroupCheck < ChanceOfLargeCanisterGroup)
                    {
                        for (int i = 0; i < 2; i++)
                        {
                            yield return new WaitForSeconds(0.8f);
                            SpawnProp(PropType.Canister);
                        }
                        canisterAmountSpawned += 5;
                    }
                }
            }
            else
            {
                chanceOfSpawningCanister += 0.025f;
            }
            yield return new WaitForSeconds(SpawningCanisterInterval);
        }
    }

    private const float ChanceToSpawnGreenClass = 0.3f;
    /// <summary>
    /// When spawning a UFO, choose UfoFollower or UfoPasser.
    /// </summary>
    public void ChooseUfoAndSpawn() {
        float randomiser = Random.Range(0f, 1f);
        if (randomiser < ChanceToSpawnGreenClass)
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
        GameObject[] listOfCheatedProp;
        switch (propStr)
        {
            case "asteroid":
                listOfCheatedProp = GameObject.FindGameObjectsWithTag(Tag_Asteroid);
                if (listOfCheatedProp.Length < 30)
                    SpawnProp(PropType.Asteroid); break;
            case "canister":
                listOfCheatedProp = GameObject.FindGameObjectsWithTag(Tag_Canister);
                if (listOfCheatedProp.Length < 10)
                    SpawnProp(PropType.Canister); break;
            case "ufo-follower":
                listOfCheatedProp = GameObject.FindGameObjectsWithTag(Tag_Ufo);
                if (listOfCheatedProp.Length < 10)
                    SpawnProp(PropType.UfoFollower); break;
            case "ufo-passer":
                listOfCheatedProp = GameObject.FindGameObjectsWithTag(Tag_Ufo);
                if (listOfCheatedProp.Length < 10)
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
