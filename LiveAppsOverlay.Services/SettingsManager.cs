using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Services
{
    public class SettingsManager : ISettingsManager
    {
        private AppSettings _settings = new AppSettings();

        #region Constructors

        public SettingsManager()
        {
            LoadSettings();
        }

        #endregion

        #region Events

        public event EventHandler? SettingsChanged;

        #endregion

        #region Properties

        public AppSettings Settings { get => _settings; set => _settings = value; }

        #endregion

        #region Event handlers

        #endregion

        #region Methods

        public void LoadSettings()
        {
            string fileName = "Config/Settings.json";
            if (File.Exists(fileName))
            {
                using FileStream stream = File.OpenRead(fileName);
                _settings = JsonSerializer.Deserialize<AppSettings>(stream) ?? new AppSettings();
            }

            SaveSettings();
        }

        public void SaveSettings()
        {
            string fileName = "Config/Settings.json";
            string path = Path.GetDirectoryName(fileName) ?? string.Empty;
            Directory.CreateDirectory(path);

            using FileStream stream = File.Create(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(stream, _settings, options);

            SettingsChanged?.Invoke(this, EventArgs.Empty);
        }

        #endregion
    }
}
