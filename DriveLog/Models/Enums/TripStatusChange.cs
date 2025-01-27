using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveLog.Models.Enums
{
	public enum TripStatusChange
	{
		TripListChanged,
		TripRecordingStarted,
		TripRecordingUpdate,
		TripRecordingStopped,
		TripRecordingError,
		AccelerometerUpdate,
		InitialisationError,
		LastTripChanged,
		LocationUpdate
	}
}
