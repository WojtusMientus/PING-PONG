using UnityEngine;


namespace PING_PONG
{
    public class ChangeButtonPosition : MonoBehaviour
    {

        [SerializeField] private Vector3 continueButtonPosition;
        [SerializeField] private Vector3 originalButtonPosition;

        public void UpdatePosition(bool continueStatus)
        {
            if (continueStatus)
                transform.position = continueButtonPosition;
            else
                transform.position = originalButtonPosition;
        }

    }
}