using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class Bullet : MonoBehaviour
{
    public ParticleSystem particle;
    private float _dmg = 5;
    // Update is called once per frame
    void Update()
    {
        Destroy(gameObject, 5);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.tag != "Player" && collision.transform.tag != "Bullet")
        {
            Instantiate(particle, transform.position, Quaternion.identity);
            if (collision.gameObject.layer == 10 || collision.gameObject.layer == 14 || collision.gameObject.layer == 19)
            {
                int random = Random.Range(0, GameManager.manager.enemyBaseHitSound.Count);

                GameManager.manager.PlaySoundWithPrefab(GameManager.manager.enemyBaseHitSound[random], collision.transform, 1f);
                EnemyBase Enemy = collision.collider.GetComponentInParent<EnemyBase>();
                if (!Enemy.dead)
                    Enemy.Dead(transform.forward, 10);

            }
            if (collision.gameObject.layer == 20)
            {
                if (!collision.gameObject.GetComponentInParent<BossControl>().dead)
                    collision.gameObject.GetComponentInParent<BossControl>().GetDamage(_dmg);
            }

            if (collision.gameObject.layer == 21)
                collision.gameObject.GetComponentInParent<Pet>().GetDamage(transform.forward, 20);

        }
        if (transform.tag == "EnemyBullet" && collision.transform.tag == "Player")
        {
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 30, ForceMode.Impulse);
            collision.gameObject.GetComponent<Rigidbody>().AddForce(transform.up * 10, ForceMode.Impulse);
            GameManager.manager.Dead();
        }
        //Fix force on Shotgun
        if (GameManager.weapons.CurrentWeapon != null)
        {
            Rigidbody rb = collision.gameObject.GetComponent<Rigidbody>();
            if(rb != null)
            {
                if (GameManager.weapons.CurrentWeapon.GetComponent<Weapon>().id != 3)
                    rb.AddForce(transform.forward * 60f, ForceMode.Impulse);
                else
                    rb.AddForce(transform.forward * 20f, ForceMode.Impulse);
            }

        }

        Destroy(gameObject);
    }
}
