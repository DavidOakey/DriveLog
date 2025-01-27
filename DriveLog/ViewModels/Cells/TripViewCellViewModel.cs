using DriveLog.Models;
using DriveLog.Pages;
using DriveLog.ViewModels.Pages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DriveLog.ViewModels.Cells
{
	public class TripViewCellViewModel : BaseViewModel
	{
		private TripData _model;
		public ICommand ShowTripCommand => new Command(ShowTripClicked);		

		public TripData Model
		{
			get
			{
				return _model;
			}
			set
			{
				if(_model != value)
				{
					_model = value;
				}
			}
		}

		public string StartTimeStamp
		{
			get
			{
				return Model.StartTimeStamp.ToString();
			}
		}

		public string RecordPoints
		{
			get
			{
				return string.Format("A:{0} , {1} L:{2}", Model.AccelerometerData.Count, Model.AccelerometerDataNotAddCount, Model?.LocationData?.Count);
			}
		}
		
		private void ShowTripClicked()
		{
			if (IsBusy)
			{
				return;
			}
			IsBusy = true;
			ShowTripPage();
			IsBusy = false;
		}

		private async void ShowTripPage()
		{
			if (((NavigationPage)App.Current?.MainPage) != null)
			{
				await ((NavigationPage)App.Current?.MainPage)?.PushAsync(new TripPage { BindingContext = new TripPageViewModel { Model = Model } });
			}
		}
	}
}
