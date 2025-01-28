namespace DriveLog.Controls.Drawables;
public class VerticalGDrawable : IDrawable
{
	private GForceView Parent { get; set; }
	private float _maxReading = 0;
	private float _minReading = 0;
	private float _maxGraphValue = 0.1f;
	private float _minGraphValue = 0;

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
				if(CalculateLimits())
				{
					Parent?.InvalidateVerticalGDrawable();
				}
			}
		}
	}

	public float MinReading
	{
		get
		{
			return _minReading;
		}
		set
		{
			if (_minReading != value)
			{
				_minReading = value;
				if (CalculateLimits())
				{
					Parent?.InvalidateVerticalGDrawable();
				}
			}
		}
	}

	public float CurrentReading { get; set; } = 0.01f;
	public Dictionary<float, string> TickMarks { get; set; } = new Dictionary<float, string>();

	public VerticalGDrawable(GForceView parent)
	{
		Parent = parent;
		CalculateLimits();
	}

	public void Draw(ICanvas canvas, RectF dirtyRect)
	{
		float edgeWidth = dirtyRect.Width * 0.15f;

		RectF barRect = new RectF(edgeWidth, 0, dirtyRect.Width - (3 * edgeWidth), dirtyRect.Height);
		canvas.StrokeColor = Colors.DarkGray;
		canvas.StrokeSize = 2.0f;
		canvas.FontColor = Colors.Black;
		canvas.FontSize = 8;

		// Draw Outline
		canvas.DrawRectangle(barRect);
		float yPos;

		// Add ticks and labels
		foreach (var item in TickMarks)
		{
			yPos = barRect.Height * (1.0f - ((item.Key - _minGraphValue) / (_maxGraphValue - _minGraphValue)));
			canvas.DrawLine(0, yPos, edgeWidth, yPos);
			canvas.DrawLine(barRect.Right, yPos,
			barRect.Right + edgeWidth, yPos);
			canvas.DrawString(item.Value, barRect.Right + edgeWidth, yPos - 4, 25, 10, HorizontalAlignment.Left, VerticalAlignment.Top);
		}

		// Add a full line at 0
		float zeroPos = dirtyRect.Height * (1.0f - ((0 - _minGraphValue) / (_maxGraphValue - _minGraphValue)));
		canvas.DrawLine(barRect.Left, zeroPos, barRect.Right, zeroPos);

		// Fill the Current(instant) reading marker 
		canvas.FillColor = Colors.Green;
		yPos = dirtyRect.Height * (1.0f - ((CurrentReading - _minGraphValue) / (_maxGraphValue - _minGraphValue)));
		
		Rect instantRect = new Rect(barRect.Left, zeroPos, barRect.Width, yPos - zeroPos);
		canvas.FillRectangle(instantRect);
	}

	private bool SetGraphLimits(float min, float max)
	{
		bool returnValue = _minGraphValue != min || _maxGraphValue != max;
		_minGraphValue = min;
		_maxGraphValue = max;
		return returnValue;
	}

	private bool CalculateLimits()
	{
		// TODO Find a better way
		// Number of circles is not linier so no clear algorithm
		// Deliniation has to look right even is not nice mathamaticly
		float min = 0;
		float max = 0;
		Dictionary<float, string> ticks = new Dictionary<float, string>();
		if (MinReading > -0.01)
		{
			min = -0.01f;
		}
		else if (MinReading > -0.05)
		{
			min = -0.5f;
			ticks.Add(-0.05f, "-0.05");
		}
		else if (MinReading > -0.1)
		{
			min = -0.1f;
			ticks.Add(-0.1f, "-0.1");
			ticks.Add(-0.05f, "-0.05");
		}
		else if (MinReading > -0.25)
		{
			min = -0.25f;
			ticks.Add(0.25f, "-0.25");
		}
		else if (MinReading > -0.5)
		{
			min = -0.5f;
			ticks.Add(-0.5f, "-0.5");
			ticks.Add(-0.25f, "-0.25");			
		}
		else if (MinReading > -1.0)
		{
			min = -1f;
			ticks.Add(1f, "-1");
			ticks.Add(0.5f, "-0.5");			
		}
		else if (MinReading > -2.0)
		{
			min = -2f;
			ticks.Add(2f, "-2");
			ticks.Add(1f, "-1");
			
		}
		else if (MinReading > -3.0)
		{
			min = -3f;
			ticks.Add(3f, "-3");
			ticks.Add(2f, "-2");
			ticks.Add(1f, "-1");
			
			
		}
		else if (MinReading > -5.0)
		{
			min = -5f;
			ticks.Add(5f, "-5");
			ticks.Add(4f, "-4");
			ticks.Add(3f, "-3");
			ticks.Add(2f, "-2");
			ticks.Add(1f, "-1");
		}
		else if (MinReading > -10.0)
		{
			min = -10f;
			ticks.Add(-10f, "-10");
			ticks.Add(-8f, "-8");
			ticks.Add(-6f, "-6");
			ticks.Add(-4f, "-4");
			ticks.Add(-2f, "-2");
		}
		else
		{
			min = MathF.Round(MinReading, MidpointRounding.AwayFromZero);
			ticks.Add(min, min.ToString());
		}

		ticks.Add(0.0f, "0");

		if (MaxReading < 0.1)
		{
			max = 0.1f;
			ticks.Add(0.05f, "0.05");
			ticks.Add(0.1f, "0.1");
		}
		else if (MaxReading < 0.25)
		{
			max = 0.25f;
			ticks.Add(0.25f, "0.25");
		}
		else if (MaxReading < 0.5)
		{
			max = 0.5f;
			ticks.Add(0.25f, "0.25");
			ticks.Add(0.5f, "0.5");
		}
		else if (MaxReading < 1.0)
		{
			max = 1f;
			ticks.Add(0.5f, "0.5");
			ticks.Add(1f, "1");
		}
		else if (MaxReading < 2.0)
		{
			max = 2f;
			ticks.Add(1f, "1");
			ticks.Add(2f, "2");
		}
		else if (MaxReading < 3.0)
		{
			max = 3f;
			ticks.Add(1f, "1");
			ticks.Add(2f, "2");
			ticks.Add(3f, "3");
		}
		else if (MaxReading < 5.0)
		{
			max = 5f;
			ticks.Add(1f, "1");
			ticks.Add(2f, "2");
			ticks.Add(3f, "3");
			ticks.Add(4f, "4");
			ticks.Add(5f, "5");
		}
		else if (MaxReading < 10.0)
		{
			max = 10f;
			ticks.Add(2f, "2");
			ticks.Add(2f, "4");
			ticks.Add(2f, "6");
			ticks.Add(2f, "8");
			ticks.Add(2f, "10");
		}
		else
		{
			max = MathF.Round(MaxReading, MidpointRounding.AwayFromZero);
			ticks.Add(max, max.ToString());
		}

		bool returnValue = SetGraphLimits(min, max);
		if(returnValue)
		{
			TickMarks = ticks;
		}

		return returnValue;
	}
}
