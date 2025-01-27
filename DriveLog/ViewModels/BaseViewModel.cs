using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace DriveLog.ViewModels
{
	public class BaseViewModel : INotifyPropertyChanged
	{
		public event PropertyChangedEventHandler? PropertyChanged;
		private bool _isBusy = false;

		public bool IsBusy
		{
			get
			{
				return _isBusy;
			}
			set 
			{
				if (_isBusy != value)
				{
					_isBusy = value;
					OnPropertyChanged();
				}
			}
		}

		public void OnPropertyChanged([CallerMemberName] string name = "") =>
		PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
	}
}
