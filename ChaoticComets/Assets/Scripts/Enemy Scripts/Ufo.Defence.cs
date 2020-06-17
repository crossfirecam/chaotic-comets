using UnityEngine;

public abstract partial class Ufo : MonoBehaviour
{
    [Header("Defence System Variables")]
    // private float difficultyIncrease = 0.95f; TODO add this functionality later
    public float alienHealth;
    public float alienMaxHealth;
    private int pointsToScore = 100;
    private readonly int teleportKillPoints = 500;

    // All UFO type enemies react to player bullets in the same way
    private void OnTriggerEnter2D(Collider2D playerBullet)
    {
        if (playerBullet.gameObject.CompareTag("bullet") || playerBullet.gameObject.CompareTag("bullet2"))
        {
            if (gM.tutorialMode && tM.ufoFollowerDocile)
            {
                FlickShieldOn();
                ReflectBullet(playerBullet);
            }

            // If UFO has shields up, don't deal damage. Instead, reflect bullet
            if (forceField.activeInHierarchy)
            {
                ReflectBullet(playerBullet);
            }
            // If UFO is not retreating, deal damage and score credits
            else if (!ufoRetreating)
            {
                alienHealth -= 10f;
                playerBullet.gameObject.GetComponent<BulletBehaviour>().DestroyBullet();

                // If UFO is teleporting & has 10 health or less, then grant more points for an escape kill
                if (ufoTeleporting && alienHealth <= 10f)
                {
                    pointsToScore = teleportKillPoints;
                    DetermineIfDead();
                }

                if (alienHealth >= 0f)
                {
                    DealDamageGrantPoints(playerBullet);
                }
            }
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {

        Vector2 force = gameObject.transform.position - collision.transform.position;
        int magnitude = 0;

        // Asteroid and player collisions do not cause damage to UFO
        if (collision.gameObject.CompareTag("asteroid"))
        {
            magnitude = 200;
            if (alienHealth > 0) { FlickShieldOn(); }
        }
        else if (collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Player 2"))
        {
            magnitude = 100;
            if (alienHealth > 0) { FlickShieldOn(); }
        }
        if (ufoRetreating) { magnitude /= 3; }
        collision.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
    }
    // If alien has 0 or less health, it is dying. Can only be set to dying state once
    private void DetermineIfDead()
    {
        if (!deathStarted && alienHealth <= 0f)
        {
            float rateOfExplosions = 0.4f;
            float durationOfExplosions = 1.5f;
            deathStarted = true;
            if (ufoTeleporting) { rateOfExplosions = 0.2f; durationOfExplosions = 2.5f; }
            InvokeRepeating("DeathExplosions", 0.0f, rateOfExplosions);
            Invoke("DeathRoutine", durationOfExplosions);
        }
    }

    // Repeating invoke that causes explosions
    private void DeathExplosions()
    {
        GameObject newExplosion = Instantiate(deathExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
    }

    // Final instructions for UFO when it's about to die
    private void DeathRoutine()
    {
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
        gM.AlienAndPowerupLogic(GameManager.PropSpawnReason.AlienRespawn);
        Destroy(gameObject);
    }

    private void ReflectBullet(Collider2D playerBullet)
    {
        Vector2 force = gameObject.transform.position - playerBullet.transform.position;
        int magnitude = 1000;
        playerBullet.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
        playerBullet.GetComponent<BulletBehaviour>().UfoReflectedBullet();
        audioAlienSfx.clip = audClipAliexSfxShieldReflect;
        audioAlienSfx.Play();
    }

    private void DealDamageGrantPoints(Collider2D playerBullet)
    {
        GameObject newExplosion = Instantiate(playerBulletExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
        audioAlienSfx.clip = audClipAlienSfxTakenDamage;
        audioAlienSfx.pitch = 1f;
        audioAlienSfx.Play();
        if (playerBullet.CompareTag("bullet")) { playerShip1.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
        if (playerBullet.CompareTag("bullet2")) { playerShip2.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
    }
}
