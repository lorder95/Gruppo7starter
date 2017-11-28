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
using Improbable.Unity.Core.EntityQueries;
using System.Collections;
using Assets.Gamelogic.UI;
using System.Collections.Generic;
using System;

namespace Assets.Gamelogic.Core {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class PlayerCreatingBehaviour : MonoBehaviour {
        [Require]
        private PlayerCreation.Writer PlayerCreationWriter;
        //[Require] private Status.Reader StatusReader;

        private int queryResult;
        private List<EntityId> ids;

        private void OnEnable() {
            Debug.LogWarning("Enabled playercreator");
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.RegisterAsyncResponse(OnCreatePlayer);
            queryResult = 0;
            StartCoroutine(SpawnWaves());
            StartCoroutine(CheckOnlinePlayers());
            StartCoroutine(UpdateScoreboard());
            ids = new List<EntityId>();

            //StatusReader.GameWonTriggered.Add(ResetAll);
        }

        private void OnDisable() {
            PlayerCreationWriter.CommandReceiver.OnCreatePlayer.DeregisterResponse();
            StopCoroutine(SpawnWaves());
            StopCoroutine(CheckOnlinePlayers());
            StopCoroutine(UpdateScoreboard());
            //StatusReader.GameWonTriggered.Remove(ResetAll);
        }

        private IEnumerator UpdateScoreboard() {
            while (true) {
                yield return new WaitForSeconds(1);
                List<KeyValuePair<string, int>> points = new List<KeyValuePair<string, int>>();
                ClearList();
                foreach (EntityId id in ids) {
                    float scale = SpatialOS.GetLocalEntityComponent<Scale>(id).Get().Value.s;
                    int point = (int)(scale * SimulationSettings.ScoreIncrement - (SimulationSettings.ScoreIncrement - 1));
                    //Debug.LogWarning("foreach: " + scale + " - " + point);
                    string name = SpatialOS.GetLocalEntityComponent<PlayerData>(id).Get().Value.name;
                    points.Add(new KeyValuePair<string, int>(name, point));
                }
                points.Sort((x, y) => y.Value.CompareTo(x.Value));
                ScoreRequest sr = new ScoreRequest();
                Improbable.Collections.List<ScoreEntry> topFive = new Improbable.Collections.List<ScoreEntry>();
                for (int i = 0; i < Math.Min(5, points.Count); i++) {
                    ScoreEntry entry = new ScoreEntry();
                    KeyValuePair<string, int> kvEntry = points[i];
                    entry.name = kvEntry.Key;
                    entry.value = (uint)kvEntry.Value;
                    topFive.Add(entry);
                    //Debug.LogWarning(entry.name + " - " + entry.value + " - " +kvEntry.ToString());
                }
                sr.points = topFive;


                foreach (EntityId id in ids) {
                    SpatialOS.Commands.SendCommand(PlayerCreationWriter, Scoreboard.Commands.SendScoreboard.Descriptor, sr, id);
                }

            }
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
                  Improbable.Collections.Map<EntityId, Entity> resultMap = result.Entities;
                  EntityId id = resultMap.First.Value.Key;
                  Entity entity = SpatialOS.GetLocalEntity(id);
                  if (entity != null) {
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
                      Debug.Log("Found " + result.EntityCount + " nearby entities with a ClientConnection component");
                      queryResult = result.EntityCount;


                  })
                      .OnFailure(errorDetails => Debug.Log("Query failed with error: " + errorDetails));


                yield return new WaitForSeconds(10);
            }
        }

        IEnumerator SpawnWaves() {

            Entity cubeEntityTemplate;
            while (true) {
                float time = 0.2f;
                if (queryResult > 0) {
                    if (queryResult < 3) {
                        time = 0.5f;
                        int v = UnityEngine.Random.Range(1, 3);
                        if (v == 1) {
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
                        } else {
                            //red cube
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate2();
                        }
                    } else if (queryResult < 11) {
                        int v = UnityEngine.Random.Range(1, 11);
                        if (v < 5) {
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
                        } else {
                            //red cube
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate2();
                        }
                    } else {
                        time = 0.05f;
                        int v = UnityEngine.Random.Range(1, 11);
                        if (v < 2) {
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
                        } else {
                            //red cube
                            cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate2();
                        }
                    }


                    SpatialOS.Commands.CreateEntity(PlayerCreationWriter, cubeEntityTemplate);
                }
                yield return new WaitForSeconds(time);
            }
        }
        private void OnCreatePlayer(ResponseHandle<PlayerCreation.Commands.CreatePlayer, CreatePlayerRequest, CreatePlayerResponse> responseHandle) {
            Debug.LogWarning("CreatePlayer: " + responseHandle.Request.playerColor + " - " + responseHandle.Request.playerName);
            var clientWorkerId = responseHandle.CallerInfo.CallerWorkerId;
            var playerEntityTemplate = EntityTemplateFactory.CreatePlayerTemplate(clientWorkerId, responseHandle.Request.playerColor, responseHandle.Request.playerName);
            SpatialOS.Commands.CreateEntity(PlayerCreationWriter, playerEntityTemplate)
                //.OnSuccess ( _ => responseHandle.Respond(new CreatePlayerResponse((int)StatusCode.Success)))
                .OnSuccess(response => responseHandle.Respond(AddEntity(response)))
                .OnFailure(failure => responseHandle.Respond(new CreatePlayerResponse((int)failure.StatusCode)));
            //var cubeEntityTemplate = EntityTemplateFactory.CreateCubeTemplate();
            //SpatialOS.Commands.CreateEntity(PlayerCreationWriter, cubeEntityTemplate);
        }

        private CreatePlayerResponse AddEntity(CreateEntityResult response) {
            EntityId playerId = response.CreatedEntityId;
            ids.Add(playerId);
            return new CreatePlayerResponse((int)StatusCode.Success);
        }

        private void ClearList() {
            List<EntityId> newIds = new List<EntityId>();
            foreach (EntityId id in ids) {
                Entity e = SpatialOS.GetLocalEntity(id);
                if (e != null) {
                    newIds.Add(id);
                }
            }
            ids = newIds;
        }
    }
}
