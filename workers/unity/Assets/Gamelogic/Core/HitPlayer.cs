using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Unity.Core;
using Improbable.Player;
using Assets.Gamelogic.Player;
using Improbable.Worker;
using Improbable.Core;

namespace Assets.GameLogic.Core {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HitPlayer : MonoBehaviour {

        [Require]
        private ClientConnection.Writer ClientConnectionWriter;
        [Require] private Scale.Writer ScaleWriter;

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.IsSpatialOsEntity()) {
                if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Cube") {
                    Debug.LogWarning("0: Collision accepted from " + gameObject.GetSpatialOsEntity().PrefabName+ "with " + other.gameObject.GetSpatialOsEntity().PrefabName);
                    var current = ScaleWriter.Data.s;
                   
                    Debug.LogWarning("1: " + current + " - " + gameObject.transform.localScale.x);
                    var scaleUpdate = new Scale.Update()
                        .SetS(current + 0.2f);
                    Debug.LogWarning("2:");
                    ScaleWriter.Send(scaleUpdate);
                    Debug.LogWarning("3:");

                }
                if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Player") {
                    Debug.LogWarning("1: Collision accepted from " + gameObject.GetSpatialOsEntity().PrefabName + "with " + other.gameObject.GetSpatialOsEntity().PrefabName);
                    //SpatialOS.Commands.DeleteEntity(ClientConnectionWriter, gameObject.EntityId());
                    GameObject loser;
                    if(other.gameObject.transform.localScale.x > gameObject.transform.localScale.x) {
                        loser = gameObject;
                    } else if (other.gameObject.transform.localScale.x < gameObject.transform.localScale.x) {
                        loser = other.gameObject;
                    } else {
                        return;
                    }
                    SpatialOS.Commands.DeleteEntity(ClientConnectionWriter, loser.EntityId(), result => {
                        if (result.StatusCode != StatusCode.Success) {
                            Debug.Log("Failed to delete entity with error: " + result.ErrorMessage);
                            return;
                        }
                        Debug.Log("Deleted entity: " + result.Response.Value);
                    });
                }
            }
        }
            
    }
}
