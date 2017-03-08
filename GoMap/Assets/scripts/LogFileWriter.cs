using UnityEngine;
using System.Collections;
using System.IO;
using System;
using GoMap;

public class LogFileWriter : MonoBehaviour
{
	private StreamWriter _writer;
	private GameObject locationObject;
	private LocationManager locationManager;
	void Awake()
	{
		_writer = File.AppendText(Application.persistentDataPath + "/log.txt");    
		_writer.Write("Scene:" + Application.loadedLevelName + "\n");
		//DontDestroyOnLoad(gameObject);
		Application.logMessageReceived += HandleLog;
		locationObject = GameObject.Find ("LocationManager");
		locationManager = (LocationManager) locationObject.GetComponent (typeof(LocationManager));

	}

	private void HandleLog(string condition, string stackTrace, LogType type)
	{
		var logEntry = string.Format("\t{0}\t{1}\t{2}\t{3}\n"
			, DateTime.Now, type, condition, stackTrace);
		_writer.Write(logEntry);
		_writer.Flush ();
	}

	void FixedUpdate () {
		Coordinates location = locationManager.currentLocation;
		Debug.Log ("GPS:" + location.latitude + "," + location.longitude);
		Debug.Log ("gyro:" + Input.gyro.attitude.x + "," + Input.gyro.attitude.y + "," + Input.gyro.attitude.z + "," + Input.gyro.attitude.w);
        Debug.Log("Compass:" + Input.compass.magneticHeading);

    }

	void OnDestroy()
	{
		Application.logMessageReceived -= HandleLog;
		Debug.Log ("exit log");
		_writer.Close();
	}

	void OnApplicationQuit() {
		Debug.Log("Application ending after " + Time.time + " seconds");
		Application.logMessageReceived -= HandleLog;
		Debug.Log ("exit log");
		_writer.Close();
	}
}

