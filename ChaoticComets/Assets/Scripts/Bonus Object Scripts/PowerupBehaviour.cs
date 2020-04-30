using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerupBehaviour : MonoBehaviour {
    
    // General purpose variables
    private GameManager gM;
    private Renderer rend;
    private GameObject player1, player2;
    public GameObject explosion;

    // Movement, physics variables
    public float maxThrust, maxSpin;
    private Rigidbody2D rbCanister;

    // Audio
    public AudioSource audioPowerupExpire;
    public AudioClip expireSound, appearSound;

    // ----------

    // Set object variables, determine random expiry time, and give random movement. If spawned at end of level, destroy canister
    void Start () {
        gM = GameObject.FindObjectOfType<GameManager>();
        rbCanister = gameObject.GetComponent<Rigidbody2D>();
        if (!gM.player1dead) { player1 = GameObject.FindGameObjectWithTag("Player"); }
        if (!gM.player2dead) { player2 = GameObject.FindGameObjectWithTag("Player 2"); }
        rend = GetComponent<Renderer>();
        float timeUntilExpiry = Random.Range(10f, 24f);
        Invoke("StartExpiry", timeUntilExpiry);
        GiveRandomMovement();
        if (gM.CheckIfEndOfLevel()) { Debug.Log("Canister attempted to spawn during level transition"); Destroy(gameObject); }
    }

    // Every frame, check if canister needs to loop screen
    void Update() {
        CheckScreenWrap();
    }

    // Destroy canister when shot by players
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.tag == "bullet" || triggerObject.gameObject.tag == "bullet2") {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.CanisterRespawn);
            triggerObject.enabled = false;
            Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
            Destroy(triggerObject.gameObject, 5f);
            GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(newExplosion, 2f);
            Destroy(gameObject);
        }
    }

    // Play a sound and give random movement to newly spawned canisters
    public void GiveRandomMovement() {
        audioPowerupExpire.clip = appearSound;
        audioPowerupExpire.Play();
        // Add random spin and thrust
        Vector2 thrust = new Vector2(Random.Range(MaxBackThrust(), MaxForwardThrust()),
            Random.Range(MaxBackThrust(), MaxForwardThrust()));
        float spin = Random.Range(-maxSpin, maxSpin);
        rbCanister.AddForce(thrust);
        rbCanister.AddTorque(spin);
    }
    float MaxBackThrust() { return Random.Range(-maxThrust, -200); }
    float MaxForwardThrust() { return Random.Range(200, maxThrust); }
    
    // Called from Start(). Sets off an expiry animation after a random time
    void StartExpiry() {
        StartCoroutine("PowerupExpiry");
    }
    IEnumerator PowerupExpiry() {
        audioPowerupExpire.clip = expireSound;
        for (float tick = 0f; tick <= 8f; tick++) {
            if (tick % 2f == 0) { rend.material.SetColor("_Color", Color.yellow); }
            else { rend.material.SetColor("_Color", Color.white);
                audioPowerupExpire.Play();
            }
            yield return new WaitForSeconds(0.8f);
        }
        gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.CanisterRespawn);
        Destroy(gameObject);
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
