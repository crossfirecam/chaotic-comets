using UnityEngine;
using static Constants;

public class BulletBehaviour : MonoBehaviour {

    // General purpose variables
    private readonly float animationStopTime = .2f;
    public bool ifBulletReflected, ifBulletAlreadyDealtDamage = false;

    // ----------
    
    void Update ()
    {
        GameManager.i.CheckScreenWrap(transform);
    }

    // PlayerMain tells bullet to be destroyed at a certain time
    public void FizzleOutBullet(float destroyTime) {
        Invoke(nameof(StopAnimation), destroyTime - animationStopTime);
    }
    private void StopAnimation() {
        if (ifBulletReflected)
        {
            FizzleOutBullet(0.5f);
            ifBulletReflected = false;
        }
        else
        {
            if (GetComponentInChildren<ParticleSystem>() != null)
            {
                if (GetComponentInChildren<ParticleSystem>().isPlaying)
                {
                    GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
            Invoke(nameof(RemoveBullet), animationStopTime);
        }
    }

    // Bullet is called to be instantly removed, delay in actual removal so sound can play
    public void DestroyBullet()
    {
        ifBulletAlreadyDealtDamage = true;
        GetComponent<Collider2D>().enabled = false;
        Destroy(GetComponentInChildren<ParticleSystem>());
        Invoke(nameof(RemoveBullet), 2f);

    }

    // Bullet is removed
    private void RemoveBullet()
    {
        Destroy(gameObject);
    }

    public void UfoReflectedBullet() {
        ifBulletReflected = true;
        print("Bullet relected from UFO shield");
    }
}
