using System.Collections;
using UnityEngine;

namespace PING_PONG
{
    public class SwapUI : MonoBehaviour
    {

        [SerializeField] private GameObject currentUI;
        [SerializeField] private GameObject nextUI;
        [SerializeField] private GameObject mainMenuTitle;

        private GameManager gameManager;

        [SerializeField] private bool hideTitle;


        private void Awake()
        {
            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        public void ChangeUI()
        {
            StartCoroutine(ChangeUICoroutine());
        }

        private IEnumerator ChangeUICoroutine()
        {
            yield return Constants.WaitForCertainSeconds(Constants.ANIMATION_EXIT_TIME);

            if (hideTitle && !gameManager.isPaused)
                mainMenuTitle.SetActive(!mainMenuTitle.activeSelf);

            currentUI.SetActive(false);
            nextUI.SetActive(true);
        }

        public void SetNextUI(GameObject UI)
        {
            nextUI = UI;
        }

    }
}