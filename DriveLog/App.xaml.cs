using DriveLog.Pages;
using DriveLog.ViewModels.Pages;
using System.Security.Cryptography.X509Certificates;

namespace DriveLog
{
	public partial class App : Application
	{
		public App()
		{
			try
			{
				InitializeComponent();

				MainPage = new NavigationPage(new DashboardPage { BindingContext = new DashboardPageViewModel() });
			}
			catch(Exception ex)
			{
				int i = 9;
			}
		}
	}
}
