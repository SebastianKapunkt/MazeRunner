using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
	//The tiles have been modeled as 4x4 unity unit squares
	private const float tileSize = 4;

	private GameObject root, floor, environment;
	public int xHalfExt = 1;
	public int zHalfExt = 1;

	public GameObject ball;
	public GameObject outerWall;
	public GameObject innerWall;
	public GameObject exitTile;
	public GameObject[] floorTiles;

	private int xExt, zExt;
	private BackTracking tracker;

	// Use this for initialization
	void Awake ()
	{
		//Gather together all refrences you will need later on
		root = GameObject.Find ("Environment");
		//Build the values for xExt and zExt from xHalfExt and zHalfExt
		xExt = xHalfExt * 2 + 1;
		zExt = zHalfExt * 2 + 1;
		//Build an offset for the dyn playfield from the BasePlatform e.g. the bigger halfExtent value in unity units
		//Calculate a scale factor for scaling the non-movable environment (and therefore the camera) and the BasePlatform 
		// the factors that the environment are scaled for right now are for x/zHalfExt =1, scale accordingly
		//i.e. the playfield/environment should be as big as the dynamic field

		//Scale Environment

		//Scale  + position BasePlate


		if (root != null) {
			
			//Create the outer walls for given extXZ
			createOuterWalls ();

			//create a maze
			createMaze ();

			//Place the PlayerBall above the playfiel
			placeBallStart ();
		}
	}

	private void createOuterWalls ()
	{
		for (int i = 0; i < xExt; i++) {
			Instantiate (
				outerWall,
				new Vector3 (transformToScale (xExt, i), 0, zExt),
				Quaternion.Euler (0, 0, 0),
				root.transform);	
			Instantiate (
				outerWall,
				new Vector3 (transformToScale (xExt, i), 0, -zExt),
				Quaternion.Euler (0, 0, 0),
				root.transform);	
		}

		for (int i = 0; i < zExt; i++) {
			Instantiate (
				outerWall,
				new Vector3 (-xExt, 0, transformToScale (zExt, i)),
				Quaternion.Euler (0, 90, 0),
				root.transform);
			Instantiate (
				outerWall,
				new Vector3 (xExt, 0, transformToScale (zExt, i)),
				Quaternion.Euler (0, 90, 0),
				root.transform);
			
		}
	}

	private void placeWallForField (int x, int z, WallsOfCell cell)
	{
		//South
		if (z < zExt - 1 && cell.south && tracker.walls [x, z + 1].north) {
			Instantiate (
				innerWall,
				new Vector3 (-xExt + (x * 2), 0, -zExt + (z * 2)) + new Vector3 (1, 0, 2),
				Quaternion.Euler (0, 0, 0),
				root.transform);
		}
		//North
		if (z > 0 && cell.north && tracker.walls [x, z - 1].south) {
			Instantiate (
				innerWall,
				new Vector3 (-xExt + (x * 2), 0, -zExt + (z * 2)) + new Vector3 (1, 0, 0),
				Quaternion.Euler (0, 0, 0),
				root.transform);
		}
		//East
		if (x > 0 && cell.east && tracker.walls [x - 1, z].west) {
			Instantiate (
				innerWall,
				new Vector3 (-xExt + (x * 2), 0, -zExt + (z * 2)) + new Vector3 (0, 0, 1),
				Quaternion.Euler (0, 90, 0),
				root.transform);
		}
		//West
		if (x < xExt - 1 && cell.west && tracker.walls [x + 1, z].east) {
			Instantiate (
				innerWall,
				new Vector3 (-xExt + (x * 2), 0, -zExt + (z * 2)) + new Vector3 (2, 0, 1),
				Quaternion.Euler (0, 90, 0),
				root.transform);
		}
	}

	private bool ne (int p, int p1)
	{
		return p1 - p == -1;
	}

	private bool sw (int p, int p1)
	{
		return p - p1 == -1;
	}

	private void createMaze ()
	{
		tracker = new BackTracking (xExt, zExt);
		tracker.generateCourse ();

		for (int x = 0; x < xExt; x++) {
			for (int z = 0; z < zExt; z++) {
				placeWallForField (x, z, tracker.walls [x, z]);
			}
		}

		//Build the maze from the given set of prefabs
		for (int x = 0; x < xExt; x++) {
			for (int z = 0; z < zExt; z++) {
				if (x == tracker.start.x && z == tracker.start.z) {
					Instantiate (
						floorTiles [3],
						new Vector3 (x, 0, z) * 2 + new Vector3 (-xExt + 1, 0, -zExt + 1),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				} else if (x == tracker.end.x && z == tracker.end.z) {
					Instantiate (
						exitTile,
						new Vector3 (x, 0, z) * 2 + new Vector3 (-xExt + 1, 0, -zExt + 1),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				} else {
					Instantiate (
						floorTiles [Random.Range (0, 5)],
						new Vector3 (x, 0, z) * 2 + new Vector3 (-xExt + 1, 0, -zExt + 1),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				}
			}
		}
	}

	private int transformToScale (int value, int factor)
	{
		return -value + 1 + factor * 2;
	}

	//You might need this more than once...
	void placeBallStart ()
	{
		//Reset Physics
		ball.GetComponent<Rigidbody>().velocity = Vector3.zero;

		//Place the ball
		ball.transform.position = new Vector3 (tracker.start.x, 1, tracker.start.z) * 2 + new Vector3 (-xExt + 1, 0, -zExt + 1);
	}

	public void EndzoneTrigger (GameObject other)
	{
		//Check if ball first...
		//Player has fallen onto ground plane, reset
	}

	public void winTrigger (GameObject other)
	{
		//Check if ball first...

		//Destroy this maze

		//Generate new maze

		//Player has fallen onto ground plane, reset
	}
}