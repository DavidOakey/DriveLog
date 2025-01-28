using DriveLog.Models;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
using System.Windows.Input;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace DriveLog.Controls;

public class MapControl : ContentView
{
	public static readonly BindableProperty TripDataProperty = BindableProperty.Create(nameof(TripData), typeof(TripData), typeof(MapControl), null, propertyChanged: OnTripDataChanged);

	private Grid _mainGrid, _topButtonGrid;
	private Image? _homeImage, _settingsImage, _streetImage, _hybridImage, _satelliteImage;
	private Map _map;

	public ICommand HomeCommand => new Command(HomeClick);
	public ICommand SettingsCommand => new Command(SettingsClick);

	private static void OnTripDataChanged(BindableObject bindable, object oldValue, object newValue)
	{
		MapControl control = (MapControl)bindable;
		
		control?.DrawTrip();
	}

	public TripData TripData
	{
		get => (TripData)GetValue(TripDataProperty);
		set => SetValue(TripDataProperty, value);
	}

	public MapControl()
	{
		Content = _mainGrid = new Grid
		{
			RowDefinitions = {
				new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) },
				new RowDefinition { Height = new GridLength(1, GridUnitType.Star) }
			},
			ColumnDefinitions = {
				new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) }
			}
		};

		_mainGrid.Add(CreateTopButtonGrid(), 0, 0);
		_mainGrid.Add(_map = new Map(), 0, 1);
	}

	private Grid CreateTopButtonGrid()
	{
		_topButtonGrid = new Grid
		{
			RowDefinitions = {
				new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) }
			},
			ColumnDefinitions = {
				new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) },
				new ColumnDefinition { Width = new GridLength(1, GridUnitType.Star) },
				new ColumnDefinition { Width = new GridLength(1, GridUnitType.Auto) }
			}
		};

		_topButtonGrid.Add(_homeImage = new Image { Margin = new Thickness(5), Source="home.png", HeightRequest= 50,WidthRequest= 50,Aspect=Aspect.Fill },0,0);
		_topButtonGrid.Add(_settingsImage = new Image { Margin = new Thickness(5), Source = "settings.png", HeightRequest = 50, WidthRequest = 50, Aspect = Aspect.Fill },0,1);

		_homeImage.GestureRecognizers.Add(new TapGestureRecognizer { Command = HomeCommand });
		_settingsImage.GestureRecognizers.Add(new TapGestureRecognizer { Command = SettingsCommand });

		return _topButtonGrid;
	}

	private void DrawTrip()
	{
		ZoomToTrip();
		_map.MapElements.Clear();
		Polyline routeLine = new Polyline { StrokeWidth = 3, StrokeColor = Colors.Red };
		TripData.LocationData.ForEach(l => routeLine.Geopath.Add(l.Point));
		_map.MapElements.Add(routeLine);
	}

	private void ZoomToTrip()
	{
		double minLat = TripData.LocationData.Min(l=>l.Point.Latitude);
		double maxLat = TripData.LocationData.Max(l => l.Point.Latitude);
		double minLong = TripData.LocationData.Min(l => l.Point.Longitude);
		double maxLong = TripData.LocationData.Max(l => l.Point.Longitude);

		Location centre = new Location((minLat + maxLat)/2.0f, (minLong + maxLong)/2.0f);
		_map.MoveToRegion(new MapSpan(centre, (maxLat - minLat) + 0.01, (maxLong - minLong) + 0.01));
	}

	private void HomeClick()
	{
		ZoomToTrip();
	}

	private void SettingsClick()
	{
		//throw new NotImplementedException();
	}
}