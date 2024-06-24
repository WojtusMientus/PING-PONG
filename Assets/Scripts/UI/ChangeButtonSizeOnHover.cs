using UnityEngine;


namespace PING_PONG
{
    public class ChangeButtonSizeOnHover : MonoBehaviour
    {

        private Animator animatorCmp;


        private void Awake()
        {
            animatorCmp = GetComponent<Animator>();
        }

        private void OnMouseEnter()
        {
            animatorCmp.SetTrigger(Constants.BUTTON_ANIMATOR_GROW);
        }

        private void OnMouseExit()
        {
            animatorCmp.SetTrigger(Constants.BUTTON_ANIMATOR_SHRINK);
        }

    }
}