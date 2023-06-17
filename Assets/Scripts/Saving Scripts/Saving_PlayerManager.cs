using System;
using System.Collections.Generic;

[Serializable]
public class Saving_PlayerManager
{
    public int playerCount = 1;
    public int difficulty = 1;
    public int level;
    public int lives;
    public bool player1dead, player2dead;
    public bool isCheatModeOn = false;

    public List<Player> playerList = new List<Player>();

    public Saving_PlayerManager(GameManager gM, PlayerMain player1Script, PlayerMain player2Script) {
        PlayerMain[] playerScripts = { player1Script, player2Script };

        playerCount = BetweenScenes.PlayerCount;
        difficulty = BetweenScenes.Difficulty;
        level = gM.levelNo;
        lives = gM.playerLives;
        player1dead = gM.player1dead;
        player2dead = gM.player2dead;

        // Only checked when loaded from main menu. Fixes an exploit where players can get to a shop, quit the game, and come back to resume normally.
        isCheatModeOn = BetweenScenes.CheaterMode;

        for (int i = 0; i < playerCount; i++)
        {
            // Powerup order: Insurance, Far Shot, Rapid Shot, Auto-Brake, Triple Shot
            int[] powerupsToSave = { 0, 0, 0, 0, 0 };
            if (playerScripts[i].plrPowerups.ifInsurance) { powerupsToSave[0] = 1; }
            if (playerScripts[i].plrPowerups.ifFarShot) { powerupsToSave[1] = 1; }
            if (playerScripts[i].plrPowerups.ifRapidShot) { powerupsToSave[2] = 1; }
            if (playerScripts[i].plrPowerups.ifAutoBrake) { powerupsToSave[3] = 1; }
            if (playerScripts[i].plrPowerups.ifTripleShot) { powerupsToSave[4] = 1; }

            // Initilise new Player
            Player currentPlayer = new Player
            {
                health = playerScripts[i].shields,
                credits = playerScripts[i].credits,
                totalCredits = playerScripts[i].totalCredits,
                bonusThreshold = playerScripts[i].bonus,
                powerups = powerupsToSave,
                upgrades = BetweenScenes.PlayerShopUpgrades[i]
            };
            playerList.Add(currentPlayer);
        }
    }

    [Serializable]
    public class Player
    {
        public float health = 0;
        public int credits = 0;
        public int totalCredits = 0;
        public int bonusThreshold = 0;
        // Powerup order: Insurance, Far Shot, Rapid Shot, Auto-Brake, Triple Shot
        public int[] powerups = { 0, 0, 0, 0, 0 };
        // Upgrade order: Shield Strength, Brake, Tele Rate, Auto Rate, Shot Limit, Shot Speed, Shot Range
        public int[] upgrades = { 0, 0, 0, 0, 0, 0, 0, 0 };
    }
}