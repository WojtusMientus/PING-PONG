using System.Collections;
using Cinemachine;
using UnityEngine;


namespace PING_PONG
{
    public class GoalManager : MonoBehaviour
    {

        [SerializeField] private CinemachineVirtualCamera mainCamera;
        private CinemachineVirtualCamera ballCamera = null;
        private CinemachineBasicMultiChannelPerlin ballNoise = null;

        private Rigidbody2D ballRigidbody = null;

        private AudioSource audioSourceComponent;
        private GameManager gameManager;

        [SerializeField] private GameObject leftBound;
        [SerializeField] private GameObject rightBound;

        private float timeInSlowMotion = 2.5f;
        private float slowedTimeSpeed;


        private void Awake()
        {
            audioSourceComponent = GetComponent<AudioSource>();

            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        private void OnEnable()
        {
            EventManager.OnSpawn += SetCameraAndRigidbody;
        }

        private void OnDisable()
        {
            EventManager.OnSpawn -= SetCameraAndRigidbody;
        }

        private void SetCameraAndRigidbody(GameObject ballObject)
        {
            ballCamera = ballObject.GetComponentInChildren<CinemachineVirtualCamera>();
            ballNoise = ballCamera.GetCinemachineComponent<CinemachineBasicMultiChannelPerlin>();

            ballRigidbody = ballObject.GetComponent<Rigidbody2D>();
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.CompareTag(Constants.BALL_TAG))
            {
                float directionMultiplier = 1 + (2 * Mathf.Abs(ballRigidbody.velocity.normalized.y));

                slowedTimeSpeed = timeInSlowMotion / ballRigidbody.velocity.magnitude * directionMultiplier;

                SetTimeScaleAndActiveCamera(slowedTimeSpeed);
            }
        }

        private void SetTimeScaleAndActiveCamera(float speedMultiplication)
        {
            EventManager.RaiseOnGoalEnter(speedMultiplication);
            mainCamera.enabled = !mainCamera.enabled;
            ballCamera.enabled = !ballCamera.enabled;
        }

        private void OnTriggerExit2D(Collider2D collision)
        {
            if (collision.CompareTag(Constants.BALL_TAG))
                StartCoroutine(ExitingGoalCoroutine());
        }

        private IEnumerator ExitingGoalCoroutine()
        {
            yield return Constants.WaitForCertainSeconds(1.2f); 

            if (GameObject.FindGameObjectWithTag(Constants.BALL_TAG) != null)
                SetTimeScaleAndActiveCamera(1 / slowedTimeSpeed);
        }

        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Constants.BALL_TAG))
            {
                PlaySound();

                RaiseEventOnPoint(collision);

                SetTimeScaleAndActiveCamera(1 / slowedTimeSpeed);

                StartCoroutine(DestroyBall(collision.gameObject));

                EventManager.RaiseRespawnBall();
            }
        }

        private void PlaySound()
        {
            if (gameManager.SFXEnabled)
            {
                audioSourceComponent.volume = gameManager.SFXVolume;
                audioSourceComponent.Play();
            }
        }

        private void RaiseEventOnPoint(Collision2D collision)
        {
            if (collision.gameObject.transform.position.x < 0)
                EventManager.RaiseOnPoint(leftBound);
            else
                EventManager.RaiseOnPoint(rightBound);
        }

        private IEnumerator DestroyBall(GameObject gameObject)
        {
            AddShakeNoise();

            gameObject.SetActive(false);

            yield return new WaitForSeconds(1);

            ResetBallComponents();

            Destroy(gameObject);
        }

        private void AddShakeNoise()
        {
            ballNoise.m_AmplitudeGain = 10f;
            ballNoise.m_FrequencyGain = 10f;
        }

        private void ResetBallComponents()
        {
            ballCamera = null;
            ballNoise = null;
            ballRigidbody = null;
        }
    }
}