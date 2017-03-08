using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HPHandler : MonoBehaviour {

	public float MaxHP;
	public float HP;
	public Image HPBar;
	// Use this for initialization
	void Start () {
		MaxHP = 100;
		HP = 100;
	}
	
	// Update is called once per frame
	void Update () {
		if (HP > MaxHP)
			HP = MaxHP;
		else if (HP < 0)
			HP = 0;
		HPBar.fillAmount = HP / MaxHP;
	}
}
