using UnityEngine;
using UnityEngine.Events;

namespace PING_PONG
{
    public static class EventManager
    {

        public static event UnityAction<GameObject> OnSpawn;
        public static event UnityAction<GameObject> OnPointScore;
        public static event UnityAction<float> OnGoalEnter;
        public static event UnityAction OnRespawnBall;
        public static event UnityAction OnStartGame;
        public static event UnityAction<bool> OnVFXToggle;
        public static event UnityAction<int> OnColorButtonClick;
        public static event UnityAction OnPauseGame;
        public static event UnityAction OnMainMenuReturn;
        public static event UnityAction<string> OnGameWin;
        public static event UnityAction PlayAudio;
        public static event UnityAction<bool> OnSFXToggle;
        public static event UnityAction<float> OnVolumeChange;


        public static void RaiseOnSpawn(GameObject ballObject)
        {
            OnSpawn?.Invoke(ballObject);
        }

        public static void RaiseOnPoint(GameObject gameObject)
        {
            OnPointScore?.Invoke(gameObject);
        }

        public static void RaiseOnGoalEnter(float speedMultiplication)
        {
            OnGoalEnter?.Invoke(speedMultiplication);
        }

        public static void RaiseRespawnBall()
        {
            OnRespawnBall?.Invoke();
        }

        public static void RaiseStartGame()
        {
            OnStartGame?.Invoke();
        }

        public static void RaiseVFXToggle(bool value)
        {
            OnVFXToggle?.Invoke(value);
        }

        public static void RaiseColorButtonClick(int buttonID)
        {
            OnColorButtonClick?.Invoke(buttonID);
        }

        public static void RaisePauseGame()
        {
            OnPauseGame?.Invoke();
        }

        public static void RaiseOnMainMenuBack()
        {
            OnMainMenuReturn?.Invoke();
        }

        public static void RaiseOnGameWin(string goalTag)
        {
            OnGameWin?.Invoke(goalTag);
        }

        public static void RaiseOnPlayAudio()
        {
            PlayAudio?.Invoke();
        }

        public static void RaiseSFXToggle(bool value)
        {
            OnSFXToggle?.Invoke(value);
        }

        public static void RaiseVolumeChange(float volumeValue)
        {
            OnVolumeChange?.Invoke(volumeValue);
        }

    }
}