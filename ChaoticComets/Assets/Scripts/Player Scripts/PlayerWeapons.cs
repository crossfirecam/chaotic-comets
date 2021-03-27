using System.Collections;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    [Header("Weapon Systems")]
    public float bulletForce = 400f;
    public float rapidFireBetweenBullets = 0.06f;
    private const float bulletRange = 320f;
    public float bulletRangeMultipler = 1f;
    internal float bulletDestroyTime;
    internal float bulletTimeIfNormal, bulletTimeIfFar;
    internal float nextFire = 0.0f, nextFireQuickFire = 0.0f; // Determines timing of weapon firing

    [Header("Upgradable Stats")]
    public float fireRateRapid = 1.4f;
    public float fireRateRapidWithTriple = 2f;
    public float fireRateTripleHeld = 0.7f, fireRateTripleQuickFire = 0.2f;
    public float fireRateNormalHeld = 0.4f, fireRateNormalQuickFire = 0.1f;

    [Header("References")]
    public GameObject bullet;
    private Transform mainCannon, tripleCannon1, tripleCannon2;

    private void Start()
    {
        mainCannon = gameObject.transform.Find("MainCannon").transform;
        tripleCannon1 = gameObject.transform.Find("TripleCannon1").transform;
        tripleCannon2 = gameObject.transform.Find("TripleCannon2").transform;
        Invoke(nameof(SetBulletTime), 0.05f);
    }

    private void SetBulletTime()
    {
        bulletTimeIfNormal = (bulletRange / bulletForce) * bulletRangeMultipler;
        bulletTimeIfFar = bulletTimeIfNormal * 1.75f;
        bulletDestroyTime = bulletTimeIfNormal;
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    internal void FiringLogic()
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
            CreateBullet(tripleCannon1.position, tripleCannon1.rotation);
            CreateBullet(tripleCannon2.position, tripleCannon2.rotation);
        }
        else
        {
            nextFire = Time.time + fireRateNormalHeld;
            nextFireQuickFire = Time.time + fireRateNormalQuickFire;
            CreateBullet(mainCannon.position, mainCannon.rotation);
        }
    }

    internal void FireSingleShot()
    {

    }
    private void CreateBullet(Vector3 position, Quaternion rotation)
    {
        GameObject newBullet = Instantiate(bullet, position, rotation);
        newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
        newBullet.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
    }

    private IEnumerator RapidShot()
    {
        if (p.plrPowerups.ifTripleShot)
        {
            for (int i = 0; i < 3; i++)
            {
                CreateBullet(mainCannon.position, mainCannon.rotation);
                CreateBullet(tripleCannon1.position, tripleCannon1.rotation);
                CreateBullet(tripleCannon2.position, tripleCannon2.rotation);
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
