using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PING_PONG
{
    public class PlayerMovement : MonoBehaviour
    {

        [SerializeField] private float originalSpeed;
        private float swapSpeed = 0f;

        private Rigidbody2D playersRigidbody;
        private float inputValue;

        private float upperBound = 14.7f;
        private float lowerBound = -12.7f;

        [SerializeField] private Vector3 originalPosition;
        private float lerpTime = 2f;


        private void Awake()
        {
            playersRigidbody = GetComponent<Rigidbody2D>();
        }

        private void FixedUpdate()
        {
            MovePlayer();
        }

        private void OnEnable()
        {
            EventManager.OnGoalEnter += SetSpeed;
            EventManager.OnPauseGame += PauseGame;
        }

        private void OnDisable()
        {
            EventManager.OnGoalEnter -= SetSpeed;
            EventManager.OnPauseGame -= PauseGame;
        }

        private void MovePlayer()
        {
            HandleOutOfBounds();

            Vector2 playersVelocity = new Vector2(0, inputValue) * originalSpeed * Time.deltaTime;
            playersRigidbody.velocity = playersVelocity;
        }

        private void HandleOutOfBounds()
        {
            if (inputValue > 0 && transform.position.y >= upperBound)
            {
                inputValue = 0;
                transform.position = new Vector3(transform.position.x, upperBound, transform.position.z);
            }

            if (inputValue < 0 && transform.position.y <= lowerBound)
            {
                inputValue = 0;
                transform.position = new Vector3(transform.position.x, lowerBound, transform.position.z);
            }
        }

        public void HandlePlayerMove(InputAction.CallbackContext context)
        {
            inputValue = context.ReadValue<float>();
        }

        private void SetSpeed(float speedMultiplier)
        {
            originalSpeed *= speedMultiplier;
        }

        public IEnumerator GetBackToOriginalPosition()
        {
            float startTime = 0f;

            while (startTime < lerpTime)
            {
                transform.position = Vector3.Lerp(transform.position, originalPosition, startTime / lerpTime);
                startTime += Time.deltaTime;
                yield return null;
            }

            transform.position = originalPosition;
        }

        private void PauseGame()
        {
            float helpSpeed = originalSpeed;
            originalSpeed = swapSpeed;
            swapSpeed = helpSpeed;
        }

    }
}