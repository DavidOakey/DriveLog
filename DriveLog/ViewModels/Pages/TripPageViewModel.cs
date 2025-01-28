using DriveLog.Models;

namespace DriveLog.ViewModels.Pages
{
	public class TripPageViewModel : BaseViewModel
	{
		private TripData _model;
		public TripData Model
		{
			get
			{
				return _model;
			}
			set
			{
				if (_model != value)
				{
					_model = value;
				}
			}
		}
	}
}
