using UnityEngine;
using System.Collections;
using GoMap;

public class SeekFood : MonoBehaviour {

	public GameObject Avatar;
	public petAnimController anim;
	public Transform character;
	public Vector3 FinalTarget;
	public float speed = 10f;
	public bool backToAvatar;

    //for AR scene
    public Vector3 directionVector;

	private Vector3 targetPosition;
	private Vector3 lastPosition;
	private Quaternion lastRotation;
    // Use this for initialization

    public Vector3 spotPosition, foodPosition;
    public bool hasPassedThroughSpot;
    public bool isDonePathGenerator;

    void Awake(){
	}

	void Start () {
//		if (Application.loadedLevelName != "OpenCamera") {
//			this.gameObject.GetComponent<NavMeshAgent> ().enabled = true;
//		}
	}

    // Update is called once per frame
    void Update () {
		Avatar.GetComponent<MoveAvatar> ().normalPuppy = false;
		//print (FinalTarget.ToString ());
		float distance = Vector3.Distance (FinalTarget, character.position);
		float PuppyCharacterDistance = Vector3.Distance (transform.position, character.position);
        float spotCharacterDistance = Vector3.Distance(spotPosition, character.position);
        //this.gameObject.GetComponent<NavMeshAgent>().SetDestination(FinalTarget);
        //print (distance.ToString ()+"//"+PuppyCharacterDistance.ToString());
        if (Application.loadedLevelName != "OpenCamera") {
            if (!hasPassedThroughSpot) {
                if (spotCharacterDistance > 30f) {
                    if (PuppyCharacterDistance < 20f)
                        targetPosition = transform.position + (spotPosition - transform.position).normalized * 30;
                    else if (backToAvatar) {
                        targetPosition = Avatar.transform.position;
                        backToAvatar = false;
                    }
                }
                else {
                    targetPosition = spotPosition;
                    hasPassedThroughSpot = true;
                }
                //for AR scene
                directionVector = (FinalTarget - character.position).normalized;
            }
            else {
                if(transform.position == spotPosition && spotCharacterDistance < 10f) {
                    targetPosition = transform.position + (FinalTarget - transform.position).normalized * 15;
                    isDonePathGenerator = true;
                    foodPosition = transform.position + (FinalTarget - transform.position).normalized * 27;
                }
            }
        } 
		else {
            /*if (distance > 55f) {
				if (PuppyCharacterDistance < 52f)
					targetPosition = transform.position + (FinalTarget - transform.position).normalized * 50;
				//print (targetPosition.ToString () + "//" + FinalTarget.ToString ()+"//"+transform.position.ToString());
				//print (PuppyCharacterDistance);
			} else {
				targetPosition = FinalTarget + (transform.position - FinalTarget).normalized * 12;
			}*/
            targetPosition = FinalTarget + (transform.position - FinalTarget).normalized * 12;
        }

        if (transform.position != targetPosition ) {
			anim.RunJumpTransition (true, false);
			//print ("targetPosition: "+targetPosition.ToString());
			movePuppy (transform.position, targetPosition);
            

		}
		else {
			anim.RunJumpTransition (false, true);
			transform.rotation = lastRotation;
		}

		if ((FinalTarget + (transform.position - FinalTarget).normalized * 12) == transform.position) {
			if(Application.loadedLevelName == "OpenCamera")
				PlayerPrefs.SetInt ("SeekFood", 1);
			anim.JumpFinalEatTransition (true);
		} 
		else {
			//print(Vector3.Distance (FinalTarget, transform.position).ToString());
			if(Application.loadedLevelName == "OpenCamera")
				PlayerPrefs.SetInt ("SeekFood", 0);
			anim.JumpFinalEatTransition (false);
		}
		
	}


	void movePuppy (Vector3 lastPosition, Vector3 targetPosition){
		StartCoroutine (movePet (lastPosition,targetPosition,0.5f));
	}

	private IEnumerator movePet(Vector3 lastPosition, Vector3 targetPosition, float time) {

		float elapsedTime = 0;

		Vector3 targetDir = targetPosition-lastPosition;
		Vector3 upwards = new Vector3 (0f,0f,-1f);
		Quaternion finalRotation;
		if (Application.loadedLevelName == "OpenCamera")
			finalRotation = Quaternion.LookRotation (targetDir, upwards);
		else
			finalRotation = Quaternion.LookRotation (targetDir);

		//print ("finalRotation: "+finalRotation.ToString ()+ "Dir: "+ targetDir.ToString());
		//print ("currentPosition " + currentPosition+" lastPosition "+lastPosition);
		while (elapsedTime < time)
		{
			transform.rotation = Quaternion.Lerp(transform.rotation, finalRotation,(elapsedTime / time));
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards(lastPosition, targetPosition, step);
 			anim.WalkRunTransition (false,true);
			lastRotation = transform.rotation;
			elapsedTime += Time.deltaTime;
            Debug.Log("NPC_Position:" + transform.position.x + "," + transform.position.z);
            yield return new WaitForEndOfFrame();
		}


		yield return new WaitForEndOfFrame();
	}


}
