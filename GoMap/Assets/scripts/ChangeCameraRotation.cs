using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeCameraRotation : MonoBehaviour {

	//public Text gyroRotationRatex;

	// Use this for initialization
	void Start () {
		if(!Input.gyro.enabled){
			Input.gyro.enabled = true;
		}
	}
	
	// Update is called once per frame
	void Update () {
		
		if(Input.gyro.enabled){
			transform.rotation = new Quaternion (-Input.gyro.attitude.x, -Input.gyro.attitude.y, Input.gyro.attitude.z, Input.gyro.attitude.w);

		}
		//print (Input.gyro.rotationRate.x);
	}


}
