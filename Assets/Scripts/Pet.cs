
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;
public class Pet : MonoBehaviour
{
    private NavMeshAgent _agent;
    private bool rangeWithPlayer;
    public float minRange, speed, rotationSpeed;
    public LayerMask WhatIsPlayer;
    private Rigidbody[] rigidbodies;
    private Animator _anim;
    public bool dead;
    public float WalkPointRange;
    Vector3 walkPoint;
    public bool WalkPointSet;
    // Start is called before the first frame update
    void Start()
    {
        _anim = GetComponentInChildren<Animator>();
        _agent = GetComponent<NavMeshAgent>();
        PrepareRigidBodies();
    }

    // Update is called once per frame
    void Update()
    {
        if (!dead)
        {
            if (SceneManager.GetActiveScene().name != "Menu")
            {
                rangeWithPlayer = Physics.CheckSphere(transform.position, minRange, WhatIsPlayer);
                if (_agent.enabled == true)
                {
                    if (!rangeWithPlayer)
                        followPlayer();
                    else
                        _agent.SetDestination(transform.position);
                }
                Animations();

            }
            else
            {
                Patroling();
            }
        }



    }
     void Patroling()
    {
        if (!WalkPointSet)
        {
            SearchWalkPoint();
        }
        if (WalkPointSet) _agent.SetDestination(walkPoint);

        Vector3 distanceToWalk = transform.position - walkPoint;

        if (distanceToWalk.magnitude < 3f) WalkPointSet = false;
    }
    void SearchWalkPoint()
    {
        float randomZ = Random.Range(WalkPointRange, -WalkPointRange);
        float randomX = Random.Range(WalkPointRange, -WalkPointRange);

        walkPoint = new Vector3(randomX, transform.position.y, randomZ);

        
          WalkPointSet = true;
    }
    void followPlayer()
    {
        _agent.SetDestination(GameManager.player.transform.position);
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
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.isKinematic = Enable;
        }
        _agent.enabled = Enable;
    }
    public void GetDamage(Vector3 dir, float Force)
    {
        int random = Random.Range(0, GameManager.manager.enemyBaseHitSound.Count);

        GameManager.manager.PlaySoundWithPrefab(GameManager.manager.enemyBaseHitSound[random], transform, 1f);
        Ragdoll(false);
        foreach (Rigidbody rigidbody in rigidbodies)
        {
            rigidbody.AddForce(dir * Force, ForceMode.Impulse);
            rigidbody.AddForce(Vector3.up * 2, ForceMode.Impulse);
        }
        transform.GetComponentInChildren<Renderer>().material = Resources.Load<Material>("Materials/Pet/PetHurt");
        if (!dead)
        {
            _anim.enabled = false;
           GameManager.manager.SpawnPet();
            Destroy(GetComponent<Collider>());
            Destroy(GetComponent<AudioSource>());
            GameManager.manager.PlaySoundWithPrefab(GameManager.manager.petDamage, transform, 1f);
            dead = true;
        }

    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, WalkPointRange);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.layer == 12 && !dead)
            GetDamage(GameManager.player.orientation.forward, 8);

    }
    void Animations()
    {
        if (_agent.velocity.magnitude != 0)
            _anim.SetBool("Move", true);
        else
            _anim.SetBool("Move", false);


    }
}
