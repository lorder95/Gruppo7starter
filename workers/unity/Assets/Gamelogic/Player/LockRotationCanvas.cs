using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LockRotationCanvas : MonoBehaviour {
    
    Vector3 iniRot= new Vector3();
	// Use this for initialization
	void Start () {
        iniRot = transform.eulerAngles;

	}
	
	// Update is called once per frame
	void LateUpdate () {
        transform.eulerAngles = iniRot;
    }
}
