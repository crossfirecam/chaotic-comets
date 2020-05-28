using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * This class acts as a general template for all enemy types to use. Different enemies have different FixedUpdates.
 */
public abstract partial class Ufo : MonoBehaviour
{
    [SerializeField] internal GameManager gM;

    // Movement, physics variables
    internal Rigidbody2D rb;
    public Vector2 direction;
    public float alienSpeedBase;
    internal float alienSpeedCurrent;

    // Weapon system variables
    public float shootingDelay = 1.5f; // Seconds between bullets fired
    public float bulletSpeed = 250; // How fast the bullet fires
    internal float lastTimeShot = 0f; // Keeps track of when UFO last fired a bullet

    // Defence system variables
    // private float difficultyIncrease = 0.95f; TODO add this functionality later
    public float alienHealth, alienMaxHealth;
    private int pointsToScore = 100;
    private readonly int teleportKillPoints = 500;
    public GameObject forceField;

    // Targetting system variables
    public GameObject playerShip1, playerShip2;
    internal Transform player; // Currently tracked player
    internal bool playerFound = false, playerTooFar = false;

    // Sound variables
    public AudioSource audioAlienHum, audioAlienSfx; // Hum: passive UFO noise, SFX: impacts/shield noises
    public AudioClip audClipAliexSfxShieldReflect, audClipAlienSfxTakenDamage;

    // Other variables
    public GameObject bullet;
    public GameObject deathExplosion, playerBulletExplosion, teleportEffect;
    internal bool deathStarted = false, ufoTeleporting = false, ufoRetreating = false;
    internal TutorialManager tM;

    // ----------

    // All UFO type enemies start by finding players, setting speed, etc
    internal virtual void Start()
    {
        gM = FindObjectOfType<GameManager>();
        rb = FindObjectOfType<Rigidbody2D>();
        if (gM.tutorialMode) { tM = gM.Refs.tutorialManager.GetComponent<TutorialManager>(); }

        alienSpeedCurrent = alienSpeedBase;

        playerShip1 = GameObject.FindGameObjectWithTag("Player");
        if (BetweenScenesScript.PlayerCount == 2)
        {
            playerShip2 = GameObject.FindGameObjectWithTag("Player 2");
        }
        // If alien ship is called during level transition, it destroys itself
        if (gM.CheckIfEndOfLevel() && !ufoTeleporting) {
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
