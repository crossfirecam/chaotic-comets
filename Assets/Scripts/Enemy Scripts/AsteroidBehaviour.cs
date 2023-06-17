using UnityEngine;
using static Constants;

public class AsteroidBehaviour : MonoBehaviour {

    [Header("General purpose variables")]
    public int points;
    public bool debugMode = false;
    private bool beenHit = false;

    [Header("Movement, physics variables")]
    public int asteroidSize; // 3 = Large, 2 = Medium, 1 = Small
    [SerializeField] private float maxThrust, maxSpin;
    private Rigidbody2D rbAsteroid;

    [Header("GameObject variables")]
    [SerializeField] private GameObject explosion;
    [SerializeField] private GameObject asteroidMedium, asteroidSmall;
    private GameObject playerShip1, playerShip2;

    // ----------

    void Start ()
    {
        if (BetweenScenes.Difficulty == 1)
            maxThrust *= 1.5f;
        else if (BetweenScenes.Difficulty == 2)
            maxThrust *= 2f;

        GetComponents();
        UsefulFunctions.FindThrust(rbAsteroid, maxThrust);
        UsefulFunctions.FindTorque(rbAsteroid, maxSpin);

        RandomiseColourIfRainbowMode();
        CheckDebugMode();
    }
    
    void Update ()
    {
        GameManager.i.CheckScreenWrap(transform, 0f, 0f, 0.5f, 0.5f);
    }

    void OnTriggerEnter2D(Collider2D otherObject) {
        if (otherObject.CompareTag(Tag_BulletP1) || otherObject.CompareTag(Tag_BulletP2))
        {
            if (!otherObject.GetComponent<BulletBehaviour>().ifBulletAlreadyDealtDamage)
            {
                otherObject.GetComponent<BulletBehaviour>().DestroyBullet();
                AsteroidWasHit();

                // Send points to player who shot
                if (otherObject.CompareTag(Tag_BulletP1)) { playerShip1.GetComponent<PlayerMain>().ScorePoints(points); }
                if (otherObject.CompareTag(Tag_BulletP2)) { playerShip2.GetComponent<PlayerMain>().ScorePoints(points); }
            }
        }
    }
    
    /// <summary>
    /// Asteroid has been touched by a player bullet.<br/>
    /// - Play an explosion effect<br/>
    /// - Split asteroid if it's a Large or Medium, into either 2 or 3 pieces<br/>
    /// - Remove the original asteroid<br/>
    /// </summary>
    public void AsteroidWasHit() {
        if (!beenHit) {
            beenHit = true;
            // Explosion effect
            GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation, GameManager.i.Refs.effectParent);
            Destroy(newExplosion, 2f);

            // Large and Medium asteroids split into 2-3 pieces
            if (!debugMode && asteroidSize != 1)
            {
                GameObject asteroidType = asteroidMedium;
                if (asteroidSize == 2) { asteroidType = asteroidSmall; }

                int asteroidAmount = Random.Range(2, 4);
                for (int i = 0; i < asteroidAmount; i++)
                {
                    Instantiate(asteroidType, transform.position, transform.rotation, GameManager.i.Refs.propParent);
                }
                GameManager.i.UpdateNumberAsteroids(asteroidAmount - 1);
            }
            // Small asteroids and ones with DebugMode enabled simply remove 1 from asteroid count
            else
            {
                GameManager.i.UpdateNumberAsteroids(-1);
            }
            // After being hit, original asteroid disappears. Destroy parent gameobject
            Destroy(transform.parent.gameObject);
        }
    }

    /// <summary>
    /// Asteroids will be destroyed in one hit if GameManager has a setting enabled.
    /// </summary>
    public void CheckDebugMode()
    {
        if (GameManager.i.instantkillAsteroids)
            debugMode = true;
    }

    private void GetComponents()
    {
        rbAsteroid = GetComponent<Rigidbody2D>();
        playerShip1 = GameObject.FindWithTag(Tag_Player1);
        playerShip2 = GameObject.FindWithTag(Tag_Player2);
    }

    private void RandomiseColourIfRainbowMode()
    {
        if (PlayerPrefs.GetInt("RainbowMode") == 1)
        {
            Color randColor = Random.ColorHSV(0f, 1f, 1f, 1f, 0.8f, 1f);
            GetComponent<Renderer>().material.SetColor("_Color", randColor);
        }
    }
}
