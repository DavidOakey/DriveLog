using DriveLog.Models;

namespace DriveLog.Extensions
{
	public static class TripDataExtensions
	{
		public static void CalculateTripMetrics(this TripData trip) 
		{
			trip.Duration = trip.EndTimeStamp - trip.StartTimeStamp;

			Location? currentPoint = null;

			if (trip.LocationData.Count > 1)
			{
				trip.LocationData.ForEach(p =>
					{
						trip.TotalDistanceM += currentPoint == null ? 0 : Location.CalculateDistance(currentPoint, p.Point, DistanceUnits.Kilometers) * 1000;
						currentPoint = p.Point;
						trip.TotalElevationIncrease += Math.Max(0, p.Point.Altitude ?? 0);
						trip.TotalElevationDecrease += Math.Min(0, p.Point.Altitude ?? 0);
					});
				trip.MaxSpeed = trip.LocationData.Max(p=>p.Point.Speed ?? 0);
			}
		}
	}
}
