using UnityEngine;

public class AsteroidBehaviour : MonoBehaviour {

    // General purpose variables
    public bool debugMode = false;
    private GameManager gM;
    public int points;
    private bool beenHit = false;

    // Movement, physics variables
    public float maxThrust, maxSpin;
    private Rigidbody2D rbAsteroid;
    public int asteroidSize; // 3 = Large, 2 = Medium, 1 = Small

    // GameObject variables
    public GameObject asteroidMedium, asteroidSmall;
    public GameObject explosion;
    private GameObject playerShip1, playerShip2;

    // ----------

    void Start ()
    {
        GetComponents();
        GiveRandomMovement();
    }
    private void GiveRandomMovement()
    {
        UsefulFunctions.FindThrust(rbAsteroid, maxThrust);
        UsefulFunctions.FindTorque(rbAsteroid, maxSpin);
    }
    
    void Update ()
    {
        gM.CheckScreenWrap(transform, 0f, 0f, 0.5f, 0.5f);
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
            if (otherObject.CompareTag("bullet")) { playerShip1.GetComponent<PlayerMain>().ScorePoints(points); }
            if (otherObject.CompareTag("bullet2")) { playerShip2.GetComponent<PlayerMain>().ScorePoints(points); }
        }
    }

    public void AsteroidWasHit() {
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

    private void GetComponents()
    {
        rbAsteroid = GetComponent<Rigidbody2D>();
        playerShip1 = GameObject.FindWithTag("Player");
        playerShip2 = GameObject.FindWithTag("Player 2");
        gM = GameObject.FindObjectOfType<GameManager>();
    }
}
