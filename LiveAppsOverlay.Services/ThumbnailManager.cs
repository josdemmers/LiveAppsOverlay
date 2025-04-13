using CommunityToolkit.Mvvm.Messaging;
using LiveAppsOverlay.Entities;
using LiveAppsOverlay.Interfaces;
using LiveAppsOverlay.Messages;
using System.Text.Json;

namespace LiveAppsOverlay.Services
{
    public class ThumbnailManager : IThumbnailManager
    {
        private List<FavoriteProcessEntry> _favoriteProcessEntries = new List<FavoriteProcessEntry>();

        #region Constructors

        public ThumbnailManager()
        {
            // Init Messages
            WeakReferenceMessenger.Default.Register<ThumbnailConfigModifiedMessage>(this, HandleThumbnailConfigModifiedMessage);

            // Load favorites
            LoadFavoriteProcessEntries();
        }

        #endregion

        #region Events

        #endregion

        #region Properties

        public List<FavoriteProcessEntry> FavoriteProcessEntries { get => _favoriteProcessEntries; }

        #endregion

        #region Event handlers

        private void HandleThumbnailConfigModifiedMessage(object recipient, ThumbnailConfigModifiedMessage message)
        {
            SaveFavorites();
        }

        #endregion

        #region Methods

        public void AddFavoriteProcessEntry(FavoriteProcessEntry favoriteProcessEntry)
        {
            _favoriteProcessEntries.Add(favoriteProcessEntry);

            _favoriteProcessEntries.Sort((x, y) =>
            {
                return string.Compare(x.DisplayName, y.DisplayName, StringComparison.OrdinalIgnoreCase);
            });

            SaveFavorites();
        }

        private void LoadFavoriteProcessEntries()
        {
            _favoriteProcessEntries.Clear();

            string fileName = "Config/Favorites.json";
            if (File.Exists(fileName))
            {
                using FileStream stream = File.OpenRead(fileName);
                _favoriteProcessEntries = JsonSerializer.Deserialize<List<FavoriteProcessEntry>>(stream) ?? new List<FavoriteProcessEntry>();
            }

            _favoriteProcessEntries.Sort((x, y) =>
            {
                return string.Compare(x.DisplayName, y.DisplayName, StringComparison.OrdinalIgnoreCase);
            });

            SaveFavorites();
        }

        public void RemoveFavoriteProcessEntry(FavoriteProcessEntry favoriteProcessEntry)
        {
            _favoriteProcessEntries.Remove(favoriteProcessEntry);

            SaveFavorites();
        }

        public void SaveFavorites()
        {
            string fileName = "Config/Favorites.json";
            string path = Path.GetDirectoryName(fileName) ?? string.Empty;
            Directory.CreateDirectory(path);

            using FileStream stream = File.Create(fileName);
            var options = new JsonSerializerOptions { WriteIndented = true };
            JsonSerializer.Serialize(stream, FavoriteProcessEntries, options);
        }

        #endregion


    }
}
