using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMain : MonoBehaviour {

    // Player Scripts
    [SerializeField] internal PlayerInput playerInput = default;
    [SerializeField] internal PlayerMovement playerMovement = default;
    [SerializeField] internal PlayerPowerups playerPowerups = default;
    [SerializeField] internal PlayerWeapons playerWeapons = default;
    [SerializeField] internal PlayerMisc playerMisc = default;
    [SerializeField] internal PlayerAbility playerAbility = default;
    [SerializeField] internal PlayerSpawnDeath playerSpawnDeath = default;

    // Player Statistics / Bar Lengths
    public int credits, bonus, lives;
    public float shields, power;

    // General purpose variables
    internal Rigidbody2D rb;
    internal SpriteRenderer sprite;
    internal GameManager gM;
    public bool helpMenuMode = false;
    public int playerNumber;
    internal string inputNameInsert;
    public GameObject canister;
    private float nextDamagePossible = 0.0F;

    // Ship sounds, impacts, death, respawning
    public bool colliderEnabled;
    public float damageThreshold;
    public AudioSource audioShipImpact, audioShipThrust;
    private bool audioThrusterPlaying = false, audioTeleportInPlaying = false, audioTeleportOutPlaying = false;
    public AudioClip smallAsteroidHit, bigAsteroidHit, deathSound, lifeGained, powerupReceived;
    public GameObject deathExplosion;
    public Color normalColor, invulnColor;

    // Upgrade Systems
    public float upgradeSpeed, upgradeBrake, upgradeFireRate, upgradeShotSpeed;

    // UI Elements
    public Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, retroThrusterPowerup;
    const int bonusInterval = 10000;
    public Image shieldBar, powerBar;
    public Sprite powerWhenCharging, powerWhenReady;
    public Text scoreText, livesText;
    internal float prevshields;

    // ----------
    
    void Start () {
        gM = GameObject.FindObjectOfType<GameManager>();
        rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        playerInput.InputChoice();
        playerMisc.OtherStartFunctions();
    }
    
       void Update () {
        if (!gM.gamePausePanel.activeInHierarchy) {
            // Determine how long bullet should stay on screen

            playerInput.GetInputs();
            playerMovement.ShipMovement();
            playerMovement.CheckScreenWrap();
            shieldBar.fillAmount = shields / 80;
            powerBar.fillAmount = power / 80;
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    internal void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        scoreText.text = "Credits:\n" + credits;
        if (credits > bonus) {
            bonus += bonusInterval;
            lives++; livesText.text = "Lives: " + lives;
            audioShipImpact.clip = lifeGained;
            audioShipImpact.Play();
        }
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
                if (col.gameObject.tag == "asteroid") { col.gameObject.SendMessage("AsteroidWasHit"); }
                // If ship rams hard enough, deal more damage
                if (col.relativeVelocity.magnitude > damageThreshold) {
                    audioShipImpact.clip = bigAsteroidHit;
                    shields = shields - 30f;
                }
                else {
                    audioShipImpact.clip = smallAsteroidHit;
                    shields = shields - 20f;
                }
                if (shields <= 0) {
                    playerSpawnDeath.ShipIsDead();
                }
                audioShipImpact.Play();
            }
        }
    }

    // When ship collides with alien bullet or powerup triggers
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.tag == "bullet3" && colliderEnabled && Time.time > nextDamagePossible) {
            Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
            nextDamagePossible = Time.time + 0.15f;
            shields = shields - 10f;
            audioShipImpact.clip = smallAsteroidHit;
            triggerObject.GetComponent<SpriteRenderer>().enabled = false;
            triggerObject.GetComponent<CircleCollider2D>().enabled = false;
            Destroy(triggerObject.gameObject, 5f);
            if (shields <= 0)
            {
                playerSpawnDeath.ShipIsDead();
            }
            audioShipImpact.Play();
        }
        if (triggerObject.gameObject.tag == "powerup") {
            Destroy(triggerObject.transform.parent.gameObject);
            playerPowerups.GivePowerup();
        }
    }


    private IEnumerator FadeShip(string inOrOut) {
        if (inOrOut == "Out") {
            colliderEnabled = false;
            playerInput.isNotTeleporting = false;
            for (float fadeTick = 1f; fadeTick >= 0f; fadeTick -= 0.1f) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
        else {
            playerInput.isNotTeleporting = true;
            for (float fadeTick = 0f; fadeTick <= 1f; fadeTick += 0.1f) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            colliderEnabled = true;
        }
    }


    void CheckSounds(int intent) {
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

    public void CheatGiveCredits() {
        credits += 10000;
    }
}
