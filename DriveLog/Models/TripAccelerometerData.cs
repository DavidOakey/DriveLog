using System.Numerics;

namespace DriveLog.Models
{
	public class TripAccelerometerData
	{
		public DateTime TimeStamp {  get; set; }
		public Vector3 Acceleration { get; set; }
		public Quaternion Orientation { get; internal set; }
	}
}
