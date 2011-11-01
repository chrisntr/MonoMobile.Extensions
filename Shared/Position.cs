using System;

namespace MonoMobile.Extensions
{
	public class Position
	{
		public Coordinates Coords { get; set; }
		public DateTimeOffset Timestamp { get; set; }

		public Position()
		{
			Coords = new Coordinates();
		}
	}
}