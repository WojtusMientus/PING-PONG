using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PING_PONG
{
    public class GameManager : MonoBehaviour
    {

        private PlayerInput inputActions;
        private InputAction leftPlayerAction;
        private InputAction rightPlayerAction;

        [SerializeField] private TextMeshProUGUI leftPlayersScoreLabel;
        [SerializeField] private TextMeshProUGUI rightPlayersScoreLabel;

        [SerializeField] private List<GameObject> gameplayElements;
        private List<GameObject> gameplayElementsReversed;
        private List<GameObject> bordersAndPlayers;
        private GameObject divider;

        [SerializeField] private List<GameObject> players;

        private int leftPlayerPoints = 0;
        private int rightPlayerPoints = 0;

        [SerializeField] private GameObject ballPrefab;

        private AudioSource audioSourceComponent;
        [SerializeField] private AudioClip onClickSound;
        [SerializeField] private AudioClip[] onEnableSounds;

        private bool canPause = false;
        [NonSerialized] public bool isPaused = false;
        [NonSerialized] public bool VFXEnabled;
        [NonSerialized] public bool SFXEnabled;
        [NonSerialized] public float SFXVolume;


        private void Awake()
        {
            SetAudio();

            CheckForSavedData();

            SetInputActions();

            SetGameplayElements();
        }

        private void Start()
        {
            audioSourceComponent.enabled = true;
        }

        private void OnEnable()
        {
            EventManager.OnStartGame += StartGame;
            EventManager.OnPointScore += AddPoint;
            EventManager.OnRespawnBall += RespawnBall;
            EventManager.OnVFXToggle += SetVFX;
            EventManager.OnGoalEnter += OnGoalEnterPreventPause;
            EventManager.PlayAudio += PlayAudioClip;
            EventManager.OnSFXToggle += SetSFX;
            EventManager.OnVolumeChange += SetVolume;
        }

        private void OnDisable()
        {
            EventManager.OnStartGame -= StartGame;
            EventManager.OnPointScore -= AddPoint;
            EventManager.OnRespawnBall -= RespawnBall;
            EventManager.OnVFXToggle -= SetVFX;
            EventManager.OnGoalEnter -= OnGoalEnterPreventPause;
            EventManager.PlayAudio -= PlayAudioClip;
            EventManager.OnSFXToggle -= SetSFX;
            EventManager.OnVolumeChange -= SetVolume;
        }

        private void SetAudio()
        {
            audioSourceComponent = GetComponent<AudioSource>();
            audioSourceComponent.clip = onClickSound;
        }

        private void CheckForSavedData()
        {
            if (!PlayerPrefs.HasKey(Constants.RIGHT_BOUND))
                ResetScore();

            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_VFX))
                VFXEnabled = true;
            else
                VFXEnabled = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_VFX) == 1;

            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_SFX))
                SFXEnabled = true;
            else
                SFXEnabled = PlayerPrefs.GetInt(Constants.PLAYER_PREFS_SFX) == 1;

            if (!PlayerPrefs.HasKey(Constants.PLAYER_PREFS_VOLUME))
                SFXVolume = 0.5f;
            else
                SFXVolume = PlayerPrefs.GetFloat(Constants.PLAYER_PREFS_VOLUME);

            audioSourceComponent.volume = SFXVolume;
        }

        private void SetInputActions()
        {
            inputActions = GetComponent<PlayerInput>();
            leftPlayerAction = inputActions.actions.FindAction(Constants.LEFT_PLAYER_ACTION);
            rightPlayerAction = inputActions.actions.FindAction(Constants.RIGHT_PLAYER_ACTION);
        }

        private void SetGameplayElements()
        {
            gameplayElementsReversed = new List<GameObject>(gameplayElements);
            gameplayElementsReversed.Reverse();

            bordersAndPlayers = gameplayElements.Take<GameObject>(6).ToList();
            divider = gameplayElements[6];
        }

        private void StartGame()
        {
            StartCoroutine(StartGameCoroutine());
        }

        private IEnumerator StartGameCoroutine()
        {
            SetScore();

            yield return Constants.WaitForCertainSeconds(0.6f);
            
            DisableActionMaps();
            
            GetBackToOriginalPlace();

            yield return Constants.ChangeStatus(gameplayElements, true, PlayAudioOnEnable);

            RespawnBall();
        }

        private void SetScore()
        {
            rightPlayerPoints = PlayerPrefs.GetInt(Constants.RIGHT_BOUND);
            leftPlayerPoints = PlayerPrefs.GetInt(Constants.LEFT_BOUND);

            UpdateUI();
        }

        private void UpdateUI()
        {
            rightPlayersScoreLabel.text = leftPlayerPoints.ToString();
            leftPlayersScoreLabel.text = rightPlayerPoints.ToString();
        }

        public void DisableActionMaps()
        {
            leftPlayerAction.Disable();
            rightPlayerAction.Disable();
        }

        public void GetBackToOriginalPlace()
        {
            foreach (GameObject player in players)
                StartCoroutine(player.GetComponent<PlayerMovement>().GetBackToOriginalPosition());
        }

        private void RespawnBall()
        {
            DisableActionMaps();

            if (CheckIfSomebodyWin())
            {
                canPause = false;

                divider.SetActive(!divider.activeSelf);

                if (leftPlayerPoints == 10)
                    EventManager.RaiseOnGameWin(Constants.LEFT_BOUND);
                else
                    EventManager.RaiseOnGameWin(Constants.RIGHT_BOUND);

                return;
            }

            GetBackToOriginalPlace();

            StartCoroutine(WaitAndRespawnBall());
        }

        private IEnumerator WaitAndRespawnBall()
        {
            yield return CalculateWaitTimeWithPauses(2);

            EnableActionMaps();

            Instantiate(ballPrefab);

            canPause = true;
        }

        private IEnumerator CalculateWaitTimeWithPauses(float finalUnpauseTime)
        {
            float startTime = 0;

            while (startTime < finalUnpauseTime)
            {
                if (!isPaused)
                    startTime += Time.deltaTime;

                yield return null;
            }
        }

        public void EnableActionMaps()
        {
            leftPlayerAction.Enable();
            rightPlayerAction.Enable();
        }

        public void RestartGame()
        {
            StopAllCoroutines();

            StartCoroutine(RestartGameCoroutine());
        }

        public IEnumerator RestartGameCoroutine()
        {
            yield return Constants.WaitForCertainSeconds(Constants.ANIMATION_EXIT_TIME);

            EventManager.RaisePauseGame();

            DisablePausing();

            DeleteOnScreenBalls();

            ResetScore();
            SetScore();

            divider.SetActive(true);

            RespawnBall();
        }

        private void DisablePausing()
        {
            isPaused = false;
            canPause = false;
        }

        private void DeleteOnScreenBalls()
        {
            GameObject[] curretBalls = GameObject.FindGameObjectsWithTag(Constants.BALL_TAG);

            foreach (GameObject ball in curretBalls)
                Destroy(ball);
        }

        private void SetScorePlayerPrefs(int leftScore, int rightScore)
        {
            PlayerPrefs.SetInt(Constants.LEFT_BOUND, leftScore);
            PlayerPrefs.SetInt(Constants.RIGHT_BOUND, rightScore);
        }

        public void BackToMainMenu()
        {
            StopAllCoroutines();

            StartCoroutine(BackToMainMenuCoroutine());
        }

        public IEnumerator BackToMainMenuCoroutine()
        {
            DisablePausing();

            DeleteOnScreenBalls();

            DisableActionMaps();

            SetScorePlayerPrefs(leftPlayerPoints, rightPlayerPoints);

            yield return Constants.ChangeStatus(gameplayElementsReversed, false, PlayAudioOnEnable);

            EventManager.RaiseOnMainMenuBack();
        }

        private void AddPoint(GameObject gameObject)
        {
            if (gameObject.CompareTag(Constants.RIGHT_BOUND))
                rightPlayerPoints++;
            else
                leftPlayerPoints++;

            UpdateUI();
        }

        private bool CheckIfSomebodyWin()
        {
            return leftPlayerPoints == 10 || rightPlayerPoints == 10;
        }

        public bool CheckIfLeftIsAboutToWin()
        {
            return leftPlayerPoints == 9;
        }

        public bool CheckIfRightIsAboutToWin()
        {
            return rightPlayerPoints == 9;
        }

        public void GamePlayElementsSetActive(bool setActive)
        {
            if (isPaused)
                return;

            Constants.ChangeStatusInstantly(bordersAndPlayers, setActive);
        }

        public void PauseGame(InputAction.CallbackContext context)
        {
            if (!context.performed || !canPause) return;

            PauseOrUnpauseGame();
        }

        public void PauseOrUnpauseGame()
        {
            StartCoroutine(PauseOrUnpauseGameCoroutine());
        }

        public IEnumerator PauseOrUnpauseGameCoroutine()
        {
            yield return Constants.WaitForCertainSeconds(Constants.ANIMATION_EXIT_TIME);

            isPaused = !isPaused;
            canPause = !canPause;

            PlayAudioClip();

            divider.SetActive(!divider.activeSelf);

            EventManager.RaisePauseGame();
        }

        private void SetVFX(bool value)
        {
            VFXEnabled = value;
        }

        private void SetSFX(bool value)
        {
            SFXEnabled = value;
        }

        private void SetVolume(float value)
        {
            SFXVolume = value;
            audioSourceComponent.volume = value;
        }

        private void OnGoalEnterPreventPause(float num)
        {
            canPause = false;
        }

        public void ResetScore()
        {
            SetScorePlayerPrefs(0,0);
        }

        public void PlayAudioClip()
        {
            if (!SFXEnabled || !audioSourceComponent.enabled)
                return;

            audioSourceComponent.clip = onClickSound;
            audioSourceComponent.Play();
        }

        public void PlayAudioOnEnable()
        {
            if (!SFXEnabled || !audioSourceComponent.enabled)
                return;

            int randomSoundIndex = UnityEngine.Random.Range(0, onEnableSounds.Length);
            audioSourceComponent.clip = onEnableSounds[randomSoundIndex];
            audioSourceComponent.Play();
        }

    }
}