using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ChangeFoodLocation : MonoBehaviour {

	public Coordinates foodLocation;
	public Vector3 foodVector3;
	public GameObject food;
	//public Text foodV;

	//public Text localV;
	//public Coordinates local;
	//public Text localC;

	// Use this for initialization
	void Start () {


	}
	
	// Update is called once per frame
	void Update () {
		foodLocation = new Coordinates (24.79528617, 120.99055480,0);
		foodVector3 = foodLocation.convertCoordinateToVector ();
		food.transform.position = foodVector3;
//		foodV.text = food.transform.position.ToString ();
//
//		local = new Coordinates (Input.location.lastData);
//		localC.text = local.latitude.ToString () + " " + local.longitude.ToString ();
//		localV.text = local.convertCoordinateToVector ().ToString();

	}
}
