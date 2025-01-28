using Microsoft.Extensions.Logging;

namespace DriveLog
{
	public static class MauiProgram
	{
		public static MauiApp CreateMauiApp()
		{
			var builder = MauiApp.CreateBuilder();
			try
			{
				builder
					.UseMauiApp<App>()
					.ConfigureFonts(fonts =>
					{
						fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
						fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
					})
					.UseMauiMaps();

#if DEBUG
				builder.Logging.AddDebug();
#endif
			}
			catch (Exception ex)
			{
				int i = 9;
			}
			return builder.Build();
		}
	}
}
