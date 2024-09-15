using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour
{
    public float radiusAttack = 10;
    public float force, UpForce;
    public ParticleSystem ExplosionPaticle;
    float _dmg = 1;
    private void OnCollisionEnter(Collision collision)
    {
            Explosion();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, radiusAttack);
    }
    public void Explosion()
    {
        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.explosion, transform, 1f);
        Vector3 ExplosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(ExplosionPos, radiusAttack);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {
                if (transform.tag == "EnemyBullet" && hit.gameObject.layer == 12)
                {                  
                    GameManager.manager.Dead();
                }
                else if (rb.gameObject.layer == 10)
                {
                    EnemyBase Enemy = rb.GetComponentInParent<EnemyBase>();
                    if (!Enemy.dead)
                        Enemy.Dead(ExplosionPos, 0);
                }
                else if (hit.gameObject.layer == 20 && transform.tag != "EnemyBullet")
                {
                    hit.GetComponentInParent<BossControl>().GetDamage(_dmg);
                }
                else if(hit.gameObject.layer == 21)
                {
                    hit.GetComponentInParent<Pet>().GetDamage(ExplosionPos, 0);
                }

                rb.AddExplosionForce(force, ExplosionPos, radiusAttack, UpForce, ForceMode.Impulse);
            }
        }
     
        ParticleSystem newParticle = Instantiate(ExplosionPaticle);
        newParticle.transform.position = transform.position;
        Destroy(gameObject);
    }

}
