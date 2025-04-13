using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiveAppsOverlay.Entities
{
    public class FavoriteProcessEntry
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;

        public List<ThumbnailConfig> ThumbnailConfigs { get; set; } = new List<ThumbnailConfig>();


    }
}
