using UnityEngine;
using System.Collections;
using GoMap;

public class AddTasks : MonoBehaviour {

	public GOMap goMap;
	public GameObject TaskPrefab;
	private GameObject[] tasks;

	// Use this for initialization
	void Awake () {
		//register this class for location notifications
		goMap.locationManager.onOriginSet += LoadData;
		goMap.locationManager.onLocationChanged += LoadData;
	}

	void LoadData (Coordinates currentLocation) {
		print (currentLocation.latitude.ToString() + currentLocation.longitude.ToString());
	}

	void Start () {
		tasks = new GameObject[1];
		for (int i = 0; i < 1; i++) {
			//tasks[i] = (GameObject)Instantiate(TaskPrefab);
			double lat = (double)24.79528617;
			double lng = (double)120.99055480;
			Coordinates coordinates = new Coordinates (lat, lng,0);
			tasks[i] = GameObject.Instantiate(TaskPrefab);
			tasks[i].transform.position = coordinates.convertCoordinateToVector(10);
			print (tasks[i].transform.position);
			tasks[i].transform.parent = transform;
			tasks[i].name = "Hambuger";
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
