using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SkiaSharp;
using ZXing;
using ZXing.Common;

namespace TracAgriApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class QrController : ControllerBase
    {

        // =========================================
        // GENERER QR DYNAMIQUE
        // api/qr/ETQ-123456
        // =========================================

        [HttpGet("{code}")]
        public IActionResult GenerateQr(string code)
        {
            // =========================
            // GENERATEUR QR
            // =========================

            var writer = new BarcodeWriterPixelData
            {
                Format = BarcodeFormat.QR_CODE,

                Options = new EncodingOptions
                {
                    Width = 400,
                    Height = 400,
                    Margin = 1
                }
            };

            // =========================
            // GENERER PIXELS
            // =========================

            var pixelData = writer.Write(code);

            // =========================
            // CREER BITMAP SKIA
            // =========================

            using var bitmap = new SKBitmap(
                pixelData.Width,
                pixelData.Height
            );

            // =========================
            // REMPLIR PIXELS
            // =========================

            for (int y = 0; y < pixelData.Height; y++)
            {
                for (int x = 0; x < pixelData.Width; x++)
                {
                    int index =
                        (y * pixelData.Width + x) * 4;

                    bitmap.SetPixel(
                        x,
                        y,
                        new SKColor(
                            pixelData.Pixels[index + 2],
                            pixelData.Pixels[index + 1],
                            pixelData.Pixels[index],
                            pixelData.Pixels[index + 3]
                        )
                    );
                }
            }

            // =========================
            // CONVERTIR PNG
            // =========================

            using var image =
                SKImage.FromBitmap(bitmap);

            using var data =
                image.Encode(SKEncodedImageFormat.Png, 100);

            // =========================
            // RETOUR IMAGE PNG
            // =========================

            return File(
                data.ToArray(),
                "image/png"
            );
        }
    }
}
