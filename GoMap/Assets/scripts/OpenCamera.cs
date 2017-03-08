using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.IO;


public class OpenCamera : MonoBehaviour
{
	public Text path;
	private WebCamTexture webcamTexture;

	void Start ()
	{

		webcamTexture = new WebCamTexture();
		webcamTexture.requestedFPS = 500;
		Renderer renderer = GetComponent<Renderer>();
		renderer.material.mainTexture = webcamTexture;
		webcamTexture.Play();
		Everyplay.StartRecording ();

	}

	// Update is called once per frame
	void Update ()
	{
        /*if (Everyplay.IsRecording())
            print("recording");
        else
            print("not recording");*/

    }

	public void PressBackMap(){
		webcamTexture.Stop ();
		if (Everyplay.IsRecording())
			Everyplay.StopRecording ();
		//path.text = GetVideoPath ();
		PlayerPrefs.SetInt ("Gotcha", 0);
		Application.LoadLevel (0);
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

			videoLocation = "file://" + file.FullName;
			#else
			videoLocation = file.FullName;
			#endif
			break;
		}

		return videoLocation;
	}
		
}