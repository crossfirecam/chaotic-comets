using System.Collections;
using UnityEngine;

public class CanisterBehaviour : MonoBehaviour {
    
    // General purpose variables
    private Renderer rend;
    public GameObject explosion, expirationPop;
    private float timeUntilWarning;
    private readonly float timeBetweenTicks = 0.4f;

    // Movement, physics variables
    private readonly float maxThrust = 250, maxSpin = 60;
    private Rigidbody2D rbCanister;

    // Audio
    public AudioSource audioPowerupExpire;
    public AudioClip expireSound, appearSound;

    // ----------

    // Set object variables, determine random expiry time, and give random movement. If spawned at end of level, destroy canister
    void Start ()
    {
        rbCanister = GetComponent<Rigidbody2D>();
        rend = GetComponentInChildren<Renderer>();
        GiveRandomMovement();

        timeUntilWarning = Random.Range(8f, 12f);
        Invoke(nameof(StartExpiry), timeUntilWarning);

        if (GameManager.i.CheckIfEndOfLevel()) { print("Canister attempted to spawn during level transition"); Destroy(gameObject); }
    }

    // Every frame, check if canister needs to loop screen
    void Update() {
        GameManager.i.CheckScreenWrap(transform, 0f, 0f, 0.5f, 0.5f);
    }

    /// <summary>
    /// Destroy canister when shot by player bullet.
    /// </summary>
    /// <param name="triggerObject"></param>
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.CompareTag("bullet") || triggerObject.gameObject.CompareTag("bullet2")) {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;

            // Disable trigger bullet and destroy it.
            triggerObject.enabled = false;
            Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
            Destroy(triggerObject.gameObject, 5f);

            // Create a red explosion.
            GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(newExplosion, 2f);

            RespawnIfTutorialMode();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Play a sound and give random movement to newly spawned canisters.
    /// </summary>
    public void GiveRandomMovement() {
        audioPowerupExpire.clip = appearSound;
        audioPowerupExpire.Play();

        UsefulFunctions.FindThrust(rbCanister, maxThrust);
        UsefulFunctions.FindTorque(rbCanister, maxSpin);
    }

    // Called from Start(). Sets off an expiry animation after a random time
    void StartExpiry() {
        StartCoroutine(nameof(PowerupExpiry));
    }
    IEnumerator PowerupExpiry() {
        audioPowerupExpire.clip = expireSound;
        audioPowerupExpire.pitch = 0.95f;
        for (int tick = 0; tick <= 6; tick++) {
            if (tick % 2 == 0)
            {
                rend.material.SetColor("_Color", Color.white);
            }
            else
            {
                audioPowerupExpire.Play();
                rend.material.SetColor("_Color", Color.yellow);
            }
            yield return new WaitForSeconds(timeBetweenTicks);
        }
        // Play canister expire effect, destroy canister.
        GameObject newPop = Instantiate(expirationPop, transform.position, transform.rotation);
        Destroy(newPop, 2f);

        RespawnIfTutorialMode();
        Destroy(gameObject);
    }

    private void RespawnIfTutorialMode()
    {
        if (GameManager.i.tutorialMode)
            GameManager.i.RespawnPropForTutorial("canister");
    }
}
