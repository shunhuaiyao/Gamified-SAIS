using UnityEngine;
using System.Collections;

public class RotateAround : MonoBehaviour {

	public GameObject aroundThisObject;
	public float speed = 50.0f;

	void Update() {
		transform.RotateAround(aroundThisObject.transform.position, Vector3.up, (1.0f+speed) * Time.deltaTime);
	}
}