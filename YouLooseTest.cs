using UnityEngine;
using System.Collections;

public class YouLooseTest : MonoBehaviour
{

    CreateLevel gameCtrl;

    // Use this for initialization
	void Start()
	{
		//Get a reference
		GameObject game = GameObject.Find("GameManager");
		gameCtrl = game.GetComponent<CreateLevel> ();
	}

	void OnTriggerEnter(Collider other)
	{
		//call a method of gameCtrl indicating a possible win situation
		//Is ist good to test for Player here?
		gameCtrl.EndzoneTrigger(other.gameObject);
	}
}
