using CommunityToolkit.Maui.Core.Extensions;
using SkiaSharp;
using System.Diagnostics;

namespace ImageBorderTool2;

public partial class EditImagePage : ContentPage
{
    private int _currentImageNumber;
    private List<string> _imagePaths;
    private string _imagePath;
    private int _imageWidth;
    private int _imageHeight;
    private bool _setupComplete;

    private Color _currentColor;
    private Color _debugColor;
    private int _currentBorderThickness;

    public EditImagePage(List<string> imagePaths, int currentImageNumber, int currentBorderThickness, Color currentColor)
    {
        InitializeComponent();

        _setupComplete = false;
        _currentImageNumber = currentImageNumber;
        _imagePaths = imagePaths;
        _imagePath = _imagePaths[_currentImageNumber];

        _currentBorderThickness = currentBorderThickness;

        _currentColor = currentColor;
        _debugColor = _currentColor;

        SetCurrentImage();
        SetupNavigationButtons();
        SetupPage();
        SetColorsToPage();
        _setupComplete = true;
    }

    private async Task<(int width, int height)> GetImageDimensionsAsync(string imagePath)
    {
        using var stream = File.OpenRead(imagePath);
        var image = SKBitmap.Decode(stream);

        return (image.Width, image.Height);
    }

    private async void SetCurrentImage()
    {
        CurrentImage.Source = _imagePath;
        (_imageWidth, _imageHeight) = await GetImageDimensionsAsync(_imagePath);
    }
    private async void SetupPage()
    {
        BorderThicknessSlider.Value = _currentBorderThickness;
        BorderThicknessLabel.Text = $"{_currentBorderThickness} px";
        PreviewFrame.Padding = new Thickness(_currentBorderThickness);

        RedSlider.Value = _currentColor.GetByteRed();
        GreenSlider.Value = _currentColor.GetByteGreen();
        BlueSlider.Value = _currentColor.GetByteBlue();

        ColorValueEntry.Text = $"{_currentColor.GetByteRed()},{_currentColor.GetByteGreen()},{_currentColor.GetByteBlue()}";
    }

    private void SetColorsToPage()
    {
        ColorPreview.Color = _currentColor;
        PreviewFrame.BackgroundColor = _currentColor;
    }

    private void SetupNavigationButtons()
    {
        if (_imagePath == _imagePaths.First())
        {
            PreviousPictureButton.IsVisible = false;
            FakePreviousPictureButton.IsVisible = true;
        }

        if (_imagePath == _imagePaths.Last())
        {
            NextPictureButton.IsVisible = false;
            FakeNextPictureButton.IsVisible = true;
        }
    }

    private void UpdateColorValueEntry()
    {
        if (!_setupComplete)
            return;

        ColorValueEntry.Text = $"{(int)RedSlider.Value},{(int)GreenSlider.Value},{(int)BlueSlider.Value}";
    }

    private void OnColorValueEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (!_setupComplete)
            return;

        if (string.IsNullOrWhiteSpace(e.NewTextValue))
            return;

        var parts = e.NewTextValue.Split(',');
        if (parts.Length == 3 &&
            int.TryParse(parts[0], out int red) &&
            int.TryParse(parts[1], out int green) &&
            int.TryParse(parts[2], out int blue))
        {
            if (red < 0 || red > 255 || green < 0 || green > 255 || blue < 0 || blue > 255)
                return;

            RedSlider.Value = red;
            GreenSlider.Value = green;
            BlueSlider.Value = blue;

            _currentColor = new Color(red, green, blue);
            SetColorsToPage();
        }
    }

    private void OnColorValueSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (!_setupComplete)
            return;

        _currentColor = _currentColor = new Color((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);

        UpdateColorValueEntry();
        SetColorsToPage();
    }

    private void OnBorderThicknessSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider)
        {
            _currentBorderThickness = (int)slider.Value;
            BorderThicknessLabel.Text = $"{_currentBorderThickness:F0} px";

            if (_imageWidth > _imageHeight)
            {
                var padding = new Thickness(_currentBorderThickness, 0);
                PreviewFrame.Padding = padding;
            }
            else if (_imageWidth < _imageHeight)
            {
                var padding = new Thickness(0, _currentBorderThickness);
                PreviewFrame.Padding = padding;
            }
            else
            {
                PreviewFrame.Padding = new Thickness(_currentBorderThickness);
            }
        }
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void OnExportImagesClicked(object sender, EventArgs e)
    {

    }

    private void OnExitClicked(object sender, EventArgs e)
    {
        Application.Current.Quit();
    }

    private async void PreviousPictureButtonClicked(object sender, EventArgs e)
    {
        _currentImageNumber--;
        await Navigation.PushAsync(new EditImagePage(_imagePaths, _currentImageNumber, _currentBorderThickness, _currentColor));
    }
    private async void NextPictureButtonClicked(object sender, EventArgs e)
    {
        _currentImageNumber++;
        await Navigation.PushAsync(new EditImagePage(_imagePaths, _currentImageNumber, _currentBorderThickness, _currentColor));
    }
}
