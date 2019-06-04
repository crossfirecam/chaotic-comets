using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class BetweenScenesScript {
    public static int PlayerCount = 1; // Defaut value of 1 player for debugging
    public static int Difficulty = 1; // Default value of Normal difficulty for debugging
    public static int ControlTypeP1 = 1; // Both players start with keyboard by default
    public static int ControlTypeP2 = 1;
    public static float MusicVolume = 1.0f;
    public static float SFXVolume = 1.0f;
    public static bool ResumingFromSave = false;
    public static float[] UpgradesP1 = { 1.0f, 1.0f, 1.0f, 1.0f };
    public static float[] UpgradesP2 = { 1.0f, 1.0f, 1.0f, 1.0f };
}
