using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;

public class SettingsManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioMixer mainAudio;

    public Dropdown resolutionDropdown;
    public Dropdown graphicsDropdown;
    private List<Resolution> resolutions = new List<Resolution>();
    public int currentresolutionIndex;
    public Camera myCamera;
    public Slider fovslider, volumeSlider;
    public Toggle fullScreenToggle;
    private void Awake()
    {
        QualitySettings.SetQualityLevel(graphicsDropdown.value);
        setResolutionOptions();
        myCamera = Camera.main;
    }
    private void Start()
    {
        fovslider.value = myCamera.fieldOfView;
        setVolume(volumeSlider.value);
    }

    public void setVolume(float volume)
    {
        mainAudio.SetFloat("Volume", volume);
    }
    public void SerGraphics(int graphics)
    {
        QualitySettings.SetQualityLevel(graphics);
    }
    public void FullScreen(bool isFullScreen)
    {
        Screen.fullScreen = isFullScreen;
    }
    public void setResolutionOptions()
    {
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if(i > 0)
            {
                if (Screen.resolutions[i].width != Screen.resolutions[i - 1].width)
                    resolutions.Add(Screen.resolutions[i]);
            }
            else
            {
                resolutions.Add(Screen.resolutions[i]);
            }
        }

        resolutionDropdown.ClearOptions();

        List<string> options = new List<string>();
        for (int i = 0; i < resolutions.Count; i++)
        {
            string option = resolutions[i].width + "x" + resolutions[i].height;
            options.Add(option);

        }
        resolutionDropdown.AddOptions(options);
        resolutionDropdown.RefreshShownValue();
    }
    public void setResolution(int resolutionIndex)
    {
        Resolution resolution = resolutions[resolutionIndex];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }
    public void Setfov(float fov)
    {
        myCamera.fieldOfView = fov;
    }



}
