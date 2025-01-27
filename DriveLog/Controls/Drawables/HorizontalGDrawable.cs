using DriveLog.Models;
using Microsoft.Maui;
using Microsoft.Maui.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DriveLog.Controls.Drawables;

public class HorizontalGDrawable : IDrawable
{
	private List<PointF>? _envelope;
	private float _maxReading;
	private float _graphMax;
	
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
		float maxRadius = MathF.Min(dirtyRect.Height, dirtyRect.Width) * 0.5f;

		canvas.FillColor = Colors.Green;
		canvas.FillCircle(dirtyRect.Center.X + (maxRadius * (CurrentReading.X / _graphMax)), 
			dirtyRect.Center.Y + (maxRadius * (CurrentReading.Y / _graphMax)), 
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
				path.MoveTo(p);
			}
			else
			{
				path.MoveTo(p);
			}
		});

		canvas.FillColor = Color.FromRgba(25, 25, 25 , 100);
		canvas.StrokeSize = 2.0f;
		canvas.FillPath(path, WindingMode.NonZero);
	}

	private void DrawRangeCircle(ICanvas canvas, RectF dirtyRect)
	{
		canvas.FontColor = Colors.Black;
		canvas.StrokeColor = Colors.GreenYellow;
		canvas.StrokeSize = 2.0f;
		canvas.FontSize = 8;

		float maxRadius = MathF.Min(dirtyRect.Height, dirtyRect.Width) * 0.5f;
		Dictionary<float, string> labels = CalculateRangeMarkers();

		foreach (var item in labels)
		{
			canvas.DrawCircle(dirtyRect.Center, maxRadius * item.Key);
			canvas.DrawString(item.Value, dirtyRect.Center.X + (maxRadius * item.Key), dirtyRect.Center.Y, 25, 10, HorizontalAlignment.Left, VerticalAlignment.Top);
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
