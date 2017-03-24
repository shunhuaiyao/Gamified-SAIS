using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using GoMap;
using System.IO;

public class ShowTasks : MonoBehaviour {

	public LocationManager locationManager;
	public Canvas mainCanvas, taskCanvas;
	public Button Task1Button, Task2Button;
	public Text Task1Text, Task2Text, NoFoodText, SelectedTaskText;
	public GameObject SelectedTaskPanel, Puppy, Food;
	public Sprite nofood, Hambuger, Icecream, Cake, Donuts, Milk, Hamegg;

	public List<Task> tasks;
	private int taskIndex;

	void Awake () {
		locationManager.onOriginSet += LoadPosition;
		locationManager.onLocationChanged += LoadPosition;
	}

	// Use this for initialization
	void Start () {
		

	}
	
	// Update is called once per frame
	void Update () {


	}

	void LoadPosition(Coordinates currentLocation){
		int i;
		for (i = 0; i < tasks.Count; i++) {
			double lat = (double)tasks[i].latitude;
			double lng = (double)tasks[i].longitude;
			Coordinates coordinates = new Coordinates (lat, lng, 0);
			tasks[i].position = coordinates.convertCoordinateToVector (10);
            //print (tasks[i].position.ToString());
            double spotLat = (double)tasks[i].spotLatitude;
            double spotLng = (double)tasks[i].spotLongitude;
            Coordinates spotCoordinates = new Coordinates(spotLat, spotLng, 0);
            tasks[i].spotPosition = spotCoordinates.convertCoordinateToVector(10);
        }
	}

	public void showTasks (){
//		if (SocketManager.TaskList.Count == 10) {
//			tasks = SocketManager.TaskList;
//			PlayerPrefs.SetInt ("taskNum",tasks.Count);
//		}
//		else {
//			SocketManager.TaskList.Clear ();
//			GameObject.Find ("Main Camera").GetComponent<StartSocket> ().AskTasks ();
//		}
		tasks = SocketManager.TaskList;
		PlayerPrefs.SetInt ("taskNum", tasks.Count);
		ChangeMeatColor.havetasks = false;
        ChangeMeatColor.calledNotification = false;
		taskCanvas.targetDisplay = 0;
        taskCanvas.GetComponent<CanvasGroup>().interactable = true;
		mainCanvas.GetComponent<CanvasGroup> ().blocksRaycasts = false;
		GameObject.Find ("Main Camera").GetComponent<TwoXOrbitGesture> ().enabled = false;
        taskIndex = 0;

        string directoryPath = Application.persistentDataPath + "/videos";
        //debug.text = Directory.Exists(directoryPath).ToString();
        if (!Directory.Exists(directoryPath))
        {
            //debug.text = directoryPath;
            Directory.CreateDirectory(directoryPath);
        }
        //debug.text = directoryPath;

        showTasksList();
        //Time.timeScale = 0;
    }

    public void cancelTasks(){
		taskCanvas.targetDisplay = 1;
        taskCanvas.GetComponent<CanvasGroup>().interactable = false;
        mainCanvas.GetComponent<CanvasGroup> ().blocksRaycasts = true;
		GameObject.Find ("Main Camera").GetComponent<TwoXOrbitGesture> ().enabled = true;
		//Time.timeScale = 1;
	}

	public void clickLeftButton(){
		if (taskIndex > 0) {
			taskIndex -= 2;
		}
		showTasksList ();
	}

	public void clickRightButton(){
		if (taskIndex + 2 < tasks.Count) {
			taskIndex += 2;
		}
		showTasksList ();
	}

	private void showTasksList(){

		if ( tasks.Count > 0 ){
			NoFoodText.text = "";
		}

		if (taskIndex + 1 < tasks.Count) {
			Task1Button.GetComponent<Image> ().sprite = correspondingSprite(tasks [taskIndex].taskFood);
			Task1Text.text = tasks [taskIndex].taskName;
			Task2Button.GetComponent<Image> ().sprite = correspondingSprite(tasks [taskIndex+1].taskFood);
			Task2Text.text = tasks [taskIndex + 1].taskName;
		} 
		else if (taskIndex < tasks.Count) {
			Task1Button.GetComponent<Image> ().sprite = correspondingSprite(tasks [taskIndex].taskFood);
			Task1Text.text = tasks [taskIndex].taskName;
			Task2Button.GetComponent<Image> ().sprite = nofood;
			Task2Text.text = "";
		}
		else if (tasks.Count == 0) {
			Task1Button.GetComponent<Image> ().sprite = nofood;
			Task1Text.text = "";
			Task2Button.GetComponent<Image> ().sprite = nofood;
			Task2Text.text = "";
			NoFoodText.text = "No Food";
		}

	}

	public void selectedTaskButton1(){
		if ( taskIndex < tasks.Count ) {
			cancelTasks ();
			string msg = "Follow your puppy to find "+tasks[taskIndex].taskFood;
			StartCoroutine(ShowTaskMessage(msg, 2, taskIndex));
		}

	}

	public void selectedTaskButton2(){

		if ( taskIndex + 1 < tasks.Count ) {
			cancelTasks ();
			string msg = "Follow your puppy to find "+tasks[taskIndex+1].taskFood;
			StartCoroutine(ShowTaskMessage(msg, 2, taskIndex+1));
		}
			
	}

	IEnumerator ShowTaskMessage (string message, float delay, int index) {
		SelectedTaskText.text = message;
		SelectedTaskText.enabled = true;
		SelectedTaskPanel.GetComponent<Image> ().enabled = true;
		LoadPosition (null);
        //Puppy.GetComponent<startTracking>().enabled = true;
        Puppy.GetComponent<SeekFood> ().enabled = true;
		Puppy.GetComponent<SeekFood> ().backToAvatar = true;
        Puppy.GetComponent<SeekFood>().spotPosition = tasks[index].spotPosition;
        Puppy.GetComponent<SeekFood>().hasPassedThroughSpot = false;
        Puppy.GetComponent<SeekFood>().isDonePathGenerator = false;
        Puppy.GetComponent<SeekFood> ().FinalTarget = tasks [index].position;
		Food.GetComponent<GO4Square> ().FinalTarget = tasks [index];
		yield return new WaitForSeconds(delay);
		SelectedTaskText.enabled = false;
		SelectedTaskPanel.GetComponent<Image> ().enabled = false;
	}

	private Sprite correspondingSprite(string foodName){
		Sprite sprite = nofood;
		switch (foodName) {
			case "Hambuger":
				sprite = Hambuger;
				break;
			case "Ice Cream":
				sprite = Icecream;
				break;
			case "Cake":
				sprite = Cake;
				break;
			case "Donuts":
				sprite = Donuts;
				break;
			case "Milk":
				sprite = Milk;
				break;
			case "HamEgg":
				sprite = Hamegg;
				break;
			default:
				break;
		}
		return sprite;
	}
}
