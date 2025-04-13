using LiveAppsOverlay.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Interfaces
{
    public interface ISettingsManager
    {
        AppSettings Settings { get; }

        event EventHandler? SettingsChanged;

        void LoadSettings();
        void SaveSettings();
    }
}
