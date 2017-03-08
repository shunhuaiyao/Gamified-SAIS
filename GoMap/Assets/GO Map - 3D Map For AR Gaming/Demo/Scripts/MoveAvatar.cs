using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using GoMap;

public class MoveAvatar : MonoBehaviour {

	public LocationManager locationManager;
	public GameObject avatarFigure;
	public GameObject petFigure;
	public petAnimController anim;
	//public Text position;
	public bool normalPuppy;
    //for AR scene
    public Vector3 currentEnterPosition;

	private Vector3 currentTransformPosition;

	// Use this for initialization
	void Start () {

		normalPuppy = true;

		locationManager.onOriginSet += OnOriginSet;
		locationManager.onLocationChanged += OnLocationChanged;
		currentTransformPosition = transform.position;

		Input.location.Start(); 
		Input.compass.enabled=true;
	}

	void Update(){
		if (currentTransformPosition != transform.position && normalPuppy) {
			anim.IdelWalkTransition(true);
		} else {
			anim.IdelWalkTransition(false);
		}
		if (currentTransformPosition == transform.position && Application.loadedLevelName != "OpenCamera") {
			Quaternion newRotation = Quaternion.Euler(0, Input.compass.magneticHeading, 0);
			StartCoroutine (CompassRotation (avatarFigure.transform.rotation, newRotation, 1f));
		} 
		currentTransformPosition = transform.position;

	}

	void OnOriginSet (Coordinates currentLocation) {

		//Position
		Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
		currentPosition.y = transform.position.y;

		transform.position = currentPosition;
		Vector3 petCurrentPosition = new Vector3 (currentPosition.x, currentPosition.y, currentPosition.z); 
		petFigure.transform.position = petCurrentPosition;
		//print ("iiiiiiiiii"+transform.position.ToString());
	}

	void OnLocationChanged (Coordinates currentLocation) {

		Vector3 lastPosition = transform.position;
		//Position
		Vector3 currentPosition = currentLocation.convertCoordinateToVector ();
		currentPosition.y = transform.position.y;

		if (lastPosition == Vector3.zero) {
			lastPosition = currentPosition;
		}

//		transform.position = currentPosition;
//		rotateAvatar (lastPosition);
		//print ("ooooooooooooo"+transform.position.ToString());
		moveAvatar (lastPosition,currentPosition);
        //for AR scene
        currentEnterPosition = currentPosition;
       

		if (normalPuppy) {
			if (Application.loadedLevelName != "OpenCamera") {
				Vector3 petlastPosition = new Vector3 (lastPosition.x + 15, lastPosition.y, lastPosition.z);
				Vector3 petCurrentPosition = new Vector3 (currentPosition.x + 15, currentPosition.y, currentPosition.z);
				movePuppy (petlastPosition, petCurrentPosition);
			} 
			else {
				Vector3 petlastPosition = new Vector3 (lastPosition.x + 20, lastPosition.y, lastPosition.z + 30);
				Vector3 petCurrentPosition = new Vector3 (currentPosition.x + 20, currentPosition.y, currentPosition.z + 35);
				movePuppy (petlastPosition, petCurrentPosition);
			}
		}
	}

	void moveAvatar (Vector3 lastPosition, Vector3 currentPosition) {

        //for AR scene
        if(Application.loadedLevelName != "OpenCamera")
		    StartCoroutine (move (lastPosition,currentPosition,0.5f));
	}

	void movePuppy (Vector3 lastPosition, Vector3 currentPosition){
		StartCoroutine (movePet (lastPosition,currentPosition,0.5f));
	}

	private IEnumerator move(Vector3 lastPosition, Vector3 currentPosition, float time) {

		float elapsedTime = 0;
		Vector3 targetDir = currentPosition-lastPosition;
		Quaternion finalRotation = Quaternion.LookRotation (targetDir);
		//print ("currentPosition " + currentPosition+" lastPosition "+lastPosition);
		while (elapsedTime < time)
		{
			transform.position = Vector3.Lerp(lastPosition, currentPosition, (elapsedTime / time));
			avatarFigure.transform.rotation = Quaternion.Lerp(avatarFigure.transform.rotation, finalRotation,(elapsedTime / time));
			//petFigure.transform.rotation = Quaternion.Lerp(petFigure.transform.rotation, finalRotation,(elapsedTime / time));

			elapsedTime += Time.deltaTime;
            Debug.Log("Gamer:" + transform.position.x + "," + transform.position.z);
            yield return new WaitForEndOfFrame();
		}

//		avatarFigure.transform.rotation = finalRotation;
	}

	private IEnumerator movePet(Vector3 lastPosition, Vector3 currentPosition, float time) {

		float elapsedTime = 0;
		Vector3 targetDir = currentPosition-lastPosition;
		Quaternion finalRotation = Quaternion.LookRotation (targetDir);
		//print ("currentPosition " + currentPosition+" lastPosition "+lastPosition);
		while (elapsedTime < time)
		{
			petFigure.transform.position = Vector3.Lerp(lastPosition, currentPosition, (elapsedTime / time));
			//avatarFigure.transform.rotation = Quaternion.Lerp(avatarFigure.transform.rotation, finalRotation,(elapsedTime / time));
			if(Application.loadedLevelName != "OpenCamera")
				petFigure.transform.rotation = Quaternion.Lerp(petFigure.transform.rotation, finalRotation,(elapsedTime / time));

			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}

		//		avatarFigure.transform.rotation = finalRotation;
	}

	private IEnumerator CompassRotation(Quaternion lastRotation, Quaternion currentRotation, float time) {

		float elapsedTime = 0;
		//print ("currentPosition " + currentPosition+" lastPosition "+lastPosition);
		while (elapsedTime < time)
		{
			avatarFigure.transform.rotation = Quaternion.Lerp(lastRotation, currentRotation,(elapsedTime / time));

			elapsedTime += Time.deltaTime;
			yield return new WaitForEndOfFrame();
		}
	}
		
//	void rotateAvatar(Vector3 lastPosition) {
//	
//		//Orient Avatar
//		Vector3 targetDir = transform.position-lastPosition;
//
//		if (targetDir != Vector3.zero) {
//			avatarFigure.transform.rotation = Quaternion.Slerp(
//				avatarFigure.transform.rotation,
//				Quaternion.LookRotation(targetDir),
//				Time.deltaTime * 10.0f
//			);
//			petFigure.transform.rotation = Quaternion.Slerp(
//				petFigure.transform.rotation,
//				Quaternion.LookRotation(targetDir),
//				Time.deltaTime * 10.0f
//			);
//		}
//	}


}
