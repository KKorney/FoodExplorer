using FoodExplorer.Helpers;
using FoodExplorer.Interfaces;
using FoodExplorer.Models;
using Microsoft.Win32;
using System;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace FoodExplorer.ViewModels
{
    /// <summary>
    /// ViewModel dedicated to creating custom product lists and exporting them as PDF reports.
    /// Uses QuestPDF for high-performance document generation.
    /// </summary>
    public class ExportViewModel : BaseViewModel
    {
        private readonly IProductFinder _productService;

        /// <summary>
        /// Temporary list of products selected by the user for the current export session.
        /// </summary>
        public ObservableCollection<Product> TempExportList { get; } = new();

        private string _exportSearchTerm = "";
        public string ExportSearchTerm
        {
            get => _exportSearchTerm;
            set { _exportSearchTerm = value; OnPropertyChanged(); }
        }

        public ICommand AddToExportCommand { get; }
        public ICommand RemoveFromExportCommand { get; }
        public ICommand GeneratePdfCommand { get; }

        public ExportViewModel(IProductFinder productService)
        {
            _productService = productService;

            // Command to fetch a product by barcode and add it to the export queue
            AddToExportCommand = new RelayCommand(async () =>
            {
                if (string.IsNullOrWhiteSpace(ExportSearchTerm)) return;

                var product = await _productService.GetProductByBarcodeAsync(ExportSearchTerm);
                if (product != null)
                {
                    TempExportList.Add(product);
                }
                else
                {
                    MessageBox.Show("Product not found in the database.", "Information");
                }

                ExportSearchTerm = ""; // Clear input after search
            });

            // Remove a specific product from the temporary list
            RemoveFromExportCommand = new RelayCommand<Product>((p) => TempExportList.Remove(p));

            // Trigger the PDF generation process
            GeneratePdfCommand = new RelayCommand(() => ExecutePdfExport());
        }

        /// <summary>
        /// Configures and generates the PDF document based on the TempExportList.
        /// </summary>
        private void ExecutePdfExport()
        {
            if (TempExportList.Count == 0)
            {
                MessageBox.Show("The export list is empty! Please add products first.", "Warning");
                return;
            }

            var saveDialog = new SaveFileDialog
            {
                Filter = "PDF Files (*.pdf)|*.pdf",
                FileName = $"FoodExplorer_Report_{DateTime.Now:yyyyMMdd}.pdf"
            };

            if (saveDialog.ShowDialog() == true)
            {
                try
                {
                    // QuestPDF Document definition
                    Document.Create(container =>
                    {
                        container.Page(page =>
                        {
                            page.Size(PageSizes.A4);
                            page.Margin(1, Unit.Centimetre);
                            page.PageColor(Colors.White);
                            page.DefaultTextStyle(x => x.FontSize(10).FontFamily(Fonts.Verdana));

                            // PDF HEADER
                            page.Header().BorderBottom(1).PaddingBottom(5).Row(row =>
                            {
                                row.RelativeItem().Column(col =>
                                {
                                    col.Item().Text("FOOD EXPLORER").FontSize(22).ExtraBold().FontColor(Colors.Green.Medium);
                                    col.Item().Text("Detailed Product Health Report").FontSize(12).Italic();
                                });
                                row.RelativeItem().AlignRight().Text($"{DateTime.Now:MM/dd/yyyy HH:mm}").FontSize(10);
                            });

                            // PDF CONTENT
                            page.Content().PaddingVertical(10).Column(column =>
                            {
                                foreach (var p in TempExportList)
                                {
                                    column.Item().PaddingBottom(20).Border(0.5f).BorderColor(Colors.Grey.Lighten2).Padding(10).Column(innerCol =>
                                    {
                                        // Product Title & Nutri-Score badge
                                        innerCol.Item().Row(row =>
                                        {
                                            row.RelativeItem().Text(p.Name).FontSize(14).Bold().FontColor(Colors.Blue.Medium);
                                            row.AutoItem().Background(Colors.Grey.Lighten3).PaddingHorizontal(5).Text($"Score: {p.NutriScore ?? "?"}").Bold();
                                        });

                                        innerCol.Item().Text($"Barcode: {p.BarCode}").FontSize(9).FontColor(Colors.Grey.Medium);
                                        innerCol.Item().PaddingVertical(5).LineHorizontal(0.5f).LineColor(Colors.Grey.Lighten2);

                                        // Nutrient Summary Table
                                        innerCol.Item().Table(t =>
                                        {
                                            t.ColumnsDefinition(c => { c.RelativeColumn(); c.RelativeColumn(); c.RelativeColumn(); });
                                            t.Cell().Text($"Fat: {p.NutrientLevels?.Fat ?? "N/A"}");
                                            t.Cell().Text($"Sugars: {p.NutrientLevels?.Sugars ?? "N/A"}");
                                            t.Cell().Text($"Salt: {p.NutrientLevels?.Salt ?? "N/A"}");
                                        });

                                        // Ingredients Section
                                        innerCol.Item().PaddingTop(5).Text(x =>
                                        {
                                            x.Span("Ingredients: ").Bold();
                                            x.Span(p.Ingredients ?? "Not specified");
                                        });

                                        // Additives (Highlighted in Red for visibility)
                                        if (!string.IsNullOrEmpty(p.Additives) && p.Additives != "None")
                                        {
                                            innerCol.Item().PaddingTop(3).Text(x =>
                                            {
                                                x.Span("Additives: ").Bold().FontColor(Colors.Red.Medium);
                                                x.Span(p.Additives);
                                            });
                                        }

                                        // Allergy Traces
                                        if (!string.IsNullOrEmpty(p.Traces) && p.Traces != "None")
                                        {
                                            innerCol.Item().PaddingTop(3).Text(x =>
                                            {
                                                x.Span("Possible traces: ").Bold();
                                                x.Span(p.Traces);
                                            });
                                        }
                                    });
                                }
                            });

                            // PDF FOOTER
                            page.Footer().AlignCenter().Text(x =>
                            {
                                x.Span("Page ");
                                x.CurrentPageNumber();
                            });
                        });
                    }).GeneratePdf(saveDialog.FileName);

                    MessageBox.Show("PDF report generated successfully!", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error during PDF generation: " + ex.Message, "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
        }
    }
}