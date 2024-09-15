using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBase : MonoBehaviour
{
    Transform _player;
    public NavMeshAgent agent;
    public bool WalkPointSet;
    public bool playerInSightRange, playerInAttackRange;
    private Vector3 walkPoint;
    public Quaternion lookDir;
    public Animator anim;
    public LayerMask WhatIsGround, WhatIsPlayer;
    public float WalkPointRange;
    public GameObject bullet, bomb;
    public Transform shootPoint, lookpoint;
    public ParticleSystem ParticleShoot;
    public int IdEnemy;
    public int Weapon;
    public float Cd, desiredRoty;
    public int rotarionSpeed;
    public bool alreadyAttack;
    public bool dead;
    public float sightRange, attackRange;
    public int _CurrentShootPoint = 0;
    public Rigidbody[] rigidbodies;
    public List<Transform> shootpoints = new List<Transform>();



    // Start is called before the first frame update
    public virtual void Start()
    {
        _player = GameManager.player.transform;
        lookpoint = _player;
        agent = GetComponent<NavMeshAgent>();
        Cd = GameManager.dataBaseItems.FechItem(Weapon).Cd;
        //Ragdoll && RigidBodies
        PrepareRigidBodies();
    }

    // Update is called once per frame
    public virtual void Update()
    {
        Animations();
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, WhatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);
        if (!GameManager.player.dead)
        {
            if (!playerInAttackRange && !playerInSightRange) Patroling();
            if (!playerInAttackRange && playerInSightRange) ChasePlayer();
            if (playerInSightRange && playerInAttackRange) AttackPlayer();
        }

    }

    public virtual void Patroling()
    {
        if (!WalkPointSet)
        {
            SearchWalkPoint();
        }
        if (WalkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalk = transform.position - walkPoint;

        if (distanceToWalk.magnitude < 1f) WalkPointSet = false;
    }
    public virtual void SearchWalkPoint()
    {
        float randomZ = Random.Range(WalkPointRange, -WalkPointRange);
        float randomX = Random.Range(WalkPointRange, -WalkPointRange);

        walkPoint = new Vector3(randomX, transform.position.y, randomZ);


        if (Physics.Raycast(walkPoint, -transform.up, 5f, WhatIsGround))
            WalkPointSet = true;
    }
    public void PrepareRigidBodies()
    {
        rigidbodies = transform.GetComponentsInChildren<Rigidbody>();
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rigidbody.mass = 1;
        }
        Ragdoll(true);
    }
    public void Ragdoll(bool Enable)
    {
        anim.enabled = Enable;
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = Enable;
        }
    }
    public virtual void Dead(Vector3 dir, float Force)
    {
        AudioSource audio = GetComponent<AudioSource>();

        if (audio)
            audio.enabled = false;


        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.deadEnemysounds[IdEnemy], transform, 1f);
        Ragdoll(false);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddForce(dir * Force);
        }
        if (gameObject.layer != 20)
        {
            GameManager.manager.DropWeapon(Weapon, shootPoint);
            GameManager.manager.DropPowerUp(shootPoint);
        }

        agent.enabled = false;
        if (IdEnemy != 3)
            this.enabled = false;

        dead = true;

        if (GameManager.manager.endLevel())
        {
           GameManager.manager.finalDoor.GetComponent<Collider>().enabled = true;
        }
        else
        {
            GameManager.manager.finalDoor.GetComponent<Collider>().enabled = false;
        }
    }

    public void ChasePlayer()
    {
        agent.SetDestination(_player.position);
    }
    public virtual void AttackPlayer()
    {
        if (gameObject.layer != 20)
            agent.SetDestination(transform.position);



        lookDir = Quaternion.LookRotation(lookpoint.position - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, lookDir, Time.deltaTime * rotarionSpeed);
        AimSystem();

        if (!alreadyAttack)
        {
            Vector3 PaticleScale = new Vector3();
            if (Weapon == 0)
            {
                GameObject newBullet = Instantiate(bullet, shootpoints[_CurrentShootPoint].transform.position, Quaternion.identity);
                newBullet.transform.forward = shootpoints[_CurrentShootPoint].transform.forward;
                Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                rb.AddForce(shootPoint.forward * 30, ForceMode.Impulse);
                PaticleScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else if (Weapon == 1)
            {
                //Rifle
                GameObject newBullet = Instantiate(bullet, shootpoints[_CurrentShootPoint].transform.position, Quaternion.identity);
                newBullet.transform.forward = shootpoints[_CurrentShootPoint].transform.forward;
                Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                rb.AddForce(shootPoint.forward * 30, ForceMode.Impulse);
                PaticleScale = new Vector3(0.8f, 0.8f, 0.8f);
            }
            else if (Weapon == 2)
            {
                //Granade
                GameObject Bomb = Instantiate(bomb, shootpoints[_CurrentShootPoint].position, Quaternion.identity);
                Rigidbody rb = Bomb.GetComponent<Rigidbody>();
                PaticleScale = new Vector3(1.4f, 1.4f, 1.4f);
                rb.AddForce(shootpoints[_CurrentShootPoint].forward * 15, ForceMode.Impulse);
                rb.AddForce(shootpoints[_CurrentShootPoint].up * 6, ForceMode.Impulse);
            }
            else if (Weapon == 3)
            {
                //Shotgun
                for (int i = 0; i < 8; i++)
                {
                    var RandomDir = shootpoints[_CurrentShootPoint].transform.rotation;
                    RandomDir.x += Random.Range(-0.1f, 0.1f);
                    RandomDir.y += Random.Range(-0.1f, 0.1f);
                    GameObject newBullet = Instantiate(bullet, shootpoints[_CurrentShootPoint].transform.position, RandomDir);

                    Rigidbody rb = newBullet.GetComponent<Rigidbody>();
                    rb.AddForce(newBullet.transform.forward * 20, ForceMode.Impulse);
                }
                PaticleScale = new Vector3(1.2f, 1.2f, 1.2f);

            }
            _CurrentShootPoint++;
            if (_CurrentShootPoint > shootpoints.Count - 1)
            {
                _CurrentShootPoint = 0;
            }

            alreadyAttack = true;
            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.Shootsounds[Weapon], shootpoints[_CurrentShootPoint].transform, 0.8f);
            Invoke(nameof(ResetAttack), Cd);


        }
    }
    private void ResetAttack()
    {
        alreadyAttack = false;
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, sightRange);
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, WalkPointRange);
    }
    public virtual void Animations()
    {
        if (!dead)
        {
            if (playerInAttackRange)
            {
                anim.SetBool("Shoot", true);

            }
            else
            {
                anim.SetBool("Shoot", false);
            }

            if (agent.velocity.magnitude > 0)
            {
                anim.SetBool("Idle", false);
                anim.SetBool("Run", true);

            }
            else
            {
                anim.SetBool("Run", false);
                anim.SetBool("Idle", true);

            }
        }
    }
    void AimSystem()
    {
        for (int i = 0; i < shootpoints.Count; i++)
        {
            shootpoints[i].transform.rotation = Quaternion.LookRotation(_player.transform.position - shootPoint.position);
            desiredRoty += shootpoints[i].localRotation.y;
            desiredRoty = Mathf.Clamp(desiredRoty, -40f, 40f);
            if (shootpoints[i].localRotation.y < -0.39f || shootpoints[i].localRotation.y > 0.39f)
            {
                shootpoints[i].transform.localRotation = Quaternion.Euler(shootPoint.transform.rotation.x, desiredRoty, shootPoint.transform.rotation.x);
            }
        }

    }
}
