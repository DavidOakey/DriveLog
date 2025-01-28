using DriveLog.Controls.Drawables;
using DriveLog.Models.Enums;

namespace DriveLog.Controls
{
	public class SpeedGraphic : Grid
	{
		public static readonly BindableProperty SpeedUnitsProperty = BindableProperty.Create(nameof(SpeedUnits), typeof(SpeedUnits), typeof(SpeedGraphic), SpeedUnits.mph, propertyChanged: OnCurrentReadingChanged);
		public static readonly BindableProperty CurrentReadingProperty = BindableProperty.Create(nameof(CurrentReading), typeof(int), typeof(SpeedGraphic), 0, propertyChanged: OnCurrentReadingChanged);
		public static readonly BindableProperty RingColorProperty = BindableProperty.Create(nameof(RingColor), typeof(Color), typeof(SpeedGraphic), Colors.Red, propertyChanged: OnAppearanceChanged);
		public static readonly BindableProperty CentreColorProperty = BindableProperty.Create(nameof(CentreColor), typeof(Color), typeof(SpeedGraphic), Colors.White, propertyChanged: OnAppearanceChanged);
		public static readonly BindableProperty TextColorProperty = BindableProperty.Create(nameof(TextColor), typeof(Color), typeof(SpeedGraphic), Colors.Black, propertyChanged: OnAppearanceChanged);

		private static void OnCurrentReadingChanged(BindableObject bindable, object oldValue, object newValue)
		{
			(bindable as SpeedGraphic)?.UpdateView();
		}

		private static void OnAppearanceChanged(BindableObject bindable, object oldValue, object newValue)
		{
			(bindable as SpeedGraphic)?.UpdateAppearance();
		}

		private GraphicsView? _speedView;
		private SpeedDrawable? _speedDrawable;

		public SpeedUnits SpeedUnits
		{
			get => (SpeedUnits)GetValue(SpeedUnitsProperty);
			set => SetValue(SpeedUnitsProperty, value);
		}

		public int CurrentReading
		{
			get => (int)GetValue(CurrentReadingProperty);
			set => SetValue(CurrentReadingProperty, value);
		}

		public Color RingColor
		{
			get => (Color)GetValue(RingColorProperty);
			set => SetValue(RingColorProperty, value);
		}

		public Color CentreColor
		{
			get => (Color)GetValue(CentreColorProperty);
			set => SetValue(CentreColorProperty, value);
		}

		public Color TextColor
		{
			get => (Color)GetValue(TextColorProperty);
			set => SetValue(TextColorProperty, value);
		}

		public SpeedGraphic()
		{
			RowDefinitions = new RowDefinitionCollection
			{
				new RowDefinition{Height = new GridLength(1, GridUnitType.Star) }
			};
			ColumnDefinitions = new ColumnDefinitionCollection
			{
				new ColumnDefinition{Width = new GridLength(1, GridUnitType.Star) }
			};

			this.Add(_speedView = new GraphicsView { Margin = 5, BackgroundColor = Colors.Transparent }, 0, 0);
			_speedView.Drawable = _speedDrawable = new SpeedDrawable();
		}

		private void UpdateView()
		{
			if (_speedDrawable != null && _speedView != null)
			{
				_speedDrawable.Speed = CurrentReading;
				_speedDrawable.Units = SpeedUnits;
				_speedView.Invalidate();
			}
		}

		private void UpdateAppearance()
		{
			if (_speedDrawable != null && _speedView != null)
			{
				_speedDrawable.RingColor = RingColor;
				_speedDrawable.CentreColor = CentreColor;
				_speedDrawable.TextColor = TextColor;
				_speedView.Invalidate();
			}
		}
	}
}
