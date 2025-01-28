using DriveLog.Models;
using Microsoft.Maui.Maps;
using System.Windows.Input;

namespace DriveLog.ViewModels
{
	public class MapViewViewModel : BaseViewModel
	{
		public ICommand MapTypeStreetCommand => new Command(() => MapTypeStreetClicked());
		public ICommand MapTypeSatelliteCommand => new Command(() => MapTypeSatelliteClicked());
		public ICommand MapTypeHybridCommand => new Command(() => MapTypeHybridClicked());

		private MapType _mapType = MapType.Street;

		public MapType MapType
		{
			get
			{
				return _mapType;
			}
			set
			{
				if (value != _mapType)
				{
					_mapType = value;
					OnPropertyChanged();
					UpdateMapTypeButtons();
				}
			}
		}

		private void UpdateMapTypeButtons()
		{
			OnPropertyChanged(nameof(ButtonMapTypeStreetEnabled));
			OnPropertyChanged(nameof(ButtonMapTypeSatelliteEnabled));
			OnPropertyChanged(nameof(ButtonMapTypeHybridEnabled));
		}

		public bool ButtonMapTypeStreetEnabled
		{
			get
			{
				return _mapType == MapType.Street;
			}
		}

		public bool ButtonMapTypeSatelliteEnabled
		{
			get
			{
				return _mapType == MapType.Satellite;
			}
		}

		public bool ButtonMapTypeHybridEnabled
		{
			get
			{
				return _mapType == MapType.Hybrid;
			}
		}

		public TripData Model { get; internal set; }

		private void MapTypeStreetClicked()
		{
			MapType = MapType.Street;
		}

		private void MapTypeSatelliteClicked()
		{
			MapType = MapType.Satellite;
		}

		private void MapTypeHybridClicked()
		{
			MapType = MapType.Hybrid;
		}
	}
}
