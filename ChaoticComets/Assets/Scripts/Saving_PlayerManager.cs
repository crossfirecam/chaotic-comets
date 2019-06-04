using System.Collections;
using System.Collections.Generic;
using System.Runtime.Serialization.Formatters.Binary;
using System;
using System.IO;
using UnityEngine;

[System.Serializable]
public class Saving_PlayerManager
{
    public int playerCount = 1;
    public int difficulty = 0;
    public int level;
    public float player1health = 0;
    public float player2health = 0;
    public int player1credits = 0;
    public int player2credits = 0;
    public int player1bonus = 0;
    public int player2bonus = 0;
    public int player1lives = 0;
    public int player2lives = 0;
    // Powerup order: Insurance, Far Shot, Retro Thruster, Rapid Shot, Triple Shot
    public int[] player1powerups = { 0, 0, 0, 0, 0 };
    public int[] player2powerups = { 0, 0, 0, 0, 0 };
    // Upgrade order: Speed, brake efficiency, fire rate, shot speed
    public float[] player1upgrades = { 0, 0, 0, 0 };
    public float[] player2upgrades = { 0, 0, 0, 0 };

    public Saving_PlayerManager(GameManager gM, GameObject player1, GameObject player2) {
        SpaceshipControls player1GameObject = player1.GetComponent<SpaceshipControls>();
        SpaceshipControls player2GameObject = player2.GetComponent<SpaceshipControls>();

        playerCount = BetweenScenesScript.PlayerCount;
        difficulty = BetweenScenesScript.Difficulty;
        level = gM.levelNo;
        player1health = player1GameObject.shields;
        player1credits = player1GameObject.credits;
        player1bonus = player1GameObject.bonus;
        player1lives = player1GameObject.lives;
        if (player1GameObject.ifInsuranceActive) { player1powerups[0] = 1; }
        if (player1GameObject.ifFarShot) { player1powerups[1] = 1; }
        if (player1GameObject.ifRetroThruster) { player1powerups[2] = 1; }
        if (player1GameObject.ifRapidShot) { player1powerups[3] = 1; }
        if (player1GameObject.ifTripleShot) { player1powerups[4] = 1; }
        player1upgrades[0] = player1GameObject.upgradeSpeed;
        player1upgrades[1] = player1GameObject.upgradeBrake;
        player1upgrades[2] = player1GameObject.upgradeFireRate;
        player1upgrades[3] = player1GameObject.upgradeShotSpeed;
        Debug.Log("Saved. Player 1: " + player1health + " shields, " + player1credits + " credits, "
            + player1bonus + " bonus threshold, " + player1lives + " lives. Powerups: " + string.Join(",", player1powerups)
            + ", Upgrades: " + string.Join(",", player1upgrades));
        if (playerCount == 2) {
            player2health = player2GameObject.shields;
            player2credits = player2GameObject.credits;
            player2bonus = player2GameObject.bonus;
            player2lives = player2GameObject.lives;
            if (player2GameObject.ifInsuranceActive) { player2powerups[0] = 1; }
            if (player2GameObject.ifFarShot) { player2powerups[1] = 1; }
            if (player2GameObject.ifRetroThruster) { player2powerups[2] = 1; }
            if (player2GameObject.ifRapidShot) { player2powerups[3] = 1; }
            if (player2GameObject.ifTripleShot) { player2powerups[4] = 1; }
            player1upgrades[0] = player1GameObject.upgradeSpeed;
            player1upgrades[1] = player1GameObject.upgradeBrake;
            player1upgrades[2] = player1GameObject.upgradeFireRate;
            player1upgrades[3] = player1GameObject.upgradeShotSpeed;
            Debug.Log("Saved. Player 2: " + player2health + " shields, " + player2credits + " credits, "
                + player2bonus + " bonus threshold, " + player2lives + " lives. Powerups: " + string.Join(",", player2powerups)
                + ", Upgrades: " + string.Join(",", player2upgrades));
        }
        else {
            Debug.Log("Saved. Player 2 does not exist - 1 player mode save");
        }
    }
}