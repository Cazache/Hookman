using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerHabilities : MonoBehaviour
{
    public Transform mycamera;
    public GameObject player;
    public float maxDistanceAttack, slowCd;
    public LayerMask WhatCanAttack;
    public ParticleSystem ImpactParticle;

    public KeyCode MeeleAttackKey;
    public float sensMultiplier = 4;
    [Header("Habilities")]
    public bool Slow;
    public bool shield;
    public bool cdReduction = false;
    public int CurrentDashes;
    public Slider slowSlider;
    public GameObject shieldObj;
    public Image ShieldImg;
    public GameObject rapidFireObj;
    public Animator slowImg;
    private float cdReductionTime = 10;
    public float dashForce;
    public ParticleSystem ShieldParticle;

    public bool readyToAttack;
    bool slowed;
    public bool iframes;
    void Update()
    {
        if (!GameManager.manager.pause)
        {
            Inputs();
            if (cdReduction && GameManager.weapons.CurrentWeapon)
                ResetCdReduction();
        }
       SlowSlider();
    }

    void Inputs()
    { 
        if(GameManager.player.canMove)
        {
            if (Input.GetMouseButtonDown(2) && readyToAttack && GameManager.katana.unlocked)
                MeeleAttack();
            if (Input.GetKeyDown(KeyCode.Q) && !Slow)
            {
                GameManager.manager.PlaySoundWithPrefab(GameManager.manager.slowMotionSound, transform, 1);
                StartCoroutine(slowTime());
            }
            if (Input.GetKeyDown(KeyCode.LeftShift) && CurrentDashes > 0)
            {
                Dash();
            }
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (GameManager.manager.pause)
                {
                    GameManager.manager.Unpause();
                }
                else
                {
                    GameManager.manager.Pause();
                }
            }
        }

    }

    public void MeeleAttack()
    {
        StartCoroutine(GameManager.player.SetAnim("MeeleAttack", true, 0f));
        StartCoroutine(GameManager.player.SetAnim("MeeleAttack", false, 0.5f));

        RaycastHit hit;
        if (Physics.Raycast(mycamera.position, mycamera.forward, out hit, maxDistanceAttack, WhatCanAttack))
        {
            if (hit.collider.gameObject.layer == 10 || hit.collider.gameObject.layer == 14 || hit.collider.gameObject.layer == 19)
            {

                EnemyBase Enemy = hit.collider.GetComponentInParent<EnemyBase>();
                if (!Enemy.dead)
                {
                    Enemy.Dead(mycamera.transform.forward, 10);
                }
                int random = Random.Range(0, GameManager.manager.enemyBaseHitSound.Count);

                GameManager.manager.PlaySoundWithPrefab(GameManager.manager.enemyBaseHitSound[random], hit.transform, 1f);

            }
            else if (hit.collider.gameObject.layer == 20 && !hit.collider.GetComponentInParent<BossControl>().dead)
            {
                hit.collider.GetComponentInParent<BossControl>().GetDamage(10);
                if (hit.collider.GetComponentInParent<BossControl>().canPush)
                    StartCoroutine(hit.collider.GetComponentInParent<BossControl>().PushPlayer(0.1f, 90));
            }
            else if( hit.collider.gameObject.layer == 21)
            {
                hit.collider.GetComponentInParent<Pet>().GetDamage(mycamera.transform.forward, 20);

            }
            hit.collider.GetComponent<Rigidbody>().AddForce(mycamera.transform.forward * 50, ForceMode.Impulse);
            ParticleSystem NewParticle = Instantiate(ImpactParticle);
            NewParticle.transform.position = hit.point;

        }
        readyToAttack = false;
     
        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.meeleAttackSound, transform, 1f);

        Invoke("ResetMeeleAttack", 0.6f);

    }

    public void ResetKatana()
    {
        GameManager.katana.transform.parent = GameManager.player.gunpos;
    }
    public void ResetMeeleAttack()
    {
        GameManager.katana.transform.parent = GameManager.player.gunpos;
        readyToAttack = true;
    }
    public IEnumerator slowTime()
    {
        Slow = true;
        slowed = true;
        slowSlider.value -= 0.1f * Time.deltaTime;
        float saveSens = GameManager.player.sensitivity;
        slowImg.SetBool("IsSlow", true);
        Time.timeScale = 0.1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        GameManager.player.sensitivity *= sensMultiplier;
        fixAudiotime(0.3f);
        yield return new WaitForSeconds(0.25f);
        slowed = false;

        GameManager.player.sensitivity = saveSens;
        Time.timeScale = 1f;
        Time.fixedDeltaTime = Time.timeScale * 0.02f;
        fixAudiotime(1);
        slowImg.SetBool("IsSlow", false);
    }
    public void fixAudiotime(float time)
    {
        GameManager.player.audioMovement.pitch = time;
        for (int i = 0; i < GameManager.manager.Shootsounds.Count; i++)
        {
            GameManager.manager.Shootsounds[i].GetComponent<AudioSource>().pitch = time;
        }
        GameManager.manager.dashSound.GetComponent<AudioSource>().pitch = time;
        GameManager.manager.grappingGunSound.GetComponent<AudioSource>().pitch = time;
    }
   void SlowSlider()
   {
       Image backImg = slowSlider.transform.GetChild(0).GetComponent<Image>();
       Image Img = slowSlider.transform.GetChild(1).GetChild(0).GetComponent<Image>();
       if (slowSlider.value == slowSlider.maxValue)
       {           
           backImg.color = new Color(backImg.color.r, backImg.color.g, backImg.color.b, 1f);
           Img.color = new Color(backImg.color.r, backImg.color.g, backImg.color.b, 1f);
       }
       else
       {
           backImg.color = new Color(backImg.color.r, backImg.color.g, backImg.color.b, 0.3f);
           Img.color = new Color(backImg.color.r, backImg.color.g, backImg.color.b, 0.3f);
       }
       if(slowed)
       {
           slowSlider.value -= 3 * Time.deltaTime;
       }
       else
       {
           slowSlider.value += Time.deltaTime;
           if (slowSlider.value == slowSlider.maxValue)
               Slow = false;
       }
   
   }
    public void Dash()
    {
        Rigidbody rb = GameManager.player.GetComponent<Rigidbody>();

        rb.AddForce(GameManager.player.playerCam.forward * dashForce, ForceMode.Impulse);

        CurrentDashes--;
        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.dashSound, transform, 0.5f);
        GameManager.manager.UpdateDashText();
    }
    void ResetCdReduction()
    {
        rapidFireObj.GetComponent<Animator>().enabled = true;
        cdReductionTime -= Time.deltaTime;
        if (cdReductionTime <= 0)
        {
            rapidFireObj.GetComponent<Animator>().enabled = false;
            Image img = rapidFireObj.GetComponent<Image>();
            img.color = new Color(img.color.r, img.color.g, img.color.b, 0.3f);
            cdReduction = false;
            cdReductionTime = 10;
            GameManager.weapons.cdShoot = GameManager.dataBaseItems.FechItem(GameManager.weapons.CurrentWeapon.GetComponent<Weapon>().id).Cd;
        }
    }
    public void StartShield()
    {
        ShieldImg.enabled = true;
        shieldObj.GetComponent<Animator>().enabled = true;
        shield = true;
    }
    public void RemoveShield()
    {
        Instantiate(ShieldParticle, transform);
        shieldObj.GetComponent<Animator>().enabled = false;
        Image img = shieldObj.GetComponent<Image>();
        img.color = new Color(img.color.r, img.color.g, img.color.b, 0.3f);
        ShieldImg.enabled = false;
        shield = false;
        iframes = true;
        Invoke("IFrames", 2f);
    }
    void IFrames()
    {
        iframes = false;
    }
}
