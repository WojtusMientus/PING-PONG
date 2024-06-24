using System.Collections.Generic;
using System.Text;
using PING_PONG;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionManager : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown resolutionDropdown;
    [SerializeField] private Toggle fullScreenToggle;

    private Resolution[] resolutions;
    private List<Resolution> filteredResolutions;
    private RefreshRate refreshRate;

    private int currentResolutionIndex;
    private bool isFullScreen;


    private void Awake()
    {
        CreateResolutionDropdown();

        CheckForSavedData();

        SetFullScreenAndToggle();

        FullScreenToggleChange(isFullScreen);
    }

    private void CreateResolutionDropdown()
    {
        SetVariablesForResolution();

        FilterResolutions();

        List<string> resolutionOptions = CreateResolutionOptions();

        if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_RESOLUTION))
            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_RESOLUTION, filteredResolutions.Count);

        currentResolutionIndex = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_RESOLUTION);

        UpdateResolutionDropdown(resolutionOptions);
    }

    private void SetVariablesForResolution()
    {
        resolutions = Screen.resolutions;
        filteredResolutions = new List<Resolution>();

        refreshRate = Screen.currentResolution.refreshRateRatio;
        resolutionDropdown.ClearOptions();
    }

    private void FilterResolutions()
    {
        foreach (Resolution resolution in resolutions)
            if (resolution.refreshRateRatio.value > refreshRate.value - 1 && resolution.refreshRateRatio.value < refreshRate.value + 1 && resolution.height / 9 == resolution.width / 16)
                filteredResolutions.Add(resolution);
    }

    private List<string> CreateResolutionOptions()
    {
        List<string> resolutionOptions = new List<string>();
        StringBuilder stringBuilder = new StringBuilder();

        for (int i = 0; i < filteredResolutions.Count; i++)
        {
            stringBuilder.Append($"{filteredResolutions[i].width} x {filteredResolutions[i].height}");

            resolutionOptions.Add(stringBuilder.ToString());
            stringBuilder.Clear();

            if (filteredResolutions[i].width == Screen.width && filteredResolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }

        return resolutionOptions;
    }

    private void UpdateResolutionDropdown(List<string> resolutionOptions)
    {
        resolutionDropdown.AddOptions(resolutionOptions);
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();
    }

    private void CheckForSavedData()
    {
        if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_FULLSCREEN))
            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_FULLSCREEN, 1);
    }

    private void SetFullScreenAndToggle()
    {
        isFullScreen = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_FULLSCREEN) == 1;

        fullScreenToggle.isOn = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_FULLSCREEN) == 1;
    }

    public void FullScreenToggleChange(bool setFullScreen)
    {
        Screen.fullScreen = setFullScreen;

        isFullScreen = setFullScreen;

        PlayerPrefs.SetInt(Constants.PLAYER_PREFS_FULLSCREEN, isFullScreen == true ? 1 : 0);

        SetResolution(currentResolutionIndex);
    }

    public void SetResolution(int resolutionIndex)
    {
        Resolution resolution = filteredResolutions[resolutionIndex];

        currentResolutionIndex = resolutionIndex;

        PlayerPrefs.SetInt(Constants.PLAYER_PREFS_RESOLUTION, resolutionIndex);

        Screen.SetResolution(resolution.width, resolution.height, isFullScreen);
    }

}
