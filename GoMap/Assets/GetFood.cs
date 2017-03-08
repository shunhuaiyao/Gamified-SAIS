using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;
using System;
using GoMap;

public class GetFood : MonoBehaviour {

	public Text hint, gotcha, debug;
	public GameObject hintPanel, gotchaPanel;
	private float point;
	bool startTimer = true;
	private GameObject locationObject;
	private LocationManager locationManager;
	// Use this for initialization
	void Start () {
		string msg = "Follow your puppy!!";
		StartCoroutine(ShowMessage(msg, 2));
		locationObject = GameObject.Find ("LocationManager");
		locationManager = (LocationManager) locationObject.GetComponent (typeof(LocationManager));
		point = 0;
		lastLocation = locationManager.currentLocation;
		lastGYRO = Input.gyro.attitude;
	}
	
	// Update is called once per frame
	void Update () {
		bool click = Input.touchCount == 1 && Input.GetTouch (0).phase == TouchPhase.Began;
		if (click) {
			RaycastHit hit = new RaycastHit ();
			Ray ray = Camera.main.ScreenPointToRay (Input.GetTouch (0).position);
			if (Physics.Raycast (ray, out hit) /*&& !IsPointerOverUIObject(canvas,/*Input.GetTouch(i).positionInput.mousePosition)*/) {
				GameObject clickedObject = hit.transform.gameObject as GameObject;
				string clickedMaterialName = clickedObject.GetComponent<MeshRenderer> ().materials [0].name;
				if (clickedMaterialName == "Hambuger (Instance)" || clickedMaterialName == "icecream (Instance)" || clickedMaterialName == "Cake (Instance)" || clickedMaterialName == "Donuts (Instance)" || clickedMaterialName == "Hamegg (Instance)" || clickedMaterialName == "Milk (Instance)") {
					Destroy (clickedObject);
					string msg = "Gotcha!!";
					PlayerPrefs.SetInt ("Gotcha", 1);
					PlayerPrefs.SetInt ("SeekFood", 0);
					StartCoroutine (ShowTaskMessage (msg, 2));
				}
			}
		} 
		if (startTimer) {
			StartCoroutine (timer());
			startTimer = false;
		}
	}

	Coordinates lastLocation;
	Quaternion lastGYRO;
	IEnumerator timer() {
		yield return new WaitForSeconds (1);
		Coordinates location = locationManager.currentLocation;
		point += (float)Math.Abs (location.latitude - lastLocation.latitude) + (float)Math.Abs (location.longitude - lastLocation.longitude);
		point += (float)Math.Abs (Input.gyro.attitude.x - lastGYRO.x) + (float)Math.Abs (Input.gyro.attitude.y - lastGYRO.y) + (float)Math.Abs (Input.gyro.attitude.z - lastGYRO.z);
		lastLocation = location;
		lastGYRO = Input.gyro.attitude;
		startTimer = true;
	}

	IEnumerator ShowTaskMessage (string message, float delay) {
		Everyplay.StopRecording();
		/*if (!CheckInternet.check ()) {
			Debug.Log ("internet not");
			gotcha.text = "Internet Unreachable";
			gotcha.enabled = true;
			gotchaPanel.GetComponent<Image> ().enabled = true;
			yield return new WaitForSeconds (delay);
			gotcha.enabled = false;
			gotchaPanel.GetComponent<Image> ().enabled = false;
		} else {*/
			string directoryPath = Application.persistentDataPath + "/videos";
			string format = "yyyyMMdd_HH:mm";
			string destFile = Path.Combine(directoryPath, DateTime.Now.ToString(format) + "_" + PlayerPrefs.GetString("taskID") + "_" + SystemInfo.deviceUniqueIdentifier + ".mp4");
			debug.text = GetVideoPath() + destFile;
			//debug.text = destFile;
			File.Copy(GetVideoPath(), destFile, true);
			//Debug.Log (destFile);
			gotcha.text = message;
			gotcha.enabled = true;
			gotchaPanel.GetComponent<Image> ().enabled = true;
			StartCoroutine(UploadFileCo(GetVideoPath(), destFile, "http://140.114.77.170/b4g/upload.php"));
			yield return new WaitForSeconds(delay);
			gotcha.enabled = false;
			gotchaPanel.GetComponent<Image> ().enabled = false;
		//}		
        //GameObject.Find("Plane").GetComponent<OpenCamera>().PressBackMap();
    }

    IEnumerator ShowMessage (string message, float delay) {
		hint.text = message;
		hint.enabled = true;
		hintPanel.GetComponent<Image> ().enabled = true;
		yield return new WaitForSeconds(delay);
		hint.enabled = false;
		hintPanel.GetComponent<Image> ().enabled = false;
	}

	IEnumerator UploadFileCo(string localFileName, string saveFileName, string uploadURL)
	{
		bool uploadVideo = false;
		WWWForm postForm = new WWWForm();

		if (uploadVideo) {
			WWW localFile = new WWW (saveFileName);
			Debug.Log (localFileName);
			yield return localFile;
			if (localFile.error == null) {
				Debug.Log ("Loaded file successfully");
			} else {
				Debug.Log ("Open file error: " + localFile.error);
				yield break; // stop the coroutine here
			}
			postForm.AddBinaryData("theFile",localFile.bytes,localFileName,"text/plain");
		}
		postForm.AddField("task_id", PlayerPrefs.GetString("taskID"));
		postForm.AddField("spot_id", PlayerPrefs.GetString("spotID"));
		postForm.AddField("user_id", SystemInfo.deviceUniqueIdentifier);
		postForm.AddField("timeFrame", PlayerPrefs.GetString("taskTimeFrame"));
		postForm.AddField("fileName", saveFileName);
		postForm.AddField("score", point.ToString());

		WWW upload = new WWW(uploadURL, postForm);        
		yield return upload;
		if (upload.error == null) {
			Debug.Log ("upload done :" + upload.text);
			point = (float)Math.Floor (point * 1000);
			PlayerPrefs.SetFloat ("Score", PlayerPrefs.GetFloat ("Score") + point);
		}
		else
			Debug.Log("Error during upload: " + upload.error);

		GameObject.Find("Plane").GetComponent<OpenCamera>().PressBackMap();
	}

	public string GetVideoPath () {
		#if UNITY_IOS

		var root = new DirectoryInfo(Application.persistentDataPath).Parent.FullName;
		var everyplayDir = root + "/tmp/Everyplay/session";

		#elif UNITY_ANDROID

		var root = new DirectoryInfo(Application.temporaryCachePath).FullName;
		var everyplayDir = root + "/sessions";

		#endif

		var files = new DirectoryInfo(everyplayDir).GetFiles("*.mp4", SearchOption.AllDirectories);
		var videoLocation = "";

		// Should only be one video, if there is one at all
		foreach (var file in files) {
			#if UNITY_ANDROID

			videoLocation = file.FullName;
			#else
			videoLocation = file.FullName;
			#endif
			break;
		}

		return videoLocation;
	}
}
