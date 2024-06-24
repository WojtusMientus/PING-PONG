using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.VFX;


namespace PING_PONG
{
    public class ColorManager : MonoBehaviour
    {

        [SerializeField] private List<Button> colorButtons;
        [SerializeField] private Material VFXColor;
        [SerializeField] private Material goalsAndPlayerColor;
        private VisualEffect backgroundStars;

        private List<Color> availableColors = new List<Color>();


        private void Awake()
        {
            CheckForSavedData();

            backgroundStars = GameObject.FindGameObjectWithTag(Constants.STARS_TAG).GetComponent<VisualEffect>();

            AddPossibleColors();

            UpdateStartingColor();
        }

        private void OnEnable()
        {
            EventManager.OnColorButtonClick += SetColor;
            EventManager.OnPauseGame += PauseAndUnapauseParticles;
        }

        private void OnDisable()
        {
            EventManager.OnColorButtonClick -= SetColor;
            EventManager.OnPauseGame -= PauseAndUnapauseParticles;
        }

        private void CheckForSavedData()
        {
            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_COLOR))
                PlayerPrefs.SetInt(Constants.PLAYER_PREFS_COLOR, 0);
        }

        private void AddPossibleColors()
        {
            for (int i = 0; i < colorButtons.Count; i++)
            {
                float colorMultiplier = 1f;

                if (i == 1)
                    colorMultiplier = 1.2f;

                availableColors.Add(colorButtons[i].GetComponent<OnButtonClick>().GetColor() * colorMultiplier);
            }
        }

        private void UpdateStartingColor()
        {
            int buttonID = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_COLOR);

            UpdateColor(buttonID);
        }

        private void UpdateColor(int buttonID)
        {
            backgroundStars.SetVector4("StarsColor", availableColors[buttonID] * 5);
            goalsAndPlayerColor.SetColor("_EmissionColor", availableColors[buttonID]);
            VFXColor.SetColor("_Color", availableColors[buttonID]);
        }

        private void SetColor(int buttonID)
        {
            UpdateColor(buttonID);

            PlayerPrefs.SetInt(Constants.PLAYER_PREFS_COLOR, buttonID);
        }

        private void PauseAndUnapauseParticles()
        {
            backgroundStars.pause = !backgroundStars.pause;
        }

    }
}