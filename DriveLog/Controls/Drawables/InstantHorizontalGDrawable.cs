namespace DriveLog.Controls.Drawables
{
	public class InstantHorizontalGDrawable : IDrawable
	{
		public PointF PointLocation { get; set; }=new PointF(0, 0);
		public float PointSize { get; set; } = 5.0f;

		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			canvas.FillColor = Colors.Green;
			canvas.FillCircle(PointLocation, PointSize);
		}
	}
}
