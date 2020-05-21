using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    private GameManager gM;
    public PlayerMain player1;
    public GameObject[] popups;
    private int popUpIndex = 0;
    private bool taskSetupDone = false;

    private void Start()
    {
        gM = FindObjectOfType<GameManager>();
        player1.canTeleport = false;
        player1.power = 0;
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
                if (Input.GetKeyDown(KeyCode.A) || Input.GetKeyDown(KeyCode.D))
                {
                    popUpIndex++;
                }
                break;

            case 1: // Thrusting
                if (Input.GetKeyDown(KeyCode.W))
                {
                    popUpIndex++;
                }
                break;

            case 2: // Braking
                if (Input.GetKeyDown(KeyCode.S))
                {
                    popUpIndex++;
                }
                break;

            case 3: // Shooting
                if (Input.GetKeyDown(KeyCode.Space))
                {
                    popUpIndex++;
                }
                break;

            case 4: // Asteroids
                if (!taskSetupDone)
                {
                    gM.SpawnProp(GameManager.PropType.Asteroid);
                    player1.collisionsCanDamage = false;
                    taskSetupDone = true;
                }
                if (gM.asteroidCount > 1)
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 5: // Asteroids 2
                if (gM.asteroidCount == 0)
                {
                    popUpIndex++;
                }
                break;

            case 6: // Shields
                if (!taskSetupDone)
                {
                    player1.collisionsCanDamage = true;
                    player1.canShoot = false;
                    CreateAsteroids(3);
                    taskSetupDone = true;
                }
                if (player1.shields == 0)
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 7: // Lives
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    taskSetupDone = true;
                }
                if (Input.GetKeyDown(KeyCode.Q) && player1.shields == 80)
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 8: // Canteen
                if (!taskSetupDone)
                {
                    gM.SpawnProp(GameManager.PropType.Canister);
                    taskSetupDone = true;
                }
                if (player1.plrPowerups.ifTripleShot)
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 9: // Powerup TripleShot
                if (!taskSetupDone)
                {
                    player1.canShoot = true;
                    DestroyAllAsteroids();
                    CreateAsteroids(3);
                    player1.collisionsCanDamage = false;
                    taskSetupDone = true;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    popUpIndex++;
                    taskSetupDone = false;
                }
                break;

            case 10: // Powerup FarShot
                if (!taskSetupDone)
                {
                    DestroyAllAsteroids();
                    CreateAsteroids(3);
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.TripleShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.FarShot);
                    taskSetupDone = true;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 11: // Powerup RetroThruster
                if (!taskSetupDone)
                {
                    player1.plrPowerups.RemovePowerup(PlayerPowerups.Powerups.FarShot);
                    player1.plrPowerups.ApplyPowerup(PlayerPowerups.Powerups.RetroThruster);
                    taskSetupDone = true;
                }
                if (Input.GetKeyDown(KeyCode.Q))
                {
                    popUpIndex++; taskSetupDone = false;
                }
                break;

            case 12: // UFO Red

                break;

            case 13: // UFO Green

                break;

            case 14: // Powerup RapidShot

                break;

            case 15: // Retreat

                break;

            case 16: // Powerup Insurance

                break;

            case 17: // Teleport

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
    }
    private void CreateAsteroids(int num)
    {
        for (int i = 0; i < num; i++)
        {
            gM.SpawnProp(GameManager.PropType.Asteroid);
        }
    }
}
