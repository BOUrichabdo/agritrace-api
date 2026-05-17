using ZXing;
using ZXing.Common;
using SkiaSharp;

public class QrService
{

    
        public byte[] Generate(string text)
        {
            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,
                Options = new EncodingOptions
                {
                    Width = 300,      // 🔥 plus net pour PDF
                    Height = 300,
                    Margin = 0        // ❌ supprime espace blanc autour
                }
            };

            var pixelData = writer.Write(text);

            using var bitmap = new SKBitmap(pixelData.Width, pixelData.Height);

            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    int i = (y * pixelData.Width + x) * 4;

                    bitmap.SetPixel(x, y,
                        new SKColor(
                            pixelData.Pixels[i + 2],
                            pixelData.Pixels[i + 1],
                            pixelData.Pixels[i],
                            pixelData.Pixels[i + 3]));
                }
            }

            using var image = SKImage.FromBitmap(bitmap);

            using var data = image.Encode(SKEncodedImageFormat.Png, 100);

            return data.ToArray();
        }
    
    //public byte[] Generate(string text)
    //{
    //    var writer = new BarcodeWriterPixelData
    //    {
    //        Format = BarcodeFormat.QR_CODE,
    //        Options = new EncodingOptions
    //        {
    //            Width = 200,
    //            Height = 200,
    //            Margin = 1
    //        }
    //    };

    //    var pixelData = writer.Write(text);

    //    using var bitmap = new SKBitmap(pixelData.Width, pixelData.Height);

    //    for (int y = 0; y < pixelData.Height; y++)
    //    {
    //        for (int x = 0; x < pixelData.Width; x++)
    //        {
    //            int i = (y * pixelData.Width + x) * 4;

    //            bitmap.SetPixel(x, y,
    //                new SKColor(
    //                    pixelData.Pixels[i + 2],
    //                    pixelData.Pixels[i + 1],
    //                    pixelData.Pixels[i],
    //                    pixelData.Pixels[i + 3]));
    //        }
    //    }

    //    using var image = SKImage.FromBitmap(bitmap);
    //    using var data = image.Encode(SKEncodedImageFormat.Png, 100);

    //    return data.ToArray();
    //}
}