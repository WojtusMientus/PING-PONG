using System.Collections;
using Cinemachine;
using UnityEngine;


namespace PING_PONG
{
    public class BallMovement : MonoBehaviour
    {

        private Rigidbody2D ballRigidbody;

        [SerializeField] private float originalSpeed;
        private Vector2 ballVelocity = Vector2.zero;

        private float maxSpawnAngle = 35f;
        private int[] sign = new int[2] { -1, 1 };

        [SerializeField] private CinemachineVirtualCamera ballCamera;
        [SerializeField] private GameObject particleSmallBurst;
        [SerializeField] private GameObject particleBigBurst;
        [SerializeField] private ParticleSystem particleTailSystem;

        private GameManager gameManager;

        private float scale = -0.5f;

        private AudioSource audioSourceComponent;
        [SerializeField] private AudioClip[] ballBounceClip;


        private void Awake()
        {
            ballRigidbody = GetComponent<Rigidbody2D>();
            audioSourceComponent = GetComponent<AudioSource>();

            gameManager = GameObject.FindGameObjectWithTag(Constants.GAME_MANAGER_TAG).GetComponent<GameManager>();
        }

        [System.Obsolete]
        void Start()
        {
            if (GameObject.FindGameObjectsWithTag(Constants.BALL_TAG).Length > 1)
                Destroy(gameObject);

            if (audioSourceComponent.isActiveAndEnabled)
                PlaySound();

            EventManager.RaiseOnSpawn(gameObject);

            StartCoroutine(SetVelocityCoroutine());
        }

        [System.Obsolete]
        private void OnEnable()
        {
            EventManager.OnGoalEnter += SetSpeed;
            EventManager.OnPauseGame += PauseBall;
            EventManager.OnVFXToggle += ClearTailParticles;
        }

        [System.Obsolete]
        private void OnDisable()
        {
            EventManager.OnGoalEnter -= SetSpeed;
            EventManager.OnPauseGame -= PauseBall;
            EventManager.OnVFXToggle -= ClearTailParticles;
        }

        [System.Obsolete]
        private IEnumerator SetVelocityCoroutine()
        {
            yield return CalculateWaitTimeWithPauses(1);

            ballRigidbody.velocity = CalculateInitialVelocity();

            if (gameManager.VFXEnabled)
                SetTailParticleRotationAndEmmision();
        }

        private IEnumerator CalculateWaitTimeWithPauses(float finalUnpauseTime)
        {
            float startTime = 0;

            while (startTime < finalUnpauseTime)
            {
                if (!gameManager.isPaused)
                    startTime += Time.deltaTime;

                yield return null;
            }
        }

        private Vector2 CalculateInitialVelocity()
        {
            float randomAngle = Random.Range(-maxSpawnAngle, maxSpawnAngle);
            Vector2 directionVector = sign[Random.Range(0, 2)] * Vector2.right;

            Vector2 finalVelocity = Quaternion.AngleAxis(randomAngle, Vector3.forward).normalized * directionVector * originalSpeed;

            return finalVelocity;
        }

        [System.Obsolete]
        private void SetTailParticleRotationAndEmmision()
        {
            if (gameManager.VFXEnabled)
                particleTailSystem.Play();

            Vector2 lookAtVector2D = ballRigidbody.velocity.normalized * scale;

            Vector3 lookAtVector3D = new Vector3(lookAtVector2D.x, lookAtVector2D.y, 20);

            UpdateTailParticleRotationAndEmission(lookAtVector3D);
        }

        [System.Obsolete]
        private void UpdateTailParticleRotationAndEmission(Vector3 lookAtVector3D)
        {
            particleTailSystem.transform.LookAt(lookAtVector3D);

            particleTailSystem.emissionRate = ballRigidbody.velocity.magnitude / 2;
        }

        private void SetSpeed(float speedMultiplier)
        {
            ballRigidbody.velocity *= speedMultiplier;
        }

        [System.Obsolete]
        private void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.gameObject.CompareTag(Constants.PLAYERS_TAG))
                AddRandomForce();
            else
                UpdateTailParticleRotationAndEmission(CalculateParticlePosition());

            PlaySoundOnCollision(collision);

            PlayVFXOnCollision(collision);
        }

        [System.Obsolete]
        private void AddRandomForce()
        {
            float minAngle = 30;
            float maxAngle = 50;
            float randomForceSpeedMultiplier = ballRigidbody.velocity.magnitude / 2.5f;
            float currentSpeedMultiplier = 0.75f;

            Vector2 normalizedVelocity = ballRigidbody.velocity.normalized;
            float randomAngle = 0f;

            if (Random.Range(0f, 1f) < 0.5f)
                randomAngle = Random.Range(-maxAngle, -minAngle);
            else
                randomAngle = Random.Range(minAngle, maxAngle);

            Vector2 randomForce = Quaternion.AngleAxis(randomAngle, Vector3.forward) * normalizedVelocity;

            randomForce *= randomForceSpeedMultiplier;

            ballRigidbody.velocity *= currentSpeedMultiplier;

            ballRigidbody.AddForce(randomForce, ForceMode2D.Impulse);

            UpdateTailParticleRotationAndEmission(CalculateParticlePosition());
        }

        private void PlaySoundOnCollision(Collision2D collision)
        {
            if (!collision.gameObject.CompareTag(Constants.LEFT_BOUND) && !collision.gameObject.CompareTag(Constants.RIGHT_BOUND))
                PlaySound();
        }

        private void PlaySound()
        {
            if (!gameManager.SFXEnabled)
                return;

            audioSourceComponent.volume = gameManager.SFXVolume;

            int index = Random.Range(0, ballBounceClip.Length);
            audioSourceComponent.clip = ballBounceClip[index];
            audioSourceComponent.Play();
        }

        private void PlayVFXOnCollision(Collision2D collision)
        {
            if (gameManager.VFXEnabled)
            {
                if (CheckIfSomebodyIsAboutToWin() && collision.gameObject.CompareTag(Constants.GOAL_MANAGER_TAG))
                    SpawnBigBurst(collision);
                else
                    SpawnSmallBurst(collision);
            }
        }

        private bool CheckIfSomebodyIsAboutToWin()
        {
            return (gameManager.CheckIfLeftIsAboutToWin() && transform.position.x < 0) || (gameManager.CheckIfRightIsAboutToWin() && transform.position.x > 0);
        }

        private void SpawnSmallBurst(Collision2D collision)
        {
            Vector3 collisionPoint = CalculateParticlePosition();

            Vector3 rotation = CalculateParticleRotation(collision);

            Instantiate(particleSmallBurst, collisionPoint, Quaternion.Euler(rotation));
        }

        private Vector3 CalculateParticlePosition()
        {
            Vector2 collisionPosition = ballRigidbody.velocity.normalized * scale;

            Vector3 collisionPoint = new Vector3(transform.position.x + collisionPosition.x, transform.position.y + collisionPosition.y, 20);

            return collisionPoint;
        }

        private Vector3 CalculateParticleRotation(Collision2D collision)
        {
            Vector3 rotation = new Vector3();

            float rotationAngle = 45f;

            if (collision.gameObject.CompareTag(Constants.PLAYERS_TAG) || WasCollisionWithGoals(collision))
            {
                if (transform.position.x < 0)
                    rotation.y = rotationAngle;
                else
                    rotation.y = -rotationAngle;
            }

            return rotation;
        }

        private void SpawnBigBurst(Collision2D collision)
        {
            Vector3 collisionPoint = CalculateParticlePosition();

            Vector3 rotation = CalculateParticleRotation(collision);

            Instantiate(particleBigBurst, collisionPoint, Quaternion.Euler(rotation));
        }

        private bool WasCollisionWithGoals(Collision2D collision)
        {
            return collision.gameObject.CompareTag(Constants.GOAL_MANAGER_TAG);
        }

        [System.Obsolete]
        private void PauseBall()
        {
            Vector2 helpVector = ballRigidbody.velocity;
            ballRigidbody.velocity = ballVelocity;
            ballVelocity = helpVector;

            ManageVFXOnPause();
        }

        private void ManageVFXOnPause()
        {
            particleTailSystem.Pause();

            if (gameManager.VFXEnabled && !gameManager.isPaused)
                particleTailSystem.Play();
        }

        private void ClearTailParticles(bool value)
        {
            if (value)
                return;

            particleTailSystem.Stop();
            particleTailSystem.Clear();
        }

    }
}