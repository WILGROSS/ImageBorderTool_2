using SixLabors.ImageSharp;
using SixLabors.ImageSharp.PixelFormats;
using SixLabors.ImageSharp.Processing;
using Color = Microsoft.Maui.Graphics.Color;
namespace ImageBorderTool
{
	public class ImageProcessor
	{
		private int _borderThickness;
		private Color color;
		public void Run(string imagePath, int borderThickness, Color color)
		{
			string directory = Path.GetDirectoryName(imagePath);
			string borderToolDirectory = Path.Combine(directory, "BorderTool");
			Directory.CreateDirectory(borderToolDirectory);

			string filenameWithoutExtension = Path.GetFileNameWithoutExtension(imagePath);
			string extension = Path.GetExtension(imagePath);
			string fullSizeOutputPath = Path.Combine(borderToolDirectory, $"{filenameWithoutExtension}_FullSize{extension}");
			string webSizeOutputPath = Path.Combine(borderToolDirectory, $"{filenameWithoutExtension}_WebSize{extension}");

			ResizeImageToSquare(imagePath, fullSizeOutputPath, null);
			ResizeImageToSquare(imagePath, webSizeOutputPath, 600);
		}

		private bool IsImage(string filepath)
		{
			try
			{
				using (System.Drawing.Image img = System.Drawing.Image.FromFile(filepath))
				{
					return true;
				}
			}
			catch
			{
				return false;
			}
		}

		private void ResizeImageToSquare(string inputImagePath, string outputImagePath, int? outputSize)
		{
			using (SixLabors.ImageSharp.Image image = SixLabors.ImageSharp.Image.Load(inputImagePath))
			{
				int size = Math.Max(image.Width, image.Height);

				//Remove +12's if you don't want the fine white line around the edges
				using (Image<Rgba32> squareImage = new Image<Rgba32>(Configuration.Default, size + 12, size + 12, SixLabors.ImageSharp.Color.White))
				{
					int x = (size + 12 - image.Width) / 2;
					int y = (size + 12 - image.Height) / 2;

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
