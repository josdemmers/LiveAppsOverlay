namespace LiveAppsOverlay.Interfaces
{
    public interface IHttpClientHandler
    {
        Task<string> GetRequest(string uri);
        Task DownloadZip(string uri);
    }
}