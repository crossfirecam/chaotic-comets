using UnityEngine;


/*
 * This class acts as a general template for all enemy types to use. Different enemies have different FixedUpdates.
 */
public abstract partial class Ufo : MonoBehaviour
{
    [Header("Movement / Physics Variables")]
    public Vector2 direction;
    internal Rigidbody2D rb;
    public float alienSpeedBase;
    internal float alienSpeedCurrent;

    [Header("Sound References")]
    public AudioSource audioAlienHum;
    public AudioSource audioAlienSfx;
    public AudioClip audClipAliexSfxShieldReflect, audClipAlienSfxTakenDamage;

    [Header("Other Variables / References")]
    public GameObject bullet;
    public GameObject deathExplosion, playerBulletExplosion, teleportEffect;
    internal TutorialManager tM;
    internal bool deathStarted = false, ufoTeleporting = false, ufoRetreating = false;
    public GameObject forceField;
    public GameObject playerShip1, playerShip2;

    // ----------

    // All UFO type enemies start by finding players, setting speed, etc
    internal virtual void Start()
    {
        rb = FindObjectOfType<Rigidbody2D>();
        if (GameManager.i.tutorialMode) { tM = GameManager.i.Refs.tutorialManager.GetComponent<TutorialManager>(); }

        alienSpeedCurrent = alienSpeedBase;

        playerShip1 = GameObject.FindGameObjectWithTag("Player");
        if (BetweenScenes.PlayerCount == 2)
        {
            playerShip2 = GameObject.FindGameObjectWithTag("Player 2");
        }
        // If alien ship is called during level transition, it destroys itself
        if (GameManager.i.CheckIfEndOfLevel() && !ufoTeleporting) {
            print("UFO attempted to spawn during level transition");
            Destroy(gameObject);
        }

    }

    public GameObject[] ReturnAlienSounds()
    {
        GameObject[] alienSfx = { audioAlienHum.gameObject, audioAlienSfx.gameObject };
        return alienSfx;
    }
}
