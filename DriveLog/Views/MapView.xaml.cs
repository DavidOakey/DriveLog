namespace DriveLog.Views;

public partial class MapView : ContentView
{
	public MapView()
	{
		InitializeComponent();
		DrawRoute();
	}

	public void DrawRoute()
	{
		mapC.MoveToRegion(new Microsoft.Maui.Maps.MapSpan(new Location(0, 0),2,2));
	}
}