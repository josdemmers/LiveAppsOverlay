using System;
using System.Collections.Generic;
using System.Linq;
using System.Printing;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace LiveAppsOverlay.Entities
{
    public class AppSettings
    {
        public bool IsCheckForUpdatesEnabled { get; set; } = true;
        public bool IsEditModeEnabled { get; set; } = true;
        public string SelectedAppLanguage { get; set; } = "en-US";

        public KeyBindingConfig KeyBindingConfigToggleEditMode { get; set; } = new KeyBindingConfig
        {
            IsEnabled = false,
            Name = "Toggle Edit Mode",
            KeyGestureKey = Key.F5,
            KeyGestureModifier = ModifierKeys.Control
        };
    }
}
