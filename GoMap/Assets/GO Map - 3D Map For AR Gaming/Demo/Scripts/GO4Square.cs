using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using Assets;

//This class uses Foursquare webservice API. 
//It's made for demo purpose only, and needs your personal foursquare API Key. 
//(No credit card is required, visit https://developer.foursquare.com/docs/venues/search)

//Full list of venues categories is given by this API https://api.foursquare.com/v2/venues/categories&oauth_token=

namespace GoMap {
	
	public class GO4Square : MonoBehaviour {

		public LocationManager locationManager;
		public string baseUrl = "https://api.foursquare.com/v2/venues/search?v=20160914";
		public string categoryID;
		public string oauth_token;
		public GameObject Hambuger, Icecream, Cake, Donuts, Milk, Hamegg, Puppy;
		public float queryRadius;
		public Task FinalTarget;
		//public Text debudtext;

		private float SEARCH_RANGE;
		Coordinates lastQueryCenter = null;

		//https://api.foursquare.com/v2/venues/search?ll=40.7,-74&radius=1000&v=20160914

		// Use this for initialization
		void Awake () {

			if (oauth_token.Length == 0) {
				Debug.LogWarning ("GO4Square - FORSQUARE OAUTH TOKEN IS REQUIRED, GET IT HERE: https://developer.foursquare.com/docs/venues/search");
				return;
			}

			//register this class for location notifications
			locationManager.onOriginSet += LoadData;
			locationManager.onLocationChanged += LoadData;


		}

		void Start(){
			if (Application.loadedLevelName == "OpenCamera")
				SEARCH_RANGE = 30f;
			else
				SEARCH_RANGE = 10f;
		}

		void Update(){
			if (Application.loadedLevelName == "OpenCamera") {
				if (PlayerPrefs.GetString ("taskID") != "" && PlayerPrefs.GetInt ("Gotcha") == 0) {
					FinalTarget = initFinalTargetPosition ();
					//FinalTarget.position.z = 25;
					Puppy.GetComponent<SeekFood> ().FinalTarget = FinalTarget.position;
                    
					StartCoroutine (delayTwoSeconds (3));
					if ( FinalTarget.ID > 0 && GameObject.Find(FinalTarget.ID.ToString()) == null && PlayerPrefs.GetInt ("SeekFood") == 1) {
						GameObject go = GameObject.Instantiate (correspondingPrefab(FinalTarget.taskFood));
						go.transform.localPosition = FinalTarget.position;
						go.transform.localScale = new Vector3 (20f, 20f, 20f);
						go.transform.parent = transform;
						if (Application.loadedLevelName == "OpenCamera") {
							go.transform.rotation = Quaternion.Euler (0, 90, -90);
						}
						go.name = FinalTarget.ID.ToString();
						PlayerPrefs.SetInt ("SeekFood", 0);
					}
				}
				else {
					FinalTarget.ID = 0;
				}
			}
		}
			
		void LoadData (Coordinates currentLocation) {//This is called when the location changes

			Coordinates CharacterCoordinates = new Coordinates (currentLocation.latitude, currentLocation.longitude, 0);
			//float distance = Vector3.Distance (FinalTarget.position, CharacterCoordinates.convertCoordinateToVector (10));

			//debudtext.text = distance.ToString () + FinalTarget.taskName;
			if (Puppy.GetComponent<SeekFood>().isDonePathGenerator && FinalTarget.ID > 0 && GameObject.Find(FinalTarget.ID.ToString()) == null) {
				
				GameObject go = GameObject.Instantiate (correspondingPrefab(FinalTarget.taskFood));
				go.transform.localPosition = Puppy.GetComponent<SeekFood>().foodPosition;
				go.transform.parent = transform;
				if (Application.loadedLevelName == "OpenCamera") {
					go.transform.rotation = Quaternion.Euler (0, 90, -90);
				}
				go.name = FinalTarget.ID.ToString();

                //for peepee
                Puppy.GetComponent<startTracking>().enabled = false;

            }

			//print ("??????????????"+go.transform.position.ToString());

//			if (lastQueryCenter == null || lastQueryCenter.DistanceFromPoint (currentLocation) >= queryRadius/1.5f) {
//				lastQueryCenter = currentLocation;
//				string url = baseUrl + "&ll=" + currentLocation.latitude + "," + currentLocation.longitude + "&radius=" + queryRadius+"&categoryId="+categoryID+"&oauth_token="+oauth_token;
//				StartCoroutine (LoadPlaces(url));
//			}
		}

		Task initFinalTargetPosition(){
			Coordinates Coordinates = new Coordinates (double.Parse(PlayerPrefs.GetString("taskLat")), double.Parse(PlayerPrefs.GetString("taskLon")), 0);
			Task task = new Task ();
			task.ID = long.Parse(PlayerPrefs.GetString ("taskID"));
			task.taskName = PlayerPrefs.GetString ("taskName");
			task.taskFood = PlayerPrefs.GetString ("taskFood");
            //task.position = Coordinates.convertCoordinateToVector (10);
            // for AR scene
            Vector3 directionVector = new Vector3(PlayerPrefs.GetFloat("directionVectorX"), PlayerPrefs.GetFloat("directionVectorY"), PlayerPrefs.GetFloat("directionVectorZ"));
            task.position = GameObject.Find("Main Camera").GetComponent<Transform>().position + directionVector * 20;
            print("oooo" + directionVector.ToString() + task.position.ToString());
            //
            task.latitude = double.Parse (PlayerPrefs.GetString ("taskLat"));
			task.longitude = double.Parse(PlayerPrefs.GetString("taskLon"));
			//Debug.Log ("IDIDIDID"+task.ID.ToString());
			return task;
		}

		private GameObject correspondingPrefab(string foodName){

			GameObject prefab = Hambuger;
			switch (foodName) {
			case "Hambuger":
				prefab = Hambuger;
				break;
			case "Ice Cream":
				prefab = Icecream;
				break;
			case "Cake":
				prefab = Cake;
				break;
			case "Donuts":
				prefab = Donuts;
				break;
			case "Milk":
				prefab = Milk;
				break;
			case "HamEgg":
				prefab = Hamegg;
				break;
			case "icecream":		//for material
				prefab = Icecream;
				break;
			case "Hamegg":			//for material
				prefab = Hamegg;
				break;
			default:
				break;
			}
			return prefab;
		
		}

		IEnumerator delayTwoSeconds (float delay){
			yield return new WaitForSeconds(delay);
			Puppy.GetComponent<SeekFood> ().enabled = true;
		}
//		public IEnumerator LoadPlaces (string url) { //Request the API
//
//			Debug.Log ("GO4Square URL: " + url);
//
//			var www = new WWW(url);
//			yield return www;
//
//			ParseJob job = new ParseJob();
//			job.InData = www.text;
//			job.Start();
//
//			yield return StartCoroutine(job.WaitFor());
//		
//			IDictionary response = (IDictionary)((IDictionary)job.OutData)["response"];
//			IList results = (IList)response ["venues"];
//
////			foreach (Transform child in transform) {
////				GameObject.Destroy (child.gameObject);
////			}
//			if (results != null) {
//				foreach (IDictionary result in results) {//This example only takes GPS location and the name of the object. There's lot more, take a look at the Foursquare API documentation
//
//					IDictionary location = ((IDictionary)result ["location"]);
//					//			double lat = (double)location ["lat"];
//					//			double lng = (double)location ["lng"];
//					double lat = (double)24.79528617;
//					double lng = (double)120.99055480;
//					//print ("ham:"+lat.ToString()+lng.ToString());
//					//			GameObject go = GameObject.Instantiate (prefab);
//					//			go.name = (string)result["name"];
//					//			goMap.dropPin (lat, lng, go);
//
////					Coordinates coordinates = new Coordinates (lat, lng, 0);
////					GameObject go = GameObject.Instantiate (prefab);
////					go.transform.localPosition = coordinates.convertCoordinateToVector (10);
////					go.transform.parent = transform;
////					if (Application.loadedLevelName == "OpenCamera") {
////						go.transform.rotation = Quaternion.Euler (-90, 0, 0);
////					}
////					go.name = (string)result ["name"];
//
//				}
//			}
//		}
	}
}
