using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Improbable.Unity;
using Improbable.Unity.Visualizer;

namespace Assets.GameLogic.Core {
    [WorkerType(WorkerPlatform.UnityWorker)]
    public class HitPlayer : MonoBehaviour {
        private void OnCollisionEnter(Collision other) {
            if (other != null && other.gameObject.GetSpatialOsEntity().PrefabName == "Player") {
                Debug.LogWarning("Collision accepted with " + gameObject.GetSpatialOsEntity().PrefabName);

            }
        }
    }
}
