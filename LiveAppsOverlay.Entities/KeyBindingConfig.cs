using System.Text.Json.Serialization;
using System.Windows.Input;

namespace LiveAppsOverlay.Entities
{
    public class KeyBindingConfig
    {
        public bool IsEnabled { get; set; } = false;
        public string Name { get; set; } = string.Empty;
        public Key KeyGestureKey { get; set; }
        public ModifierKeys KeyGestureModifier { get; set; }
        [JsonIgnore]
        public new string ToString => $"{KeyGestureModifier}+{KeyGestureKey}";
    }
}