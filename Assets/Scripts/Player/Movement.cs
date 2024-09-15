using System;
using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;


public class Movement : MonoBehaviour
{
    public bool dead = false;
    public bool canMove;
    public Transform CurrentCheckPoint;
    public Transform playerCam;
    public Transform orientation;
    public Transform gunpos;
    private Rigidbody rb;

    public float moveSpeed = 4000;
    public bool grounded;
    public LayerMask whatIsGround;
    public float maxSpeed = 20;
    public float counterMovement = 0.175f;
    private float threshold = 0.01f;

    private bool readyToJump = true;
    private float jumpCooldown = 0.25f;
    public float jumpForce = 550f;
    private Vector3 crouchScale = new Vector3(1, 0.5f, 1);
    private Vector3 playerScale;
    public float slideForce = 400;
    public float slideCounterMovement = 0.2f;

    private float x, y;
    bool jumping, sprinting, crouching;
    private float xRotation;
    public float sensitivity = 50f;
    private float sensMultiplier = 1f;
    float desiredX;

    [Header("WallRun")]
    public LayerMask WhatIsWall;
    public float wallRunningForce, maxWallRunningTime, wallMaxSpeed;
    bool isWallLeft, isWallRight;
    bool iSWallRuning;
    public float wallRunCarmeraTilt, maxWallRunCameraTilt;
    public GameObject playerFBX;


    public ParticleSystem speedPaticle;


    public AudioSource audioMovement;

    [Header("Sounds")]
    public AudioClip runSound, slideSound;
    private Vector3 normalVector = Vector3.up;

    public Animator anim;


    int result;
    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioMovement = GetComponent<AudioSource>();
        canMove = true;
    }
    // Start is called before the first frame update
    void Start()
    {
        playerScale = transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        if(CurrentCheckPoint != null)
        {
            CurrentCheckPoint.GetChild(1).gameObject.SetActive(true);
            CurrentCheckPoint.GetChild(0).gameObject.SetActive(false);
        }

        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.manager.pause)
        {
            if (!dead)
            {
                Inputs();
                Look();

                if (rb.velocity.magnitude > 2 && grounded && !crouching)
                {
                    if (!audioMovement.isPlaying)
                        GameManager.manager.PlaySound(audioMovement, runSound);
                }
                else if (rb.velocity.magnitude < 2 || !grounded)
                    GameManager.manager.StopSound(audioMovement);
            }
            Move();
            CheckForGround();
            CheckForWall();
            WallRunInput();
            if (rb.velocity.magnitude > maxSpeed)
                SpeedParticle();
            else
            {
                speedPaticle.gameObject.SetActive(false);
            }

        }
        else
        {
            GameManager.manager.StopSound(audioMovement);
        }
    }

    private void Inputs()
    {
        if (canMove)
        {
            x = Input.GetAxisRaw("Horizontal");
            y = Input.GetAxisRaw("Vertical");
            jumping = Input.GetButtonDown("Jump");
            crouching = Input.GetKey(KeyCode.LeftControl);

            //Crouching
            if (Input.GetKeyDown(KeyCode.LeftControl))
                StartCrouch();
            if (Input.GetKeyUp(KeyCode.LeftControl))
                StopCrouch();
        }
    }
    private void Move()
    {

        CounterMovement(x, y, FindMoveDir());
        FixVel();

        if (jumping)
        {
            jump();
        }

        //Fix Movement if sliding down a ramp
        if (crouching && grounded)
        {
            rb.AddForce(Vector3.down * Time.deltaTime * 3000);
            return;
        }

        rb.AddForce(Vector3.down * Time.deltaTime * 10);

        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime);
    }
    private Vector2 FindMoveDir()
    {
        //I dont understant this so much but is actually working :D
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitue = rb.velocity.magnitude;
        float yMag = magnitue * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitue * Mathf.Cos(v * Mathf.Deg2Rad);

        return new Vector2(xMag, yMag);
    }

    public void jump()
    {
        if (grounded)
        {
            readyToJump = false;

            rb.AddForce(Vector2.up * jumpForce * 1.5f);
            rb.AddForce(normalVector * jumpForce * 0.5f);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
        //WallRunJump
        if (iSWallRuning && readyToJump)
        {
            if (isWallRight && !Input.GetKey(KeyCode.D) || isWallLeft && !Input.GetKey(KeyCode.A))
            {
                rb.AddForce(Vector2.up * jumpForce);
                rb.AddForce(normalVector * jumpForce * 0.3f);
            }

            if (isWallLeft && Input.GetKey(KeyCode.D)) rb.AddForce(orientation.right * jumpForce);
            if (isWallRight && Input.GetKey(KeyCode.A)) rb.AddForce(-orientation.right * jumpForce * 2f);

            rb.AddForce(orientation.forward * jumpForce);

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }
    private void FixVel()
    {
        Vector2 mag = FindMoveDir();
        float xMag = mag.x;
        float yMag = mag.y;

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;
    }

    private void StartCrouch()
    {
        GameManager.manager.PlaySound(audioMovement, slideSound);

        transform.localScale = crouchScale;
        transform.position = new Vector3(transform.position.x, transform.position.y - 0.5f, transform.position.z);
        if (rb.velocity.magnitude > 0.5f)
        {
            if (grounded)
            {
                rb.AddForce(orientation.transform.forward * slideForce);
            }
        }
    }
    private void StopCrouch()
    {
        GameManager.manager.StopSound(audioMovement);
        transform.localScale = playerScale;
        transform.position = new Vector3(transform.position.x, transform.position.y + 0.5f, transform.position.z);
    }

    private void ResetJump()
    {
        readyToJump = true;
    }
    private void Look()
    {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;


        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        //Limit the rotation of camera
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, wallRunCarmeraTilt);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);

        //Camera tilt on wall run

        if (Math.Abs(wallRunCarmeraTilt) < maxWallRunCameraTilt && isWallLeft)
            wallRunCarmeraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        if (Math.Abs(wallRunCarmeraTilt) < maxWallRunCameraTilt && isWallRight)
            wallRunCarmeraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;

        if (wallRunCarmeraTilt > 0 && !isWallLeft && !isWallRight)
            wallRunCarmeraTilt -= Time.deltaTime * maxWallRunCameraTilt * 2;
        if (wallRunCarmeraTilt < 0 && !isWallLeft && !isWallRight)
            wallRunCarmeraTilt += Time.deltaTime * maxWallRunCameraTilt * 2;


    }
    private void CounterMovement(float x, float y, Vector2 mag)
    {
        //Linear and angular drag doesn´t work like i want so i´ll use this funcion for fix that
        if (!grounded || jumping) return;


        if (crouching)
        {
            rb.AddForce(moveSpeed * Time.deltaTime * -rb.velocity.normalized * slideCounterMovement);
            return;
        }


        if (Math.Abs(mag.x) > threshold && Math.Abs(x) < 0.05f || (mag.x < -threshold && x > 0) || (mag.x > threshold && x < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.right * Time.deltaTime * -mag.x * counterMovement);
        }
        if (Math.Abs(mag.y) > threshold && Math.Abs(y) < 0.05f || (mag.y < -threshold && y > 0) || (mag.y > threshold && y < 0))
        {
            rb.AddForce(moveSpeed * orientation.transform.forward * Time.deltaTime * -mag.y * counterMovement);
        }

        if (Mathf.Sqrt((Mathf.Pow(rb.velocity.x, 2) + Mathf.Pow(rb.velocity.z, 2))) > maxSpeed)
        {
            float fallspeed = rb.velocity.y;
            Vector3 n = rb.velocity.normalized * maxSpeed;
            rb.velocity = new Vector3(n.x, fallspeed, n.z);
        }
    }

    private void CheckForGround()
    {
        grounded = Physics.Raycast(transform.position, -orientation.up, 1f, whatIsGround);
    }
    private void StopGrounded()
    {
        grounded = false;
    }
    private void WallRunInput()
    {
        if (Input.GetKey(KeyCode.D) && isWallRight) WallRunStart();
        if (Input.GetKey(KeyCode.A) && isWallLeft) WallRunStart();
    }
    private void WallRunStart()
    {
        rb.useGravity = false;
        iSWallRuning = true;

        if (rb.velocity.magnitude <= wallMaxSpeed)
        {
            rb.AddForce(orientation.forward * wallRunningForce * Time.deltaTime);

            if (isWallRight)
                rb.AddForce(orientation.right * wallRunningForce / 5 * Time.deltaTime);
            else
                rb.AddForce(-orientation.right * wallRunningForce / 5 * Time.deltaTime);

        }
    }
    private void WallRunStop()
    {
        rb.useGravity = true;
        iSWallRuning = false;
    }
    private void CheckForWall()
    {
        isWallRight = Physics.Raycast(transform.position, orientation.right, 1f, WhatIsWall);
        isWallLeft = Physics.Raycast(transform.position, -orientation.right, 1f, WhatIsWall);

        if (!isWallLeft && !isWallRight) WallRunStop();
    }

    private void SpeedParticle()
    {

        speedPaticle.gameObject.SetActive(true);
        ParticleSystem speedParticle = speedPaticle.GetComponent<ParticleSystem>();
        ParticleSystem.MainModule main = speedParticle.main;
        ParticleSystem.EmissionModule emission = speedParticle.emission;

        main.startSpeed = rb.velocity.magnitude / 2;


        emission.rateOverTime = rb.velocity.magnitude / 1.6f;

    }

    void OnTriggerEnter(Collider collision)
    {
        if (collision.transform.tag == "CheckPoint")
        {
            CurrentCheckPoint.GetChild(1).gameObject.SetActive(false);
            CurrentCheckPoint.GetChild(0).gameObject.SetActive(true);
            CurrentCheckPoint = collision.transform;
            CurrentCheckPoint.GetChild(1).gameObject.SetActive(true);
            CurrentCheckPoint.GetChild(0).gameObject.SetActive(false);
        }
        if(collision.tag == "Dead")
        {
            GameManager.manager.Respawn();
        }

    }
    private void OnTriggerStay(Collider collision)
    {
        //CollidersForDialogs
        if(SceneManager.GetActiveScene().name == "primeraSala")
        {
            if (collision.gameObject.layer == 18)
            {

                GameManager.manager.EImg.SetActive(true);
                if (Input.GetKey(KeyCode.E))
                {
                    if (collision.tag == "1" || collision.tag == "2")
                    {
                        GameManager.manager.EImg.SetActive(false);
                        GameManager.dialogs.ShowDialog(int.Parse(collision.tag));
                        collision.gameObject.SetActive(false);

                    }
                    GameManager.manager.PlaySoundWithPrefab(GameManager.manager.buttonSound, collision.transform, 0.1f);
                }

            }
        }

        if (collision.gameObject.layer == 22)
        {

            GameManager.manager.EImg.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                GameManager.manager.LoadScene(int.Parse(collision.transform.tag));
                GameManager.manager.EImg.SetActive(false);
            }
        }
        if (collision.gameObject.layer == 23)
        {
            GameManager.manager.EImg.SetActive(true);
            if (Input.GetKey(KeyCode.E))
            {
                if (collision.tag == "FinalDoor1")
                    GameManager.data.level1 = true;
                else if (collision.tag == "FinalDoor2")
                    GameManager.data.level2 = true;
                else if (collision.tag == "FinalDoor3")
                    GameManager.data.level3 = true;
                else if(collision.tag == "End")
                {
                    collision.gameObject.SetActive(false);
                    StartCoroutine(GameManager.manager.EndRutine());
                    return;
                }


                GameManager.data.Save();

                GameManager.manager.LoadScene(1);
                GameManager.manager.EImg.SetActive(false);
            }

        }

    }
    private void OnTriggerExit(Collider collision)
    {

        GameManager.manager.EImg.SetActive(false);

    }
    public IEnumerator SetAnim(String currentAnim, bool state, float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        anim.SetBool(currentAnim, state);
    }
}
