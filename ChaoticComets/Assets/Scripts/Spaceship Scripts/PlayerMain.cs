using UnityEngine;

/*
 * This class contains all general code for the Player objects, referring to many other scripts for extra functionality.
 */

public class PlayerMain : MonoBehaviour {

    [Header("Player Statistics")]
    public int playerNumber;
    public int credits = 0, totalCredits = 0, bonus = 4999;
    public float shields = 80, power = 80;

    [Header("Impacts, Death, Respawning")]
    private float nextDamagePossible = 0.0F;
    internal bool collisionsCanDamage;
    internal float highDamageThreshold = 6f;
    private readonly float minTimeBetweenDamage = 0.15f;

    [Header("Player Scripts")]
    internal PlayerInput plrInput = default;
    internal PlayerMovement plrMovement = default;
    internal PlayerPowerups plrPowerups = default;
    internal PlayerWeapons plrWeapons = default;
    internal PlayerMisc plrMisc = default;
    internal PlayerAbility plrAbility = default;
    internal PlayerSpawnDeath plrSpawnDeath = default;
    internal PlayerUiSounds plrUiSound = default;

    [Header("References")]
    public GameObject deathExplosion;
    public GameObject modelPlayer;
    public GameObject canister;
    internal Rigidbody2D rbPlayer;
    internal CapsuleCollider2D capsCollider;
    internal GameManager gM;

    // ----------

    void Start () {
        GetComponents();
        plrMisc.OtherStartFunctions();
    }
    
    // If game is not paused, then run per-frame updates
    void Update () {
        if (!gM.Refs.gamePausePanel.activeInHierarchy) {
            plrInput.CheckInputs();
            plrMovement.ShipMovement();
            gM.CheckScreenWrap(transform);
            UiManager.i.SetPlayerStatusBars(playerNumber, shields, power);
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    public void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        totalCredits += pointsToAdd;
        plrUiSound.UpdatePointDisplays();
    }

    // When ship collides with asteroid or ufo colliders
    void OnCollisionEnter2D(Collision2D col) {
        if (col.gameObject.CompareTag("asteroid") || col.gameObject.CompareTag("ufo")) {
            // Slightly push spaceship away from collided object, whether invulnerable or not. UFO pushback stacks with this
            Vector2 force = gameObject.transform.position - col.transform.position;
            int magnitude = 40;
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
                plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxImpactSoft;

                if (shields <= 0)
                {
                    plrUiSound.audioShipSFX.clip = plrUiSound.audClipPlrSfxDeath;
                    plrSpawnDeath.ShipIsDead();
                }
                plrUiSound.audioShipSFX.Play();
            }
        }
        if (triggerObject.gameObject.CompareTag("powerup") && modelPlayer.activeInHierarchy) {
            Destroy(triggerObject.gameObject);
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
