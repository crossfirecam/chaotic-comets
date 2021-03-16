using UnityEngine;

public partial class GameManager : MonoBehaviour
{
    [Header("Player Status Variables")]
    public bool player1dead = false;
    public bool player2dead = true; // Only one player by default
    [HideInInspector] public bool player1TEMPDEAD = false, player2TEMPDEAD = false; // Only used to alert UFO that player is temporarily inactive
    
    // If a player dies, set the value for their death. If both are dead, game is over
    public void PlayerDied(int playerThatDied)
    {
        print($"Player {playerThatDied} has died.");
        if (playerThatDied == 1) { player1dead = true; }
        else if (playerThatDied == 2) { player2dead = true; }
        if (player1dead && player2dead)
        {
            Invoke(nameof(GameOver), 2f);
        }
    }

    // When either ship is destroyed, alien will change target
    public void PlayerLostLife(int playerNumber)
    {
        if (playerNumber == 1) { player1TEMPDEAD = true; }
        else if (playerNumber == 2) { player2TEMPDEAD = true; }
        GameObject[] listOfUfos = GameObject.FindGameObjectsWithTag("ufo");
        foreach (GameObject ufo in listOfUfos)
        {
            ufo.GetComponent<Ufo>().PlayerDied();
        }
    }
}
