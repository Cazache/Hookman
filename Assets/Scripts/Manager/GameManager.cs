using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Reload")]
    public GameObject EnemiesObj;
    private List<Vector3> EnemiesPos = new List<Vector3>();
    [Header("Manager")]
    public static DataBaseItems dataBaseItems;
    public static Movement player;
    public static WeaponShoot weapons;
    public static GameManager manager;
    public static PlayerHabilities habilities;
    public static DialogManager dialogs;
    public static Katana katana;
    public static GrapplingGun grapplingGun;
    public static GameData data;
    [Header("UI")]
    public Image dashImg;
    public Slider bossLifeSlider;
    public GameObject ammoImg;
    public Image rapidFireImg;
    public GameObject EImg;
    //  public Slider volumeSlider;
    //  public Dropdown quality;
    //  public Dropdown resolution;


    [Header("Pause")]
    public bool pause;
    public GameObject pauseMenu;


    [Header("Sounds")]
    public List<GameObject> Shootsounds = new List<GameObject>();
    public GameObject dashSound;
    public GameObject grappingGunSound;
    public GameObject slowMotionSound;
    public GameObject grabWeaponShoot;
    public GameObject powerUpSound;
    public GameObject deadPlayerSound;
    public List<GameObject> deadEnemysounds = new List<GameObject>();
    public GameObject hitBossSound;
    public GameObject meeleAttackSound;
    public List<GameObject> enemyBaseHitSound = new List<GameObject>();
    public GameObject petDamage;
    public GameObject buttonSound;
    public AudioClip bossWalk;
    public GameObject bossDead;
    public GameObject explosion;
    public List<GameObject> doors = new List<GameObject>();
    public Transform initPetPos;
    public GameObject pet;
    private BossControl _boss;
    public GameObject finalDoor;
    //Animations
    public Animator DeadImgAnim;
    bool god;

    public bool endLevel()
    {
        if (EnemiesObj != null)
        {
            for (int i = 0; i < EnemiesObj.transform.childCount; i++)
            {
                if (!EnemiesObj.transform.GetChild(i).GetComponent<EnemyBase>().dead)
                {
                    return false;
                }
            }
            return true;
        }
        return true;
    }
    // Start is called before the first frame update
    void Awake()
    {
        //Load items and weapons
        GameObject Go = new GameObject("DataBaseItems");
        Go.transform.parent = transform;
        GameManager.dataBaseItems = Go.AddComponent<DataBaseItems>();
        GameManager.dataBaseItems.loadData();
        GameManager.manager = GameObject.FindObjectOfType<GameManager>();
        GameManager.player = GameObject.FindObjectOfType<Movement>();
        GameManager.weapons = GameObject.FindObjectOfType<WeaponShoot>();
        GameManager.habilities = GameObject.FindObjectOfType<PlayerHabilities>();
        GameManager.dialogs = GameObject.FindObjectOfType<DialogManager>();
        GameManager.katana = GameObject.FindObjectOfType<Katana>();
        GameManager.grapplingGun = GameObject.FindObjectOfType<GrapplingGun>();
        GameManager.data = GameObject.FindObjectOfType<GameData>();
        if (SceneManager.GetActiveScene().name == "primeraSala")
        {
            initPetPos = GameObject.Find("PetInitPos").transform;
            CloseDoors();
        }
        else if (SceneManager.GetActiveScene().name == "SalaBoss")
        {
            InitBoss();
        }



        EnemiesObj = GameObject.Find("Enemies");
        SaveLevel();
        UpdateAmmoText();

    }
    private void Start()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            OpenDoors();
            if (data.firstTime)
                GameManager.manager.SpawnPet();
        }
        else
        {
            if(!finalDoor)
            {
                Debug.Log("Finaldoor no esta asiganada");
                return; 
            }
            if (endLevel())
            {
                finalDoor.GetComponent<Collider>().enabled = true;
            }
            else
            {
                finalDoor.GetComponent<Collider>().enabled = false;
            }
        }

    }


    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F8))
            god = !god;

    }
    void OpenDoors()
    {
        if (data.level1)
        {
            doors[0].SetActive(true);
            if (data.level2)
            {
                doors[1].SetActive(true);
                if (data.level3)
                {
                    doors[2].SetActive(true);
                }
            }
        }
    }
    void CloseDoors()
    {
        for (int i = 0; i < doors.Count; i++)
        {
            doors[i].SetActive(false);
        }
    }
    public void UpdateAmmoText()
    {
        if (ammoImg)
            if (weapons.CurrentWeapon != null)
            {
                ammoImg.SetActive(true);
                ammoImg.GetComponentInChildren<Text>().text = weapons.CurrentWeapon.GetComponent<Weapon>().ammo.ToString();
            }
            else
                ammoImg.SetActive(false);
    }
    void InitBoss()
    {
        _boss = GameObject.FindWithTag("Boss").GetComponent<BossControl>();
        if (_boss != null)
        {
            bossLifeSlider.gameObject.SetActive(true);
            bossLifeSlider.maxValue = _boss.maxLife;
            UpdateBossSlider();
        }
    }
    public void UpdateDashText()
    {
        if (habilities.CurrentDashes > 0)
            dashImg.GetComponent<Animator>().enabled = true;
        else
        {
            dashImg.GetComponent<Animator>().enabled = false;
            dashImg.color = new Color(dashImg.color.r, dashImg.color.g, dashImg.color.b, 0.3f);
        }



        if (habilities.CurrentDashes > 1)
            dashImg.GetComponentInChildren<Text>().enabled = true;
        else
            dashImg.GetComponentInChildren<Text>().enabled = false;
    }
    public void Dead()
    {
        if (GameManager.habilities.shield)
        {
            GameManager.habilities.RemoveShield();
        }
        else
        {
            if (!GameManager.habilities.iframes && !god)
            {
             
                PlaySoundWithPrefab(deadPlayerSound, player.transform, 1f);
                DeadImgAnim.SetBool("IsDead", true);
                Time.timeScale = 0.5f;
                player.dead = true;
                player.audioMovement.Stop();
                Invoke("Respawn", 1f);
            }
        }
    }
    public void SpawnPet()
    {
        if (SceneManager.GetActiveScene().buildIndex == 1)
            Instantiate(pet, initPetPos.position, initPetPos.rotation);
    }
    public void Respawn()
    {
        if (SceneManager.GetActiveScene().name == "SalaBoss")
        {
            LoadScene(5);
            return;
        }
        grapplingGun.StopGrapple();
        Time.timeScale = 1;
        DeadImgAnim.SetBool("IsDead", false);
        player.transform.position = player.CurrentCheckPoint.position;
        player.GetComponent<Rigidbody>().velocity = Vector3.zero;
        loadLevel();
        GameManager.player.dead = false;
        GameManager.player.GetComponent<CapsuleCollider>().enabled = true;
    }
    public void UpdateBossSlider()
    {
        bossLifeSlider.value = _boss.life;
    }
    void loadLevel()
    {
        if (EnemiesObj)
            for (int i = 0; i < EnemiesObj.transform.childCount; i++)
            {
                if (!EnemiesObj.transform.GetChild(i).GetComponent<EnemyBase>().dead)
                {
                    EnemiesObj.transform.GetChild(i).transform.position = EnemiesPos[i];
                }
            }
    }
    void SaveLevel()
    {
        if (EnemiesObj)
            for (int i = 0; i < EnemiesObj.transform.childCount; i++)
            {
                EnemiesPos.Add(EnemiesObj.transform.GetChild(i).transform.position);
            }
    }
    public void DropWeapon(int weapon, Transform shootPoint)
    {
        int random = Random.Range(1, 3);
        if (random == 1)
        {
            GameObject NewWeapon = Instantiate(Resources.Load<GameObject>("Prefabs/Weapons/Weapon_" + weapon), shootPoint.position, shootPoint.rotation);
            NewWeapon.GetComponent<Rigidbody>().AddForce(Vector3.up * 10, ForceMode.Impulse);
        }
    }

    public void DropPowerUp(Transform Pos)
    {
        int random = Random.Range(0, 100);
        if (random <= 60)
        {
            int randomDrop = Random.Range(0, 3);
            GameObject NewPowerUp = Instantiate(Resources.Load<GameObject>("Prefabs/PowerUps/PowerUp"), Pos.position, Quaternion.identity);
            NewPowerUp.transform.position = new Vector3(NewPowerUp.transform.position.x, NewPowerUp.transform.position.y - 0.5f, NewPowerUp.transform.position.z);
            NewPowerUp.GetComponent<PowerUp>().id = randomDrop;
            Instantiate(Resources.Load<GameObject>("Prefabs/Particles/PowerUps/" + NewPowerUp.GetComponent<PowerUp>().id), NewPowerUp.transform);
        }

    }



    public void Pause()
    {
        pause = true;
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    public void Unpause()
    {
        pause = false;
        if (pauseMenu)
            pauseMenu.SetActive(false);


        Time.timeScale = 1;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
    private void OnLevelWasLoaded(int level)
    {
        Unpause();
    }
    public void LoadScene(int Scene)
    {
        SceneManager.LoadScene(Scene);
    }
    public void PlaySound(AudioSource Source, AudioClip sound)
    {
        Source.clip = sound;
        Source.Play();
    }
    public void StopSound(AudioSource source)
    {
        source.Stop();
    }
    public void PlaySoundWithPrefab(GameObject sound, Transform pos, float duration)
    {
        Destroy(Instantiate(sound, pos.position, Quaternion.identity), duration);
    }
    public IEnumerator CameraShake(float duration, float magnitude)
    {
        if (GameManager.habilities.Slow)
            yield break;


        Vector3 originalPos = Camera.main.transform.localPosition;
        float elapsed = 0;

        while (elapsed < duration)
        {
            float x = Random.Range(-1, 1) * magnitude;
            float y = Random.Range(-1, 1) * magnitude;

            Camera.main.transform.localPosition = new Vector3(x, y, originalPos.z);

            elapsed += Time.deltaTime;

            yield return null;
        }

        Camera.main.transform.localPosition = originalPos;
    }
    public IEnumerator EndRutine()
    {
        habilities.slowImg.SetBool("End", true);
        yield return new WaitForSeconds(1f);
        dialogs.ShowDialog(4);
        yield return new WaitForSeconds(16f);
        LoadScene(1);
    }

}
