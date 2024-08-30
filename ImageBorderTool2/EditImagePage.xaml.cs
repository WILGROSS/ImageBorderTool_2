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
		BorderThicknessLabel.Text = $"{_defaultThickness} px";
		PreviewFrame.Padding = new Thickness(_defaultThickness);

		RedSlider.Value = _defaultRed;
		GreenSlider.Value = _defaultGreen;
		BlueSlider.Value = _defaultBlue;

		ColorPreview.Color = Color.FromRgb(_defaultRed, _defaultGreen, _defaultBlue);
		PreviewFrame.BackgroundColor = ColorPreview.Color;

		UpdateColorValueEntry();
	}

	private void UpdateColorValueEntry()
	{
		ColorValueEntry.Text = $"{(int)RedSlider.Value},{(int)GreenSlider.Value},{(int)BlueSlider.Value}";
	}

	private void OnColorValueEntryTextChanged(object sender, TextChangedEventArgs e)
	{
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

			ColorPreview.Color = Color.FromRgb(red, green, blue);
			PreviewFrame.BackgroundColor = ColorPreview.Color;
		}
	}

	private void OnColorValueSliderChanged(object sender, ValueChangedEventArgs e)
	{
		int red = (int)RedSlider.Value;
		int green = (int)GreenSlider.Value;
		int blue = (int)BlueSlider.Value;

		ColorPreview.Color = Color.FromRgb(red, green, blue);
		PreviewFrame.BackgroundColor = ColorPreview.Color;

		UpdateColorValueEntry();
	}

	private void OnBorderThicknessSliderValueChanged(object sender, ValueChangedEventArgs e)
	{
		if (sender is Slider slider)
		{
			BorderThicknessLabel.Text = $"{slider.Value:F0} px";

			if (_imageWidth > _imageHeight)
			{
				var padding = new Thickness(slider.Value, 0);
				PreviewFrame.Padding = padding;
			}
			else if (_imageWidth < _imageHeight)
			{
				var padding = new Thickness(0, slider.Value);
				PreviewFrame.Padding = padding;
			}
			else
			{
				PreviewFrame.Padding = new Thickness(slider.Value);
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
}
