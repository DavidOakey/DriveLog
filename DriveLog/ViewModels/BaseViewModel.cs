using System.ComponentModel;
using System.Runtime.CompilerServices;

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
