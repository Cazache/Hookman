using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossControl : EnemyBase
{
    public float changeWeaponTimer, changeWeaponCd;
    public float distanceWithPlayer, pushRange;
    public float pushCd;
    public ParticleSystem pushParticle;
    public bool _canAttack = true, objInPushRange, playerInPushRange, _canChangeWeapon = true;
    public bool canPush = true;
    public LayerMask WhatCanPush;
    public float life, maxLife;
    bool canMove = false;
    public AudioSource music;
    public override void Start()
    {
        GameManager.player.canMove = false;
        life = maxLife;
        base.Start();
        GetComponent<Rigidbody>().mass = 200;
        Invoke("CanMoveOn", 4.7f);

    }
    public override void Update()
    {
        if(canMove && !dead)
        {
            Animations();

            if (_canAttack)
                Defense();


            playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
            playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);
            if (!GameManager.player.dead)
            {
                ChasePlayer();
                if (playerInSightRange && playerInAttackRange) AttackPlayer();
            }

        }
        if (dead) 
        {
            music.volume -= 0.25f * Time.deltaTime;
        }
    }
    public void Defense()
    {
        if (canPush)
        {
            objInPushRange = Physics.CheckSphere(transform.position, pushRange, WhatCanPush);
            if (objInPushRange)
            {
                StartCoroutine(PushPlayer(0.4f, 50));
            }

        }

    }
    void CanMoveOn()
    {
        canMove = true;
        GameManager.player.canMove = true;
        anim.SetBool("Entry", false);
    }
    void ResetPush()
    {
        canPush = true;
    }
    public override void AttackPlayer()
    {
        if (_canAttack)
        {
            ChangeWeapon();
            base.AttackPlayer();
        }

    }
    public void GetDamage(float dmg)
    {
 
        life -= dmg;
        GameManager.manager.UpdateBossSlider();
        if (life <= 0)
        {
            GameManager.manager.StopSound(transform.GetComponentInChildren<AudioSource>());
            Dead(Vector3.up, 50);
        }
           

        
    }
    public void ChangeWeapon()
    {
        playerInPushRange = Physics.CheckSphere(transform.position, pushRange, WhatIsPlayer);
        if (playerInPushRange)
        {
            Weapon = 3;
            Cd = GameManager.dataBaseItems.FechItem(Weapon).Cd;
            return;
        }
        if (_canChangeWeapon)
        {
            int lastWeapon = Weapon;

            while (Weapon == lastWeapon)
                Weapon = Random.Range(0, 3);

            _canChangeWeapon = false;
            Invoke("ResetChangeWeapon", changeWeaponCd);
            Cd = GameManager.dataBaseItems.FechItem(Weapon).Cd;
        }
  
    }
    void ResetChangeWeapon()
    {
        _canChangeWeapon = true;
    }

    public override void Animations()
    {
        if (!dead)
        {
            if (agent.velocity.magnitude > 0)
            {
                anim.SetBool("Move", true);
                GameManager.manager.PlaySound(transform.GetComponentInChildren<AudioSource>(), GameManager.manager.bossWalk);
            }
            else
            {
                anim.SetBool("Move", false);
                GameManager.manager.StopSound(transform.GetComponentInChildren<AudioSource>());
            }
        }
    }
    public IEnumerator PushPlayer(float wait, int prob)
    {
        
        float random = Random.Range(0, 100);

        if (random > prob)
        {
            yield break;
        }    
        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.hitBossSound, transform, 1f);
        _canAttack = false;
        canPush = false;
        anim.SetBool("Push", true);
        yield return new WaitForSeconds(wait);
        Explosion();
        Invoke("ResetPush", pushCd);
        yield return new WaitForSeconds(0.2f);
        anim.SetBool("Push", false);
        _canAttack = true;
    }
    public void Explosion()
    {
        Vector3 ExplosionPos = transform.position;
        Collider[] colliders = Physics.OverlapSphere(ExplosionPos, pushRange);
        foreach (Collider hit in colliders)
        {
            Rigidbody rb = hit.GetComponent<Rigidbody>();
            if (rb != null)
            {

                rb.AddExplosionForce(120, ExplosionPos, pushRange, 7, ForceMode.Impulse);
            }
        }
        StartCoroutine(GameManager.manager.CameraShake(.15f, .25f));
        ParticleSystem newParticle = Instantiate(pushParticle, transform.position, Quaternion.Euler(-90, 0, 0));
        newParticle.transform.position = new Vector3(newParticle.transform.position.x, newParticle.transform.position.y - 2, newParticle.transform.position.z);


    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, pushRange);
    }
}
