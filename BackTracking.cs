using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;

public class BackTracking
{
	private int xSize;
	private int zSize;
	private bool[,] visitTracker;
	public WallsOfCell[,] walls;
	private Backtracker[,] field;
	public Backtracker start;
	public Backtracker end;

	private int depth = 0;
	private int maxDepth = 0;

	public BackTracking (int x, int z)
	{
		xSize = x;
		zSize = z;

		visitTracker = new bool[xSize, zSize];
		field = new Backtracker[xSize, zSize];
		walls = new WallsOfCell[xSize, zSize];

		initField ();
	}

	private void initField ()
	{
		for (int x = 0; x < xSize; x++) {
			for (int z = 0; z < zSize; z++) {
				//Debug.Log ("Init Field: (" + x + " " + z + ")");
				field [x, z] = new Backtracker (x, z, getPossibleMoves (new Tuple2<int,int> (x, z)));
				visitTracker [x, z] = false;
				walls [x, z] = new WallsOfCell ();
			}
		}
	}

	public void generateCourse ()
	{
		Tuple2<int,int> startField = new Tuple2<int,int> (Random.Range (0, xSize - 1), Random.Range (0, zSize - 1));
		//Tuple2<int,int> startField = new Tuple2<int,int> (0, 0);
		start = field [startField.item1, startField.item2];
		visitTracker [startField.item1, startField.item2] = true;

		backtracking (start, 0);
	}

	private void backtracking (Backtracker move, int depth)
	{
		if (depth > maxDepth) {
			maxDepth = depth;
			end = move;
		}
		//Debug.Log (depth);
		if (existsUnvistedCell ()) {
			if (hasUnvistedNeigbours (move.possibleMoves)) {
				//Choose randomly one of the unvisited neighbours
				Backtracker nextMove = randomUnvisitedNeigbour (move.possibleMoves);
				//Push the current cell to the stack
				move.next = nextMove;
				nextMove.before = move;
				//remove Wall
				removeWall (move.x, move.z, nextMove.x, nextMove.z);
				//Make the chosen cell the current cell and mark it as visited
				visitTracker [nextMove.x, nextMove.z] = true;
				backtracking (nextMove, ++depth);
			} else {
				backtracking (move.before, --depth);
			}
		}
	}

	private void removeWall (int x, int z, int toX, int toZ)
	{
		//South
		if (z - toZ == -1) {
			walls [x, z].south = false;
		}
		//North
		if (toZ - z == -1) {
			walls [x, z].north = false;
		}
		//East
		if (toX - x == -1) {
			walls [x, z].east = false;
		}
		//West
		if (x - toX == -1) {
			walls [x, z].west = false;
		}
	}
		
	private bool existsUnvistedCell ()
	{
		for (int x = 0; x < xSize; x++) {
			for (int z = 0; z < zSize; z++) {
				if (!visitTracker [x, z]) {
					return true;
				}
			}
		}
		return false;
	}

	private bool hasUnvistedNeigbours (List<Tuple2<int,int>> possibleMoves)
	{
		string log = "";
		for (int x = 0; x < xSize; x++) {
			for (int z = 0; z < zSize; z++) {
				log = log + visitTracker [x, z];
			}
			log = log + "\n";
		}
		//Debug.Log (log);
		for (int i = 0; i < possibleMoves.Count; i++) {
			if (!visitTracker [possibleMoves [i].item1, possibleMoves [i].item2]) {
				return true;
			} 
		}

		return false;
	}

	private Backtracker randomUnvisitedNeigbour (List<Tuple2<int,int>> possibleMoves)
	{
		List<Tuple2<int,int>> unvistedNeigbours = new List<Tuple2<int,int>> ();

		//Debug.Log ("Before removing: " + possibleMoves.Count);

		for (int i = 0; i < possibleMoves.Count; i++) {
			//Debug.Log ("item: " + possibleMoves [i].item1 + " " + possibleMoves [i].item2);
			//Debug.Log ("isVisited: " + visitTracker [possibleMoves [i].item1, possibleMoves [i].item2]);
			if (!visitTracker [possibleMoves [i].item1, possibleMoves [i].item2]) {
				unvistedNeigbours.Add (new Tuple2<int, int> (possibleMoves [i].item1, possibleMoves [i].item2));
			}
		}
	
		//Debug.Log ("After removing: " + unvistedNeigbours.Count);
		int selectRnd = Random.Range (0, unvistedNeigbours.Count);
		Tuple2<int,int> selected = unvistedNeigbours [selectRnd];
		//Tuple2<int,int> selected = unvistedNeigbours [0];
		return field [selected.item1, selected.item2];
	}

	private List<Tuple2<int,int>> getPossibleMoves (Tuple2<int,int> cell)
	{
		List<Tuple2<int,int>> validMoves = new List<Tuple2<int,int>> ();

		if (cell.item1 - 1 >= 0) {
			validMoves.Add (new Tuple2<int,int> (cell.item1 - 1, cell.item2));
			//Debug.Log ("  possibleMoves: " + (cell.item1 - 1) + " " + (cell.item2));
		}

		if (cell.item2 - 1 >= 0) {
			validMoves.Add (new Tuple2<int,int> (cell.item1, cell.item2 - 1));
			//Debug.Log ("  possibleMoves: " + (cell.item1) + " " + (cell.item2 - 1));
		}

		if (cell.item2 + 1 < zSize) {
			validMoves.Add (new Tuple2<int,int> (cell.item1, cell.item2 + 1));
			//Debug.Log ("  possibleMoves: " + (cell.item1) + " " + (cell.item2 + 1));
		}

		if (cell.item1 + 1 < xSize) {
			validMoves.Add (new Tuple2<int,int> (cell.item1 + 1, cell.item2));
			//Debug.Log ("  possibleMoves: " + (cell.item1 + 1) + " " + (cell.item2));
		}
			
		return validMoves;
	}
}

