using System;
using UnityEngine;

public abstract partial class Ufo : MonoBehaviour
{
    [Header("Defence System Variables")]
    public float alienHealth;
    public float alienMaxHealth;
    private int pointsToScore = 50;
    private const int teleportKillPoints = 500;

    private BulletBehaviour lastTouchedPlrBullet;

    // All UFO type enemies react to player bullets in the same way
    private void OnTriggerEnter2D(Collider2D playerBullet)
    {
        if (playerBullet.gameObject.CompareTag("bullet") || playerBullet.gameObject.CompareTag("bullet2"))
        {
            lastTouchedPlrBullet = playerBullet.GetComponent<BulletBehaviour>();
            HandleDocileTutorialUFOs();

            // If UFO has shields up, don't deal damage. Instead, reflect bullet.
            if (forceField.activeInHierarchy)
            {
                ReflectBullet();
            }
            // If UFO does not have shields up, deal damage and score credits
            else
            {
                DealDamage();
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

    private void DealDamage()
    {
        alienHealth -= 10f;
        lastTouchedPlrBullet.DestroyBullet();

        // If UFO is teleporting & has 10 health or less, then grant more points for an escape kill
        if (ufoTeleporting && alienHealth <= 10f)
        {
            pointsToScore = teleportKillPoints;
            SetToDyingState();
        }

        if (alienHealth >= 0f)
            CreateExplosionGrantPoints();

    }

    /// <summary>
    /// If alien has 0 or less health, it is dying. Can only be set to dying state once.
    /// </summary>
    private void SetToDyingState()
    {
        if (!deathStarted && alienHealth <= 0f)
        {
            float rateOfExplosions = 0.4f;
            float durationOfExplosions = 1.5f;
            deathStarted = true;
            if (ufoTeleporting) { rateOfExplosions = 0.2f; durationOfExplosions = 2.5f; }
            InvokeRepeating(nameof(DeathExplosions), 0.0f, rateOfExplosions);
            Invoke(nameof(DeathRoutine), durationOfExplosions);
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
        CancelInvoke(nameof(DeathExplosions));
        Destroy(gameObject);
    }

    private void ReflectBullet()
    {
        Vector2 force = gameObject.transform.position - lastTouchedPlrBullet.transform.position;
        int magnitude = 1000;
        lastTouchedPlrBullet.gameObject.GetComponent<Rigidbody2D>().AddForce(-force * magnitude);
        lastTouchedPlrBullet.GetComponent<BulletBehaviour>().UfoReflectedBullet();
        audioAlienSfx.clip = audClipAliexSfxShieldReflect;
        audioAlienSfx.Play();
    }

    private void CreateExplosionGrantPoints()
    {
        GameObject newExplosion = Instantiate(playerBulletExplosion, transform.position, transform.rotation);
        Destroy(newExplosion, 2f);
        audioAlienSfx.clip = audClipAlienSfxTakenDamage;
        audioAlienSfx.pitch = 1f;
        audioAlienSfx.Play();
        if (lastTouchedPlrBullet.CompareTag("bullet")) { playerShip1.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
        if (lastTouchedPlrBullet.CompareTag("bullet2")) { playerShip2.GetComponent<PlayerMain>().ScorePoints(pointsToScore); }
    }

    private void HandleDocileTutorialUFOs()
    {
        // If in tutorial mode and marked as docile, do not deal damage.
        if (GameManager.i.tutorialMode && TutorialManager.i.ufoFollowerDocile)
        {
            FlickShieldOn();
            ReflectBullet();
        }
    }
}
