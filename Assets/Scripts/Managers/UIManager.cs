using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


namespace PING_PONG
{
    public class UIManager : MonoBehaviour
    {

        [SerializeField] private List<GameObject> mainMenuUI; 
        [SerializeField] private GameObject continueButton;

        [SerializeField] private GameObject optionsUI;
        [SerializeField] private GameObject colorUI;
        [SerializeField] private GameObject pauseUI;
        [SerializeField] private GameObject endgameUI;
        [SerializeField] private GameObject controlsUI;

        [SerializeField] private GameObject optionsBackButton;
        [SerializeField] private GameObject optionsControlsButton;

        private GameManager gameManager;

        private GameObject currentUI;

        private RectTransform optionsTransform;
        private float optionsUIPausePosition = 5f;

        [SerializeField] private TextMeshProUGUI endgameText;

        private bool mainMenuUIStatus = false;
        private bool continueStatus = false;


        private void Awake()
        {
            optionsTransform = optionsUI.GetComponent<RectTransform>();

            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void Start()
        {
            StartGame();
        }

        private void OnEnable()
        {
            EventManager.OnPauseGame += ShowOrHidePauseUI;
            EventManager.OnMainMenuReturn += ChangeUIState;
            EventManager.OnGameWin += ShowEndGameUI;
        }

        private void OnDisable()
        {
            EventManager.OnPauseGame -= ShowOrHidePauseUI;
            EventManager.OnMainMenuReturn -= ChangeUIState;
            EventManager.OnGameWin -= ShowEndGameUI;
        }

        private void StartGame()
        {
            StartCoroutine(StartingGame());
        }

        private IEnumerator StartingGame()
        {
            currentUI = pauseUI;

            yield return Constants.WaitForCertainSeconds(2);

            yield return ChangeState();
        }

        private IEnumerator ChangeState()
        {
            mainMenuUIStatus = !mainMenuUIStatus;

            UpdateButtonPositions();

            yield return Constants.ChangeStatus(mainMenuUI, mainMenuUIStatus, gameManager.PlayAudioOnEnable);

            if (mainMenuUIStatus == false)
                EventManager.RaiseStartGame();
        }

        private void UpdateButtonPositions()
        {
            SetContinueButtonStatus();

            AddOrRemoveContinueButton();

            UpdatePositions();
        }

        private void SetContinueButtonStatus()
        {
            if (PlayerPrefs.GetInt(Constants.LEFT_BOUND) == 0 && PlayerPrefs.GetInt(Constants.RIGHT_BOUND) == 0)
                continueStatus = false;

            else if (PlayerPrefs.GetInt(Constants.LEFT_BOUND) != 0 || PlayerPrefs.GetInt(Constants.RIGHT_BOUND) != 0)
                continueStatus = true;
        }

        private void AddOrRemoveContinueButton()
        {
            if (continueStatus && !mainMenuUI.Contains(continueButton))
                mainMenuUI.Insert(2, continueButton);

            else if (!continueStatus && mainMenuUI.Contains(continueButton))
                mainMenuUI.RemoveAt(2);
        }

        private void UpdatePositions()
        {
            for (int i = 1; i < mainMenuUI.Count; i++)
                mainMenuUI[i].GetComponent<ChangeButtonPosition>().UpdatePosition(continueStatus);
        }

        public void ChangeUIState()
        {
            StartCoroutine(ChangeState());
        }

        public void ExitGame()
        {

#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
            Application.Quit();
#endif
        }

        public void ShowOrHidePauseUI()
        {
            currentUI.SetActive(!currentUI.activeSelf);

            UpdateOptionsUIPosition();

            if (!currentUI.activeSelf)
                currentUI = pauseUI;
        }

        private void UpdateOptionsUIPosition()
        {
            if (optionsTransform.position.y == 0)
            {
                optionsTransform.position = new Vector3(0, optionsUIPausePosition, 25);
                optionsControlsButton.SetActive(false);
                optionsBackButton.transform.position = new Vector3(0f, -7, 25);
            }
            else
            {
                optionsTransform.position = new Vector3(0, 0, 25);
                optionsControlsButton.SetActive(true);
                optionsBackButton.transform.position = new Vector3(12.5f, -12, 25);
            }
        }

        public void GetBackToMainMenu()
        {
            StartCoroutine(GetBackToMainMenuCoroutine());
        }

        private IEnumerator GetBackToMainMenuCoroutine()
        {
            yield return Constants.WaitForCertainSeconds(Constants.ANIMATION_EXIT_TIME);

            EventManager.RaisePauseGame();
        }

        private void ShowEndGameUI(string goalTag)
        {
            StartCoroutine(ShowEndGameUICoroutine(goalTag));
        }

        private IEnumerator ShowEndGameUICoroutine(string goalTag)
        {
            currentUI = endgameUI;

            UpdateWinnersText(goalTag);

            yield return Constants.WaitForCertainSeconds(1f);

            EventManager.RaisePauseGame();
        }
        
        private void UpdateWinnersText(string goalTag)
        {

            if (goalTag.Equals(Constants.LEFT_BOUND))
                endgameText.text = "Right Player Wins!";
            else
                endgameText.text = "Left Player Wins!";
        }

    }
}