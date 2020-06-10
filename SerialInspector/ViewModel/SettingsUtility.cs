using SerialInspector.Model;
using System;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Windows;

namespace SerialInspector
{
    internal static class SettingsUtility
    {
        internal static string SettingsFilePath
        {
            get
            {
                var thisExePath = new FileInfo(Environment.GetCommandLineArgs().First());
                return Path.Combine(thisExePath.Directory.FullName, "SerialInspector.Settings.json");
            }
        }

        internal static SerialConnectionSettings Get()
        {
            SerialConnectionSettings settings = null;

            try
            {
                byte[] fileContent = File.ReadAllBytes(SettingsFilePath);
                settings = FromBytes(fileContent);
                Debug.WriteLine($"Settings loaded from: '{SettingsFilePath}'.");
            }
            catch (FileNotFoundException)
            {
                Debug.WriteLine($"Settings file: '{SettingsFilePath}' not found.");
            }

            if (settings == null)
            {
                return new SerialConnectionSettings();
            }

            try
            {
                settings.Validate();
            }
            catch (ValidationException e)
            {
                MessageBox.Show(
                    $"Incorrect setting values: {e.Message}\n\n Please check: '{SettingsFilePath}' for correct values.",
                    "Serial Inspector",
                    MessageBoxButton.OK,
                    MessageBoxImage.Warning);

                return new SerialConnectionSettings();
            }

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

        internal static void SaveIfValid(in SerialConnectionSettings settings)
        {
            try
            {
                if (!settings.TryValidate())
                {
                    return; // Prevent saving erraneous values
                }

                byte[] data = ToBytes(settings);

                if (data != null)
                {
                    File.WriteAllBytes(SettingsFilePath, data);
                    Debug.WriteLine($"Settings saved to: '{SettingsFilePath}'.");
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