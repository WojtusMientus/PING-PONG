using System.Collections;
using UnityEngine;

namespace PING_PONG
{
    public class ParticleBurst : MonoBehaviour
    {

        [SerializeField] private int numberOfParticles;
        private ParticleSystem particleBurst;


        private void Awake()
        {
            particleBurst = GetComponent<ParticleSystem>();
        }

        private void Start()
        {
            PlayParticles();
        }

        private void PlayParticles()
        {
            StartCoroutine(PlayParticlesCoroutine());
        }

        private IEnumerator PlayParticlesCoroutine()
        {
            particleBurst.Emit(numberOfParticles);

            yield return Constants.WaitForCertainSeconds(3f);

            Destroy(gameObject);
        }

    }
}