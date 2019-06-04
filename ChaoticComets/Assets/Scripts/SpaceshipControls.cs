using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpaceshipControls : MonoBehaviour {

    // Debug accessible variables
    public bool ifInsuranceActive, ifFarShot, ifRetroThruster, ifRapidShot, ifTripleShot;
    public int credits, bonus, lives;
    public float shields, power;

    // General purpose variables
    private Rigidbody2D rb; private SpriteRenderer sprite;
    private GameManager gM;
    public bool helpMenuMode = false;
    public int playerNumber;
    private string inputNameInsert;
    public GameObject canister;
    private bool powerupUndecided;

    // Ship movement & teleport variables
    public float thrust, turnThrust;
    private float thrustInput, turnInput;
    private GameObject teleportIn, teleportOut;
    private bool isNotTeleporting = true;
    public ParticleSystem thruster1, thruster2;

    // Ship sounds, impacts, death, respawning
    public bool colliderEnabled;
    public float damageThreshold;
    public AudioSource audioShipImpact, audioShipThrust;
    private bool audioThrusterPlaying = false, audioTeleportInPlaying = false, audioTeleportOutPlaying = false;
    public AudioClip smallAsteroidHit, bigAsteroidHit, deathSound, lifeGained, powerupReceived;
    public GameObject deathExplosion;
    public Color normalColor, invulnColor;

    // Weapon Systems
    public GameObject bullet;
    public float bulletForce;
    private float bulletDestroyTime;
    private GameObject mainCannon, tripleCannon1, tripleCannon2;
    private float fireRateRapid = .9f;
    private float fireRateTriple = .6f;
    private float fireRateNormal = .4f;
    private float nextFire = 0.0F;
    private float nextDamagePossible = 0.0F;

    // Upgrade Systems
    public float upgradeSpeed, upgradeBrake, upgradeFireRate, upgradeShotSpeed;

    // UI Elements
    public Image insurancePowerup, farShotPowerup, tripleShotPowerup, rapidShotPowerup, retroThrusterPowerup;
    const int bonusInterval = 10000;
    public Image shieldBar, powerBar;
    public Sprite powerWhenCharging, powerWhenReady;
    public Text scoreText, livesText;
    private float prevshields;

    // ----------
    
    void Start () {
        gM = GameObject.FindObjectOfType<GameManager>();
        mainCannon = gameObject.transform.Find("P" + playerNumber + "-MainCannon").gameObject;
        tripleCannon1 = gameObject.transform.Find("P" + playerNumber + "-TripleCannon1").gameObject;
        tripleCannon2 = gameObject.transform.Find("P" + playerNumber + "-TripleCannon2").gameObject;
        teleportIn = gameObject.transform.Find("P" + playerNumber + "-TeleportParticlesIn").gameObject;
        teleportOut = gameObject.transform.Find("P" + playerNumber + "-TeleportParticlesOut").gameObject;
        rb = gameObject.GetComponent<Rigidbody2D>();
        sprite = gameObject.GetComponent<SpriteRenderer>();
        InputChoice();
        if (BetweenScenesScript.Difficulty != 9) {
            if (!BetweenScenesScript.ResumingFromSave) { // If spaceship object created on first load, set score to 0 and bonus to 9999
                credits = 0; bonus = 9999; lives = 3; shields = 80;
            }
            else { // If spaceship object resumed from savefile, ask savefile
                Saving_PlayerManager data = Saving_SaveManager.LoadData();
                if (playerNumber == 1) {
                    credits = data.player1credits;
                    shields = data.player1health;
                    bonus = data.player1bonus;
                    lives = data.player1lives;
                    if (data.player1powerups[0] == 1) { ifInsuranceActive = true; insurancePowerup.gameObject.SetActive(true); }
                    if (data.player1powerups[1] == 1) { ifFarShot = true; farShotPowerup.gameObject.SetActive(true); }
                    if (data.player1powerups[2] == 1) { ifRetroThruster = true; retroThrusterPowerup.gameObject.SetActive(true); }
                    if (data.player1powerups[3] == 1) { ifRapidShot = true; rapidShotPowerup.gameObject.SetActive(true); }
                    if (data.player1powerups[4] == 1) { ifTripleShot = true; tripleShotPowerup.gameObject.SetActive(true); }
                }
                else { // (playerNumber == 2)
                    credits = data.player2credits;
                    shields = data.player2health;
                    bonus = data.player2bonus;
                    lives = data.player2lives;
                    if (data.player2powerups[0] == 1) { ifInsuranceActive = true; insurancePowerup.gameObject.SetActive(true); }
                    if (data.player2powerups[1] == 1) { ifFarShot = true; farShotPowerup.gameObject.SetActive(true); }
                    if (data.player2powerups[2] == 1) { ifRetroThruster = true; retroThrusterPowerup.gameObject.SetActive(true); }
                    if (data.player2powerups[3] == 1) { ifRapidShot = true; rapidShotPowerup.gameObject.SetActive(true); }
                    if (data.player2powerups[4] == 1) { ifTripleShot = true; tripleShotPowerup.gameObject.SetActive(true); }
                }
                // If a two player game is resumed, tell GameManager the player is dead, then disable sprite/colliders
                if (lives == 0) {
                    PretendShipDoesntExist();
                    gM.SendMessage("PlayerDied", playerNumber);
                }
                else if (lives > 0 && shields == 0) {
                    prevshields = 80;
                }
                // Report back what was loaded from ships.
                Debug.Log("Loaded. Player " + playerNumber + ": " + shields + " shields, " + credits + " credits, "
                    + bonus + " bonus threshold, " + lives + " lives.");
            }
            if (playerNumber == 1) {
                upgradeSpeed = BetweenScenesScript.UpgradesP1[0];
                upgradeBrake = BetweenScenesScript.UpgradesP1[1];
                upgradeFireRate = BetweenScenesScript.UpgradesP1[2];
                upgradeShotSpeed = BetweenScenesScript.UpgradesP1[3];
            }
            else if (playerNumber == 2) {
                upgradeSpeed = BetweenScenesScript.UpgradesP2[0];
                upgradeBrake = BetweenScenesScript.UpgradesP2[1];
                upgradeFireRate = BetweenScenesScript.UpgradesP2[2];
                upgradeShotSpeed = BetweenScenesScript.UpgradesP2[3];
            }
            scoreText.text = "Credits:\n" + credits;
            livesText.text = "Lives: " + lives;
            bulletForce = bulletForce * upgradeShotSpeed;
            fireRateNormal = fireRateNormal / upgradeFireRate;
            fireRateRapid = fireRateRapid / upgradeFireRate;
            fireRateTriple = fireRateTriple / upgradeFireRate;
        }
    }
	
	   void Update () {
        if (!gM.gamePausePanel.activeInHierarchy) {
            // Determine how long bullet should stay on screen
            if (ifFarShot) { bulletDestroyTime = 1.4f; }
            else { bulletDestroyTime = 0.8f; } //TODO put somewhere else

            GetInputs();
            ShipMovement();
            CheckScreenWrap();
            shieldBar.fillAmount = shields / 80;
            powerBar.fillAmount = power / 80;
        }
    }

    // Contains code for receiving inputs from player
    private void GetInputs() {
        // Get axis-based inputs
        thrustInput = -Input.GetAxis("Thrust" + inputNameInsert);
        turnInput = -Input.GetAxis("Rotate Ship" + inputNameInsert);

        // Get button-based inputs
        // If fire button is pressed, and ship is not teleporting, not dead, and able to fire, then fire
        if (Input.GetButton("Primary Fire" + inputNameInsert)
            && isNotTeleporting && shields != 0 && Time.time > nextFire) {
            FiringLogic();
        }
        // If power button is pressed, and ship has full power with colliders enabled, and level has no asteroids, then use power
        if (Input.GetButtonDown("Power" + inputNameInsert) && gM.asteroidCount != 0) {
            if (colliderEnabled && power == 80) {
                powerBar.sprite = powerWhenCharging;
                teleportIn.SetActive(true);
                StartCoroutine("FadeShip", "Out");
                Invoke("Hyperspace", 2f);
            }
        }
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    private void FiringLogic() {
        if (ifRapidShot) {
            nextFire = Time.time + fireRateRapid;
            StartCoroutine(RapidShot());
        }
        else if (ifTripleShot) {
            nextFire = Time.time + fireRateTriple;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.SendMessage("DestroyBullet", bulletDestroyTime);
            GameObject newBullet2 = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
            newBullet2.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet2.SendMessage("DestroyBullet", bulletDestroyTime);
            GameObject newBullet3 = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
            newBullet3.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet3.SendMessage("DestroyBullet", bulletDestroyTime);
        }
        else {
            nextFire = Time.time + fireRateNormal;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.SendMessage("DestroyBullet", bulletDestroyTime);
        }
    }

    private void ShipMovement() {
        // Rotate the ship
        if (turnInput != 0 && sprite.enabled) {
            transform.Rotate(Vector3.forward * turnInput * Time.deltaTime * turnThrust);
        }

        // Apply force on Y axis of spaceship, multiply by thrust
        if (thrustInput != 0 && sprite.enabled && isNotTeleporting) {
            if (!audioShipThrust.isPlaying) { audioShipThrust.Play(); }
            if (!thruster1.isPlaying) { thruster1.Play(); }
            if (!thruster2.isPlaying) { thruster2.Play(); }
            // If thrust is less than 0, then ship is braking. On difficulty hard, ignore brake.
            if (thrustInput > 0) {
                if (BetweenScenesScript.Difficulty != 2) {
                    rb.drag = rb.velocity.magnitude / 2f;
                }
            }
            else {
                rb.AddRelativeForce(Vector2.up * -thrustInput * Time.deltaTime * thrust);
                rb.drag = rb.velocity.magnitude / 10f;
            }
        }
        // Apply passive drag depending on if retro thrusters are equipped or not
        else {
            if (audioShipThrust.isPlaying) { audioShipThrust.Stop(); }
            if (thruster1.isPlaying) { thruster1.Stop(); }
            if (thruster2.isPlaying) { thruster2.Stop(); }
            if (ifRetroThruster) {
                if (BetweenScenesScript.Difficulty != 2) {
                    rb.drag = rb.velocity.magnitude / 0.2f;
                }
                else {
                    rb.drag = rb.velocity.magnitude / 2f;
                }
            }
            else { rb.drag = rb.velocity.magnitude / 8; }
        }
    }

    private IEnumerator RapidShot() {
        GameObject[] rapidShotArray = new GameObject[10];
        GameObject[] rapidShotArray2 = new GameObject[10];
        GameObject[] rapidShotArray3 = new GameObject[10];
        if (ifTripleShot) {
            for (int i = 0; i < 2; i++) {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].SendMessage("DestroyBullet", bulletDestroyTime);

                rapidShotArray2[i] = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
                rapidShotArray2[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray2[i].SendMessage("DestroyBullet", bulletDestroyTime);

                rapidShotArray3[i] = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
                rapidShotArray3[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray3[i].SendMessage("DestroyBullet", bulletDestroyTime);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else {
            for (int i = 0; i < 4; i++) {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].SendMessage("DestroyBullet", bulletDestroyTime);
                yield return new WaitForSeconds(0.08f);
            }
        }
    }

    // Receives points scored from latest asteroid hit, UFO hit, or canister reward
    void ScorePoints(int pointsToAdd) {
        credits += pointsToAdd;
        scoreText.text = "Credits:\n" + credits;
        if (credits > bonus) {
            bonus += bonusInterval;
            lives++; livesText.text = "Lives: " + lives;
            audioShipImpact.clip = lifeGained;
            audioShipImpact.Play();
        }
    }

    private void Hyperspace() {
        Vector2 newPosition = new Vector2(0, 0);
        if (!helpMenuMode) {
            newPosition = new Vector2(Random.Range(-9f, 9f), Random.Range(-5f, 5f));
        }
        else { newPosition = new Vector2(Random.Range(-7.4f, -2.6f), Random.Range(-4.0f, 1.2f)); }
        transform.position = newPosition;
        rb.velocity = Vector2.zero;
        teleportIn.SetActive(false);
        StartCoroutine("FadeShip", "In");
        teleportOut.SetActive(true);
        StartCoroutine("PowerTimer", "Hyperspace");
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
                    ShipIsDead();
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
            if (shields <= 0) {
                ShipIsDead();
            }
            audioShipImpact.Play();
        }
        if (triggerObject.gameObject.tag == "powerup") {
            Destroy(triggerObject.transform.parent.gameObject);
            GivePowerup();
        }
    }

    void ShipIsDead() {
        if (ifInsuranceActive) {
            insurancePowerup.gameObject.SetActive(false); ifInsuranceActive = false;
        }
        else {
            // If difficulty is easy, do not remove retro thruster
            if (BetweenScenesScript.Difficulty != 0) {
                retroThrusterPowerup.gameObject.SetActive(false); ifRetroThruster = false;
            }
            rapidShotPowerup.gameObject.SetActive(false); ifRapidShot = false;
            tripleShotPowerup.gameObject.SetActive(false); ifTripleShot = false;
            farShotPowerup.gameObject.SetActive(false); ifFarShot = false;
        }
        shields = 0;
        lives--; livesText.text = "Lives: " + lives;
        GameObject newExplosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
        audioShipImpact.clip = deathSound;
        sprite.enabled = false;
        colliderEnabled = false;
        gM.SendMessage("PlayerLostLife", playerNumber);
        if (lives < 1) {
            gM.SendMessage("PlayerDied", playerNumber);
        }
        else { prevshields = 80; Invoke("RespawnShip", 3f); }
    }

    void ShipIsRecovering() {
        if (shields < 80 && shields > 0) {
            gM.SendMessage("ShowRechargeText");
            StartCoroutine("RecoveringTimer");
        }
    }

    void RespawnShip() {
        if (lives > 0 && gM.asteroidCount != 0) {
            // If difficulty is Easy, equip Retro Thruster every respawn
            if (BetweenScenesScript.Difficulty == 0) {
                ifRetroThruster = true; retroThrusterPowerup.gameObject.SetActive(true);
            }
            sprite.enabled = true;
            colliderEnabled = false;
            rb.velocity = Vector2.zero;

            // If at least one player is dead, place the other in the center of the screen
            if (gM.player1dead || gM.player2dead) {
                transform.position = new Vector2(0f, 0f);
            }
            // If both players exist, place them apart
            else {
                if (playerNumber == 1) { transform.position = new Vector2(-3f, 0f); }
                else { transform.position = new Vector2(3f, 0f); }
            }
            // Alert GameManger that their temporary death is over for UFO tracking
            if (playerNumber == 1) {
                gM.player1TEMPDEAD = false;
            }
            else {
                gM.player2TEMPDEAD = false;
            }

            sprite.color = invulnColor;
            StartCoroutine("InvulnTimer");
        }
    }

    // When ship is killed, take 4 seconds total to recharge shields. Ship becomes hittable after those 4s.
    private IEnumerator InvulnTimer() {
        // Or, if ship is respawning at start of a level, set the previousShields level to current shield level instead
        if (prevshields != 80) { prevshields = shields; }
        shields = 0;
        powerBar.sprite = powerWhenCharging; // Set power bar to have no text
        for (int shieldsTick = 0; shieldsTick <= prevshields; shieldsTick++) {
            shields = shieldsTick;
            yield return new WaitForSeconds(3f/prevshields);
        }
        powerBar.sprite = powerWhenReady; // Set power bar to have text informing power can be used
        sprite.color = normalColor;
        colliderEnabled = true;
        prevshields = 0;
    }

    // When level transition happens, take a moment to recharge shields by 20, or if above 60 heal up to 80.
    private IEnumerator RecoveringTimer() {
        // By default, recover to full shields, and determine how much shields need to be healed for UI timing
        float shieldToRecoverTo = 80;
        float shieldTimer = 80 - shields;
        // If shields are less than 60, determine where to recover to, and set shield timer to 20s for UI timing
        if (shields < 60) {
            shieldToRecoverTo = shields + 20;
            shieldTimer = 20;
        }
        for (float shieldsTick = shields; shieldsTick <= shieldToRecoverTo; shieldsTick++) {
            if (shields == 80) { break; } // If a canister is picked up during regen, break the loop
            shields = shieldsTick;
            yield return new WaitForSeconds(1f / shieldTimer);
    }
    }

    // When power is used, take 12 seconds total to recharge power. Ship can use power after those 12s.
    private IEnumerator PowerTimer(string powerType) {
        if (powerType == "Hyperspace") {
            power = 0;
            for (int powerTick = 0; powerTick <= 80; powerTick++) {
                power = powerTick;
                yield return new WaitForSeconds(0.15f);
            }
            teleportOut.SetActive(false);
            power = 80f;
            powerBar.sprite = powerWhenReady;
            StopCoroutine("PowerTimer");
        }
    }

    private IEnumerator FadeShip(string inOrOut) {
        if (inOrOut == "Out") {
            colliderEnabled = false;
            isNotTeleporting = false;
            for (float fadeTick = 1f; fadeTick >= 0f; fadeTick -= 0.1f) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
        }
        else {
            isNotTeleporting = true;
            for (float fadeTick = 0f; fadeTick <= 1f; fadeTick += 0.1f) {
                gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, fadeTick);
                yield return new WaitForSeconds(0.1f);
            }
            gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 1f);
            colliderEnabled = true;
        }
    }

    void GivePowerup() {
        powerupUndecided = true;
        while (powerupUndecided) {
            gM.AlienAndPowerupLogic("powerupRespawn");
            float randomiser = Random.Range(0f, 6f);
            if (randomiser < 0.2f) { // Award 1000 credits (least likely - 1/30 chance)
                powerupUndecided = false; ScorePoints(1000);
            }
            else if (randomiser < 1f && !ifInsuranceActive && AtLeastOneOtherPowerup()) { // Give insurance powerup
                powerupUndecided = false; ifInsuranceActive = true; insurancePowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 2f && !ifFarShot) { // Give far shot powerup
                powerupUndecided = false; ifFarShot = true; farShotPowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 3f && !ifTripleShot) { // Give triple shot powerup
                powerupUndecided = false; ifTripleShot = true; tripleShotPowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 4f && !ifRapidShot) { // Give rapid shot powerup
                powerupUndecided = false; ifRapidShot = true; rapidShotPowerup.gameObject.SetActive(true);
            }
            else if (randomiser < 5f && !ifRetroThruster) { // Give retro thruster powerup
                powerupUndecided = false; ifRetroThruster = true; retroThrusterPowerup.gameObject.SetActive(true);
            }
            // Give shield top-up if less than 60 (if respawning, select another powerup)
            else if (randomiser < 6f && shields <= 60f && colliderEnabled) {
                powerupUndecided = false; shields = 80;
            }
            // If all powerups have been collected, refill shields or award 1000/200 credits
            else if (ifInsuranceActive && ifFarShot && ifTripleShot && ifRapidShot && ifRetroThruster) {
                powerupUndecided = false;
                if (randomiser < 0.2f) { ScorePoints(1000); }
                else if (randomiser < 4f && shields <= 60f && colliderEnabled) { shields = 80; } // (if respawning, select a score prize)
                else { ScorePoints(200); }
            }
        }
        Destroy(canister);
        audioShipImpact.clip = powerupReceived;
        audioShipImpact.Play();
    }

    // Basically only gives insurance powerup once at least one other powerup is received
    // Does this by determining if at least one of the others is received,
    // Then if Easy mode is selected, check that retro thrusters isn't the only one equipped (pointless to insure it)
    // If not at least one powerup has been received yet, tell GivePowerup() to select another powerup
    private bool AtLeastOneOtherPowerup() {
        if (ifFarShot || ifTripleShot || ifRapidShot || ifRetroThruster) {
            if (BetweenScenesScript.Difficulty == 0 && ifRetroThruster && !ifFarShot && !ifTripleShot && !ifRapidShot) {
                return false;
            }
            else { return true; }
        }
        else {
            return false;
        }
    }

    void CheckSounds(int intent) {
        if (intent == 1) {
            if (audioShipThrust.isPlaying) {
                audioThrusterPlaying = true;
                audioShipThrust.Pause();
            }
            if (teleportIn.GetComponent<AudioSource>().isPlaying) {
                audioTeleportInPlaying = true;
                teleportIn.GetComponent<AudioSource>().Pause();
            }
            if (teleportOut.GetComponent<AudioSource>().isPlaying) {
                audioTeleportOutPlaying = true;
                teleportOut.GetComponent<AudioSource>().Pause();
            }
        }
        else if (intent == 2) {
            if (audioThrusterPlaying) {
                audioShipThrust.UnPause();
            }
            if (audioTeleportInPlaying) {
                teleportIn.GetComponent<AudioSource>().UnPause();
            }
            if (audioTeleportOutPlaying) {
                teleportOut.GetComponent<AudioSource>().UnPause();
            }
        }
    }

    // Alters inputTypeAdd string, so that only buttons on the selected controller work
    void InputChoice() {
        if (BetweenScenesScript.ControlTypeP1 == 0 && playerNumber == 1) {
            inputNameInsert = " (P1joy)";
        }
        else if (BetweenScenesScript.ControlTypeP1 == 1 && playerNumber == 1) {
            inputNameInsert = " (P1key)";
        }
        else if (BetweenScenesScript.ControlTypeP2 == 0 && playerNumber == 2) {
            inputNameInsert = " (P2joy)";
        }
        else if(BetweenScenesScript.ControlTypeP2 == 1 && playerNumber == 2) {
            inputNameInsert = " (P2key)";
        }
        else {
            Debug.Log("Invalid player/controller configuration.");
        }
    }

    public void PretendShipDoesntExist() {
        sprite.enabled = false;
        colliderEnabled = false;
    }

    // Screen Wrapping
    public void CheckScreenWrap() {
        Vector2 newPosition = transform.position;
        if (transform.position.y > gM.screenTop) { newPosition.y = gM.screenBottom; }
        if (transform.position.y < gM.screenBottom) { newPosition.y = gM.screenTop; }
        if (transform.position.x > gM.screenRight) { newPosition.x = gM.screenLeft; }
        if (transform.position.x < gM.screenLeft) { newPosition.x = gM.screenRight; }
        transform.position = newPosition;
    }
}
