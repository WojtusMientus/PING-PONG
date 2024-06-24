using Unity.Burst.CompilerServices;
using UnityEngine;
using UnityEngine.UI;

namespace PING_PONG
{
    public class SFXManager : MonoBehaviour
    {

        [SerializeField] private Toggle SFXToggle;
        [SerializeField] private Slider volumeSlider;
        [SerializeField] private GameManager gameManager;

        private void Awake()
        {
            CheckForSavedData();

            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();

            SFXToggle.isOn = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_SFX) == 1;

            volumeSlider.value = PlayerPrefs.GetFloat(Constants.PLAYER_PREFS_VOLUME);
        }

        private void CheckForSavedData()
        {
            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_SFX))
                PlayerPrefs.SetInt(Constants.PLAYER_PREFS_SFX, 1);

            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_VOLUME))
                PlayerPrefs.SetFloat(Constants.PLAYER_PREFS_VOLUME, 0.5f);
        }

        public void SFXToggleChange(bool value)
        {
            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_SFX, value == true ? 1 : 0);
            EventManager.RaiseSFXToggle(value);
            gameManager.PlayAudioClip();
        }

        public void VolumeSliderChange(float sliderValue)
        {
            PlayerPrefs.SetFloat(Constants.PLAYER_PREFS_VOLUME, sliderValue);
            EventManager.RaiseVolumeChange(sliderValue);
        }

    }
}