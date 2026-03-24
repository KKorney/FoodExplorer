using FoodExplorer.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace FoodExplorer.Interfaces
{
    /// <summary>
    /// Manages the recording and removal of product viewings by the user.
    /// </summary>
    public interface IHistoryManager
    {
        /// <summary>
        /// Adds a product to the history when it is viewed.
        /// If the product already exists in history, updates the last viewing date.
        /// </summary>
        /// <param name="product">The product object to add or update.</param>
        Task AddToHistoryAsync(Product product);

        /// <summary>
        /// Retrieves the complete history list, sorted from most recent to oldest.
        /// </summary>
        /// <returns>A list containing all history entries in chronological order.</returns>
        Task<List<History>> GetFullHistoryAsync();

        
    }
}