using System;
using System.Collections.Generic;

[Serializable]
public class Saving_PlayerManager
{
    public int playerCount = 1;
    public int difficulty = 0;
    public int level;

    public List<Player> playerList = new List<Player>();

    public Saving_PlayerManager(GameManager gM, PlayerMain player1Script, PlayerMain player2Script) {
        PlayerMain[] playerScripts = { player1Script, player2Script };

        playerCount = BetweenScenes.PlayerCount;
        difficulty = BetweenScenes.Difficulty;
        level = gM.levelNo;

        for (int i = 0; i < playerCount; i++)
        {
            // Powerup order: Insurance, Far Shot, Auto-Brake, Rapid Shot, Triple Shot
            int[] powerupsToSave = { 0, 0, 0, 0, 0 };
            if (playerScripts[i].plrPowerups.ifInsurance) { powerupsToSave[0] = 1; }
            if (playerScripts[i].plrPowerups.ifFarShot) { powerupsToSave[1] = 1; }
            if (playerScripts[i].plrPowerups.ifAutoBrake) { powerupsToSave[2] = 1; }
            if (playerScripts[i].plrPowerups.ifRapidShot) { powerupsToSave[3] = 1; }
            if (playerScripts[i].plrPowerups.ifTripleShot) { powerupsToSave[4] = 1; }

            // Upgrade order: Speed, brake efficiency, fire rate, shot speed
            int[] upgradesToSave = { 10, 10, 10, 10 };

            // Determine which BetweenScenes upgrade array to use. TODO make this nicer
            int[] currentUpgradeArray = new int[4];
            switch (i)
            {
                case 0: BetweenScenes.UpgradesP1.CopyTo(currentUpgradeArray, 0); break;
                case 1: BetweenScenes.UpgradesP2.CopyTo(currentUpgradeArray, 0); break;
            }
            upgradesToSave[0] = currentUpgradeArray[0];
            upgradesToSave[1] = currentUpgradeArray[1];
            upgradesToSave[2] = currentUpgradeArray[2];
            upgradesToSave[3] = currentUpgradeArray[3];

            // Initilise new Player
            Player currentPlayer = new Player
            {
                health = playerScripts[i].shields,
                credits = playerScripts[i].credits,
                totalCredits = playerScripts[i].totalCredits,
                bonusThreshold = playerScripts[i].bonus,
                lives = playerScripts[i].lives,
                powerups = powerupsToSave,
                upgrades = upgradesToSave
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
        public int lives = 0;
        // Powerup order: Insurance, Far Shot, Auto-Brake, Rapid Shot, Triple Shot
        public int[] powerups = { 0, 0, 0, 0, 0 };
        // Upgrade order: Speed, brake efficiency, fire rate, shot speed
        public int[] upgrades = { 10, 10, 10, 10 };
    }
}