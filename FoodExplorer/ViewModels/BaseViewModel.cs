using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FoodExplorer.ViewModels
{
    /// <summary>
    /// Base class for all ViewModels. 
    /// Implements INotifyPropertyChanged to enable Data Binding between the Model and the UI.
    /// </summary>
    public class BaseViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Occurs when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Notifies the UI that a specific property has changed so it can refresh the binding.
        /// [CallerMemberName] automatically picks up the name of the calling property.
        /// </summary>
        /// <param name="propertyName">Name of the property used in binding.</param>
        protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}