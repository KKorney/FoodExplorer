using FoodExplorer.Helpers;
using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FoodExplorer.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing and displaying the user's favorite products.
    /// Handles data retrieval and the command logic for removing favorites.
    /// </summary>
    public class FavoritesViewModel : BaseViewModel
    {
        private readonly IFavoriteManager _favoriteService;

        /// <summary>
        /// Collection of favorite entries bound to the FavoritesView UI.
        /// </summary>
        public ObservableCollection<Favorite> FavoritesList { get; } = new();

        /// <summary>
        /// Command to remove a product from the favorites list and database.
        /// </summary>
        public ICommand DeleteFavoriteCommand { get; }

        /// <summary>
        /// Command to edit the note attached to a favorite.
        /// Parameter: Favorite instance (the view should provide it) and the new note via bound behavior.
        /// For simplicity we accept the Favorite as parameter and open a simple dialog to enter the note.
        /// </summary>
        public ICommand EditNoteCommand { get; }

        public FavoritesViewModel(IFavoriteManager favoriteService)
        {
            _favoriteService = favoriteService ?? throw new ArgumentNullException(nameof(favoriteService));

            // Initialize the deletion command
            DeleteFavoriteCommand = new RelayCommand<Favorite>(async (fav) =>
            {
                if (fav == null) return;

                // 1. Delete from the SQLite database
                await _favoriteService.DeleteFavoriteEntryAsync(fav.ProductBarCode);

                // 2. Remove from the observable collection to update the UI instantly
                FavoritesList.Remove(fav);
            });

            // Initialize the edit-note command: persist the note currently stored on the Favorite instance
            EditNoteCommand = new RelayCommand<Favorite>(async (fav) =>
            {
                if (fav == null) return;

                await _favoriteService.UpdateFavoriteNoteAsync(fav.ProductBarCode, fav.Note ?? string.Empty);
            });

            // Load saved favorites when the ViewModel is initialized
            _ = LoadFavoritesAsync();
        }

        /// <summary>
        /// Fetches the full list of favorites from the service and populates the collection.
        /// </summary>
        private async Task LoadFavoritesAsync()
        {
            try
            {
                var favs = await _favoriteService.GetFullFavoritesAsync();

                FavoritesList.Clear();
                if (favs != null)
                {
                    foreach (var f in favs)
                    {
                        FavoritesList.Add(f);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading favorites: {ex.Message}");
            }
        }
    }
}