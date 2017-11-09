using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;
using Improbable.Unity.Core;
using Improbable.Player;
using Assets.Gamelogic.Player;
using Improbable.Worker;

namespace Assets.GameLogic.Core {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HitPlayer : MonoBehaviour {

        [Require]
        private ClientConnection.Writer ClientConnectionWriter;

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.IsSpatialOsEntity()) {
                if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Player") {
                    Debug.LogWarning("Collision accepted with " + gameObject.GetSpatialOsEntity().PrefabName);
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
