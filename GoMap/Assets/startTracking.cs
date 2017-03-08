using UnityEngine;
using System.Collections;

public class startTracking : MonoBehaviour {

    public GameObject PuppyPee, AvatarPee;

    private GameObject Puppy, Avatar;
    private bool addPuppyStamp = true;
    private bool addAvatarStamp = true;

    // Use this for initialization
    void Start () {
        Puppy = GameObject.Find("Corgi");
        Avatar = GameObject.Find("Avatar");
	}
	
	// Update is called once per frame
	void Update () {
        if (addPuppyStamp)
        {
            StartCoroutine("addPuppyStampObject");
            addPuppyStamp = false;
        }
        if (addAvatarStamp)
        {
            StartCoroutine("addAvatarStampObject");
            addAvatarStamp = false;
        }
    }

    IEnumerator addPuppyStampObject()
    {
        GameObject PuppyGo = GameObject.Instantiate(PuppyPee);
        PuppyGo.transform.localPosition = Puppy.GetComponent<Transform>().position;
        PuppyGo.name = "puppypee";
        yield return new WaitForSeconds(0.5f);
        addPuppyStamp = true;
    }

    IEnumerator addAvatarStampObject()
    {
        GameObject AvatarGo = GameObject.Instantiate(AvatarPee);
        AvatarGo.transform.localPosition = Avatar.GetComponent<Transform>().position;
        AvatarGo.name = "avatarpee";
        yield return new WaitForSeconds(3);
        addAvatarStamp = true;
    }
}
