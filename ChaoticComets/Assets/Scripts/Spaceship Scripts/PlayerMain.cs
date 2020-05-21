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
    public GameObject modelPlayer;
    internal CapsuleCollider2D capsCollider;
    internal GameManager gM;
    public int playerNumber;
    internal string inputNameInsert;
    public GameObject canister;
    private float nextDamagePossible = 0.0F;

    // Ship impacts, death, respawning
    internal bool collisionsCanDamage;
    internal float highDamageThreshold = 6f;
    public GameObject deathExplosion;
    private readonly float minTimeBetweenDamage = 0.15f;

    // Player Scripts
    internal PlayerInput plrInput = default;
    internal PlayerMovement plrMovement = default;
    internal PlayerPowerups plrPowerups = default;
    internal PlayerWeapons plrWeapons = default;
    internal PlayerMisc plrMisc = default;
    internal PlayerAbility plrAbility = default;
    internal PlayerSpawnDeath plrSpawnDeath = default;
    internal PlayerUiSounds plrUiSound = default;

    public bool canShoot = true, canTeleport = true; // Only used to disable ship actions during some tutorial sections

    // ----------

    void Start () {
        GetComponents();
        plrInput.InputChoice();
        plrMisc.OtherStartFunctions();
    }
    
    // If game is not paused, then run per-frame updates
    void Update () {
        if (!gM.Refs.gamePausePanel.activeInHierarchy) {
            plrInput.GetInputs();
            plrMovement.ShipMovement();
            gM.CheckScreenWrap(transform);
            plrUiSound.UpdateBars();
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    public void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        plrUiSound.UpdatePointDisplays();
    }

    // When ship collides with asteroid or ufo colliders
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("asteroid") || col.gameObject.CompareTag("ufo")) {
            // Slightly push spaceship away from collided object, whether invulnerable or not. UFO pushback stacks with this
            Vector2 force = gameObject.transform.position - col.transform.position;
            int magnitude = 80;
            gameObject.GetComponent<Rigidbody2D>().AddForce(force * magnitude);

            if (collisionsCanDamage && Time.time > nextDamagePossible) {
                nextDamagePossible = Time.time + minTimeBetweenDamage;
                if (col.gameObject.CompareTag("asteroid")) { col.gameObject.GetComponent<AsteroidBehaviour>().AsteroidWasHit(); }
                // If ship rams hard enough, deal more damage
                if (col.relativeVelocity.magnitude > highDamageThreshold) {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactHard;
                    shields -= 30f;
                }
                else {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactSoft;
                    shields -= 20f;
                }
                if (shields <= 0)
                {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxDeath;
                    plrSpawnDeath.ShipIsDead();
                }
                plrUiSound.audioShipSFX.Play();
            }
        }
    }

    // When ship collides with alien bullet or powerup triggers
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.CompareTag("bullet3") || triggerObject.gameObject.CompareTag("bullet4"))
        {
            if (collisionsCanDamage && Time.time > nextDamagePossible) {
                Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
                triggerObject.GetComponent<CircleCollider2D>().enabled = false;
                Destroy(triggerObject.gameObject, 5f);

                if (triggerObject.gameObject.CompareTag("bullet3")) { shields -= 10f; }
                else { shields -= 20f; }
                nextDamagePossible = Time.time + minTimeBetweenDamage;

                if (shields <= 0)
                {
                    plrSpawnDeath.ShipIsDead();
                }
                plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactSoft;
                plrUiSound.audioShipSFX.Play();
            }
        }
        if (triggerObject.gameObject.CompareTag("powerup") && modelPlayer.activeInHierarchy) {
            Destroy(triggerObject.transform.parent.gameObject);
            plrPowerups.GivePowerup();
        }
    }

    private void GetComponents()
    {
        gM = FindObjectOfType<GameManager>();
        rbPlayer = gameObject.GetComponent<Rigidbody2D>();
        capsCollider = gameObject.GetComponent<CapsuleCollider2D>();
        plrInput = gameObject.GetComponent<PlayerInput>();
        plrMovement = gameObject.GetComponent<PlayerMovement>();
        plrPowerups = gameObject.GetComponent<PlayerPowerups>();
        plrWeapons = gameObject.GetComponent<PlayerWeapons>();
        plrMisc = gameObject.GetComponent<PlayerMisc>();
        plrAbility = gameObject.GetComponent<PlayerAbility>();
        plrSpawnDeath = gameObject.GetComponent<PlayerSpawnDeath>();
        plrUiSound = gameObject.GetComponent<PlayerUiSounds>();
    }
}
