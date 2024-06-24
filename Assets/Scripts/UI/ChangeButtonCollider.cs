using System.Collections;
using UnityEngine;


namespace PING_PONG
{
    public class ChangeButtonCollider : MonoBehaviour
    {

        private Animator animatorComponent;
        private Collider2D colliderComponent;


        private void Awake()
        {
            animatorComponent = GetComponent<Animator>();
            colliderComponent = GetComponent<Collider2D>();
        }

        private void OnEnable()
        {
            colliderComponent.enabled = true;
        }

        public void DisableButton()
        {
            StartCoroutine(DisableButtonCoroutine());
        }

        private IEnumerator DisableButtonCoroutine()
        {
            colliderComponent.enabled = false;
            animatorComponent.SetTrigger(Constants.BUTTON_ANIMATOR_SHRINK);

            yield return Constants.WaitForCertainSeconds(Constants.ANIMATION_EXIT_TIME);
        }

    }
}