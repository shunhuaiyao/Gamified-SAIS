using UnityEngine;
using System.Collections;

public class FitScreenSize : MonoBehaviour {

	// Use this for initialization
	void Start () {

		float height = (float) (Camera.main.orthographicSize * 2.0);
		float width = (float) (height * Screen.width / Screen.height);

		transform.localScale = new Vector3 (Screen.width, 1, Screen.height);
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
