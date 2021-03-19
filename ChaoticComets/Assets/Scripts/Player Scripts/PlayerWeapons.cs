using System.Collections;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    [Header("Weapon Systems")]
    public float bulletForce = 400f;
    public float rapidFireBetweenBullets = 0.1f;
    internal float bulletRange = 320f;
    internal float bulletDestroyTime;
    internal static float bulletTimeIfNormal, bulletTimeIfFar;
    internal float nextFire = 0.0f; // Determines timing of weapon firing

    [Header("Upgradable Stats")]
    public float fireRateRapid = 1.0f;
    public float fireRateTriple = 0.7f;
    public float fireRateNormal = 0.45f;

    [Header("References")]
    public GameObject bullet;
    private GameObject mainCannon, tripleCannon1, tripleCannon2;

    private void Start()
    {
        mainCannon = gameObject.transform.Find("MainCannon").gameObject;
        tripleCannon1 = gameObject.transform.Find("TripleCannon1").gameObject;
        tripleCannon2 = gameObject.transform.Find("TripleCannon2").gameObject;
        bulletTimeIfNormal = bulletRange / bulletForce;
        bulletTimeIfFar = bulletTimeIfNormal * 1.75f;
        bulletDestroyTime = bulletTimeIfNormal;
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    internal void FiringLogic()
    {
        if (p.plrPowerups.ifRapidShot)
        {
            nextFire = Time.time + fireRateRapid;
            StartCoroutine(RapidShot());
        }
        else if (p.plrPowerups.ifTripleShot)
        {
            nextFire = Time.time + fireRateTriple;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
            GameObject newBullet2 = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
            newBullet2.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet2.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
            GameObject newBullet3 = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
            newBullet3.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet3.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
        }
        else
        {
            nextFire = Time.time + fireRateNormal;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
        }
    }

    private IEnumerator RapidShot()
    {
        GameObject[] rapidShotArray = new GameObject[10];
        GameObject[] rapidShotArray2 = new GameObject[10];
        GameObject[] rapidShotArray3 = new GameObject[10];
        if (p.plrPowerups.ifTripleShot)
        {
            for (int i = 0; i < 2; i++)
            {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);

                rapidShotArray2[i] = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
                rapidShotArray2[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray2[i].GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);

                rapidShotArray3[i] = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
                rapidShotArray3[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray3[i].GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
                yield return new WaitForSeconds(rapidFireBetweenBullets);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].GetComponent<BulletBehaviour>().FizzleOutBullet(bulletDestroyTime);
                yield return new WaitForSeconds(rapidFireBetweenBullets);
            }
        }
    }
}
