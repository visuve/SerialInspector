using SerialInspector.Model;
using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace SerialInspector
{
    internal static class SettingsUtility
    {
        // TODO: move to the calling class
        private static bool settingsLoadedSuccesfully = false;

        internal static string SettingsFilePath
        {
            get
            {
                var thisExePath = new FileInfo(Environment.GetCommandLineArgs().First());
                return Path.Combine(thisExePath.Directory.FullName, "SerialInspector.Settings.json");
            }
        }

        // TODO: implement some sort of error reporting on what went wrong
        internal static bool IsValidSettings(in SerialConnectionOptions options, in SerialConnectionSettings settings)
        {
            return options.Ports.Contains(settings.Port) &&
                options.BaudRates.Contains(settings.BaudRate) &&
                options.Parities.Contains(settings.Parity) &&
                options.DataBits.Contains(settings.DataBits) &&
                options.StopBits.Contains(settings.StopBits);
        }

        internal static SerialConnectionSettings Get(in SerialConnectionOptions options)
        {
            SerialConnectionSettings settings = null;

            try
            {
                byte[] fileContent = File.ReadAllBytes(SettingsFilePath);
                settings = FromBytes(fileContent);
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine($"Settings file: '{SettingsFilePath}' not found.");

                // This is okay, eventually will create a new settings file in "SaveIfNeeded"
                settingsLoadedSuccesfully = true; 
            }

            if (settings == null)
            {
                return options.Defaults;
            }

            if (!IsValidSettings(options, settings))
            {
                MessageBox.Show(
                    $"Incorrect setting values. Please check: '{SettingsFilePath}' for correct values.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return options.Defaults;
            }

            settingsLoadedSuccesfully = true;
            return settings;
        }

        internal static SerialConnectionSettings FromBytes(in byte[] data)
        {
            try
            {
                var utf8Reader = new Utf8JsonReader(data);
                return JsonSerializer.Deserialize<SerialConnectionSettings>(ref utf8Reader);
            }
            catch (JsonException e)
            {
                MessageBox.Show(
                    $"Failed to read settings JSON:\n{e.Message}.\n\nPlease check: '{SettingsFilePath}' for correct syntax.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Failed to load settings:\n{e.Message}.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);
            }

            return null;
        }

        internal static void SaveIfNeeded(in SerialConnectionSettings settings)
        {
            if (!settingsLoadedSuccesfully)
            {
                // This is to prevent overriding previous settings, which may have had a typo
                return;
            }

            try
            {
                byte[] data = ToBytes(settings);

                if (data != null)
                {
                    File.WriteAllBytes(SettingsFilePath, data);
                }
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Failed to save settings:\n{e.Message}.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }
        }

        internal static byte[] ToBytes(in SerialConnectionSettings settings, bool indent = true)
        {
            try
            {
                var options = new JsonSerializerOptions
                {
                    WriteIndented = indent
                };

                return JsonSerializer.SerializeToUtf8Bytes(settings, options);
            }
            catch (Exception e)
            {
                MessageBox.Show(
                    $"Failed to convert settings to JSON:\n{e.Message}.\nPlease file a bug to https://github.com/visuve/SerialInspector",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error);
            }

            return null;
        }
    }
}