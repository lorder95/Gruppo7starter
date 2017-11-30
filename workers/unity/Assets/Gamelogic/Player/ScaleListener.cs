using Improbable.Core;
using Improbable.Unity.Visualizer;
using Improbable.Worker;
using Improbable.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Player;
using Improbable.Unity.Core.EntityQueries;
using Improbable.Unity.Core;
using Improbable.Collections;
using Improbable;
using Assets.Gamelogic.Core;

[WorkerType(WorkerPlatform.UnityWorker)]

public class ScaleListener : MonoBehaviour {
    [Require] private Scale.Reader ScaleReader;
    [Require] private Status.Writer StatusWriter;
    [Require] private PlayerData.Reader PlayerDataReader;

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
            Debug.LogWarning("value = " + v);
            if (v >= SimulationSettings.MaxScore) {
                Debug.LogWarning("if");

                ResetQuery();
                return;
            }
            transform.localScale = new Vector3(v, v, v);
        }

    }
    private void OnGameWinSuccess(Win response) {
        Debug.LogWarning("Gamewin command succeeded.");
    }

    private void OnGameWinFailure(ICommandErrorDetails response) {
        Debug.LogError("Failed to send GameWin command with error: " + response.ErrorMessage);
    }


    void ResetQuery() {

        Debug.LogWarning("called reset");
        var query = Query.HasComponent<ClientConnection>().ReturnOnlyEntityIds();

        Debug.LogWarning("queryied");
        SpatialOS.Commands.SendQuery(StatusWriter, query)
          .OnSuccess(result => {
              Debug.LogWarning("Found " + result.EntityCount + " nearby entities with a health component");
              if (result.EntityCount < 1) {
                  return;
              }
              Map<EntityId, Entity> resultMap = result.Entities;
              foreach (EntityId id in resultMap.Keys) {
                  Entity entity = SpatialOS.GetLocalEntity(id);
                  if (entity != null) {
                      Debug.LogWarning("entity found");

                      SpatialOS.Commands.SendCommand(StatusWriter, Status.Commands.GameWon.Descriptor, new Winner(PlayerDataReader.Data.name), id)
                          .OnSuccess(OnGameWinSuccess)
                          .OnFailure(OnGameWinFailure);


                      //StatusWriter.Send(new Status.Update().AddGameWon(new Win(gameObject.EntityId().ToString())));
                  }
              }

          })
              .OnFailure(errorDetails => Debug.LogWarning("Query failed with error: " + errorDetails));
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
