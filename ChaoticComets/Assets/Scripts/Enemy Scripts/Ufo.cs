using UnityEngine;
using static Constants;

/*
 * This class acts as a general template for all enemy types to use. Different enemies have different FixedUpdates.
 */
public abstract partial class Ufo : MonoBehaviour
{
    [Header("Movement / Physics Variables")]
    public Vector2 direction;
    internal Rigidbody2D rb;
    public float alienSpeedBase;
    internal float alienSpeedCurrent;

    [Header("Sound References")]
    [SerializeField] private AudioSource audioAlienHum;
    [SerializeField] private AudioSource audioAlienSfx, audioAlienTeleport;
    [SerializeField] private AudioClip audClipAliexSfxShieldReflect, audClipAlienSfxTakenDamage;

    [Header("Other Variables / References")]
    [SerializeField] private GameObject bullet;
    [SerializeField] private GameObject deathExplosion, playerBulletExplosion, teleportEffect;
    internal bool deathStarted = false, ufoTeleporting = false, ufoRetreating = false;
    [SerializeField] private GameObject forceField;
    [SerializeField] private GameObject playerShip1, playerShip2;

    // ----------

    // All UFO type enemies start by finding players, setting speed, etc
    internal virtual void Start()
    {
        ChangeDifficultyStats();
        rb = FindObjectOfType<Rigidbody2D>();

        alienSpeedCurrent = alienSpeedBase;

        playerShip1 = GameObject.FindGameObjectWithTag(Tag_Player1);
        if (BetweenScenes.PlayerCount == 2)
        {
            playerShip2 = GameObject.FindGameObjectWithTag(Tag_Player2);
        }
        // If alien ship is called during level transition, it destroys itself
        if (GameManager.i.CheckIfEndOfLevel() && !ufoTeleporting) {
            print("UFO attempted to spawn during level transition");
            Destroy(gameObject);
        }

        bulletExpireTime = bulletRange / bulletForce;

    }

    internal virtual void Update()
    {
        if (ShipAbleToShoot())
            ShootBullet();

        // Stabilise 3D model
        transform.rotation = Quaternion.Euler(-50, 0, 0);
    }

    internal virtual void ChangeDifficultyStats()
    {
        if (BetweenScenes.Difficulty == 1)
        {
            bulletForce *= 1.5f;
            alienSpeedBase *= 1.2f;
            bulletRange *= 1.2f;
            shootingDelay *= 0.85f;
            bulletDeviationLimit *= 0.75f;
        }
        else if (BetweenScenes.Difficulty >= 2)
        {
            bulletForce *= 2f;
            alienSpeedBase *= 1.4f;
            bulletRange *= 1.4f;
            shootingDelay *= 0.7f;
            bulletDeviationLimit *= 0.5f;
        }
    }

    public AudioSource[] ReturnAlienSounds()
    {
        AudioSource[] alienSfx = { audioAlienHum, audioAlienSfx, audioAlienTeleport };
        return alienSfx;
    }
}
