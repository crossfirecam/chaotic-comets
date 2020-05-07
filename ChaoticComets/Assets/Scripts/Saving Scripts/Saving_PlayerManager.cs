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
        PlayerMain player1GameObject = player1.GetComponent<PlayerMain>();
        PlayerMain player2GameObject = player2.GetComponent<PlayerMain>();

        playerCount = BetweenScenesScript.PlayerCount;
        difficulty = BetweenScenesScript.Difficulty;
        level = gM.levelNo;
        player1health = player1GameObject.shields;
        player1credits = player1GameObject.credits;
        player1bonus = player1GameObject.bonus;
        player1lives = player1GameObject.lives;
        if (player1GameObject.plrPowerups.ifInsuranceActive) { player1powerups[0] = 1; }
        if (player1GameObject.plrPowerups.ifFarShot) { player1powerups[1] = 1; }
        if (player1GameObject.plrPowerups.ifRetroThruster) { player1powerups[2] = 1; }
        if (player1GameObject.plrPowerups.ifRapidShot) { player1powerups[3] = 1; }
        if (player1GameObject.plrPowerups.ifTripleShot) { player1powerups[4] = 1; }
        player1upgrades[0] = BetweenScenesScript.UpgradesP1[0];
        player1upgrades[1] = BetweenScenesScript.UpgradesP1[1];
        player1upgrades[2] = BetweenScenesScript.UpgradesP1[2];
        player1upgrades[3] = BetweenScenesScript.UpgradesP1[3];
        Debug.Log($"Saved. Player 1: {player1health} shields, {player1credits} credits, {player1bonus} bonus threshold, " +
            $"{player1lives} lives. Powerups: {string.Join(",", player1powerups)}, Upgrades: {string.Join(",", player1upgrades)}");
        if (playerCount == 2) {
            player2health = player2GameObject.shields;
            player2credits = player2GameObject.credits;
            player2bonus = player2GameObject.bonus;
            player2lives = player2GameObject.lives;
            if (player2GameObject.plrPowerups.ifInsuranceActive) { player2powerups[0] = 1; }
            if (player2GameObject.plrPowerups.ifFarShot) { player2powerups[1] = 1; }
            if (player2GameObject.plrPowerups.ifRetroThruster) { player2powerups[2] = 1; }
            if (player2GameObject.plrPowerups.ifRapidShot) { player2powerups[3] = 1; }
            if (player2GameObject.plrPowerups.ifTripleShot) { player2powerups[4] = 1; }
            player2upgrades[0] = BetweenScenesScript.UpgradesP2[0];
            player2upgrades[1] = BetweenScenesScript.UpgradesP2[1];
            player2upgrades[2] = BetweenScenesScript.UpgradesP2[2];
            player2upgrades[3] = BetweenScenesScript.UpgradesP2[3];
            Debug.Log($"Saved. Player 2: {player2health} shields, {player2credits} credits, {player2bonus} bonus threshold, " +
                $"{player2lives} lives. Powerups: {string.Join(",", player2powerups)}, Upgrades: {string.Join(",", player2upgrades)}");
        }
        else {
            Debug.Log("Saved. Player 2 does not exist - 1 player mode save");
        }
    }
}