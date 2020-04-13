using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapons : MonoBehaviour
{
    [SerializeField] PlayerMain p = default;

    // Weapon Systems
    public GameObject bullet;
    public float bulletForce;
    internal float bulletDestroyTime = 0.8f;
    private GameObject mainCannon, tripleCannon1, tripleCannon2;

    // Upgradable Weapon Stats
    internal float fireRateRapid = .9f;
    internal float fireRateTriple = .6f;
    internal float fireRateNormal = .4f;

    // Determines timing of weapon firing
    internal float nextFire = 0.0F;

    private void Start()
    {
        mainCannon = gameObject.transform.Find("P" + p.playerNumber + "-MainCannon").gameObject;
        tripleCannon1 = gameObject.transform.Find("P" + p.playerNumber + "-TripleCannon1").gameObject;
        tripleCannon2 = gameObject.transform.Find("P" + p.playerNumber + "-TripleCannon2").gameObject;
    }

    // If rapid shot or triple shot, shoot uniquely. If not, shoot typical projectile
    internal void FiringLogic()
    {
        if (p.playerPowerups.ifRapidShot)
        {
            nextFire = Time.time + fireRateRapid;
            StartCoroutine(RapidShot());
        }
        else if (p.playerPowerups.ifTripleShot)
        {
            nextFire = Time.time + fireRateTriple;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.SendMessage("DestroyBullet", bulletDestroyTime);
            GameObject newBullet2 = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
            newBullet2.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet2.SendMessage("DestroyBullet", bulletDestroyTime);
            GameObject newBullet3 = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
            newBullet3.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet3.SendMessage("DestroyBullet", bulletDestroyTime);
        }
        else
        {
            nextFire = Time.time + fireRateNormal;
            GameObject newBullet = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
            newBullet.GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
            newBullet.SendMessage("DestroyBullet", bulletDestroyTime);
        }
    }

    private IEnumerator RapidShot()
    {
        GameObject[] rapidShotArray = new GameObject[10];
        GameObject[] rapidShotArray2 = new GameObject[10];
        GameObject[] rapidShotArray3 = new GameObject[10];
        if (p.playerPowerups.ifTripleShot)
        {
            for (int i = 0; i < 2; i++)
            {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].SendMessage("DestroyBullet", bulletDestroyTime);

                rapidShotArray2[i] = Instantiate(bullet, tripleCannon1.transform.position, tripleCannon1.transform.rotation);
                rapidShotArray2[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray2[i].SendMessage("DestroyBullet", bulletDestroyTime);

                rapidShotArray3[i] = Instantiate(bullet, tripleCannon2.transform.position, tripleCannon2.transform.rotation);
                rapidShotArray3[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray3[i].SendMessage("DestroyBullet", bulletDestroyTime);
                yield return new WaitForSeconds(0.08f);
            }
        }
        else
        {
            for (int i = 0; i < 4; i++)
            {
                rapidShotArray[i] = Instantiate(bullet, mainCannon.transform.position, mainCannon.transform.rotation);
                rapidShotArray[i].GetComponent<Rigidbody2D>().AddRelativeForce(Vector2.up * bulletForce);
                rapidShotArray[i].SendMessage("DestroyBullet", bulletDestroyTime);
                yield return new WaitForSeconds(0.08f);
            }
        }
    }
}
