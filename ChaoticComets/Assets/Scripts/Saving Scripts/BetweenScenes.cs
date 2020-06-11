public static class BetweenScenes {
    public static int PlayerCount = 1; // Defaut value of 1 player for debugging. Alternative is '2'.
    public static int Difficulty = 1; // Default value of Normal difficulty for debugging. Alternatives are '0' Easy, and '2' Hard.

    // Default value of volume is full volume. Once Chaotic Comets is run once on a system, PlayerPrefs overwrites this value upon startup.
    public static float MusicVolume = 1.0f;
    public static float SFXVolume = 1.0f;

    // By default, the game assumes it's not being resumed from a save. Until it either...
    // A: New game is started, and first level is finished
    // B: Game is resumed from a found save file
    public static bool ResumingFromSave = false;

    public static string BackToMainMenuButton = "";

    // Tutorial mode on or off
    public static bool TutorialMode = false;

    // Cheats been activated this round
    public static bool CheaterMode = false;

    // Below are integer arrays that will be turned into floats when used in Spaceship gameobjects.
    // This is because iterating on floats causes counting errors eventually with any programming language.
    // 10 = 1.0, or the base of upgrades. They'll be iterated by 1 (converted to 0.1 float) each time an upgrade is performed.
    // In order, upgrades are: Top speed, braking efficiency, fire rate, shot speed.
    public static int[] UpgradesP1 = { 10, 10, 10, 10 };
    public static int[] UpgradesP2 = { 10, 10, 10, 10 };

    // Credits and life counter are tracked here, because after a store is over, saves are erased but these two stats are still required
    public static int[] playerShopCredits = { 0, 0 };
    public static int[] playerShopLives = { 0, 0 };
}
