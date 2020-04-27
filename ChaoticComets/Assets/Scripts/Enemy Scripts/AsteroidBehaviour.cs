using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour {

    // General purpose variables
    public bool debugMode = false;
    public GameManager gM;
    public int points;
    private bool beenHit = false;

    // Movement, physics variables
    public float maxThrust, maxSpin;
    public Rigidbody2D rb;
    public int asteroidSize; // 3 = Large, 2 = Medium, 1 = Small

    // GameObject variables
    public GameObject asteroidMedium, asteroidSmall;
    public GameObject playerShip1, playerShip2;
    public GameObject explosion;

    // ----------

    void Start () {
        // Add random spin and thrust at the beginning
        Vector2 thrust = new Vector2(Random.Range(MaxBackThrust(), MaxForwardThrust()),
            Random.Range(MaxBackThrust(), MaxForwardThrust()));
        float spin = Random.Range(-maxSpin, maxSpin);
        rb.AddForce(thrust);
        rb.AddTorque(spin);

        playerShip1 = GameObject.FindWithTag("Player");
        playerShip2 = GameObject.FindWithTag("Player 2");
        gM = GameObject.FindObjectOfType<GameManager>();
	   }

    float MaxBackThrust() { return Random.Range(-maxThrust, -100); }
    float MaxForwardThrust() { return Random.Range(100, maxThrust); }
    
    void Update () {
        if (asteroidSize == -1) {
            float chanceOfExpiring = Random.Range(0f, 100f);
            if (chanceOfExpiring < 0.5f) {
            }
        }
        CheckScreenWrap();
    }

    void OnTriggerEnter2D(Collider2D otherObject) {
        // Check for bullet
        if (otherObject.CompareTag("bullet") || otherObject.CompareTag("bullet2")) {
            // Destroy bullet (delayed to allow for audio)
            otherObject.enabled = false;
            Destroy(otherObject.GetComponentInChildren<ParticleSystem>());
            Destroy(otherObject.gameObject, 5f);
            AsteroidWasHit();
            // Send points to player who shot
            if (otherObject.CompareTag("bullet")) { playerShip1.SendMessage("ScorePoints", points); }
            if (otherObject.CompareTag("bullet2")) { playerShip2.SendMessage("ScorePoints", points); }
        }
    }

    void AsteroidWasHit() {
        if (!beenHit) {
            beenHit = true;
            // Explosion effect
            GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(newExplosion, 2f);
            // Split and destroy asteroid
            float randomiser = Random.Range(0f, 1f);
            if (asteroidSize == 3) {
                if (!debugMode) {
                    Instantiate(asteroidMedium, transform.position, transform.rotation);
                    Instantiate(asteroidMedium, transform.position, transform.rotation);
                    gM.UpdateNumberAsteroids(1);
                    if (randomiser < 0.5f) {
                        Instantiate(asteroidMedium, transform.position, transform.rotation);
                        gM.UpdateNumberAsteroids(1);
                    }
                }
                else { gM.UpdateNumberAsteroids(-1); }
            }
            else if (asteroidSize == 2) {
                Instantiate(asteroidSmall, transform.position, transform.rotation);
                Instantiate(asteroidSmall, transform.position, transform.rotation);
                gM.UpdateNumberAsteroids(1);
                if (randomiser < 0.5f) {
                    Instantiate(asteroidSmall, transform.position, transform.rotation);
                    gM.UpdateNumberAsteroids(1);
                }
            }
            else if (asteroidSize == 1) {
                gM.UpdateNumberAsteroids(-1);
            }
            Destroy(transform.parent.gameObject); // Script is attached to child - destroy parent gameobject
        }
    }

    // Set large asteroids to only take one hit to destroy
    public void DebugMode() {
        debugMode = true;
    }

    // Screen Wrapping
    public void CheckScreenWrap() {
        Vector2 newPosition = transform.position;
        if (transform.position.y > gM.screenTop) { newPosition.y = gM.screenBottom + 0.5f; }
        if (transform.position.y < gM.screenBottom) { newPosition.y = gM.screenTop - 0.5f; }
        if (transform.position.x > gM.screenRight) { newPosition.x = gM.screenLeft + 0.5f; }
        if (transform.position.x < gM.screenLeft) { newPosition.x = gM.screenRight - 0.5f; }
        transform.position = newPosition;
    }
}
