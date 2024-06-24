using TMPro;
using UnityEngine;

namespace PING_PONG
{
    public class OptionsBackButtonChangeName : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI buttonText;

        private SwapUI swapUI;
        private GameManager gameManager;

        [SerializeField] private GameObject mainMenuUI;
        [SerializeField] private GameObject pauseUI;


        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();
            swapUI = GetComponent<SwapUI>();
        }

        private void OnEnable()
        {
            ChangeText();
        }

        private void ChangeText()
        {
            if (gameManager.isPaused)
            {
                buttonText.text = "Back to pause";
                swapUI.SetNextUI(pauseUI);
            }
            else
            {
                buttonText.text = "Back to menu";
                swapUI.SetNextUI(mainMenuUI);
            }
        }

    }
}