using FoodExplorer.Data;
using FoodExplorer.Repositories;
using FoodExplorer.Services;
using FoodExplorer.ViewModels;
using Microsoft.EntityFrameworkCore;
using System.Windows;
using QuestPDF.Infrastructure;

namespace FoodExplorer
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// This is the entry point of the application where Dependency Injection is configured.
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            // Set the QuestPDF license to Community (Required for PDF generation)
            QuestPDF.Settings.License = LicenseType.Community;

            base.OnStartup(e);

            // 1. Database Initialization
            // This ensures the SQLite file and tables are created upon the first launch
            var context = new FoodExplorerContext();
            context.Database.EnsureCreated();

            // 2. Instantiate Infrastructure Layer (API & Repositories)
            var apiService = new OpenFoodFactsApiService();
            var productRepo = new ProductRepository(context);
            var favoriteRepo = new FavoriteRepository(context);
            var historyRepo = new HistoryRepository(context);

            // 3. Instantiate Domain Layer (Services)
            // Services act as the bridge between repositories and the API
            var productService = new ProductService(productRepo, historyRepo, apiService);
            var favoriteService = new FavoriteService(favoriteRepo);

            // 4. Instantiate Presentation Layer (Main ViewModel)
            // Injecting services into the ViewModel via Constructor Injection
            var mainVM = new MainViewModel(productService, favoriteService, historyRepo);

            // 5. Launch the Main Window
            // Manually setting the DataContext allows for a clean MVVM startup
            var window = new MainWindow { DataContext = mainVM };
            window.Show();
        }
    }
}