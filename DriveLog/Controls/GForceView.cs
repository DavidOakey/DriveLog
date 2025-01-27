﻿using DriveLog.Controls.Drawables;
using DriveLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace DriveLog.Controls
{
	public class GForceView : Grid
	{
		public static readonly BindableProperty AccelerometerDataProperty = BindableProperty.Create(nameof(AccelerometerData), typeof(IList<Vector3>), typeof(GForceView), null, propertyChanged: OnAccelerometerChanged);
		public static readonly BindableProperty CurrentReadingProperty = BindableProperty.Create(nameof(CurrentReading), typeof(Vector3), typeof(GForceView), new Vector3(0, 0, 0), propertyChanged: OnCurrentReadingChanged);

		//private double ControlHeight = 0;
		//private double ControlWidth = 0;

		private GraphicsView VerticalGView;
		private GraphicsView HorizontalGView;
		//private GraphicsView VerticalInstantGView;
		//private GraphicsView HorizontalInstantGView;

		private HorizontalGDrawable? _horizontalGDrawable;
		private VerticalGDrawable? _verticalGDrawable;

		private float MinVertical;
		private float MaxVertical;
		private float MaxHorizontal;		
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

				if (!IsInPolygon(Envelope, value.X, value.Y))
				{
					AccelerometerData.Add(value);
					CalculateLimits();
				}

				HorizontalGView.Invalidate();
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

			//this.Add(VerticalInstantGView = new GraphicsView { Margin = 5, BackgroundColor = Colors.Transparent }, 0, 0);
			//this.Add(HorizontalInstantGView = new GraphicsView { Margin = 5, BackgroundColor = Colors.Transparent }, 1, 0);

			VerticalGView.Drawable = _verticalGDrawable = new VerticalGDrawable(this);
			HorizontalGView.Drawable = _horizontalGDrawable = new HorizontalGDrawable(this);

			//VerticalInstantGView.Drawable = _instantVerticalGDrawable = new InstantVerticalGDrawable();
			//HorizontalInstantGView.Drawable = _instantHorizontalGDrawable = new InstantHorizontalGDrawable();
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
				_horizontalGDrawable.MaxReading = MaxHorizontal;
				_horizontalGDrawable.UpdateEnvelope(Envelope);
				_horizontalGDrawable.CurrentReading = new PointF(CurrentReading.X, CurrentReading.Y);
			}

			if (_verticalGDrawable != null)
			{
				_verticalGDrawable.MaxReading = MaxVertical;
				_verticalGDrawable.MinReading = MinVertical;
				_verticalGDrawable.CurrentReading = CurrentReading.Z;
			}
		}
		
		private void CalculateLimits()
		{
			Envelope = new List<PointF>();
			MinVertical = 0;
			MaxVertical = 0;
			MaxHorizontal = 0;

			if (AccelerometerData == null)
			{
				return;
			}

			MinVertical = AccelerometerData.Min(r => r.Z);
			MaxVertical = AccelerometerData.Max(r => r.Z);
			
			MaxHorizontal = AccelerometerData.Max(r => MathF.Min(r.X, r.Y));

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
