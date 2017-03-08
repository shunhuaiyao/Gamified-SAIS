using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class updateScore : MonoBehaviour {
	private GameObject scoreText;
	// Use this for initialization
	void Start () {
		if (!PlayerPrefs.HasKey ("Score")) {
			PlayerPrefs.SetFloat ("Score", 0);
		}
		scoreText = GameObject.FindGameObjectWithTag("ScoreText");
		scoreText.GetComponent<Text>().text = "Score:" + PlayerPrefs.GetFloat ("Score");
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
