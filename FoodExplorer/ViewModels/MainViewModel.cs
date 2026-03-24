using FoodExplorer.Helpers;
using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace FoodExplorer.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IProductFinder _productService;
        private readonly IFavoriteManager _favoriteService;
        private string _barcodeSearchTerm = "";
        private object? _currentView;

        public string BarcodeSearchTerm
        {
            get => _barcodeSearchTerm;
            set { _barcodeSearchTerm = value; OnPropertyChanged(); }
        }

        public object? CurrentView
        {
            get => _currentView;
            set
            {
                _currentView = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<Product> SearchResults { get; } = new();

        // --- COMMANDS ---
        public ICommand ShowSearchCommand { get; }
        public ICommand ShowFavoritesCommand { get; }
        public ICommand ShowHistoryCommand { get; }
        public ICommand SearchCommand { get; }
        public ICommand AddToFavoriteCommand { get; }
        public ICommand ShowProductFromHistoryCommand { get; }
        public ICommand ShowExportCommand { get; }
        public ICommand ShowScannerCommand { get; }

        public MainViewModel(IProductFinder productService, IFavoriteManager favoriteService, IHistoryRepository historyRepository)
        {
            _productService = productService ?? throw new ArgumentNullException(nameof(productService));
            _favoriteService = favoriteService ?? throw new ArgumentNullException(nameof(favoriteService));

            // Navigation Initialization
            ShowSearchCommand = new RelayCommand(() => CurrentView = this);
            ShowFavoritesCommand = new RelayCommand(() => CurrentView = new FavoritesViewModel(_favoriteService));
            ShowHistoryCommand = new RelayCommand(() => CurrentView = new HistoryViewModel(historyRepository));
            ShowExportCommand = new RelayCommand(() => CurrentView = new ExportViewModel(_productService));

            // Scanner Initialization
            ShowScannerCommand = new RelayCommand(() =>
            {
                var scannerVM = new ScannerViewModel();
                scannerVM.BarcodeFound += (code) =>
                {
                    BarcodeSearchTerm = code;
                    _ = ExecuteSearch();
                    CurrentView = this;
                };
                CurrentView = scannerVM;
            });

            // Action Commands
            SearchCommand = new RelayCommand(async () => await ExecuteSearch());
            AddToFavoriteCommand = new RelayCommand<Product>(async (p) => await ExecuteAddToFavorite(p));

            ShowProductFromHistoryCommand = new RelayCommand<Product>((product) =>
            {
                if (product == null) return;
                SearchResults.Clear();
                SearchResults.Add(product);
                CurrentView = this;
            });

            CurrentView = this;
        }

        private async Task ExecuteSearch()
        {
            if (string.IsNullOrWhiteSpace(BarcodeSearchTerm)) return;

            try
            {
                SearchResults.Clear();
                var product = await _productService.GetProductByBarcodeAsync(BarcodeSearchTerm);

                if (product != null)
                {
                    SearchResults.Add(product);
                    BarcodeSearchTerm = ""; // Nettoie le champ après recherche réussie
                }
                else
                {
                    MessageBox.Show("Product not found.", "Info");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error occurred: " + ex.Message, "Error");
            }
        }

        private async Task ExecuteAddToFavorite(Product product)
        {
            if (product == null) return;
            try
            {
                await _favoriteService.AddFavoriteAsync(product);
                MessageBox.Show($"{product.Name} added to favorites!", "Success");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error saving favorite: " + ex.Message);
            }
        }
    }
}