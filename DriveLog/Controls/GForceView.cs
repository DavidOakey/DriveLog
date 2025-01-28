using DriveLog.Controls.Drawables;
using System.Numerics;

namespace DriveLog.Controls
{
	public class GForceView : Grid
	{
		public static readonly BindableProperty AccelerometerDataProperty = BindableProperty.Create(nameof(AccelerometerData), typeof(IList<Vector3>), typeof(GForceView), new List<Vector3>(), propertyChanged: OnAccelerometerChanged);
		public static readonly BindableProperty CurrentReadingProperty = BindableProperty.Create(nameof(CurrentReading), typeof(Vector3), typeof(GForceView), new Vector3(0, 0, 0), propertyChanged: OnCurrentReadingChanged);

		private GraphicsView VerticalGView;
		private GraphicsView HorizontalGView;

		private HorizontalGDrawable? _horizontalGDrawable;
		private VerticalGDrawable? _verticalGDrawable;

		private float _minVertical = 0;
		private float _maxVertical = 0;
		private float _maxHorizontal = 0;

		private List<PointF> _envelope = new List<PointF>();

		private static void OnAccelerometerChanged(BindableObject bindable, object oldValue, object newValue)
		{
			(bindable as GForceView)?.UpdateView();
		}

		private static void OnCurrentReadingChanged(BindableObject bindable, object oldValue, object newValue)
		{
			(bindable as GForceView)?.UpdateCurrentReading();
		}

		public IList<Vector3> AccelerometerData
		{
			get => (IList<Vector3>)GetValue(AccelerometerDataProperty);
			set => SetValue(AccelerometerDataProperty, value);
		}

		public Vector3 CurrentReading
		{
			get => (Vector3)GetValue(CurrentReadingProperty);
			set
			{
				SetValue(CurrentReadingProperty, value);
				
				AccelerometerData.Add(value);
				CalculateLimits();

				HorizontalGView.Invalidate();
				VerticalGView.Invalidate();
			}
		}		

		public List<PointF> Envelope
		{
			get
			{
				return _envelope;
			}
			set
			{
				_envelope = value;
			}
		}

		public GForceView()
		{
			RowDefinitions = new RowDefinitionCollection
			{
				new RowDefinition{Height = new GridLength(1, GridUnitType.Star) }
			};
			ColumnDefinitions = new ColumnDefinitionCollection
			{
				new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star) },
				new ColumnDefinition{Width = new GridLength(5, GridUnitType.Star) }
			};

			this.Add(VerticalGView = new GraphicsView { Margin = 5, BackgroundColor = Colors.Transparent}, 0, 0);
			this.Add(HorizontalGView = new GraphicsView { Margin = 5, BackgroundColor = Colors.Transparent }, 1, 0);

			VerticalGView.Drawable = _verticalGDrawable = new VerticalGDrawable(this);
			HorizontalGView.Drawable = _horizontalGDrawable = new HorizontalGDrawable(this);
		}

		private void UpdateView()
		{
			CalculateLimits();
			_horizontalGDrawable?.UpdateEnvelope(Envelope);
		}

		private void UpdateCurrentReading()
		{
			CalculateLimits();

			if (_horizontalGDrawable != null)
			{
				_horizontalGDrawable.MaxReading = _maxHorizontal;
				_horizontalGDrawable.UpdateEnvelope(Envelope);
				_horizontalGDrawable.CurrentReading = new PointF(CurrentReading.X, CurrentReading.Y);
			}

			if (_verticalGDrawable != null)
			{
				_verticalGDrawable.MaxReading = _maxVertical;
				_verticalGDrawable.MinReading = _minVertical;
				_verticalGDrawable.CurrentReading = CurrentReading.Z;
			}
		}
		
		private void CalculateLimits()
		{
			Envelope = new List<PointF>();
			_maxHorizontal = MathF.Max(_maxHorizontal, new Vector2(CurrentReading.X, CurrentReading.Y).Length()); ;
			_minVertical = MathF.Min(_minVertical, CurrentReading.Z);
			_maxVertical = MathF.Max(_maxVertical, CurrentReading.Z);

			if (AccelerometerData == null || AccelerometerData.Count < 1)
			{
				return;
			}

			_minVertical = MathF.Min(_minVertical, AccelerometerData.Min(r => r.Z));
			_maxVertical = MathF.Max(_maxVertical, AccelerometerData.Max(r => r.Z));

			_maxHorizontal = AccelerometerData.Max(r => new Vector2(r.X, r.Y).Length());

			AccelerometerData.ToList().ForEach(r =>
			{
				if(Envelope.Count < 3)
				{
					Envelope.Add(new PointF(r.X, r.Y));
				}
				if(!IsInPolygon(Envelope, r.X, r.Y))
				{
					Envelope.Add(new PointF(r.X, r.Y));

					// Points need to flow around a centre
					Envelope = Envelope.OrderBy(p => MathF.Atan2(p.Y, p.X)).ToList();
				}
			});
		}

		protected override void OnSizeAllocated(double width, double height)
		{
			base.OnSizeAllocated(width, height);
		}

		internal void InvalidateHorizontalGDrawable()
		{
			HorizontalGView.Invalidate();
		}

		internal void InvalidateVerticalGDrawable()
		{
			VerticalGView.Invalidate();
		}

		private static bool IsInPolygon(List<PointF> poly, float x, float y)
		{
			// Ref https://stackoverflow.com/questions/4243042/c-sharp-point-in-polygon
			bool result = false;
			var a = poly.Last();
			foreach (var b in poly)
			{
				if ((b.X == x) && (b.Y == y))
				{
					return true;
				}

				if ((b.Y == a.Y) && (y == a.Y))
				{
					if ((a.X <= x) && (x <= b.X))
					{
						return true;
					}

					if ((b.X <= x) && (x <= a.X))
					{
						return true;
					}
				}

				if ((b.Y < y) && (a.Y >= y) || (a.Y < y) && (b.Y >= y))
				{
					if (b.X + (y - b.Y) / (a.Y - b.Y) * (a.X - b.X) <= x)
					{
						result = !result;
					}
				}

				a = b;
			}
			return result;
		}
	}
}
