using DriveLog.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

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
