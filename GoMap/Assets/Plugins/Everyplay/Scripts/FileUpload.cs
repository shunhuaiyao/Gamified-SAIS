using UnityEngine;
using System.Collections;

public class FileUpload : MonoBehaviour {
	
	private string m_LocalFileName = "/logFile.txt";
	private string m_URL = "http://140.114.77.170/b4g/upload.php";

	IEnumerator UploadFileCo(string localFileName, string uploadURL)
	{
		WWW localFile = new WWW("file:///" + Application.persistentDataPath + localFileName);
		Debug.Log("file:///" + Application.dataPath + localFileName);
		yield return localFile;
		if (localFile.error == null)
			Debug.Log("Loaded file successfully");
		else
		{
			Debug.Log("Open file error: "+localFile.error);
			yield break; // stop the coroutine here
		}
		WWWForm postForm = new WWWForm();
		// version 1
		//postForm.AddBinaryData("theFile",localFile.bytes);
		// version 2
		postForm.AddBinaryData("theFile",localFile.bytes,localFileName,"text/plain");
		WWW upload = new WWW(uploadURL, postForm);        
		yield return upload;
		if (upload.error == null)
			Debug.Log("upload done :" + upload.text);
		else
			Debug.Log("Error during upload: " + upload.error);
	}

	public void UploadFile(string localFileName, string uploadURL)
	{
		StartCoroutine(UploadFileCo(localFileName, m_URL));
	}

	void OnGUI()
	{
		GUILayout.BeginArea(new Rect(0,0,Screen.width,Screen.height));
		m_LocalFileName = GUILayout.TextField(m_LocalFileName, GUILayout.Width(300), GUILayout.Height(300));
		m_URL           = GUILayout.TextField(m_URL);
		if (GUILayout.Button("Upload", GUILayout.Width(300), GUILayout.Height(300)))
		{
			UploadFile(m_LocalFileName, m_URL);
		}
		GUILayout.EndArea();
	}
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
