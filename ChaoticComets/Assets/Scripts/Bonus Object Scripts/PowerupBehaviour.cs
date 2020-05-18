using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PowerupBehaviour : MonoBehaviour {
    
    // General purpose variables
    public GameManager gM;
    private Renderer rend;
    public GameObject explosion, expirationPop;
    private float timeUntilWarning;
    private readonly float timeToDisappear = 4f;
    private readonly float timeBetweenTicks = 0.4f;

    // Movement, physics variables
    private float maxThrust = 250, maxSpin = 60;
    private Rigidbody2D rbCanister;

    // Audio
    public AudioSource audioPowerupExpire;
    public AudioClip expireSound, appearSound;

    // ----------

    // Set object variables, determine random expiry time, and give random movement. If spawned at end of level, destroy canister
    void Start ()
    {
        gM = FindObjectOfType<GameManager>();
        rbCanister = GetComponent<Rigidbody2D>();
        rend = GetComponent<Renderer>();
        GiveRandomMovement();

        timeUntilWarning = Random.Range(8f, 12f);
        Invoke("StartExpiry", timeUntilWarning);

        if (gM.CheckIfEndOfLevel()) { Debug.Log("Canister attempted to spawn during level transition"); Destroy(gameObject); }
    }

    // Every frame, check if canister needs to loop screen
    void Update() {
        gM.CheckScreenWrap(transform, 0.5f);
    }

    // Destroy canister when shot by players
    void OnTriggerEnter2D(Collider2D triggerObject) {
        if (triggerObject.gameObject.CompareTag("bullet") || triggerObject.gameObject.CompareTag("bullet2")) {
            gameObject.GetComponent<CircleCollider2D>().enabled = false;
            gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.CanisterRespawn);
            triggerObject.enabled = false;
            Destroy(triggerObject.GetComponentInChildren<ParticleSystem>());
            Destroy(triggerObject.gameObject, 5f);
            GameObject newExplosion = Instantiate(explosion, transform.position, transform.rotation);
            Destroy(newExplosion, 2f);
            Destroy(transform.parent.gameObject);
        }
    }

    // Play a sound and give random movement to newly spawned canisters
    public void GiveRandomMovement() {
        audioPowerupExpire.clip = appearSound;
        audioPowerupExpire.Play();

        UsefulFunctions.FindThrust(rbCanister, maxThrust);
        UsefulFunctions.FindTorque(rbCanister, maxSpin);
    }

    // Called from Start(). Sets off an expiry animation after a random time
    void StartExpiry() {
        StartCoroutine("PowerupExpiry");
    }
    IEnumerator PowerupExpiry() {
        audioPowerupExpire.clip = expireSound;
        audioPowerupExpire.pitch = 0.95f;
        for (int tick = 0; tick <= 10; tick++) {
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
        GameObject newPop = Instantiate(expirationPop, transform.position, transform.rotation);
        Destroy(newPop, 2f);
        gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.CanisterRespawn);
        Destroy(transform.parent.gameObject);
    }
}
