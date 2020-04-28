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
    
        void Update () {
        Vector2 newPosition = transform.position;
        if (transform.position.y > gM.screenTop) {
            newPosition.y = gM.screenBottom;
        }
        if (transform.position.y < gM.screenBottom) {
            newPosition.y = gM.screenTop;
        }
        if (transform.position.x > gM.screenRight) {
            newPosition.x = gM.screenLeft;
        }
        if (transform.position.x < gM.screenLeft) {
            newPosition.x = gM.screenRight;
        }
        transform.position = newPosition;
    }

    // PlayerMain tells bullet to be destroyed at a certain time
    public void DestroyBullet(float destroyTime) {
        if (ifBulletReflected) {
            DestroyBullet(0.5f);
            ifBulletReflected = false;
        }
        else {
            Invoke("StopAnimation", destroyTime - animationStopTime);
        }
    }

    // Animation is stopped a fraction of a second before, allows bullet to fizzle out
    private void StopAnimation() {
        if (gameObject.GetComponentInChildren<ParticleSystem>() != null) {
            if (gameObject.GetComponentInChildren<ParticleSystem>().isPlaying) {
                gameObject.GetComponentInChildren<ParticleSystem>().Stop();
            }
        }
        Invoke("RemoveBullet", animationStopTime);
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
