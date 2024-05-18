using System.Text.Json;
using System;
using System.IO;
using System.Windows;
using System.Text.Json.Serialization;

namespace Photobox
{
	public class ConfigLoader
	{
        public int CountDown { get; set; }
		public string? TextOnPicture { get; set; }
		public string? TextOnPictureFont { get; set; }
		public int TextOnPictureFontSize { get; set; }
		public string? TextOnPictureColor { get; set; }
		public int TextPositionFromRight { get; set; }
		public int TextPositionFromBottom { get; set; }

		public static ConfigLoader? LoadFromJsonFile(string filePath, MainWindow mainWindow)
		{
			try
			{
				if (!File.Exists(filePath))
				{
					mainWindow.ReportError($"Error reading JSON file: {filePath} does not exist");
					return null;
				}

				string json = File.ReadAllText(filePath);

                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };

                return JsonSerializer.Deserialize<ConfigLoader>(json, options);
			}
			catch (Exception ex)
			{
				// Handle any exceptions, such as file not found or invalid JSON format
				Console.WriteLine($"Error reading JSON file: {ex.Message}");
				return null;
			}
		}
    }
}
