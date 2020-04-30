using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * This class contains all general code for the Player objects, referring to many other scripts for extra functionality.
 */

public class PlayerMain : MonoBehaviour {

    // Player Statistics
    public int credits, bonus, lives;
    public float shields, power;

    // General purpose variables
    internal Rigidbody2D rbPlayer;
    internal SpriteRenderer spritePlayer;
    internal CapsuleCollider2D capsCollider;
    internal GameManager gM;
    public bool helpMenuMode = false;
    public int playerNumber;
    internal string inputNameInsert;
    public GameObject canister;
    private float nextDamagePossible = 0.0F;

    // Ship impacts, death, respawning
    internal bool colliderEnabled;
    internal float damageThreshold = 6f;
    public GameObject deathExplosion;
    public Color normalColor, invulnColor;

    // Sound Systems
    public AudioSource audioShipThrust, audioShipSFX; // Thrust: passive thruster noise, SFX: powerup, extra life, impact noises
    public AudioClip smallAsteroidHit, bigAsteroidHit, deathSound;
    private bool audioThrusterPlaying = false, audioTeleportInPlaying = false, audioTeleportOutPlaying = false;

    // Player Scripts
    internal PlayerInput playerInput = default;
    internal PlayerMovement playerMovement = default;
    internal PlayerPowerups playerPowerups = default;
    internal PlayerWeapons playerWeapons = default;
    internal PlayerMisc playerMisc = default;
    internal PlayerAbility playerAbility = default;
    internal PlayerSpawnDeath playerSpawnDeath = default;
    internal PlayerUI playerUI = default;

    // ----------

    void Start () {
        GetComponents();
        playerInput.InputChoice();
        playerMisc.OtherStartFunctions();
    }
    
    // If game is not paused, then run per-frame updates
    void Update () {
        if (!gM.gamePausePanel.activeInHierarchy) {
            playerInput.GetInputs();
            playerMovement.ShipMovement();
            playerMovement.CheckScreenWrap();
            playerUI.UpdateBars();
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    public void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        playerUI.UpdatePointDisplays();
    }

    // When ship collides with asteroid or ufo colliders
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.tag == "asteroid" || col.gameObject.tag == "ufo") {
            // Slightly push spaceship away from collided object, whether invulnerable or not. UFO pushback stacks with this
            Vector2 force = gameObject.transform.position - col.transform.position;
            int magnitude = 80;
            gameObject.GetComponent<Rigidbody2D>().AddForce(force * magnitude);

            if (colliderEnabled && Time.time > nextDamagePossible) {
                nextDamagePossible = Time.time + 0.15f;
                if (col.gameObject.tag == "asteroid") { col.gameObject.GetComponent<AsteroidBehaviour>().AsteroidWasHit(); }
                // If ship rams hard enough, deal more damage
                if (col.relativeVelocity.magnitude > damageThreshold) {
                    audioShipSFX.clip = bigAsteroidHit;
                    shields = shields - 30f;
                }
                else {
                    audioShipSFX.clip = smallAsteroidHit;
                    shields = shields - 20f;
                }
                if (shields <= 0)
                {
                    audioShipSFX.clip = deathSound;
                    playerSpawnDeath.ShipIsDead();
                }
                audioShipSFX.Play();
            }
        }
    }

    // When ship collides with alien bullet or powerup triggers
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.tag == "bullet3" && colliderEnabled && Time.time > nextDamagePossible) {
            Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
            nextDamagePossible = Time.time + 0.15f;
            shields = shields - 10f;
            triggerObject.GetComponent<SpriteRenderer>().enabled = false;
            triggerObject.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(triggerObject.gameObject, 5f);
            if (shields <= 0)
            {
                playerSpawnDeath.ShipIsDead();
            }
            audioShipSFX.clip = smallAsteroidHit;
            audioShipSFX.Play();
        }
        if (triggerObject.gameObject.tag == "powerup" && spritePlayer.enabled) {
            Destroy(triggerObject.transform.parent.gameObject);
            playerPowerups.GivePowerup();
        }
    }


    public void CheckSounds(int intent) {
        if (intent == 1) {
            if (audioShipThrust.isPlaying) {
                audioThrusterPlaying = true;
                audioShipThrust.Pause();
            }
            if (playerAbility.teleportIn.GetComponent<AudioSource>().isPlaying) {
                audioTeleportInPlaying = true;
                playerAbility.teleportIn.GetComponent<AudioSource>().Pause();
            }
            if (playerAbility.teleportOut.GetComponent<AudioSource>().isPlaying) {
                audioTeleportOutPlaying = true;
                playerAbility.teleportOut.GetComponent<AudioSource>().Pause();
            }
        }
        else if (intent == 2) {
            if (audioThrusterPlaying) {
                audioShipThrust.UnPause();
            }
            if (audioTeleportInPlaying) {
                playerAbility.teleportIn.GetComponent<AudioSource>().UnPause();
            }
            if (audioTeleportOutPlaying) {
                playerAbility.teleportOut.GetComponent<AudioSource>().UnPause();
            }
        }
    }

    private void GetComponents()
    {
        gM = GameObject.FindObjectOfType<GameManager>();
        rbPlayer = gameObject.GetComponent<Rigidbody2D>();
        capsCollider = gameObject.GetComponent<CapsuleCollider2D>();
        spritePlayer = gameObject.GetComponent<SpriteRenderer>();
        playerInput = gameObject.GetComponent<PlayerInput>();
        playerMovement = gameObject.GetComponent<PlayerMovement>();
        playerPowerups = gameObject.GetComponent<PlayerPowerups>();
        playerWeapons = gameObject.GetComponent<PlayerWeapons>();
        playerMisc = gameObject.GetComponent<PlayerMisc>();
        playerAbility = gameObject.GetComponent<PlayerAbility>();
        playerSpawnDeath = gameObject.GetComponent<PlayerSpawnDeath>();
        playerUI = gameObject.GetComponent<PlayerUI>();
    }
}
