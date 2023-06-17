using UnityEngine;
using static Constants;

public partial class GameManager : MonoBehaviour
{
    [Header("Player Status Variables")]
    public int playerLives = 3;
    public bool player1dead = false;
    public bool player2dead = true; // Only one player by default
    [HideInInspector] public bool player1TEMPDEAD = false, player2TEMPDEAD = false; // Only used to alert UFO that player is temporarily inactive
    
    // If a player depletes reserve lives, set the value for their death. If both are dead, game is over
    public void PlayerDied(int playerThatDied)
    {
        print($"Player {playerThatDied} has died.");
        if (playerThatDied == 0) { player1dead = true; }
        else if (playerThatDied == 1) { player2dead = true; }

        if (player1dead && player2dead)
            UiManager.i.Invoke(nameof(UiManager.i.GameOver), 2f);

    }

    // When either ship is destroyed, alien will change target
    public void PlayerLostLife(int playerNumber)
    {
        if (playerNumber == 0) { player1TEMPDEAD = true; }
        else if (playerNumber == 1) { player2TEMPDEAD = true; }
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag(Tag_Ufo);
        foreach (GameObject ufo in listOfUfos)
        {
            ufo.GetComponent<Ufo>().PlayerDied();
        }

        // Get both players to check for respawn eligibility.

        // When the team of 2 players loses all reserve lives, respawning is impossible. Any dead player remains dead, alive players cannot respawn.
        UiManager.i.SetPlayerRespawnStatus(0, playerLives);
        if (BetweenScenes.PlayerCount == 2)
            UiManager.i.SetPlayerRespawnStatus(1, playerLives);
    }

    // When a life is gained, add to total lives. If in 2P mode, notify dead player that they can respawn.
    public void PlayerGainedLife()
    {
        playerLives++;
        if (BetweenScenes.PlayerCount == 2)
        {
            if (player1dead)
                UiManager.i.SetPlayerRespawnStatus(0, playerLives);
            else
                UiManager.i.SetPlayerRespawnStatus(1, playerLives);
        }
    }

    public void PlayerChoseToRespawn()
    {
        player1dead = false;
        player2dead = false;
        playerLives--;
    }
}
