namespace DriveLog.Controls.Drawables;

public class HorizontalGDrawable : IDrawable
{
	private List<PointF>? _envelope;
	private float _maxReading;
	private float _graphMax;
	private float _maxRadius;
	private float _graphUnitFactor;

	private GForceView Parent {  get; set; }

	public PointF CurrentReading { get; set; } = new PointF(0, 0);
	public float InstantPointSize { get; set; } = 3.0f;

	public List<PointF> Envelope
	{
		get
		{
			return _envelope ?? new List<PointF>();
		}
		set
		{
			_envelope = value;
		}
	}

	public float MaxReading
	{
		get
		{
			return _maxReading;
		}
		set
		{
			if (_maxReading != value)
			{
				_maxReading = value;
				Parent?.InvalidateHorizontalGDrawable();
			}
		}
	}

	public HorizontalGDrawable(GForceView parent)
	{
		Parent = parent;
	}

	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		DrawRangeCircle(canvas, dirtyRect);		
		DrawEnvelope(canvas, dirtyRect);
		DrawCurrent(canvas, dirtyRect);
	}	

	public void UpdateEnvelope(List<PointF> envelope)
	{
		Envelope = envelope;
	}
	
	private void DrawCurrent(ICanvas canvas, RectF dirtyRect)
	{
		SolidColorBrush instantPaint = new SolidColorBrush
		{
			Color = Colors.Green
		};

		canvas.SetFillPaint(instantPaint, dirtyRect);
		//canvas.FillColor = Colors.Green;
		canvas.FillCircle(dirtyRect.Center.X + (CurrentReading.X * _graphUnitFactor), 
			dirtyRect.Center.Y - (CurrentReading.Y * _graphUnitFactor), 
			InstantPointSize);
	}

	private void DrawEnvelope(ICanvas canvas, RectF dirtyRect)
	{
		if (Envelope?.Count < 2)
		{
			return;
		}

		PathF? path = null;
		Envelope?.ForEach(p =>
		{
			if (path == null)
			{
				path = new PathF();
				path.MoveTo(dirtyRect.Center.Y + (p.X * _graphUnitFactor), dirtyRect.Center.Y - (p.Y * _graphUnitFactor));
			}
			else
			{
				path.LineTo(dirtyRect.Center.Y + (p.X * _graphUnitFactor), dirtyRect.Center.Y - (p.Y * _graphUnitFactor));
			}
		});

		Color endColor = Colors.LightBlue.WithAlpha(255 / 150);
		RadialGradientPaint backGroundPaint = new RadialGradientPaint
		{
			StartColor = Color.FromRgba(255, 255, 255, 150),
			EndColor = endColor
		};

		canvas.SetFillPaint(backGroundPaint, dirtyRect);
		canvas.FillPath(path, WindingMode.NonZero);
	}

	private void DrawRangeCircle(ICanvas canvas, RectF dirtyRect)
	{
		_maxRadius = MathF.Min(dirtyRect.Height, dirtyRect.Width) * 0.5f;

		canvas.FontColor = Colors.Black;
		canvas.StrokeColor = Colors.GreenYellow;
		canvas.StrokeSize = 2.0f;
		canvas.FontSize = 8;

		RadialGradientPaint backGroundPaint = new RadialGradientPaint
		{
			StartColor = Colors.White,
			EndColor = Colors.LightGray
		};

		canvas.SetFillPaint(backGroundPaint, dirtyRect);
		canvas.FillCircle(dirtyRect.Center, _maxRadius);

		Dictionary<float, string> labels = CalculateRangeMarkers();
		_graphUnitFactor = _maxRadius / _graphMax;

		foreach (var item in labels)
		{
			canvas.DrawCircle(dirtyRect.Center, _maxRadius * item.Key);
			canvas.DrawString(item.Value, dirtyRect.Center.X + (_maxRadius * item.Key), dirtyRect.Center.Y, 25, 10, HorizontalAlignment.Left, VerticalAlignment.Top);
		};
	}

	private Dictionary<float, string> CalculateRangeMarkers()
	{
		// TODO Find a better way
		// Number of circles is not linier so no clear algorithm
		// Deliniation has to look right even is not nice mathamaticly
		if (MaxReading < 0.1)
		{
			_graphMax = 0.1f;
			return new Dictionary<float, string>() {
				{ 0.5f, "0.05" },
				{ 1.0f, "0.1" }
			};
		}
		if (MaxReading < 0.5)
		{
			_graphMax = 0.5f;
			return new Dictionary<float, string>() { 
				{ 0.5f, "0.25" }, 
				{ 1.0f, "0.5" } 
			};
		}
		if (MaxReading < 1)
		{
			_graphMax = 1.0f;
			return new Dictionary<float, string>() {
				{ 0.5f, "0.5" },
				{ 1.0f, "1" }
			};
		}
		if (MaxReading < 2)
		{
			_graphMax = 2.0f;
			return new Dictionary<float, string>() {
				{ 0.5f, "1" },
				{ 1.0f, "2" }
			};
		}
		if (MaxReading < 3)
		{
			_graphMax = 3.0f;
			return new Dictionary<float, string>() {
				{ 0.333f, "1" },
				{ 0.666f, "2" },
				{ 1.0f, "3" }
			};
		}
		if (MaxReading < 5)
		{
			_graphMax = 5.0f;
			return new Dictionary<float, string>() {
				{ 0.5f, "2.5" },
				{ 1.0f, "5" }
			};
		}
		if (MaxReading < 6)
		{
			_graphMax = 6.0f;
			return new Dictionary<float, string>() {
				{ 0.333f, "2" },
				{ 0.666f, "4" },
				{ 1.0f, "6" }
			};
		}
		if (MaxReading < 10)
		{
			_graphMax = 10.0f;
			return new Dictionary<float, string>() {
				{ 0.2f, "2" },
				{ 0.4f, "4" },
				{ 0.6f, "6" },
				{ 0.8f, "8" },
				{ 1.0f, "10" }
			};
		}
		_graphMax = MaxReading + 1.0f;
		return new Dictionary<float, string>() {
			{ 0.2f, "" },
			{ 0.4f, "" },
			{ 0.6f, "" },
			{ 0.8f, "" },
			{ 1.0f, (MaxReading + 1).ToString() }
		};
	}
}
