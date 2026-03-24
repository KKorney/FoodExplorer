using FlashCap;
using FoodExplorer.Helpers;
using System;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using ZXing;
using ZXing.Windows.Compatibility;

namespace FoodExplorer.ViewModels
{
    /// <summary>
    /// Manages the live camera feed and performs real-time barcode scanning.
    /// Uses FlashCap for high-performance frame capture and ZXing for decoding.
    /// </summary>
    public class ScannerViewModel : BaseViewModel
    {
        private CaptureDevice? _captureDevice;
        private readonly BarcodeReaderGeneric _barcodeReader;

        /// <summary>
        /// Event triggered when a valid barcode is successfully decoded.
        /// </summary>
        public event Action<string>? BarcodeFound;

        private BitmapSource? _cameraImage;
        public BitmapSource? CameraImage
        {
            get => _cameraImage;
            set { _cameraImage = value; OnPropertyChanged(); }
        }

        public ScannerViewModel()
        {
            _barcodeReader = new BarcodeReaderGeneric();
            // Start the camera initialization in a background task to keep the UI responsive
            Task.Run(() => StartCameraAsync());
        }

        /// <summary>
        /// Enumerates available cameras and starts the first found device.
        /// </summary>
        private async Task StartCameraAsync()
        {
            var devices = new CaptureDevices();
            var descriptors = devices.EnumerateDescriptors().ToArray();
            var descriptor = descriptors.FirstOrDefault();

            if (descriptor != null)
            {
                // Select the first available characteristic (resolution/format)
                var characteristics = descriptor.Characteristics.First();

                // Open and start the capture device using the frame arrival callback
                _captureDevice = await descriptor.OpenAsync(characteristics, OnFrameArrived);
                await _captureDevice.StartAsync();
            }
        }

        /// <summary>
        /// Callback method executed every time a new frame is received from the camera.
        /// </summary>
        private void OnFrameArrived(PixelBufferScope scope)
        {
            try
            {
                // FlashCap provides direct access to the image bytes for high-speed processing
                var imageBytes = scope.Buffer.ExtractImage();

                using (var ms = new MemoryStream(imageBytes))
                {
                    using (var bitmap = new Bitmap(ms))
                    {
                        // Create a luminance source for ZXing to analyze the frame
                        var luminanceSource = new BitmapLuminanceSource(bitmap);
                        var result = _barcodeReader.Decode(luminanceSource);

                        if (result != null)
                        {
                            // Success! Stop the camera and notify the MainViewModel via the event
                            StopCamera();
                            App.Current.Dispatcher.Invoke(() => BarcodeFound?.Invoke(result.Text));
                            return;
                        }
                    }

                    // Update the UI's CameraImage property so the user sees the live preview
                    App.Current.Dispatcher.BeginInvoke(() =>
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.BeginInit();
                        bitmapImage.StreamSource = new MemoryStream(imageBytes);
                        bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                        bitmapImage.EndInit();
                        bitmapImage.Freeze(); // Freeze allows cross-thread access for WPF
                        CameraImage = bitmapImage;
                    });
                }
            }
            catch (Exception ex)
            {
                // Silent catch for potential camera stream interruptions during disposal
                System.Diagnostics.Debug.WriteLine($"Camera processing error: {ex.Message}");
            }
        }

        /// <summary>
        /// Safely stops and disposes of the camera hardware.
        /// </summary>
        public void StopCamera()
        {
            _captureDevice?.StopAsync();
            _captureDevice?.Dispose();
        }
    }
}