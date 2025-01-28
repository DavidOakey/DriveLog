using DriveLog.Models;
using System.Text.Json;

namespace DriveLog.Controlers
{
	public static class FileData
	{
		public static void SaveAppSettings(AppSettings settings)
		{
			// Write the file content to the app data directory  
			string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, "App.as");
			using FileStream outputStream = File.OpenWrite(targetFile);
			using StreamWriter streamWriter = new StreamWriter(outputStream);
			streamWriter.Write(JsonSerializer.Serialize(settings));
		}

		public static AppSettings LoadAppSettings(string username)
		{
			string json = string.Empty;
			try
			{
				using FileStream inputStream = File.OpenRead(Path.Combine(FileSystem.Current.AppDataDirectory, "App.as"));
				using StreamReader streamReader = new StreamReader(inputStream);
				json = streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				int i = 9;
				return new AppSettings();
			}
			try
			{
				return JsonSerializer.Deserialize<AppSettings>(json) ?? new AppSettings();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				File.Delete(Path.Combine(FileSystem.Current.AppDataDirectory, "App.as"));
				int i = 9;
				return new AppSettings();
			}
		}

		public static void SaveAppUserSettings(AppUserSettings settings)
		{
			// Write the file content to the app data directory  
			string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, (string.IsNullOrEmpty(settings.Username) ? "Default" :settings.Username) + ".as");
			using FileStream outputStream = File.OpenWrite(targetFile);
			using StreamWriter streamWriter = new StreamWriter(outputStream);
			streamWriter.Write(JsonSerializer.Serialize(settings));
		}

		public static AppUserSettings LoadAppUserSettings(string username)
		{
			string json = string.Empty;
			try
			{
				using FileStream inputStream = File.OpenRead(Path.Combine(FileSystem.Current.AppDataDirectory, (string.IsNullOrEmpty(username) ? "Default" : username) + ".as"));
				using StreamReader streamReader = new StreamReader(inputStream);
				json = streamReader.ReadToEnd();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				int i = 9;
				return new AppUserSettings();
			}
			try
			{
				return JsonSerializer.Deserialize<AppUserSettings>(json);
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				File.Delete(Path.Combine(FileSystem.Current.AppDataDirectory, (string.IsNullOrEmpty(username) ? "Default" : username) + ".as"));
				int i = 9;
				return new AppUserSettings();
			}
		}

		public static void SaveTrip(TripData trip)
		{
			// Write the file content to the app data directory  
			string targetFile = Path.Combine(FileSystem.Current.AppDataDirectory, trip.TripID.ToString() + ".trp");
			using FileStream outputStream = File.OpenWrite(targetFile);
			using StreamWriter streamWriter = new StreamWriter(outputStream);
			streamWriter.Write(JsonSerializer.Serialize(trip));
		}

		public static List<TripData> LoadTripAll()
		{
			try
			{
				return Directory.GetFiles(FileSystem.Current.AppDataDirectory, "*.trp").ToList().Select(f =>
				{
					string json = string.Empty;
					try
					{
						using FileStream inputStream = File.OpenRead(Path.Combine(FileSystem.Current.AppDataDirectory, f));
						using StreamReader streamReader = new StreamReader(inputStream);
						json = streamReader.ReadToEnd();
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						int i = 9;
						return new TripData();
					}
					try
					{
						return JsonSerializer.Deserialize<TripData>(json);
					}
					catch (Exception ex)
					{
						string message = ex.Message;
						File.Delete(Path.Combine(FileSystem.Current.AppDataDirectory, f));
						int i = 9;
						return new TripData();
					}
				}).Where(t => t != null).ToList();
			}
			catch (Exception ex)
			{
				string message = ex.Message;
				int i = 9;
				return new List<TripData>();
			}
		}
	}
}
