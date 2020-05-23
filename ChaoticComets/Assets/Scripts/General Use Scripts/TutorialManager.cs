using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    [SerializeField] private int popUpIndex = 0;
    private GameManager gM;
    public PlayerMain player1;
    public GameObject[] popups;
    private bool taskSetupDone = false, ufoHit = false;
    public bool ufoGone = false, ufoFollowerDocile = false;
    private int playerCreditsBefore = 0;

    private void Start()
    {
        gM = FindObjectOfType<GameManager>();
        player1.canTeleport = false;
        player1.power = 0;
        player1.plrUiSound.UpdatePointDisplays();
    }

    private void Update()
    {
        DisplayCurrentPopup();
        ProgressCriteria();
    }

    private void DisplayCurrentPopup()
    {
        for (int i = 0; i < popups.Length; i++)
        {
            popups[i].SetActive(i == popUpIndex);
        }
    }

    private void ProgressCriteria()
    {
        switch (popUpIndex)
        {
            case 0: // Rotation
                ContinueIf(Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D));
                break;

            case 1: // Thrusting
                ContinueIf(Input.GetKeyDown(KeyCode.W));
                break;

            case 2: // Braking
                ContinueIf(Input.GetKeyDown(KeyCode.S));
                break;

            case 3: // Shooting
                ContinueIf(Input.GetKeyDown(KeyCode.Space));
                break;

            case 4: // Asteroids
                
                if (!taskSetupDone)
                {
                    gM.SpawnProp(GameManager.PropType.Asteroid, default, true); // Spawn safe asteroid
                    player1.collisionsCanDamage = false;
                    taskSetupDone = true;
                }
                ContinueIf(gM.asteroidCount > 1);
                break;

            case 5: // Asteroids 2
                ContinueIf(gM.asteroidCount == 0 || Input.GetKeyDown(KeyCode.Q));
                break;

            case 6: // Shields
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = true;
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(false);
                ContinueIf(player1.shields == 0);
                break;

            case 7: // Lives
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    taskSetupDone = true;
                }
                ContinueIf(Input.GetKeyDown(KeyCode.Q) && player1.shields == 80);
                break;

            case 8: // Canteen
                if (!taskSetupDone)
                {
                    gM.SpawnProp(GameManager.PropType.Canister);
                    taskSetupDone = true;
                }
                ContinueIf(player1.plrPowerups.ifTripleShot);
                break;

            case 9: // Powerup TripleShot
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = false;
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.TripleShot); // In case debugging sets popUpIndex to 9
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 10: // Powerup FarShot
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.TripleShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot);
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 11: // Powerup RetroThruster
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.FarShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RetroThruster);
                    taskSetupDone = true;
                }
                ResetAsteroidsIfZero(true);
                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 12: // UFO Red
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    player1.collisionsCanDamage = true;
                    playerCreditsBefore = player1.credits;
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.RetroThruster);
                    gM.SpawnProp(GameManager.PropType.UfoPasser);
                    taskSetupDone = true;
                }
                if (player1.credits > playerCreditsBefore && !ufoHit)
                {
                    ufoHit = true;
                    UfoPasser currentUfo = FindObjectOfType<UfoPasser>().GetComponent<UfoPasser>();
                    currentUfo.TeleportStart();
                }
                ContinueIf(ufoGone);
                break;

            case 13: // UFO Green
                if (!taskSetupDone)
                {
                    ufoHit = false; ufoGone = false; ufoFollowerDocile = true;
                    gM.SpawnProp(GameManager.PropType.UfoFollower);
                    taskSetupDone = true;
                }
                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 14: // Powerup RapidShot
                if (!taskSetupDone)
                {
                    ufoFollowerDocile = false;
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RapidShot);
                    taskSetupDone = true;
                }
                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 15: // Retreat

                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 16: // Powerup Insurance

                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 17: // Teleport

                ContinueIf(Input.GetKeyDown(KeyCode.Q));
                break;

            case 18: // End

                break;

        }
    }

    private void DestroyAllAsteroids()
    {
        GameObject[] listOfAsteroids = GameObject.FindGameObjectsWithTag("asteroidParent");
        foreach (GameObject asteroid in listOfAsteroids)
        {
            Destroy(asteroid.gameObject);
        }
        gM.asteroidCount = 0;
    }

    private void ResetAsteroidsIfZero(bool safeAsteroids)
    {
        if (gM.asteroidCount == 0)
        {
            CreateAsteroids(3, safeAsteroids);
        }
    }
    private void CreateAsteroids(int num, bool safeAsteroids)
    {
        for (int i = 0; i < num; i++)
        {
            if (!safeAsteroids)
            {
                gM.SpawnProp(GameManager.PropType.Asteroid);
            }
            else
            {
                gM.SpawnProp(GameManager.PropType.Asteroid, default, true);
            }
        }
    }
    private void ContinueIf(bool continuationCriteria)
    {
        if (continuationCriteria)
        {
            popUpIndex++;
            taskSetupDone = false;
        }
    }
}
