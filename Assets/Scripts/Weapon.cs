using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    public int id;
    private Transform GunPos, fix;
    private float speed = 6, rotationSpeed = 5;
    public int ammo;
    public GameObject weapon;
    public bool Equiped;
    public bool OnPos, OnRot;
    public ParticleSystem particle;


    void Start()
    {
        GunPos = GameManager.player.gunpos;
        ammo = GameManager.dataBaseItems.FechItem(id).Ammunition;
    }
    void Update()
    {
        if (Equiped && gameObject.layer == 17 && !GameManager.manager.pause)
        {



            SetPos();

            SetRotate();

        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (!Equiped)
        {
            if (collision.gameObject.layer == 12 && gameObject.layer == 16)
            {
                gameObject.layer = 17;
                for (int i = 0; i < transform.childCount; i++)
                {
                    transform.GetChild(i).gameObject.layer = 17;
                }
                GetComponent<Outline>().enabled = false;
                transform.parent = GunPos;
                GameManager.weapons.ChangeWeapons(gameObject);
            }

        }
        if (transform.parent == null && gameObject.layer != 16 && collision.gameObject.layer != 12)
        {
            if (collision.gameObject.layer == 10 || collision.gameObject.layer == 14)
                collision.gameObject.GetComponentInParent<EnemyBase>().Dead(transform.forward, 15);
            int random = Random.Range(0, GameManager.manager.enemyBaseHitSound.Count);
            if (collision.gameObject.layer == 21)
                collision.gameObject.GetComponentInParent<Pet>().GetDamage(transform.forward, 15);

            if (collision.gameObject.layer == 20)
                collision.gameObject.GetComponentInParent<BossControl>().GetDamage(7);

            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.enemyBaseHitSound[random], collision.transform, 1f);
            gameObject.layer = 16;
            for (int i = 0; i < transform.childCount; i++)
            {
                transform.GetChild(i).gameObject.layer = 16;
            }
            Instantiate(particle, transform.position, Quaternion.identity);
        }
    }
    void SetPos()
    {
        transform.position = Vector3.MoveTowards(transform.position, GunPos.position, speed * Time.deltaTime);
    }
    void SetRotate()
    {

        transform.rotation = Quaternion.Lerp(transform.rotation, GunPos.rotation, Time.deltaTime * rotationSpeed);
    }
    public void Unequip()
    {
        Equiped = false;
        OnRot = false;
        OnPos = false;
        GetComponent<Outline>().enabled = true;
    }
}
