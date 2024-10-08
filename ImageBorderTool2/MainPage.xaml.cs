﻿namespace ImageBorderTool2
{
    public partial class MainPage : ContentPage
    {
        private int _imageCount = 0;
        private List<string> _selectedImagePaths = new List<string>();

        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnAddImagesClicked(object sender, EventArgs e)
        {
            var files = await PickImagesAsync();
            if (files != null)
            {
                foreach (var file in files)
                {
                    AddImageToGrid(file);
                }
            }
        }

        private async Task<IEnumerable<FileResult>> PickImagesAsync()
        {
            var result = await FilePicker.PickMultipleAsync(new PickOptions
            {
                FileTypes = FilePickerFileType.Images,
                PickerTitle = "Select Images"
            });

            return result;
        }

        private void AddImageToGrid(FileResult file)
        {
            var removeButtonSize = 24;

            if (file == null) return;

            _selectedImagePaths.Add(file.FullPath);

            var imageSource = ImageSource.FromFile(file.FullPath);

            var image = new Image
            {
                Source = imageSource,
                Aspect = Aspect.AspectFit
            };

            var removeButton = new Button
            {
                Text = "X",
                BackgroundColor = (Color)Application.Current.Resources["Warning"],
                TextColor = (Color)Application.Current.Resources["PrimaryText"],
                FontSize = 16,
                HorizontalOptions = LayoutOptions.End,
                VerticalOptions = LayoutOptions.Start,
                Padding = new Thickness(0),
                HeightRequest = removeButtonSize,
                WidthRequest = removeButtonSize,
                CornerRadius = removeButtonSize / 2,
            };

            var frame = new Frame
            {
                BackgroundColor = (Color)Application.Current.Resources["Gray300"],
                CornerRadius = 4,
                HasShadow = false,
                Margin = new Thickness(8),
                Padding = 0,
                Content = new Grid
                {
                    Children = { image, removeButton }
                }
            };

            Grid.SetRow(removeButton, 0);
            Grid.SetColumn(removeButton, 1);

            removeButton.Clicked += (s, e) => RemoveImageFromGrid(frame, file.FullPath);

            ImageGrid.Children.Add(frame);

            int column = _imageCount % 3;
            int row = _imageCount / 3;

            Grid.SetColumn(frame, column);
            Grid.SetRow(frame, row);

            if (column == 0)
            {
                ImageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
            }

            _imageCount++;

            UpdateImageBoxVisibility();
            UpdateNextButtonVisibility();
        }

        private void RemoveImageFromGrid(Frame frame, string filePath)
        {
            ImageGrid.Children.Remove(frame);
            _selectedImagePaths.Remove(filePath);
            _imageCount--;
            UpdateImageBoxVisibility();
            UpdateNextButtonVisibility();
            UpdateImageGrid();
        }

        private void UpdateImageGrid()
        {
            ImageGrid.RowDefinitions.Clear();

            for (int i = 0; i < ImageGrid.Children.Count; i++)
            {
                var frame = (Frame)ImageGrid.Children[i];

                int column = i % 3;
                int row = i / 3;

                Grid.SetColumn(frame, column);
                Grid.SetRow(frame, row);

                if (column == 0)
                {
                    ImageGrid.RowDefinitions.Add(new RowDefinition { Height = new GridLength(1, GridUnitType.Auto) });
                }
            }
        }

        private void UpdateImageBoxVisibility() => ImageBox.IsVisible = _imageCount > 0;
        private void UpdateNextButtonVisibility() => ContinueButton.IsVisible = _imageCount > 0;

        private void OnExitClicked(object sender, EventArgs e)
        {
            Application.Current.Quit();
        }

        private async void OnContinueClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new EditImagePage(_selectedImagePaths, 0, 0, new Color(255, 255, 255)));
        }
    }
}