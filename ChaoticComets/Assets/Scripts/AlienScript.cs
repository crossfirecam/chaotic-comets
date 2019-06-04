using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AlienScript : MonoBehaviour {

    // General purpose variables
    public GameManager gM;

    // Movement, physics variables
    public Rigidbody2D rb;
    public Vector2 direction;
    public float alienSpeed;

    // Weapon system variables
    public float shootingDelay; // In seconds
    public float bulletSpeed;
    public float lastTimeShot = 0f;

    // Defence system variables
    private float difficultyIncrease = 0.95f;
    public AudioSource audioAlienImpact, audioAlienHum;
    public AudioClip audioClipShieldReflect, audioClipTakenDamage;
    public float alienHealth, alienMaxHealth;
    private float pointsToScore = 100, teleportKillPoints = 500;
    public GameObject forceField;

    // Other variables
    private float frameCountForRetreating;
    private Transform player;
    public GameObject bullet;
    public GameObject deathExplosion, explosion, teleportEffect;
    public GameObject playerShip1, playerShip2;
    private bool playerFound = false, deathStarted = false, ufoTeleporting = false, ufoRetreating = false, audioAlienHumPlaying;

    // ----------

    void Start () {
        lastTimeShot = Time.time + 1.5f; // UFO will not shoot for the first 1.5 seconds
        gM = GameObject.FindObjectOfType<GameManager>();
        playerShip1 = GameObject.FindGameObjectWithTag("Player");
        if (BetweenScenesScript.PlayerCount == 2) {
            playerShip2 = GameObject.FindGameObjectWithTag("Player 2");
        }
        // If alien ship is called during level transition, it destroys itself
        if (gM.CheckIfEndOfLevel() && !ufoTeleporting) { Debug.Log("UFO attempted to spawn during level transition"); Destroy(gameObject); }
    }
	
	    void Update () {
        // Stabilising 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);
        // Weapon systems. If it is time to shoot, there is a player to shoot at...
        if (Time.time > lastTimeShot + shootingDelay && playerFound) {
            // and the UFO is not dying, teleporting, or retreating, then shoot
            if (!deathStarted && !ufoTeleporting && !ufoRetreating) {
                float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg - 90f;
                Quaternion q = Quaternion.AngleAxis(angle, Vector3.forward);

                Vector3 bulletPosition = new Vector3(transform.position.x, transform.position.y, transform.position.z - 4);
                GameObject newBullet = Instantiate(bullet, bulletPosition, q);
                newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(new Vector2(0f, bulletSpeed));
                Destroy(newBullet, 2f);

                lastTimeShot = Time.time;
            }
        }
        CheckScreenWrap();
    }
    
    // If the UFO is not dying, then start the teleport sequence at the end of a level
    void TeleportStart() {
        if (!deathStarted) {
            forceField.SetActive(false);
            ufoRetreating = false;
            ufoTeleporting = true;
            teleportEffect.SetActive(true);
            Renderer[] listOfUFOparts = GetComponentsInChildren<Renderer>();
            foreach (Renderer rend in listOfUFOparts) {
                StartCoroutine(FadeOut(rend));
            }
            Invoke("TeleportEnd", 2f);
        }
    }

    // Fade the ship's material color as it teleports
    private IEnumerator FadeOut(Renderer ufoPart) {
        Material partMaterial = ufoPart.material;
        Color origColor = partMaterial.color;
        float speedOfFade = 0.5f;
        float alpha = 1f;
        
        while (alpha > 0f) {
            if (deathStarted) { break; }
            alpha -= speedOfFade * Time.deltaTime;
            partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
            yield return null;
        }
        // If during the while loop, death is started - then UFO will have a unique death animation
        if (deathStarted) {
            ufoTeleporting = false;
            teleportEffect.SetActive(false);
            speedOfFade = 2f;
            while (alpha < 1f) {
                alpha += speedOfFade * Time.deltaTime;
                partMaterial.color = new Color(origColor.r, origColor.g, origColor.b, alpha);
                yield return null;
            }
        }
    }

    // Destroy gameobject
    void TeleportEnd() {
        if (!deathStarted) {
            Destroy(gameObject);
        }
    }

    void FixedUpdate() {
        if (!ufoTeleporting && !deathStarted) {
            // If a player is not found, find one
            if (!playerFound) {
                float randomFloat = Random.Range(0.0f, 1f);
                if (randomFloat >= 0.5f && !gM.player1dead && !gM.player1TEMPDEAD) {
                    player = GameObject.FindWithTag("Player").transform;
                }
                else if (randomFloat <= 0.49f && !gM.player2dead && !gM.player2TEMPDEAD) {
                    player = GameObject.FindWithTag("Player 2").transform;
                }
                if (player != null) {
                    lastTimeShot = Time.time + 1f; // Once player is found, don't shoot for 1 second
                    playerFound = true;
                }
                if (alienHealth <= 10f && !ufoRetreating) { // Fringe case - one player alive, they die, a bullet hits almost dead UFO, and it wouldn't activate shields
                    AlienRetreat();
                }
                if (direction == Vector2.zero) { //Fringe case - UFO spawns while both players are dead
                    direction = Random.insideUnitCircle.normalized;
                }
                // Continue moving in same direction if both players are dead
                rb.MovePosition(rb.position + direction * alienSpeed * Time.fixedDeltaTime);
            }
            // If a player IS found, then...
            else {
                // If UFO has more than 10 health, continue chasing player
                if (alienHealth > 10f) {
                    direction = (player.position - transform.position).normalized;
                }
                // If alien has less than 10 health, it will run away in a single direction and attempt to teleport
                else {
                    if (!ufoRetreating) {
                        direction = (player.position + transform.position).normalized;
                        AlienRetreat();
                    }
                }
                rb.MovePosition(rb.position + direction * alienSpeed * Time.fixedDeltaTime);
            }
        }
    }

    private void AlienRetreat() {
        alienSpeed = alienSpeed * 4f;
        ufoRetreating = true;
        forceField.SetActive(true);
        Invoke("TeleportStart", 3f);
    }

    void OnTriggerEnter2D(Collider2D playerBullet) {
        if (playerBullet.gameObject.tag == "bullet" || playerBullet.gameObject.tag == "bullet2") {
            // If UFO is not retreating, deal damage and score credits
            if (!ufoRetreating) {
                // Explosion
                GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
                Destroy(newExplosion, 2f);
                // Bullet invisible, allows sound to continue playing
                playerBullet.enabled = false;
                Destroy(playerBullet.GetComponentInChildren<ParticleSystem>());
                Destroy(playerBullet.gameObject, 5f);

                alienHealth = alienHealth - 10f;
                // If UFO is teleporting, has 0 health or less, then grant more points for an escape kill
                if (ufoTeleporting && alienHealth <= 0f) { pointsToScore = teleportKillPoints; }
                // Send points to the player who shot, if alien has not taken fatal damage
                if (alienHealth > -10f) {
                    if (playerBullet.CompareTag("bullet")) { playerShip1.SendMessage("ScorePoints", pointsToScore); }
                    if (playerBullet.CompareTag("bullet2")) { playerShip2.SendMessage("ScorePoints", pointsToScore); }
                    audioAlienImpact.clip = audioClipTakenDamage;
                    audioAlienImpact.Play();
                    DetermineIfDead();
                }
            }
            // If UFO IS retreating, don't deal damage. Instead, reflect bullet
            else {
                Debug.Log("Bullet relected from UFO shield");
                Vector2 force = gameObject.transform.position - playerBullet.transform.position;
                int magnitude = 1000;
                playerBullet.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
                playerBullet.SendMessage("UfoReflectedBullet");
                audioAlienImpact.clip = audioClipShieldReflect;
                audioAlienImpact.Play();
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision) {
        Vector2 force = gameObject.transform.position - collision.transform.position;
        int magnitude = 0;
        // Asteroid and player collisions do not cause damage to UFO
        if (collision.gameObject.tag == "asteroid") { magnitude = 300; }
        else if (collision.gameObject.tag == "Player" || collision.gameObject.tag == "Player 2") { magnitude = 100; }
        if (ufoRetreating) { magnitude = magnitude / 3; }
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
    }

    // If alien has 0 or less health, it is dying. Can only be set to dying state once
    private void DetermineIfDead() {
        if (!deathStarted && alienHealth <= 0f) {
            float rateOfExplosions = 0.4f;
            float durationOfExplosions = 1.5f;
            deathStarted = true;
            if (ufoTeleporting) { rateOfExplosions = 0.2f; durationOfExplosions = 2.5f; }
            InvokeRepeating("DeathExplosions", 0.0f, rateOfExplosions);
            Invoke("DeathRoutine", durationOfExplosions);
        }
    }

    // Repeating invoke that causes explosions
    void DeathExplosions() {
        GameObject newExplosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
    }

    // Final instructions for UFO when it's about to die
    void DeathRoutine() {
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
        gM.AlienAndPowerupLogic("alienRespawn");
        Destroy(gameObject);
    }

    // If a player the UFO is following has died, reset variables. So it chases after another player
    void PlayerDied() {
        playerFound = false;
        player = null;
    }

    void CheckSounds(int intent) {
        if (intent == 1) {
            if (audioAlienHum.isPlaying) {
                audioAlienHumPlaying = true;
                audioAlienHum.Pause();
            }
        }
        else if (intent == 2) {
            if (audioAlienHumPlaying) {
                audioAlienHum.UnPause();
            }
        }
    }

    void CheckScreenWrap() {
        // Screen Wrapping. UFO does not screen wrap when in the first 3 seconds of spawning onto level
        if (Time.time > 3) {
            Vector2 newPosition = transform.position;
            if (transform.position.y > gM.screenTop) { newPosition.y = gM.screenBottom; }
            if (transform.position.y < gM.screenBottom) { newPosition.y = gM.screenTop; }
            if (transform.position.x > gM.screenRight) { newPosition.x = gM.screenLeft; }
            if (transform.position.x < gM.screenLeft) { newPosition.x = gM.screenRight; }
            transform.position = newPosition;
        }
    }
}
