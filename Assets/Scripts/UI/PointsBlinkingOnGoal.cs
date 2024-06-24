using System.Collections;
using TMPro;
using UnityEngine;


namespace PING_PONG
{
    public class PointsBlinkingOnGoal : MonoBehaviour
    {

        [SerializeField] private TextMeshProUGUI leftText;
        [SerializeField] private TextMeshProUGUI rightText;
        private TextMeshProUGUI currentText;

        [SerializeField] private GameObject leftBound;
        [SerializeField] private GameObject rightBound;
        private float timeInterval = 0.2f;
        

        private void OnEnable()
        {
            EventManager.OnPointScore += BlinkOnGoal;
        }

        private void OnDisable()
        {
            EventManager.OnPointScore -= BlinkOnGoal;
        }

        private void BlinkOnGoal(GameObject boundHit)
        {
            StartCoroutine(BlinkOnGoalCoroutine(boundHit));
        }

        private IEnumerator BlinkOnGoalCoroutine(GameObject boundHit)
        {
            currentText = boundHit.CompareTag(leftBound.tag) ? rightText : leftText;

            for (int i = 0; i < 5; i++)
            {
                currentText.color *= 1.5f;

                yield return Constants.WaitForCertainSeconds(timeInterval);

                currentText.color /= 1.5f;

                yield return Constants.WaitForCertainSeconds(timeInterval);
            }
        }

    }
}