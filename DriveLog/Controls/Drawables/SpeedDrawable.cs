using DriveLog.Models.Enums;

namespace DriveLog.Controls.Drawables
{
	public class SpeedDrawable : IDrawable
	{
		public int Speed {get; set; }
		public SpeedUnits Units { get; set; } = SpeedUnits.mph;
		public Color RingColor { get; set; } = Colors.Red;
		public Color CentreColor { get; set; } = Colors.White;

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			float maxRadius = MathF.Min(dirtyRect.Height, dirtyRect.Width) * 0.5f;

			canvas.FontColor = Colors.Black;
			canvas.StrokeColor = Colors.Red;
			canvas.FillColor = Colors.White;
			canvas.StrokeSize = maxRadius * 0.1f;
			canvas.FontSize = maxRadius;

			canvas.FillCircle(dirtyRect.Center, maxRadius);

			canvas.DrawString(Speed.ToString(), dirtyRect.Center.X , dirtyRect.Center.Y, 25, 10, HorizontalAlignment.Center, VerticalAlignment.Center);

			canvas.FontSize = maxRadius * 0.25f;
			canvas.DrawString(GetUnitsText(), dirtyRect.Center.X, dirtyRect.Center.Y + (maxRadius * 0.5f) + 5, 25, 10, HorizontalAlignment.Center, VerticalAlignment.Top);
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
}
