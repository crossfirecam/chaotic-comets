using UnityEngine;

public class BulletBehaviour : MonoBehaviour {

    // General purpose variables
    public GameManager gM;
    public bool ifBulletNotAlien;
    private readonly float animationStopTime = .2f;
    public bool ifBulletReflected;

    // ----------
    
    void Update ()
    {
        gM.CheckScreenWrap(transform);
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
            if (gameObject.GetComponentInChildren<ParticleSystem>() != null)
            {
                if (gameObject.GetComponentInChildren<ParticleSystem>().isPlaying)
                {
                    gameObject.GetComponentInChildren<ParticleSystem>().Stop();
                }
            }
            Invoke(nameof(RemoveBullet), animationStopTime);
        }
    }

    // Bullet is called to be instantly removed, delay in actual removal so sound can play
    public void DestroyBullet()
    {
        Destroy(gameObject.GetComponentInChildren<ParticleSystem>());
        gameObject.GetComponent<Collider2D>().enabled = false;
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
