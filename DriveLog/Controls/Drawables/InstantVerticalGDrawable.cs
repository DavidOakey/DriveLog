namespace DriveLog.Controls.Drawables
{
	public class InstantVerticalGDrawable : IDrawable
	{
		public RectF DrawRect { get; set; }= new RectF();
		public void Draw(ICanvas canvas, RectF dirtyRect)
		{
			canvas.FillColor = Colors.Green;
			canvas.DrawRectangle(DrawRect);
		}
	}
}
