using DriveLog.Controlers;
using DriveLog.Models;
using DriveLog.Models.Enums;
using DriveLog.Pages;
using DriveLog.ViewModels.Cells;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveLog.ViewModels.Pages
{
	public class DashboardPageViewModel : BaseViewModel
	{
		public enum DashBoardTabs
		{
			LastTrips,
			AllTrips,
			CarTripsOnly,
			FootTripsOnly,
			BikeTripsOnly
		}

		List<TripViewCellViewModel>? _viewData;
		private string _errorText = string.Empty;

		private DashBoardTabs _currentTabTooShow = DashBoardTabs.LastTrips;
		public ICommand MapButtonCommand => new Command(MapButtonClicked);
		public ICommand StartStopTripCommand => new Command(StartStopTripClicked);

		public ICommand LastTripsButtonCommand => new Command(LastTripsButtonClicked);
		public ICommand AllTripsButtonCommand => new Command(AllTripsButtonClicked);

		public DashboardPageViewModel()
		{			
			TripManager.OnStatusChanged += TripManagerUpdate;
			TripManager.Init();
		}

		~DashboardPageViewModel()
		{
			TripManager.OnStatusChanged -= TripManagerUpdate;
		}

		public DashBoardTabs CurrentTabTooShow
		{
			get
			{
				return _currentTabTooShow;
			}
			set
			{
				if(_currentTabTooShow != value)
				{
					_currentTabTooShow = value;
					OnPropertyChanged();
					UpdateVisibleTabElements();
				}
			}
		}

		private void UpdateVisibleTabElements()
		{
			OnPropertyChanged(nameof(ShowAllTrips));
		}

		private void TripManagerUpdate(TripStatusChange status, TripData? data)
		{
			MainThread.InvokeOnMainThreadAsync(() =>
			{
				switch (status)
				{
					case TripStatusChange.LastTripChanged:
						UpdateLastTrip();
						break;
					case TripStatusChange.TripListChanged:
						UpdateTripList();
						break;
					case TripStatusChange.TripRecordingStarted:
					case TripStatusChange.TripRecordingStopped:
						UpdateRecordingStatus();
						break;
					case TripStatusChange.TripRecordingUpdate:
						UpdateRecordingStatus();
						break;
					case TripStatusChange.AccelerometerUpdate:
						AccelerometerReadingChange();
						break;
					case TripStatusChange.LocationUpdate:
						UpdateCurrentLocation();
						break;
					case TripStatusChange.TripRecordingError:
						UpdateError("RecordingError");
						break;
					case TripStatusChange.InitialisationError:
						UpdateError("Init Error");
						break;
				}
			});
		}

		private void UpdateCurrentLocation()
		{
			OnPropertyChanged(nameof(CurrentSpeedText));
		}
		private void UpdateError(string error)
		{
			ErrorText = error;
			OnPropertyChanged(nameof(CurrentErrorStatus));
		}

		private void UpdateRecordingStatus()
		{
			OnPropertyChanged(nameof(CurrentRecordingStatus));
		}
		
		private void UpdateTripList()
		{
			ViewData = TripManager.Trips?.Select(t => new TripViewCellViewModel { Model = t }).ToList() ?? new List<TripViewCellViewModel>();
		}

		private void UpdateLastTrip()
		{
			OnPropertyChanged(nameof(LastTripTimeStamp));
			OnPropertyChanged(nameof(LastTripDistance));
			OnPropertyChanged(nameof(LastTripDuration));
			OnPropertyChanged(nameof(LastTripTripData));
		}

		public string CurrentSpeedText
		{
			get
			{
				double? speed = GetSpeed(TripManager.LastLocationReading, SpeedUnits.mph);
				return string.Format("{0} mph", speed == null ? "NA" : double.Round(speed.Value));
			}
		}

		private double? GetSpeed(Location location, SpeedUnits units)
		{
			if(location?.Speed == null)
			{
				return null;
			}
			switch (units)
			{
				default:
				case SpeedUnits.mph:
					return UnitConverters.KilometersToMiles((location.Speed?? 0) * 3.6);
				case SpeedUnits.kmph:
					return location.Speed * 3.6;
				case SpeedUnits.mps:
					return location.Speed;
			}
		}

		public string LastTripTimeStamp
		{
			get
			{
				return TripManager.LastTrip.StartTimeStamp.ToShortDateString() + 
					" " + 
					TripManager.LastTrip.StartTimeStamp.ToShortTimeString();
			}
		}

		public string LastTripDistance
		{
			get
			{
				return string.Format("{0} mls", 
				UnitConverters.KilometersToMiles(TripManager.LastTrip.TotalDistanceM / 1000));
			}
		}

		public string LastTripDuration
		{
			get
			{
				return string.Format("{0} mins", TripManager.LastTrip?.Duration.Minutes);
			}
		}

		public TripData LastTripTripData
		{
			get
			{
				return TripManager.LastTrip;
			}
		}

		public List<TripData> Trips
		{
			get
			{
				return TripManager.Trips;
			}
		}

		public bool ShowAllTrips
		{
			get
			{
				return CurrentTabTooShow != DashBoardTabs.LastTrips;
			}
		}

		public List<TripViewCellViewModel>? ViewData
		{
			get
			{
				return _viewData;
			}
			set
			{
				if (_viewData != value)
				{
					_viewData = value;
					OnPropertyChanged();
				}
			}
		}

		public bool IsRecording
		{
			get
			{
				return TripManager.IsRecording;
			}
		}

		public bool CurrentRecordingStatus
		{
			get
			{
				return TripManager.IsRecording;
			}
		}

		public bool CurrentErrorStatus
		{
			get
			{
				return string.IsNullOrEmpty(_errorText);
			}
		}

		private void AccelerometerReadingChange()
		{
			try
			{
				OnPropertyChanged(nameof(AccelerationText));
				OnPropertyChanged(nameof(AccelerationMaxText));
				OnPropertyChanged(nameof(LastAccelerationReading));
			}
			catch (Exception ex)
			{
				int i = 9;
			}
		}

		public Vector3 LastAccelerationReading
		{
			get
			{
				return TripManager.CorrectedLastAccelerationReading;
			}
		}

		public string ErrorText
		{
			get
			{
				return _errorText;
			}
			set
			{
				if (_errorText != value)
				{
					_errorText = value;
					OnPropertyChanged();
				}
			}
		}

		public string AccelerationText
		{
			get
			{
				return string.Format(" X: {0}\n, Y: {1}\n, Z: {2}\n, Mag: {3}",
					TripManager.LastAccelerationReading.X,
					TripManager.LastAccelerationReading.Y,
					TripManager.LastAccelerationReading.Z,
					TripManager.LastAccelerationReading.Length());
			}
		}
		public string AccelerationMaxText
		{
			get
			{
				return string.Format(" X: {0}\n, Y: {1}\n, Z: {2}\n, Mag: {3}",
					TripManager.MaxAccelerationReading.X,
					TripManager.MaxAccelerationReading.Y,
					TripManager.MaxAccelerationReading.Z,
					TripManager.MaxAccelerationReading.Length());
			}
		}

		public bool IsRecordingTrip
		{
			get
			{
				return TripManager.IsRecording;
			}
		}

		private void StartStopTripClicked()
		{
			if (TripManager.IsRecording)
			{
				TripManager.StopCurrentTrip();
			}
			else
			{
				TripManager.StartTripNewTrip();
			}
			OnPropertyChanged(nameof(IsRecordingTrip));
		}	

		private void MapButtonClicked()
		{
			if (IsBusy)
			{
				return;
			}
			IsBusy = true;
			ShowTripPage();
			IsBusy = false;
		}

		private void LastTripsButtonClicked()
		{
			CurrentTabTooShow = DashBoardTabs.LastTrips;
		}
		private void AllTripsButtonClicked()
		{
			CurrentTabTooShow = DashBoardTabs.AllTrips;
		}

		private async void ShowTripPage()
		{
			if (((NavigationPage?)App.Current?.MainPage) != null)
			{
				await ((NavigationPage)App.Current.MainPage).PushAsync(new TripPage { BindingContext = new TripPageViewModel() });
			}
		}
	}
}
