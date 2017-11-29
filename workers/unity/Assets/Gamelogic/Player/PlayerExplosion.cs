using System.Collections;
using System.Collections.Generic;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using UnityEngine;

namespace Assets.Gamelogic
{
    // Add this MonoBehaviour on client workers only
    [WorkerType(WorkerPlatform.UnityClient)]
    public class PlayerExplosion : MonoBehaviour
    {

        // Inject access to the entity's Health component
        [SerializeField] private ParticleSystem expl;
        public bool explosion = false;



        public void OnEnable()

        {
            Debug.LogWarning("Explosion Worker");
            if(explosion)
                Explode();
        }



        private void OnDisable()

        {
            explosion = false;
            // Deregister callback for when components change

        }



        public void Explode()
        {
            expl.Play();
            if (expl.isPlaying)
            {
                Debug.LogWarning("Worker Explosion");
            }
            Invoke("Chiusura", expl.time + 1F);
        }

        void Chiusura()
        {

            expl.Stop();
            Debug.LogWarning("Stop Worker Explosion: Respawn");

        }

    }

}