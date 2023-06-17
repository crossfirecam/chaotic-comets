using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    [Header("Weapon Systems")]
    public float bulletForce = 700f;
    internal float bulletForceBase = 0f;
    public float rapidFireBetweenBullets = 0.06f;
    private const float BulletRange = 280f, FarShotForceIncrease = 1.4f, FarShotRangeIncrease = 0.75f;
    public float bulletRangeMultipler = 1f;
    internal float bulletDestroyTime;
    internal float bulletTimeIfNormal, bulletTimeIfFar;
    internal float nextFire = 0f, nextFireQuickFire = 0f; // Determines timing of weapon firing
    internal int numOfActiveBullets, capOfActiveBullets = 2;

    [Header("Upgradable Stats")]
    internal float fireRateRapid = 1.2f;
    internal float fireRateRapidWithTriple = 1.6f;
    internal float fireRateTripleHeld = 0.6f, fireRateTripleQuickFire = 0.2f;
    internal float fireRateNormalHeld = 0.4f, fireRateNormalQuickFire = 0.1f;

    [Header("References")]
    public GameObject bullet;
    private Transform mainCannon, tripleCannon1, tripleCannon2;

    private void Start()
    {
        mainCannon = gameObject.transform.Find("MainCannon").transform;
        tripleCannon1 = gameObject.transform.Find("TripleCannon1").transform;
        tripleCannon2 = gameObject.transform.Find("TripleCannon2").transform;
        nextFire = Time.time + 0.2f; nextFireQuickFire = Time.time + 0.2f; // Avoids firing sound when holding down Fire between certain scenes
    }

    internal void SetBulletTime()
    {
        // Save the base bulletForce, because Far Shot can be added or removed later and change it.
        if (bulletForceBase == 0f)
            bulletForceBase = bulletForce;

        if (p.plrPowerups.ifFarShot)
            bulletForce *= FarShotForceIncrease;
        else
            bulletForce = bulletForceBase;

        bulletTimeIfNormal = (BulletRange / bulletForce) * bulletRangeMultipler;
        bulletTimeIfFar = (BulletRange / bulletForce) * (bulletRangeMultipler + FarShotRangeIncrease);

        // Start of level setting of bulletDestroyTime
        if (!p.plrPowerups.ifFarShot)
            bulletDestroyTime = bulletTimeIfNormal;
        else
        {
            bulletDestroyTime = bulletTimeIfFar;
        }
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    /// <summary>
    /// <br>Fire bullets if the on-screen amount are below a cap.</br>
    /// <br>- Normal: fire one bullet, minimal cooldown.</br>
    /// <br>- Triple shot: fire three in a pattern, same cooldown as normal.</br>
    /// <br>- Rapid shot: fire many in rapid succession, long cooldown.</br>
    /// </summary>
    internal void FiringLogic()
    {
        if (numOfActiveBullets < capOfActiveBullets)
        {
            if (p.plrPowerups.ifRapidShot)
            {
                if (p.plrPowerups.ifTripleShot)
                    nextFire = Time.time + fireRateRapidWithTriple;
                else
                    nextFire = Time.time + fireRateRapid;
                nextFireQuickFire = nextFire; // Quick firing does not work with Rapid Shot
                StartCoroutine(RapidShot());
            }
            else if (p.plrPowerups.ifTripleShot)
            {
                nextFire = Time.time + fireRateTripleHeld;
                nextFireQuickFire = Time.time + fireRateTripleQuickFire;
                CreateBullet(mainCannon.position, mainCannon.rotation);
                CreateBullet(tripleCannon1.position, tripleCannon1.rotation, false);
                CreateBullet(tripleCannon2.position, tripleCannon2.rotation, false);
            }
            else
            {
                nextFire = Time.time + fireRateNormalHeld;
                nextFireQuickFire = Time.time + fireRateNormalQuickFire;
                CreateBullet(mainCannon.position, mainCannon.rotation);
            }
        }
    }

    private void CreateBullet(Vector3 position, Quaternion rotation, bool contributesToActiveBulletCount = true)
    {
        GameObject newBullet = Instantiate(bullet, position, rotation, GameManager.i.Refs.effectParent);
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
        newBullet.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
        if (contributesToActiveBulletCount)
        {
            numOfActiveBullets += 1;
            Invoke(nameof(RemoveBulletFromList), bulletDestroyTime);
        }
    }

    private void RemoveBulletFromList()
    {
        numOfActiveBullets -= 1;
    }

    private IEnumerator RapidShot()
    {
        if (p.plrPowerups.ifTripleShot)
        {
            for (int i = 0; i < 4; i++)
            {
                CreateBullet(mainCannon.position, mainCannon.rotation);
                CreateBullet(tripleCannon1.position, tripleCannon1.rotation, false);
                CreateBullet(tripleCannon2.position, tripleCannon2.rotation, false);
                yield return new WaitForSeconds(rapidFireBetweenBullets);
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            {
                CreateBullet(mainCannon.position, mainCannon.rotation);
                yield return new WaitForSeconds(rapidFireBetweenBullets);
            }
        }
    }

}
