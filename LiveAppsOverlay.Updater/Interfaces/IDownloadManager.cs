namespace LiveAppsOverlay.Updater.Interfaces
{
    public interface IDownloadManager
    {
        void DeleteReleases();
        void DownloadRelease(string url);
        void ExtractRelease(string fileName);
    }
}
