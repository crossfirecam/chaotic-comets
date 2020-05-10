using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


/*
 * This class acts as a general template for all enemy types to use. Different enemies have different FixedUpdates.
 */
public partial class UfoAllTypes : MonoBehaviour
{
    internal GameManager gM;
    public UfoFollowerType ufoFollower;

    // Movement, physics variables
    internal Rigidbody2D rb;
    public Vector2 direction;
    public float alienSpeedBase;
    internal float alienSpeedCurrent;

    // Weapon system variables
    public float shootingDelay = 1.5f; // Seconds between bullets fired
    public float bulletSpeed = 250; // How fast the bullet fires
    public float lastTimeShot = 0f; // Keeps track of when UFO last fired a bullet

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

    // ----------

    void Start()
    {
        gM = GameObject.FindObjectOfType<GameManager>();
        rb = GameObject.FindObjectOfType<Rigidbody2D>();

        alienSpeedCurrent = alienSpeedBase;

        playerShip1 = GameObject.FindGameObjectWithTag("Player");
        if (BetweenScenesScript.PlayerCount == 2)
        {
            playerShip2 = GameObject.FindGameObjectWithTag("Player 2");
        }
        // If alien ship is called during level transition, it destroys itself
        if (gM.CheckIfEndOfLevel() && !ufoTeleporting) {
            Debug.Log("UFO attempted to spawn during level transition");
            Destroy(gameObject);
        }

    }

    void Update()
    {
        // Weapon systems. If criteria are met, then shoot depending on enemy type
        if (ShipAbleToShoot())
        {
            if (!ufoFollower.Equals(null))
            {
                ufoFollower.WeaponLogicFollower();
            }
        }

        // Stabilise 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);

        CheckScreenWrap();
    }


    void OnTriggerEnter2D(Collider2D playerBullet)
    {
        if (playerBullet.gameObject.CompareTag("bullet") || playerBullet.gameObject.CompareTag("bullet2"))
        {
            // If UFO has shields up, don't deal damage. Instead, reflect bullet
            if (forceField.activeInHierarchy)
            {
                Vector2 force = gameObject.transform.position - playerBullet.transform.position;
                int magnitude = 1000;
                playerBullet.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
                playerBullet.GetComponent<BulletBehaviour>().UfoReflectedBullet();
                audioAlienSfx.clip = audClipAliexSfxShieldReflect;
                audioAlienSfx.Play();
            }
            // If UFO is not retreating, deal damage and score credits
            else if (!ufoRetreating)
            {
                alienHealth -= 10f;
                playerBullet.gameObject.GetComponent<BulletBehaviour>().DestroyBullet();

                // If UFO is teleporting & has 10 health or less, then grant more points for an escape kill
                if (ufoTeleporting && alienHealth <= 10f)
                {
                    pointsToScore = teleportKillPoints;
                    DetermineIfDead();
                }

                if (alienHealth >= 0f)
                {
                    GameObject newExplosion = Instantiate(playerBulletExplosion, transform.position, transform.rotation);
                    Destroy(newExplosion, 2f);
                    audioAlienSfx.clip = audClipAlienSfxTakenDamage;
                    audioAlienSfx.pitch = 1f;
                    audioAlienSfx.Play();
                    if (playerBullet.CompareTag("bullet")) { playerShip1.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
                    if (playerBullet.CompareTag("bullet2")) { playerShip2.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Vector2 force = gameObject.transform.position - collision.transform.position;
        int magnitude = 0;

        // Asteroid and player collisions do not cause damage to UFO
        if (collision.gameObject.CompareTag("asteroid"))
        {
            magnitude = 200;
            if (alienHealth > 0) { FlickShieldOn(); }
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player 2"))
        {
            magnitude = 100;
            if (alienHealth > 0) { FlickShieldOn(); }
        }
        if (ufoRetreating) { magnitude /= 3; }
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
    }

    // If alien has 0 or less health, it is dying. Can only be set to dying state once
    private void DetermineIfDead()
    {
        if (!deathStarted && alienHealth <= 0f)
        {
            float rateOfExplosions = 0.4f;
            float durationOfExplosions = 1.5f;
            deathStarted = true;
            if (ufoTeleporting) { rateOfExplosions = 0.2f; durationOfExplosions = 2.5f; }
            InvokeRepeating("DeathExplosions", 0.0f, rateOfExplosions);
            Invoke("DeathRoutine", durationOfExplosions);
        }
    }

    // Repeating invoke that causes explosions
    void DeathExplosions()
    {
        GameObject newExplosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
    }

    // Final instructions for UFO when it's about to die
    void DeathRoutine()
    {
        CancelInvoke("DeathExplosions");
        /* Increase stats for next time TODO fix these
        shootingDelay = shootingDelay * difficultyIncrease;
        alienSpeed = alienSpeed / difficultyIncrease;
        alienMaxHealth = alienMaxHealth / difficultyIncrease;
        alienHealth = alienMaxHealth;
        // Maximum Stats
        if (shootingDelay < 1.3f) { shootingDelay = 1.3f; }
        if (alienSpeed > 1.6f) { alienSpeed = 1.6f; }
        if (alienMaxHealth > 90f) { alienMaxHealth = 90f; }*/
        gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.AlienRespawn);
        Destroy(gameObject);
    }

    // If a player the UFO is following has died, reset variables. So it chases after another player
    public void PlayerDied()
    {
        playerFound = false;
        player = null;
    }

    // Play/pause UFO hum depending on game's pause state
    public void CheckAlienSounds(int intent)
    {
        if (intent == 1 && audioAlienHum.isPlaying)
        {
            audioAlienHum.Pause();
        }
        else if (intent == 2 && !audioAlienHum.isPlaying)
        {
            audioAlienHum.UnPause();
        }
    }
}
