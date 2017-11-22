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
using UnityEngine.SceneManagement;
using Assets.Gamelogic.Core;

namespace Assets.GameLogic.Core {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HitPlayer : MonoBehaviour {

        [Require]
        private ClientConnection.Writer ClientConnectionWriter;
        [Require] private Scale.Writer ScaleWriter;
        [Require] private Status.Writer StatusWriter;
        [SerializeField] private GameObject Model;
        private Rigidbody rigidbody;
        void OnEnable() {
            rigidbody = GetComponent<Rigidbody>();
            Model.GetComponent<MeshRenderer>().enabled = false;
        }

        private void OnCollisionEnter(Collision other) {
            if (other.gameObject.IsSpatialOsEntity()) {
                if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Cube") {
                    //Debug.LogWarning("0: Collision accepted from " + gameObject.GetSpatialOsEntity().PrefabName+ "with " + other.gameObject.GetSpatialOsEntity().PrefabName);
                    var current = ScaleWriter.Data.s;
                   
                    Debug.LogWarning("1: " + current + " - " + gameObject.transform.localScale.x);
                    var scaleUpdate = new Scale.Update()
                        .SetS(current + 0.2f);
                    Debug.LogWarning("2:");
                    ScaleWriter.Send(scaleUpdate);
                    Debug.LogWarning("3:");

                }
                if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Player") {
                    //Debug.LogWarning("1: Collision accepted from " + gameObject.GetSpatialOsEntity().PrefabName + "with " + other.gameObject.GetSpatialOsEntity().PrefabName);
                    //SpatialOS.Commands.DeleteEntity(ClientConnectionWriter, gameObject.EntityId());
                    if(other.gameObject.transform.localScale.x > gameObject.transform.localScale.x) {
                        //Model.SetActive(false);
                        //Debug.LogWarning("HERE 1");
                        //gameObject.GetComponentInChildren<Renderer>().enabled = false;
                        //Model.GetComponent<SphereCollider>().isTrigger = true;
                        //rigidbody.isKinematic = true;

                        //SceneManager.LoadSceneAsync(BuildSettings.SplashScreenScene, LoadSceneMode.Additive);
                        
                        Debug.LogWarning("Calling");

                        StatusWriter.Send(new Status.Update().AddPlayerDead(new Dead()));
                        /*SpatialOS.Commands.DeleteEntity(ClientConnectionWriter, gameObject.EntityId(), result => {
                        if (result.StatusCode != StatusCode.Success) {
                            Debug.Log("Failed to delete entity with error: " + result.ErrorMessage);
                            return;
                            }
                       Debug.Log("Deleted entity: " + result.Response.Value);
                       });*/
                    } else if (other.gameObject.transform.localScale.x < gameObject.transform.localScale.x) {
                    } else {
                        return;
                    }
                    
                   
                }
            }
        }
            
    }
}
