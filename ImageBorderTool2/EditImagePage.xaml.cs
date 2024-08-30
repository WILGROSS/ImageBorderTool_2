using SkiaSharp;

namespace ImageBorderTool2;

public partial class EditImagePage : ContentPage
{
    private int _currentImage;
    private string _imagePath;
    private int _imageWidth;
    private int _imageHeight;
    private int _defaultThickness = 2;
    private int _defaultRed = 255;
    private int _defaultGreen = 255;
    private int _defaultBlue = 255;
    public EditImagePage(List<string> imagePaths, int currentImage)
    {
        InitializeComponent();

        _currentImage = currentImage;
        _imagePath = imagePaths[_currentImage];

        SetCurrentImage();

        if (currentImage == 0)
            SetDefaultValues();
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

    private void SetDefaultValues()
    {
        BorderThicknessSlider.Value = _defaultThickness;
        BorderThicknessLabel.SetValue(XProperty, _defaultThickness);
        PreviewFrame.Padding = new Thickness(_defaultThickness);

        RedSlider.Value = _defaultRed;
        GreenSlider.Value = _defaultGreen;
        BlueSlider.Value = _defaultBlue;

        ColorPreview.Color = Color.FromRgb(_defaultRed, _defaultGreen, _defaultBlue);
        PreviewFrame.BackgroundColor = ColorPreview.Color;
    }

    private void UpdateBorderThickness()
    {

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

    private void OnBorderThicknessSliderValueChanged(object sender, ValueChangedEventArgs e)
    {
        if (sender is Slider slider)
        {
            if (_imageWidth > _imageHeight)
            {
                var padding = new Thickness(slider.Value, 0);
                PreviewFrame.Padding = padding;
                return;
            }
            else if ((_imageWidth < _imageHeight))
            {
                var padding = new Thickness(0, slider.Value);
                PreviewFrame.Padding = padding;
                return;
            }
            PreviewFrame.Padding = new Thickness(slider.Value);
        }
    }

    private void OnBorderColorEntryCompleted(object sender, EventArgs e)
    {

    }

    private void OnColorValueChanged(object sender, ValueChangedEventArgs e)
    {
        int red = (int)RedSlider.Value;
        int green = (int)GreenSlider.Value;
        int blue = (int)BlueSlider.Value;

        ColorPreview.Color = Color.FromRgb(red, green, blue);
        PreviewFrame.BackgroundColor = ColorPreview.Color;
    }
}