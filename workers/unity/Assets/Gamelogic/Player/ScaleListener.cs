using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using Improbable.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[WorkerType(WorkerPlatform.UnityWorker)]

public class ScaleListener : MonoBehaviour {
    [Require] private Scale.Reader ScaleReader;

    void OnEnable() {
        transform.localScale = new Vector3(ScaleReader.Data.s, ScaleReader.Data.s, ScaleReader.Data.s);
        ScaleReader.ComponentUpdated.Add(OnScaleUpdated);
    }

    void OnDisable() {
        ScaleReader.ComponentUpdated.Remove(OnScaleUpdated);
    }

    void OnScaleUpdated(Scale.Update update) {
        if (update.s.HasValue) {
            var v = update.s.Value;
            if (v == 10) {
                // win!
                //send event
            }
            transform.localScale = new Vector3(v, v, v);
        }
        
    }
}


/*
 * namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityClient)]
    public class CharacterDeathVisualizer : MonoBehaviour
    {
        [Require] private Health.Reader health;

        [SerializeField] private CharacterModelVisualizer characterModelVisualizer;

        private void Awake()
        {
            characterModelVisualizer = gameObject.GetComponentIfUnassigned(characterModelVisualizer);
        }

        private void OnEnable()
        {
            characterModelVisualizer.SetModelVisibility(true);
            health.ComponentUpdated.Add(HealthUpdated);
        }

        private void OnDisable()
        {
            health.ComponentUpdated.Remove(HealthUpdated);
        }

        private void HealthUpdated(Health.Update update)
        {
            if (update.currentHealth.HasValue && update.currentHealth.Value <= 0)
            {
                PlayDeathAnimation();
            }
        }

        private void PlayDeathAnimation()
        {
            DeathAnimVisualizerPool.ShowEffect(transform.position);
            characterModelVisualizer.SetModelVisibility(false);
        }
    }
}
*/