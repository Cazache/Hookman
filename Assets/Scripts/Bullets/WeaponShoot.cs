using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponShoot : MonoBehaviour
{

    public GameObject CurrentWeapon;
    public Transform shootPoint, AimPoint;
    public GameObject Bullet, bomb;
    public GameObject player;
    public float cdShoot;
    public bool ReadyToShoot;
    public ParticleSystem ShootParticle;
    public GameObject Mycamera, katana;


    // Start is called before the first frame update
    void Start()
    {
        ReadyToShoot = true;
    }

    private void FixedUpdate()
    {
        if (shootPoint != null)
            AimSystem();

        if (Input.GetMouseButton(0) && CurrentWeapon != null)
        {
            Shoot(CurrentWeapon.GetComponent<Weapon>().id);
            GameManager.manager.UpdateAmmoText();
        }
        else if (Input.GetMouseButton(0) && GameManager.habilities.readyToAttack && GameManager.katana.unlocked)
        {
            GameManager.habilities.MeeleAttack();
        }
    }
    public void Shoot(int Weapon)
    {
        if (ReadyToShoot && CurrentWeapon.GetComponent<Weapon>().ammo > 0)
        {
            GameManager.player.anim.SetBool("Shooting", true);
            StartCoroutine(GameManager.player.SetAnim("Shooting", false, 0.1f));
            Vector3 PaticleScale = new Vector3();
            if (Weapon == 0)
            {

                GameObject newBullet = Instantiate(Bullet, shootPoint.transform.position, Quaternion.identity);
                newBullet.transform.forward = shootPoint.transform.forward;
                Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                rb.AddForce(shootPoint.forward * 20, ForceMode.Impulse);
                PaticleScale = new Vector3(0.8f, 0.8f, 0.8f);
                StartCoroutine(GameManager.manager.CameraShake(.15f, .05f));
            }
            else if (Weapon == 1)
            {
                //Rifle
                GameObject newBullet = Instantiate(Bullet, shootPoint.transform.position, Quaternion.identity);
                newBullet.transform.forward = shootPoint.transform.forward;
                Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                rb.AddForce(shootPoint.forward * 20, ForceMode.Impulse);
                PaticleScale = new Vector3(0.8f, 0.8f, 0.8f);
                StartCoroutine(GameManager.manager.CameraShake(.01f, .01f));
            }
            else if (Weapon == 2)
            {
                //Granade
                GameObject Bomb = Instantiate(bomb, shootPoint.position, Quaternion.identity);
                Rigidbody rb = Bomb.GetComponent<Rigidbody>();
                PaticleScale = new Vector3(1.4f, 1.4f, 1.4f);
                rb.AddForce(shootPoint.forward * 15, ForceMode.Impulse);
                rb.AddForce(shootPoint.up * 6, ForceMode.Impulse);
                StartCoroutine(GameManager.manager.CameraShake(.15f, .05f));

            }
            else if (Weapon == 3)
            {
                //Shotgun
                for (int i = 0; i < 8; i++)
                {
                    var RandomDir = shootPoint.transform.rotation;
                    RandomDir.x += Random.Range(-0.1f, 0.1f);
                    RandomDir.y += Random.Range(-0.1f, 0.1f);
                    GameObject newBullet = Instantiate(Bullet, shootPoint.transform.position, RandomDir);

                    Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                    rb.AddForce(newBullet.transform.forward * 20, ForceMode.Impulse);
                }
                PaticleScale = new Vector3(1.2f, 1.2f, 1.2f);
                player.GetComponent<Rigidbody>().AddForce(-shootPoint.transform.forward * 20, ForceMode.Impulse);
                StartCoroutine(GameManager.manager.CameraShake(0.15f, 0.04f));



            }
            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.Shootsounds[Weapon], shootPoint.transform, 0.8f);
            CurrentWeapon.GetComponent<Weapon>().ammo--;
            ReadyToShoot = false;
            ParticleSystem newParticle = Instantiate(ShootParticle, shootPoint.transform.position, shootPoint.transform.rotation);
            newParticle.transform.localScale = PaticleScale;

            Invoke("ResetShoot", cdShoot);
        }
        else if (CurrentWeapon.GetComponent<Weapon>().ammo <= 0 && ReadyToShoot)
        {
            GameManager.player.anim.SetBool("Droping", true);
            StartCoroutine(GameManager.player.SetAnim("Droping", false, 0.3f));
            Invoke("LauchWeapon", 0.2f);

            katana.GetComponent<Katana>().Equip();
        }
    }
    public void ChangeWeapons(GameObject NewWeapon)
    {
        if (CurrentWeapon != null)
            DropWeapon(5, 6, 16);


        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.grabWeaponShoot, transform, 1f);
        NewWeapon.GetComponent<Weapon>().Equiped = true;
        NewWeapon.GetComponent<Rigidbody>().isKinematic = true;
        NewWeapon.GetComponent<Weapon>().weapon.GetComponent<Collider>().enabled = false;
        CurrentWeapon = NewWeapon;
        shootPoint = CurrentWeapon.transform.GetChild(0);
        cdShoot = GameManager.dataBaseItems.FechItem(NewWeapon.GetComponent<Weapon>().id).Cd;
        GameManager.manager.UpdateAmmoText();
        //Anims
        int id = CurrentWeapon.GetComponent<Weapon>().id;
        if (id == 0)
        {
            GameManager.player.anim.SetBool("PistolEquiped", true);
        }
        else if (id == 1)
        {
            GameManager.player.anim.SetBool("RifleEquiped", true);
        }
        else if (id == 2)
        {
            GameManager.player.anim.SetBool("GranadeLauncher", true);
        }
        else if (id == 3)
        {
            GameManager.player.anim.SetBool("ShotGunEquiped", true);
        }
        if (GameManager.habilities.cdReduction)
            cdShoot = cdShoot / 2;
        if (katana.GetComponent<Katana>().equiped)
            katana.GetComponent<Katana>().UnEquip();
    }
    public void DropWeapon(float forceU, float forceF, int layer)
    {
        int id = CurrentWeapon.GetComponent<Weapon>().id;
        if (id == 0)
        {
            GameManager.player.anim.SetBool("PistolEquiped", false);
        }
        else if (id == 1)
        {
            GameManager.player.anim.SetBool("RifleEquiped", false);
        }
        else if (id == 2)
        {
            GameManager.player.anim.SetBool("GranadeLauncher", false);
        }
        else if (id == 3)
        {
            GameManager.player.anim.SetBool("ShotGunEquiped", false);
        }


        CurrentWeapon.GetComponent<Weapon>().weapon.GetComponent<Collider>().enabled = true;
        CurrentWeapon.GetComponent<Weapon>().Invoke("Unequip", 0.5f);
        CurrentWeapon.GetComponent<Rigidbody>().isKinematic = false;
        CurrentWeapon.gameObject.layer = layer;
        for (int i = 0; i < CurrentWeapon.transform.childCount; i++)
        {
            CurrentWeapon.transform.GetChild(i).gameObject.layer = layer;
        }
        CurrentWeapon.transform.parent = null;
        CurrentWeapon.GetComponent<Rigidbody>().AddForce(Vector3.up * forceU, ForceMode.Impulse);
        CurrentWeapon.GetComponent<Rigidbody>().AddForce(shootPoint.forward * forceF, ForceMode.Impulse);

        CurrentWeapon = null;


    }
    void LauchWeapon()
    {
        if (CurrentWeapon != null)
            DropWeapon(5, 60, 13);

        GameManager.manager.UpdateAmmoText();
    }
    private void ResetShoot()
    {
        ReadyToShoot = true;
    }
    void AimSystem()
    {
        RaycastHit hit;

        if (Physics.Raycast(AimPoint.position, AimPoint.forward, out hit, 300f))
        {
            shootPoint.rotation = Quaternion.LookRotation(hit.point - shootPoint.position);
        }
        else
        {
            shootPoint.rotation = Quaternion.LookRotation(AimPoint.forward);
        }
    }
}
