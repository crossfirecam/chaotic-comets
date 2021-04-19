using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// This class is used for testing during gameplay without needing to change other files. Attached to GameManager gameobject.
public class Testing : MonoBehaviour
{
    private float timer = 0f, nextUpdate = 0f;
    private readonly float updateInterval = 0.5f;

    [Header("Testing Environments")]
    public bool onLevelNegativeOne = false;

    [Header("What's Reported in Console")]
    public bool speedReportTest2P = false;

    [Header("Upgrades (Shi, Tele, Spd, Brk, AutoR, ShotLi, ShotSp, ShotRa)")]
    public int[] player1Upgrades = new int[8];
    public int[] player2Upgrades = new int[8];
    // Testing Shi & Tele upgrade doesn't work on level -1.

    private void Awake()
    {
        BetweenScenes.CheaterMode = true;
        BetweenScenes.PlayerCount = 2;
        UiManager.i.ShowP2UI();
        if (onLevelNegativeOne)
            GameManager.i.levelNo = -1;

        // Set testing upgrades for players
        for (int j = 0; j < 8; j++)
        {
            BetweenScenes.PlayerShopUpgrades[0][j] = player1Upgrades[j];
            BetweenScenes.PlayerShopUpgrades[1][j] = player2Upgrades[j];
        }
    }

    private void Start()
    {
        if (onLevelNegativeOne)
        {
            Debug.LogWarning("Testing: Manually enable PlayerShip2");
            GameManager.i.Refs.playerShip1.plrSpawnDeath.RespawnShip();
            GameManager.i.Refs.playerShip2.plrSpawnDeath.RespawnShip();
        }
    }
    private void Update()
    {
        timer += Time.deltaTime;
        if (timer > nextUpdate)
        {
            nextUpdate += updateInterval;
            if (speedReportTest2P)
            {
                print($"Speed Test: P1: {Math.Round(GameManager.i.Refs.playerShip1.rbPlayer.velocity.magnitude, 2)}" +
                                $"| P2: {Math.Round(GameManager.i.Refs.playerShip2.rbPlayer.velocity.magnitude, 2)}");
            }
        }
    }
}
