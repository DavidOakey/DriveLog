using DriveLog.Models.Enums;
using Microsoft.Maui.Devices.Sensors;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace DriveLog.Controls.Drawables;

public class SpeedDrawable : IDrawable
{
	public int Speed { get; set; }
	public SpeedUnits Units { get; set; } = SpeedUnits.mph;
	public Color RingColor { get; set; } = Colors.Red;
	public Color CentreColor { get; set; } = Colors.White;
	public Color TextColor { get; set; } = Colors.Black;

	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		float maxRadius = MathF.Min(dirtyRect.Height, dirtyRect.Width) * 0.5f;

		canvas.FontColor = TextColor;
		canvas.StrokeColor = RingColor;
		canvas.FillColor = CentreColor;
		canvas.StrokeSize = maxRadius * 0.1f;
		canvas.FontSize = maxRadius;

		canvas.FillCircle(dirtyRect.Center, maxRadius);
		canvas.DrawCircle(dirtyRect.Center, maxRadius);
		canvas.DrawString(ConvertSpeedToText(Speed), dirtyRect.Center.X, dirtyRect.Center.Y + (maxRadius * 0.25f), HorizontalAlignment.Center);

		canvas.FontSize = maxRadius * 0.25f;
		canvas.DrawString(GetUnitsText(), dirtyRect.Center.X, dirtyRect.Center.Y + (maxRadius * 0.5f) + 5, HorizontalAlignment.Center);
	}

	private string ConvertSpeedToText(int speed)
	{
		double converted = 0;
		switch (Units)
		{
			default:
			case SpeedUnits.mph:
				converted = double.Round(UnitConverters.KilometersToMiles(speed * 3.6));
				break;
			case SpeedUnits.kmph:
				converted = double.Round(speed * 3.6);
				break;
			case SpeedUnits.mps:
				converted = speed;
				break;
		}

		return converted.ToString();
	}

	private string GetUnitsText()
	{
		switch (Units)
		{
			case SpeedUnits.mph:
				return "MPH";
			case SpeedUnits.kmph:
				return "KMPH";
			case SpeedUnits.mps:
				return "m/s";
			default:
				return "m/s";
		}
	}
}

