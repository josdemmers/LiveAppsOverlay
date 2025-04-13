using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows;

namespace LiveAppsOverlay.Entities
{
    public class ThumbnailConfig
    {
        public int Height { get; set; } = 450;
        public bool IsEnabled { get; set; } = true;
        public double Left { get; set; } = 0;
        public string Name { get; set; } = string.Empty;
        public int Opacity { get; set; } = 255;
        public RegionSourceRECT RegionSource { get; set; } = new RegionSourceRECT();
        public double Top { get; set; } = 0;
        public int Width { get; set; } = 800;

        [JsonIgnore]
        public bool IsActive { get; set; } = false;
    }
}
