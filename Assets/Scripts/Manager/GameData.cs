using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameData : MonoBehaviour
{
    public int currentlevel;
    public bool level1, level2, level3;
    public SettingsManager settings;
    public bool firstTime, firstTimeLevel1, end;

    public static GameData gameData;
    int boolToInt(bool level)
    {
        if (level)
            return 1;
        else
            return 0;
    }

    bool intToBool(int level)
    {
        if (level != 0)
            return true;
        else
            return false;
    }
    // Start is called before the first frame update
    void Awake()
    {
        if (!gameData)
        {
            gameData = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
        settings = GameObject.Find("GameManager").GetComponent<SettingsManager>();

        Load();


    }

    private void Update()
    {

        if(Input.GetKey(KeyCode.LeftAlt) && Input.GetKey(KeyCode.LeftControl))
        {
            level1 = true;
            level2 = true;
            level3 = true;
        }
    }
    public void Save()
    {
        print("Save");
        currentlevel = SceneManager.GetActiveScene().buildIndex;
        PlayerPrefs.SetInt("CurrentLevel", currentlevel);
        PlayerPrefs.SetInt("Level1", boolToInt(level1));
        PlayerPrefs.SetInt("Level2", boolToInt(level2));
        PlayerPrefs.SetInt("Level3", boolToInt(level3));
        PlayerPrefs.SetInt("FirstTime", boolToInt(firstTime));
        PlayerPrefs.SetInt("End", boolToInt(end));
        PlayerPrefs.SetInt("FirstTimeLevel1", boolToInt(firstTimeLevel1));
        PlayerPrefs.SetFloat("Fov", settings.fovslider.value);
        PlayerPrefs.SetFloat("Volume", settings.volumeSlider.value);
        PlayerPrefs.SetInt("Resolution", settings.resolutionDropdown.value);
        PlayerPrefs.SetInt("FullScreen", boolToInt(settings.fullScreenToggle.isOn));
        PlayerPrefs.SetInt("Quality", settings.graphicsDropdown.value);

    }
    public void LoadCurrentScene()
    {
        SceneManager.LoadScene(currentlevel);
    }
    public void delete()
    {
        PlayerPrefs.DeleteAll();
        level1 = false;
        level2 = false;
        level3 = false;
        firstTime = false;
        firstTimeLevel1 = false;
        end = false;
        Save();
        Load();
    }
    public void Load()
    {
        print("Load");

        currentlevel = PlayerPrefs.GetInt("CurrentLevel", 0);
        level1 = intToBool(PlayerPrefs.GetInt("Level1", 0));
        level2 = intToBool(PlayerPrefs.GetInt("Level2", 0));
        level3 = intToBool(PlayerPrefs.GetInt("Level3", 0));
        firstTime = intToBool(PlayerPrefs.GetInt("FirstTime", 0));
        end = intToBool(PlayerPrefs.GetInt("End", 0));
        firstTimeLevel1 = intToBool(PlayerPrefs.GetInt("FirstTimeLevel1", 0));
        settings.fovslider.value = PlayerPrefs.GetFloat("Fov");
        settings.volumeSlider.value = PlayerPrefs.GetFloat("Volume");
        settings.resolutionDropdown.value = PlayerPrefs.GetInt("Resolution", 20);
        settings.fullScreenToggle.isOn = intToBool(PlayerPrefs.GetInt("FullScreen"));
        settings.graphicsDropdown.value = PlayerPrefs.GetInt("Quality", 3);
        currentlevel = SceneManager.GetActiveScene().buildIndex;
    }
    private void OnLevelWasLoaded(int level)
    {
        settings = GameObject.Find("GameManager").GetComponent<SettingsManager>();
        Load();
    }
}
