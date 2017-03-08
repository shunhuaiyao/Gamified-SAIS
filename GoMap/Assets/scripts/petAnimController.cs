using UnityEngine;
using System.Collections;
using GoMap;

public class petAnimController : MonoBehaviour {


	private Animator anim;

	// Use this for initialization
	void Start () {
		anim = GetComponent<Animator> ();
	}
	
	// Update is called once per frame
	void Update () {
	}

	public void IdelWalkTransition(bool walk){
		anim.SetBool ("Walk", walk);
	}

	public void IdelRunTransition(bool run){
		anim.SetBool ("Run", run);
	}

	public void WalkRunTransition(bool walk, bool run){
		anim.SetBool ("Run", run);		
		anim.SetBool ("Walk", walk);
	}

	public void RunJumpTransition(bool run, bool jump){
		anim.SetBool ("Run", run);
		anim.SetBool ("Jump", jump);			
	}
	public void JumpFinalEatTransition(bool eat){
		anim.SetBool ("FinalEat", eat);
	}
}
