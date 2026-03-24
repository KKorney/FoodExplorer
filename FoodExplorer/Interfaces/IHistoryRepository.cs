using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Handles database-level operations for storing and managing user search history.
    /// </summary>
    public interface IHistoryRepository
    {
        /// <summary>
        /// Persists a new history entry to the database.
        /// </summary>
        /// <param name="entry">The history object containing product and timestamp data.</param>
        Task AddEntryAsync(History entry);

        /// <summary>
        /// Retrieves all search history records, typically including the associated product details.
        /// </summary>
        /// <returns>A list of history entries from the database.</returns>
        Task<List<History>> GetFullHistoryAsync();

      
    }
}