using DriveLog.Extensions;
using DriveLog.Models;
using DriveLog.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace DriveLog.Controlers
{
	public static class TripManager
	{
		private static bool _isRecording = false;
		private static TripData? _currentTripData;
		private static TripData? _lastTripData;
		private static List<TripData> _trips = new List<TripData>();
		
		// TODO Update This
		public static Vector3 LastAccelerationReading, MaxAccelerationReading, MinAccelerationReading;
		public static Vector3 LastRecordedAccelerationReading, CorrectedLastAccelerationReading;
		public static Vector3 Gravity = new Vector3(0, 0, 9.8f);
		private static OrientationSensorData CurrentOrientation;
		private static bool _accelerometerRecording = false;
		private static bool _tripRecording = false;
		private static bool _initialised = false;

		public static Action<TripStatusChange, TripData?>? OnStatusChanged;
		private static Location _lastLocationReading;

		public static void Init()
		{
			if(_initialised)
			{
				return;
			}
			_initialised = true;

			Geolocation.Default.LocationChanged += LocationChanged;

			var request = new GeolocationListeningRequest(GeolocationAccuracy.High);
			Geolocation.StartListeningForegroundAsync(request);

			Trips = FileData.LoadTripAll();

			if (IsAccelerometerSupported)
			{
				if (!Accelerometer.Default.IsMonitoring)
				{
					// Turn on accelerometer
					Accelerometer.Default.ReadingChanged += AccelerometerReadingChanged;
					Accelerometer.Default.Start(SensorSpeed.UI);
					IsAccelerometerRecording = true;
				}
			}
			else
			{
				StatusChanged(TripStatusChange.InitialisationError);
			}

			if (!OrientationSensor.Default.IsMonitoring)
			{
				// Turn on orientation
				OrientationSensor.Default.ReadingChanged += OrientationReadingChanged;
				OrientationSensor.Default.Start(SensorSpeed.UI);
			}
			else
			{
				StatusChanged(TripStatusChange.InitialisationError);
			}
		}

		private static void OrientationReadingChanged(object? sender, OrientationSensorChangedEventArgs e)
		{
			CurrentOrientation = e.Reading;
		}

		public static void StartTripNewTrip()
		{
			Init();
			if (!IsRecording)
			{
				CurrentTripData = new TripData { TripID = Guid.NewGuid(), StartTimeStamp = DateTime.Now };
				IsRecording = true;
				StatusChanged(TripStatusChange.TripRecordingStarted);
			}
		}
		public static void StopCurrentTrip()
		{
			if (IsRecording)
			{
				IsRecording = false;
				if (CurrentTripData != null)
				{
					CurrentTripData.EndTimeStamp = DateTime.Now;
					CurrentTripData.CalculateTripMetrics();
					StatusChanged(TripStatusChange.TripRecordingStopped);
					FileData.SaveTrip(CurrentTripData);
					AddTrip(CurrentTripData);
					LastTrip = CurrentTripData;
					CurrentTripData = null;
				}
			}
		}

		private static void AddTrip(TripData currentTripData)
		{
			Trips.Add(currentTripData);
			StatusChanged(TripStatusChange.TripListChanged, currentTripData);
		}

		public static bool IsAccelerometerSupported
		{
			get
			{
				return Accelerometer.Default.IsSupported;
			}
		}

		public static bool IsAccelerometerRecording
		{
			get
			{
				return _accelerometerRecording;
			}
			set
			{
				if (_accelerometerRecording != value)
				{
					_accelerometerRecording = value;
				}
			}
		}

		private static void LocationChanged(object? sender, GeolocationLocationChangedEventArgs e)
		{
			LastLocationReading = e.Location;
			if (IsRecording && CurrentTripData != null)
			{
				CurrentTripData.LocationData.Add(new TripLocationData { TimeStamp = DateTime.Now, Point = e.Location });
				StatusChanged(TripStatusChange.TripRecordingUpdate, CurrentTripData);
			}
		}

		private static void AccelerometerReadingChanged(object? sender, AccelerometerChangedEventArgs e)
		{
			UpdateAccelerometerData(e.Reading.Acceleration);
		}

		private static void UpdateAccelerometerData(Vector3 acceleration)
		{
			
			CorrectedLastAccelerationReading = Vector3.Transform(acceleration, CurrentOrientation.Orientation);
			LastAccelerationReading = acceleration;
			MaxAccelerationReading = CorrectedLastAccelerationReading.Length() > MaxAccelerationReading.Length() 
				? CorrectedLastAccelerationReading : MaxAccelerationReading;

			MinAccelerationReading = CorrectedLastAccelerationReading.Length() < MinAccelerationReading.Length()
				? CorrectedLastAccelerationReading : MinAccelerationReading;

			bool add = ReadingMeetsAddCriteria(CorrectedLastAccelerationReading) || CurrentTripData?.AccelerometerData?.Count == 0;
			if (IsRecording && CurrentTripData != null && !add)
			{
				CurrentTripData.AccelerometerDataNotAddCount++;
			}
			if (IsRecording && CurrentTripData != null &&  add)//ReadingMeetsAddCriteria(acceleration))
			{
				LastRecordedAccelerationReading = CorrectedLastAccelerationReading;
				CurrentTripData.AccelerometerData.Add(new TripAccelerometerData { TimeStamp = DateTime.Now, Acceleration = acceleration, Orientation = CurrentOrientation.Orientation });
				StatusChanged(TripStatusChange.TripRecordingUpdate, CurrentTripData);
			}
			else if (IsAccelerometerRecording)
			{
				StatusChanged(TripStatusChange.AccelerometerUpdate);
			}
		}

		private static bool ReadingMeetsAddCriteria(Vector3 acceleration)
		{
			float threshold = 0.01f;
			double angleThreshold = (2 * Math.PI) * 0.01;
			Vector3 range = MaxAccelerationReading - MinAccelerationReading;
			Vector3 delta = LastRecordedAccelerationReading - acceleration;

			var deltaAngleRadians = Math.Acos(Vector3.Dot(LastRecordedAccelerationReading, acceleration) / 
				(LastRecordedAccelerationReading.Length() * acceleration.Length()));

			return delta.X > (range.X * threshold) || 
				delta.Y > (range.Y * threshold) || 
				delta.Z > (range.Z * threshold) ||
				deltaAngleRadians > angleThreshold;
		}

		public static List<TripData> Trips
		{
			get
			{
				Init();
				return _trips;
			}
			set
			{
				if (_trips != value)
				{
					_trips = value;
					StatusChanged(TripStatusChange.TripListChanged);
				}
			}
		}

		public static TripData? CurrentTripData
		{
			get
			{
				return _currentTripData;
			}
			set
			{
				if (_currentTripData != value)
				{
					_currentTripData = value;
				}
			}
		}

		public static bool IsRecording
		{
			get
			{
				return _isRecording;
			}
			set
			{
				if (_isRecording != value)
				{
					_isRecording = value;
				}
			}
		}

		public static TripData? LastTrip 
		{ 
			get
			{
				return _lastTripData;
			}
			set
			{
				_lastTripData = value;
				StatusChanged(TripStatusChange.LastTripChanged);
			}
		}

		public static Location LastLocationReading
		{
			get
			{
				return _lastLocationReading;
			}
			set
			{
				if(_lastLocationReading != value)
				{
					_lastLocationReading = value;
					StatusChanged(TripStatusChange.LocationUpdate);
				}
			}
		}

		private static void StatusChanged(TripStatusChange status, TripData? trip = null)
		{
			OnStatusChanged?.Invoke(status, trip);
		}
	}
}
