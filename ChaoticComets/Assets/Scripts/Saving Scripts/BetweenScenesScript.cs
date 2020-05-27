using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BetweenScenesScript {
    public static int PlayerCount = 1; // Defaut value of 1 player for debugging. Alternative is '2'.
    public static int Difficulty = 1; // Default value of Normal difficulty for debugging. Alternatives are '0' Easy, and '2' Hard.

    // Default value of volume is full volume. Once Chaotic Comets is run once on a system, PlayerPrefs overwrites this value upon startup.
    public static float MusicVolume = 1.0f;
    public static float SFXVolume = 1.0f;

    // By default, the game assumes it's not being resumed from a save. Until it either...
    // A: New game is started, and first level is finished
    // B: Game is resumed from a found save file
    public static bool ResumingFromSave = false;

    // Tutorial mode on or off
    public static bool TutorialMode = false;

    // Below are integer arrays that will be turned into floats when used in Spaceship gameobjects.
    // This is because iterating on floats causes counting errors eventually with any programming language.
    // 10 = 1.0, or the base of upgrades. They'll be iterated by 1 (converted to 0.1 float) each time an upgrade is performed.
    // In order, upgrades are: Top speed, braking efficiency, fire rate, shot speed.
    public static int[] UpgradesP1 = { 10, 10, 10, 10 };
    public static int[] UpgradesP2 = { 10, 10, 10, 10 };

    // Credits count for each player is stored here, and only saved to a file when the shop at the end of a level is loaded.
    public static int player1TempCredits = 0;
    public static int player2TempCredits = 0;
    public static int player1TempLives = 0;
    public static int player2TempLives = 0;
}
