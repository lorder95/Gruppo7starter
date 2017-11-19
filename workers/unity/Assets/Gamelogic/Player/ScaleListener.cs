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
            var old = transform.localScale.x;
            transform.localScale = new Vector3(v, v, v);
        }
        
    }
}
