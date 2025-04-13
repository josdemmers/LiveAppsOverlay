using LiveAppsOverlay.Entities;

namespace LiveAppsOverlay.Interfaces
{
    public interface IReleaseManager
    {
        List<Release> Releases { get; }
        string Repository { get; }
        bool UpdateAvailable { get; set; }
    }
}
