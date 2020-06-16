using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

// This class is used for testing during gameplay without needing to change other files. Attached to GameManager gameobject.
public class Testing : MonoBehaviour
{
    private GameManager gM;
    private float timer = 0f, nextUpdate = 0f, updateInterval = 0.5f;

    [Header("Testing Environments")]
    public bool onLevelNegativeOne = false;
    public int playerCount = 1;

    [Header("What's Reported in ")]
    public bool speedReportTest2P = false;
    private void Awake()
    {
        gM = FindObjectOfType<GameManager>();
        BetweenScenes.CheaterMode = true;
        if (onLevelNegativeOne)
        {
            gM.levelNo = -1;
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
                print($"Speed Test: P1: {Math.Round(gM.Refs.playerShip1.rbPlayer.velocity.magnitude, 2)} | P2: {Math.Round(gM.Refs.playerShip2.rbPlayer.velocity.magnitude, 2)}");
            }
        }
    }
}
