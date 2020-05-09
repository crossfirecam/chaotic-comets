using System.Collections;
using System.Collections.Generic;
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
        gM.CheckScreenWrap(transform, 0f);
    }

    // PlayerMain tells bullet to be destroyed at a certain time
    public void DestroyBullet(float destroyTime) {
        Invoke("StopAnimation", destroyTime - animationStopTime);
    }

    // Animation is stopped a fraction of a second before, allows bullet to fizzle out
    private void StopAnimation() {
        if (ifBulletReflected)
        {
            DestroyBullet(0.5f);
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
            Invoke("RemoveBullet", animationStopTime);
        }
    }

    // Bullet is removed
    private void RemoveBullet() {
        Destroy(gameObject);
    }

    public void UfoReflectedBullet() {
        ifBulletReflected = true;
        Debug.Log("Bullet relected from UFO shield");
    }
}
