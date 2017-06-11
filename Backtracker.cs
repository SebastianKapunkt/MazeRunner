using System;
using System.Collections.Generic;

public class Backtracker
{
	public int x {get; set;} 
	public int z {get; set;} 

	public List<Tuple2<int,int>> possibleMoves { set; get; }

	public Backtracker next { get; set; }

	public Backtracker before { get; set; }

	public Backtracker (int x, int z, List<Tuple2<int,int>> possibleMoves)
	{
		this.x = x;
		this.z = z;
		this.possibleMoves = possibleMoves;
	}
}



