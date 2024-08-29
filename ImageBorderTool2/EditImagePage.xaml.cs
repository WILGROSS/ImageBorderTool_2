namespace ImageBorderTool2;

public partial class EditImagePage : ContentPage
{
	public EditImagePage(List<string> imagePaths)
	{
		InitializeComponent();
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

	}

	private void OnBorderThicknessEntryCompleted(object sender, EventArgs e)
	{
		if (sender is Entry entry && double.TryParse(entry.Text, out double value))
		{
			if (value < BorderThicknessSlider.Minimum)
			{
				value = BorderThicknessSlider.Minimum;
			}
			else if (value > BorderThicknessSlider.Maximum)
			{
				value = BorderThicknessSlider.Maximum;
			}

			BorderThicknessSlider.Value = value;
		}
	}

	private void OnBorderColorEntryCompleted(object sender, EventArgs e)
	{

	}
}