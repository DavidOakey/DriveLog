namespace DriveLog.Models
{
	public class TripData
	{
		public bool IsRecording { get; set; } = false;
		public Guid TripID {get;set;}
		public string AppVersion { get; set; }
		public DateTime StartTimeStamp { get; set; }
		public DateTime EndTimeStamp { get; set; }
		public string Title { get; set; } = string.Empty;
		public string Description { get; set; } = string.Empty;
		public double TotalDistanceM { get; set; } = 0;
		public TimeSpan Duration;
		public List<TripLocationData> LocationData {get;set;} = new List<TripLocationData>();
		public List<TripAccelerometerData> AccelerometerData { get; set; } = new List<TripAccelerometerData>();
		public int AccelerometerDataNotAddCount { get; set; } = 0;
		public double TotalElevationIncrease { get; set; }
		public double TotalElevationDecrease { get; set; }
		public double MaxSpeed { get; set; }
	}
}
