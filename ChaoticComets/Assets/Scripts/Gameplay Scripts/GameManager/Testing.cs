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
    public int playerCount = 1;

    [Header("What's Reported in ")]
    public bool speedReportTest2P = false;
    private void Awake()
    {
        BetweenScenes.CheaterMode = true;
        if (onLevelNegativeOne)
        {
            GameManager.i.levelNo = -1;
        }
        BetweenScenes.PlayerCount = playerCount;
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
