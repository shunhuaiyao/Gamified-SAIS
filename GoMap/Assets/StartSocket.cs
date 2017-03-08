using UnityEngine;
using System.Collections;
using System;

public class StartSocket : MonoBehaviour {
	public SocketManager mSocketManager;

	private const string IP = "140.114.77.170";
	private const int Port = 9528;
	private bool AskingFlag;

	// Use this for initialization
	void Start () {
		mSocketManager = new SocketManager();
		mSocketManager.Connect(IP, Port);
		//AskTasks ();
		AskingFlag = false;
	}
	
	// Update is called once per frame
	void Update () {
		if(!AskingFlag){
			AskingFlag = true;
			StartCoroutine(DoDelay(60.0f));
		}
	}

	void OnApplicationQuit() {
		Debug.Log("Application ending after " + Time.time + " seconds");
		mSocketManager.Close ();
	}

	public void AskTasks(){
		mSocketManager.SendServer("getTask\n");
	}

	IEnumerator DoDelay (float delay){
		AskTasks ();
		yield return new WaitForSeconds(delay);
		AskingFlag = false;
	}

}
