using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;


namespace PING_PONG
{
    public class VFXManager : MonoBehaviour
    {

        [SerializeField] private Toggle VFXToggle;

        [SerializeField] private GameObject particleTailPrefab;
        [SerializeField] private GameObject particleBigBurstPrefab;
        [SerializeField] private GameObject particleSmallBurstPrefab;
        [SerializeField] private VisualEffect backgroundStars;

        [SerializeField] private GameObject optionsUI;

        [SerializeField] private int originalVFXSpawnRate;


        private void Awake()
        {
            CheckForSavedData();

            VFXToggle.isOn = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_VFX) == 1;
        }

        private void CheckForSavedData()
        {
            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_VFX))
                PlayerPrefs.SetInt(Constants.PLAYER_PREFS_VFX, 1);
        }

        public void ToggleChange(bool value)
        {
            SetVFXGraphSpawnRate(value);

            SetVFXActive(value);

            EventManager.RaiseOnPlayAudio();

            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_VFX, value == true ? 1 : 0);

            EventManager.RaiseVFXToggle(value);
        }

        private void SetVFXGraphSpawnRate(bool value)
        {
            if (value == true)
                backgroundStars.SetInt(Constants.VFX_SPAWNRATE, originalVFXSpawnRate);
            else
            {
                backgroundStars.SetInt(Constants.VFX_SPAWNRATE, 0);
                backgroundStars.Reinit();
            }
        }

        private void SetVFXActive(bool value)
        {
            particleTailPrefab.SetActive(value);
            particleBigBurstPrefab.SetActive(value);
            particleSmallBurstPrefab.SetActive(value);
        }

    }
}