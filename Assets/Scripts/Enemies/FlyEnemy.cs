using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyEnemy : EnemyBase
{
    private Vector3 _walkPoint, _startPoint;
    private SpringJoint _joint;
    public GameObject orientation;
    public float speed, attackCounterMov;

    private Rigidbody _rb;


    // Start is called before the first frame update
    public override void Start()
    {

        base.Start();
        _startPoint = transform.localPosition;
        _rb = GetComponent<Rigidbody>();
        _rb.isKinematic = false;
        alreadyAttack = true;
 
    }

    // Update is called once per frame
    public override void Update()
    {
        Animations();
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, WhatIsPlayer);
        if (!GameManager.player.dead)
        {

            if (!playerInAttackRange) Patroling();
            else
            {
                if (_joint == null)
                    _joint = gameObject.AddComponent<SpringJoint>();

                GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Audio/FlyEnemy/Attack");
                if (!GetComponent<AudioSource>().isPlaying)
                    GetComponent<AudioSource>().Play();
                AttackPlayer();
            }
        }
    }
    public override void Dead(Vector3 dir, float Force)
    {
        if (GetComponent<AudioSource>().isPlaying)
            GetComponent<AudioSource>().Stop();
        Ragdoll(false);
        this.enabled = false;
        dead = true;
    }
    public override void Patroling()
    {

        if (!WalkPointSet)
        {
            SearchWalkPoint();
        }
        if (WalkPointSet)
        {
            fixRotation(_walkPoint);
            _rb.AddForce(orientation.transform.forward * speed * Time.deltaTime);
        }

        Vector3 distanceToWalk = transform.position - _walkPoint;

        if (distanceToWalk.magnitude < 10f) WalkPointSet = false;
    }
    public override void Animations()
    {
        if (!dead)
        {
            if (_rb.velocity.magnitude != 0)
            {
                anim.SetBool("Move", true);
            }
            else
                anim.SetBool("Move", true);

            if (playerInAttackRange)
                anim.SetBool("Attack", true);
        }
    }
    public override void SearchWalkPoint()
    {
        float randomZ = Random.Range(WalkPointRange, -WalkPointRange);
        float randomX = Random.Range(WalkPointRange, -WalkPointRange);


        _walkPoint = new Vector3(_startPoint.x + randomX, _startPoint.y, _startPoint.z + randomZ);
        WalkPointSet = true;
    }


    public override void AttackPlayer()
    {
        _joint.autoConfigureConnectedAnchor = false;
        _joint.spring = 7;
        Vector3 Anchor = new Vector3(GameManager.player.transform.position.x, GameManager.player.transform.position.y - 10, GameManager.player.transform.position.z);
        _joint.connectedAnchor = Anchor;
        fixRotation(Anchor);
    }
    void fixRotation(Vector3 OrientationDir)
    {
        lookDir = Quaternion.LookRotation(OrientationDir - transform.position);
        transform.rotation = Quaternion.Lerp(transform.rotation, orientation.transform.rotation, Time.deltaTime * rotarionSpeed);
        orientation.transform.LookAt(OrientationDir);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!dead)
        {
            if (collision.gameObject.layer != 14 && collision.gameObject.layer != 13)
            {
                GameObject NewBomb = Instantiate(bomb, transform.position, Quaternion.identity);
                NewBomb.GetComponent<Bomb>().radiusAttack -= 5;
                NewBomb.GetComponent<Bomb>().Explosion();
                Destroy(gameObject);
            }
        }


    }
}
