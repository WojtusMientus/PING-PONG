using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace PING_PONG
{
    public static class Constants
    {

        public const string PLAYERS_TAG = "Player";
        public const string BALL_TAG = "Ball";
        public const string GAME_MANAGER_TAG = "GameManager";
        public const string STARS_TAG = "Stars";
        public const string PARTICLES_TAG = "Particles";

        public const string LEFT_PLAYER_ACTION = "LeftPaddle";
        public const string RIGHT_PLAYER_ACTION = "RightPaddle";

        public const string LEFT_BOUND = "LeftBound";
        public const string RIGHT_BOUND = "RightBound";
        public const string GOAL_MANAGER_TAG = "GoalManager";

        public const float SEQUENTIONAL_ENABLING = 0.2f;

        public const string BUTTON_ANIMATOR_GROW = "Grow";
        public const string BUTTON_ANIMATOR_SHRINK = "Shrink";
        public const float ANIMATION_EXIT_TIME = 0.04f;

        public const string VFX_SPAWNRATE = "SpawnRate";
        public const string VFX_ORIGINAL_SPAWNRATE = "OriginalSpawnRate";

        public const string PLAYER_PREFS_COLOR = "SavedColor";
        public const string PLAYER_PREFS_VFX = "SavedVFX";
        public const string PLAYER_PREFS_SFX = "SavedSFX";
        public const string PLAYER_PREFS_VOLUME = "SavedVolume";
        public const string PLAYER_PREFS_FULLSCREEN = "SavedFullScreen";
        public const string PLAYER_PREFS_RESOLUTION = "SavedResolution";


        public static IEnumerator WaitForCertainSeconds(float seconds)
        {
            yield return new WaitForSeconds(seconds);
        }

        public static IEnumerator ChangeStatus(List<GameObject> objects, bool status, Action audioFunction)
        {
            for (int i = 0; i < objects.Count; i++)
            {
                if (Time.time > 2.1)
                    audioFunction();

                objects[i].SetActive(status);

                yield return new WaitForSeconds(Constants.SEQUENTIONAL_ENABLING);
            }
        }

        public static void ChangeStatusInstantly(List<GameObject> objects, bool status)
        {
            foreach (GameObject gameObject in objects)
                gameObject.SetActive(status);
        }

    }
}