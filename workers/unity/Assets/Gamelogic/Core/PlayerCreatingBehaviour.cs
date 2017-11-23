using Assets.Gamelogic.EntityTemplates;
using Improbable;
using Improbable.Entity.Component;
using Improbable.Core;
using Improbable.Unity;
using Improbable.Unity.Core;
using Improbable.Unity.Visualizer;
using UnityEngine;
using Improbable.Worker;
using Improbable.Player;
using Improbable.Collections;
using Improbable.Unity.Core.EntityQueries;
using System.Collections;
using Assets.Gamelogic.UI;

namespace Assets.Gamelogic.Core
{
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerCreatingBehaviour : MonoBehaviour
    {
        [Require]
        private PlayerCreation.Writer PlayerCreationWriter;
        //[Require] private Status.Reader StatusReader;

        private bool emptyRoom;

        private void OnEnable()
        {
            Debug.LogWarning("Enabled playercreator");
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterAsyncResponse(OnCreatePlayer);
            emptyRoom = true;
            StartCoroutine(SpawnWaves());
            StartCoroutine(CheckOnlinePlayers());
            //StatusReader.GameWonTriggered.Add(ResetAll);
        }

        private void OnDisable()
        {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
            StopCoroutine(SpawnWaves());
            StopCoroutine(CheckOnlinePlayers());
            //StatusReader.GameWonTriggered.Remove(ResetAll);
        }
        private void ResetAll(Win win) {
            Debug.LogWarning("Reset all");

            var query = Query.HasComponent<ClientConnection>().ReturnOnlyEntityIds();


            SpatialOS.Commands.SendQuery(PlayerCreationWriter, query)
              .OnSuccess(result => {
                  Debug.Log("Found " + result.EntityCount + " nearby entities with a health component");
                  if (result.EntityCount < 1) {
                      return;
                  }
                  Map<EntityId, Entity> resultMap = result.Entities;
                  EntityId id = resultMap.First.Value.Key;
                  Entity entity = SpatialOS.GetLocalEntity(id);
                  if(entity != null) {
                      Debug.Log("entity found");
                  }
                  //StatusWriter.Send(new Status.Update().AddGameOver(new GameOver()));
              })
              .OnFailure(errorDetails => Debug.Log("Query failed with error: " + errorDetails));
        }

        IEnumerator CheckOnlinePlayers() {
            yield return new WaitForSeconds(1);
            while (true) {
                var query = Query.HasComponent<ClientConnection>().ReturnOnlyEntityIds();
                SpatialOS.Commands.SendQuery(PlayerCreationWriter, query)
                  .OnSuccess(result => {
                      Debug.Log("Found " + result.EntityCount + " nearby entities with a health component");
                      if (result.EntityCount < 1) {
                          emptyRoom = true;
                      } else {
                          emptyRoom = false;
                      }
                      

                  })
                      .OnFailure(errorDetails => Debug.Log("Query failed with error: " + errorDetails));


                yield return new WaitForSeconds(10);
            }
        }

        IEnumerator SpawnWaves() {
            yield return new WaitForSeconds(2);
            while (true) {
                if(emptyRoom == false) {
                    var cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
                    SpatialOS.Commands.CreateEntity(PlayerCreationWriter, cubeEntityTemplate);
                }
                


                yield return new WaitForSeconds(0.5f);
            }
        }
        private void OnCreatePlayer(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle)
        {
            Debug.LogWarning("CreatePlayer: " + responseHandle.Request.playerColor + " - " + responseHandle.Request.playerName);
            var clientWorkerId = responseHandle.CallerInfo.CallerWorkerId;
            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId,responseHandle.Request.playerColor, responseHandle.Request.playerName);
            SpatialOS.Commands.CreateEntity (PlayerCreationWriter, playerEntityTemplate)
                .OnSuccess (_ => responseHandle.Respond (new CreatePlayerResponse ((int) StatusCode.Success)))
                .OnFailure (failure => responseHandle.Respond (new CreatePlayerResponse ((int) failure.StatusCode)));
            //var cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
            //SpatialOS.Commands.CreateEntity(PlayerCreationWriter, cubeEntityTemplate);
        }
    }
}
