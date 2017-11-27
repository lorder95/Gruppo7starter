using Improbable.Unity;
using Improbable.Unity.Visualizer;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[WorkerType(WorkerPlatform.UnityClient)]
public class LockRotationCanvas : MonoBehaviour {
    
    Vector3 iniRot= new Vector3();
    private Camera cameraToLookAt;
	// Use this for initialization
	void Start () {
        cameraToLookAt = Camera.main;
        //iniRot = transform.eulerAngles;

	}
	
	// Update is called once per frame
	void Update () {
        //transform.eulerAngles = iniRot;

        Vector3 v = cameraToLookAt.transform.position - transform.position;
        v.x = v.z = 0.0f;
        
        transform.LookAt(cameraToLookAt.transform.position - v);
        transform.Rotate(0, 180, 0);
    }
}
