using LiveAppsOverlay.Entities;

namespace LiveAppsOverlay.Interfaces
{
    public interface IThumbnailManager
    {
        List<FavoriteProcessEntry> FavoriteProcessEntries { get; }

        void AddFavoriteProcessEntry(FavoriteProcessEntry favoriteProcessEntry);
        void RemoveFavoriteProcessEntry(FavoriteProcessEntry favoriteProcessEntry);
        void SaveFavorites();
    }
}
