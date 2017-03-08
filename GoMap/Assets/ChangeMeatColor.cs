using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System;
using Assets.SimpleAndroidNotifications;


public class ChangeMeatColor : MonoBehaviour {

	public Button MeatButton;
	public static bool havetasks;
    public static bool calledNotification = false;
	private Color white;
	private Color gray;
	private float timeInterval;
	// Use this for initialization
	void Start () {
		white = new Color (1f,1f,1f,1f);
		gray = new Color (0.72f, 0.72f, 0.72f, 1f);
		timeInterval = Time.fixedTime + 0.3f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		
		if (Time.fixedTime >= timeInterval) {
			if (havetasks) {
				if (MeatButton.GetComponent<Image> ().color == white)
					MeatButton.GetComponent<Image> ().color = gray;
				else
					MeatButton.GetComponent<Image> ().color = white;

            }
            else {
				MeatButton.GetComponent<Image> ().color = white;
			}
			timeInterval = Time.fixedTime + 0.3f;
		}

        if (havetasks && !calledNotification)
        {
            NotificationManager.SendWithAppIcon(1, TimeSpan.FromSeconds(3), "Survival", "Get New Tasks!", new Color(0, 0.6f, 1), NotificationIcon.Star);
            calledNotification = true;
        }


    }



}