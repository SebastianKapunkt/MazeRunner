using UnityEngine;
using System.Collections;

public class BaseMovement : MonoBehaviour
{
	public GameObject horzPedal = null;
	public GameObject vertPedal = null;
	public float horzRot, vertRot;
	public float pedalFactor = -10;
	public float rotFactor = 30f;

	void Start ()
	{
		//Get References to the game pedals for applying a rotation in Update()
		vertPedal = GameObject.Find ("VertPedal");
		horzPedal = GameObject.Find ("HorzPedal");
		//initialize all values for use
	}

	// Update is called once per frame
	void Update ()
	{
		//Get Input from Input.GetAxis
		//Stretch it from 0-1 to 0 - maxRotFactor
		horzRot = Input.GetAxis ("Horizontal") * rotFactor;
		vertRot = Input.GetAxis ("Vertical") * rotFactor;
		//Apply a part of the rotation to this (and children) to rotate the plafield
		//Use Quaternion.Slerp und Quaternion.Euler for doing it
		//REM: Maybe you have to invert one axis to get things right visualy
		transform.rotation = Quaternion.Slerp (transform.rotation, Quaternion.Euler (vertRot, 0, -horzRot), Time.deltaTime * 2.0f);
		//Apply an exaggerated ammount of rotation to the pedals to visualize the players input
		//Make the rotation look right
		Vector3 moveHorizontal = new Vector3 (0, horzRot, 0) * pedalFactor;
		Vector3 moveVertical = new Vector3 (0, vertRot, 0) * pedalFactor;

		horzPedal.transform.Rotate (moveHorizontal * Time.deltaTime);
		vertPedal.transform.Rotate (moveVertical * Time.deltaTime);
	}
}
