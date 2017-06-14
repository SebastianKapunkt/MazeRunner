using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class CreateLevel : MonoBehaviour
{
	//The tiles have been modeled as 4x4 unity unit squares
	private const float tileSize = 4;

	private GameObject environment, floor, root, ball;
	public int xHalfExt = 1;
	public int zHalfExt = 1;

	public GameObject outerWall;
	public GameObject innerWall;
	public GameObject exitTile;
	public GameObject[] floorTiles;

	private int xExt, zExt;
	private BackTracking tracker;
	private float scaleEnv;
	private float scale;
	private float baseScale;

	// Use this for initialization
	void Awake ()
	{
		//Gather together all refrences you will need later on
		environment = GameObject.Find ("Environment");
		ball = GameObject.Find ("DSPlayerBall");
		root = GameObject.Find ("MovablePlayfield");
		floor = GameObject.Find ("DSBasementFloor");

		//Build the values for xExt and zExt from xHalfExt and zHalfExt
		xExt = xHalfExt * 2 + 1;
		zExt = zHalfExt * 2 + 1;

		//Build an offset for the dyn playfield from the BasePlatform e.g. the bigger halfExtent value in unity units
		int dynamicOffset = Mathf.Max (xHalfExt, zHalfExt);
		//Calculate a scale factor for scaling the non-movable environment (and therefore the camera) and the BasePlatform 
		// the factors that the environment are scaled for right now are for x/zHalfExt =1, scale accordingly
		//i.e. the playfield/environment should be as big as the dynamic field
		scaleEnv = dynamicOffset * 0.2f + 0.25f;

		//Scale Environment
		environment.transform.localScale = new Vector3 (1, 1, 1) * scaleEnv;

		//Scale  + position BasePlate
		scale = dynamicOffset * tileSize;
		baseScale = dynamicOffset * 6f + 6f;
		floor.transform.localScale = new Vector3 (1 * baseScale, 1, 1 * baseScale);
		ball.transform.localScale = new Vector3 (1, 1, 1) * (scaleEnv + 1);

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
				new Vector3 (i * tileSize, 0, scale * 2 + 2) + new Vector3 (-scale, 0, -scale),
				Quaternion.Euler (0, 0, 0),
				root.transform
			);	
			Instantiate (
				outerWall,
				new Vector3 (i * tileSize, 0, -(scale * 2 + 2)) + new Vector3 (-scale, 0, scale),
				Quaternion.Euler (0, 0, 0),
				root.transform
			);
		}
		for (int i = 0; i < zExt; i++) {
			Instantiate (
				outerWall,
				new Vector3 (-(scale * 2 + 2), 0, i * tileSize) + new Vector3 (scale, 0, -scale),
				Quaternion.Euler (0, 90, 0),
				root.transform
			);
			Instantiate (
				outerWall,
				new Vector3 (scale * 2 + 2, 0, i * tileSize) + new Vector3 (-scale, 0, -scale),
				Quaternion.Euler (0, 90, 0),
				root.transform
			);
		}
	}

	private void placeWallForField (int x, int z, WallsOfCell cell)
	{
		//South
		if (z < zExt - 1 && cell.south && tracker.walls [x, z + 1].north) {
			Instantiate (
				innerWall,
				innerWalls (x, z) + new Vector3 (-1, 0, 1) + new Vector3 (-2, 0, -2) * (xHalfExt - 1),
				Quaternion.Euler (0, 0, 0),
				root.transform);
		}
		//East
		if (x > 0 && cell.east && tracker.walls [x - 1, z].west) {
			Instantiate (
				innerWall,
				innerWalls (x, z) + new Vector3 (-3, 0, -1) + new Vector3 (-2, 0, -2) * (xHalfExt - 1),
				Quaternion.Euler (0, 90, 0),
				root.transform);
		}
	}

	private Vector3 innerWalls (int x, int z)
	{
		return new Vector3 (-xExt + x * tileSize, 0, -zExt + z * tileSize);
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
						floorTileVector (x, z),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				} else if (x == tracker.end.x && z == tracker.end.z) {
					Instantiate (
						exitTile,
						floorTileVector (x, z),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				} else {
					Instantiate (
						floorTiles [Random.Range (0, floorTiles.Length)],
						floorTileVector (x, z),
						Quaternion.Euler (0, 90, 0),
						root.transform);
				}
			}
		}
	}

	private Vector3 floorTileVector (int x, int z)
	{
		return new Vector3 (x, 0, z) * tileSize + new Vector3 (-scale, 0, -scale);
	}

	//You might need this more than once...
	void placeBallStart ()
	{
		//Reset Physics
		ball.GetComponent<Rigidbody> ().velocity = Vector3.zero;

		//Place the ball
		Debug.Log ("START: " + tracker.start.x + " " + tracker.start.z);
		ball.transform.position = new Vector3 (tracker.start.x - xHalfExt, 1, tracker.start.z - zHalfExt) * tileSize;
	}

	public void EndzoneTrigger (GameObject other)
	{
		//Check if ball first...
		//Player has fallen onto ground plane, reset
		if (other == ball) {
			placeBallStart ();
		}
	}

	public void winTrigger (GameObject other)
	{
		//Check if ball first...
		if (other == ball) {
			Application.LoadLevel (0);
		}
		//Destroy this maze

		//Generate new maze

		//Player has fallen onto ground plane, reset
	}
}