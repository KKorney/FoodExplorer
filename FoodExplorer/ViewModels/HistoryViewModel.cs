using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace FoodExplorer.ViewModels
{
    /// <summary>
    /// ViewModel responsible for displaying the user's product scan history.
    /// Interacts with the IHistoryRepository to retrieve persisted data.
    /// </summary>
    public class HistoryViewModel : BaseViewModel
    {
        private readonly IHistoryRepository _historyRepository;

        /// <summary>
        /// Data-bound collection of history entries displayed in the UI.
        /// </summary>
        public ObservableCollection<History> HistoryList { get; } = new();

        public HistoryViewModel(IHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;

            // Fire-and-forget call to load data when the ViewModel is initialized
            _ = LoadHistoryAsync();
        }

        /// <summary>
        /// Fetches the history from the database and updates the observable collection.
        /// </summary>
        private async Task LoadHistoryAsync()
        {
            try
            {
                var history = await _historyRepository.GetFullHistoryAsync();

                HistoryList.Clear();
                if (history != null)
                {
                    foreach (var entry in history)
                    {
                        HistoryList.Add(entry);
                    }
                }
            }
            catch (System.Exception ex)
            {
                // Log error or notify the user if necessary
                System.Diagnostics.Debug.WriteLine($"Error loading history: {ex.Message}");
            }
        }
    }
}