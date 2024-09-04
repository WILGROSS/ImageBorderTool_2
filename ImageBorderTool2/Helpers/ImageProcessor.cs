using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Microsoft.Maui.Graphics.Color;
namespace ImageBorderTool
{
	public class ImageProcessor
	{
		private int _borderThickness;
		private SixLabors.ImageSharp.Color _selectedColor;
		public void Run(string imagePath, int borderThickness, Color color, bool fullSize, bool webSize)
		{
			string directory = Path.GetDirectoryName(imagePath);
			string borderToolDirectory = Path.Combine(directory, "BorderTool");
			Directory.CreateDirectory(borderToolDirectory);

			_borderThickness = borderThickness;
			byte red;
			byte green;
			byte blue;
			color.ToRgb(out red, out green, out blue);

			_selectedColor = SixLabors.ImageSharp.Color.FromRgb(red, green, blue);

			string filenameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
			string extension = Path.GetExtension(imagePath);
			string fullSizeOutputPath = Path.Combine(borderToolDirectory, $"{filenameWithoutExtension}_FullSize{extension}");
			string webSizeOutputPath = Path.Combine(borderToolDirectory, $"{filenameWithoutExtension}_WebSize{extension}");

			if (fullSize)
				ResizeImageToSquare(imagePath, fullSizeOutputPath, null);
			if (webSize)
				ResizeImageToSquare(imagePath, webSizeOutputPath, 600);
		}

		private void ResizeImageToSquare(string inputImagePath, string outputImagePath, int? outputSize)
		{
			using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(inputImagePath))
			{
				int size = Math.Max(image.Width, image.Height) + _borderThickness;

				using (Image<Rgba32> squareImage = new Image<Rgba32>(Configuration.Default, size, size, _selectedColor))
				{
					int x = (size - image.Width) / 2;
					int y = (size - image.Height) / 2;

					squareImage.Mutate(ctx => ctx.DrawImage(image, new SixLabors.ImageSharp.Point(x, y), 1f));
					if (outputSize != null)
					{
						SixLabors.ImageSharp.Size newSize = new((int)outputSize, (int)outputSize);
						squareImage.Mutate(ctx => ctx.Resize(newSize));
					}

					squareImage.Save(outputImagePath);
				}
			}
		}
	}
}
