using System;

public class Tuple2<T1, T2>
{
	public T1 item1 { get; set; }

	public T2 item2 { get; set; }

	internal Tuple2 (T1 first, T2 second)
	{
		item1 = first;
		item2 = second;
	}
}


