using CommunityToolkit.Maui.Core.Extensions;
using SkiaSharp;

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
    private int _currentBorderThickness;

    public EditImagePage(List<string> imagePaths, int currentImageNumber, int currentBorderThickness, Color currentColor)
    {
        InitializeComponent();

        _currentImageNumber = currentImageNumber;
        _imagePaths = imagePaths;
        _imagePath = _imagePaths[_currentImageNumber];
        _currentBorderThickness = currentBorderThickness;
        _currentColor = currentColor;

        InitializePage();
    }

    private async void InitializePage()
    {
        await SetCurrentImage();
        SetupNavigationButtons();
        SetupPage();
        UpdateUI();
        _setupComplete = true;
    }

    private async Task SetCurrentImage()
    {
        CurrentImage.Source = _imagePath;
        (_imageWidth, _imageHeight) = await GetImageDimensionsAsync(_imagePath);
    }

    private async Task<(int width, int height)> GetImageDimensionsAsync(string imagePath)
    {
        using var stream = File.OpenRead(imagePath);
        var image = SKBitmap.Decode(stream);
        return (image.Width, image.Height);
    }

    private void SetupPage()
    {
        BorderThicknessSlider.Value = _currentBorderThickness;
        BorderThicknessLabel.Text = $"{_currentBorderThickness} px";
        UpdatePreviewPadding();

        RedSlider.Value = _currentColor.GetByteRed();
        GreenSlider.Value = _currentColor.GetByteGreen();
        BlueSlider.Value = _currentColor.GetByteBlue();

        ColorValueEntry.Text = $"{(int)RedSlider.Value},{(int)GreenSlider.Value},{(int)BlueSlider.Value}";
    }

    private void SetupNavigationButtons()
    {
        PreviousPictureButton.IsVisible = _currentImageNumber > 0;
        FakePreviousPictureButton.IsVisible = _currentImageNumber == 0;

        NextPictureButton.IsVisible = _currentImageNumber < _imagePaths.Count - 1;
        FakeNextPictureButton.IsVisible = _currentImageNumber == _imagePaths.Count - 1;
    }

    private void UpdateUI()
    {
        ColorPreview.Color = _currentColor;
        PreviewFrame.BackgroundColor = _currentColor;
    }

    private void UpdateColorValueEntry()
    {
        if (_setupComplete)
        {
            ColorValueEntry.Text = $"{(int)RedSlider.Value},{(int)GreenSlider.Value},{(int)BlueSlider.Value}";
        }
    }

    private void UpdatePreviewPadding()
    {
        PreviewFrame.Padding = _imageWidth == _imageHeight
            ? new Thickness(_currentBorderThickness)
            : _imageWidth > _imageHeight
                ? new Thickness(_currentBorderThickness, 0)
                : new Thickness(0, _currentBorderThickness);
    }

    private void OnColorValueEntryTextChanged(object sender, TextChangedEventArgs e)
    {
        if (_setupComplete && ParseColorEntry(e.NewTextValue, out var newColor))
        {
            _currentColor = newColor;
            UpdateUI();
        }
    }

    private bool ParseColorEntry(string colorText, out Color color)
    {
        color = default;
        var parts = colorText?.Split(',');
        if (parts?.Length == 3 &&
            int.TryParse(parts[0], out int red) &&
            int.TryParse(parts[1], out int green) &&
            int.TryParse(parts[2], out int blue) &&
            IsValidColorValue(red) && IsValidColorValue(green) && IsValidColorValue(blue))
        {
            color = new Color(red, green, blue);
            return true;
        }
        return false;
    }

    private bool IsValidColorValue(int value) => value >= 0 && value <= 255;

    private void OnColorValueSliderChanged(object sender, ValueChangedEventArgs e)
    {
        if (_setupComplete)
        {
            _currentColor = new Color((int)RedSlider.Value, (int)GreenSlider.Value, (int)BlueSlider.Value);
            UpdateColorValueEntry();
            UpdateUI();
        }
    }

    private void OnBorderThicknessSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (_setupComplete && sender is Slider slider)
        {
            _currentBorderThickness = (int)slider.Value;
            BorderThicknessLabel.Text = $"{_currentBorderThickness:F0} px";
            UpdatePreviewPadding();
        }
    }

    private async void NavigateToPage(int newImageNumber)
    {
        await Navigation.PushAsync(new EditImagePage(_imagePaths, newImageNumber, _currentBorderThickness, _currentColor));
    }

    private async void OnHomeClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new MainPage());
    }

    private void OnExportImagesClicked(object sender, EventArgs e) { /* Implement export functionality here */ }

    private void OnExitClicked(object sender, EventArgs e) => Application.Current.Quit();

    private void PreviousPictureButtonClicked(object sender, EventArgs e) => NavigateToPage(_currentImageNumber - 1);

    private void NextPictureButtonClicked(object sender, EventArgs e) => NavigateToPage(_currentImageNumber + 1);
}
