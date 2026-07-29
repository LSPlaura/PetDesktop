using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.SpriteSheetErrors;
using SkiaSharp;

namespace PetDesktop.Back.Services.SpriteSheet;

public class SpriteSheetGenerator
{
    public async Task<Stream> CreateSpriteSheetAsync(int frameWidth, int frameHeight, List<Stream> pngStreams)
    {
        int totalFrames = pngStreams.Count;
        int canvasWidth = totalFrames * frameWidth;

        //crea la información que el spritesheet tendrá (tamaño, como trata el color, como trata la transparencia)
        var imageInfo = new SKImageInfo(canvasWidth, frameHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
        
       //reserva una imagen en la memoria ram con la anterior información
        using var bitmapImage = new SKBitmap(imageInfo);
        //se crea un lienzo para poder dibujar sobre la imagen
        using (var canvas = new SKCanvas(bitmapImage))
        {
            //se limpia el fondo y se decide dejar como transparente
            canvas.Clear(SKColors.Transparent);

            for (int i = 0; i < totalFrames; i++)
            {
                var stream = pngStreams[i];
                if (!stream.CanSeek) throw new InvalidDataException($"Stream no válido en el índice {i}");
                stream.Position = 0;

                //se asegura de que el stream esté en el formato correcto(png)
                using (var codec = SKCodec.Create(stream))
                using (var spriteUserBytes = SKBitmap.Decode(codec)) //convierte el stream en una matriz de pixeles añmacenados en la ram
                {
                    int lastPixelX = i * frameWidth;

                    // Definimos el rectángulo (donde se va a estampar el sprite) de destino en el lienzo giga
                    var drawingPositionArea = SKRect.Create(lastPixelX, 0, frameWidth, frameHeight);

                    // Se dibuja el sprite en el lienzo
                    // Skia maneja el redimensionamiento automáticamente si el origen y destino difieren
                    canvas.DrawBitmap(spriteUserBytes, drawingPositionArea);
                }
            }
        }

        //se crea el memory stream para posteriormente poder guardarlo en un archivo
        var outputStream = new MemoryStream();
        //se crea la imagen como imagen (en bytes)
        using (var image = SKImage.FromBitmap(bitmapImage))
        using (var codedImage = image.Encode(SKEncodedImageFormat.Png, 100)) //se codifica en el formato que se quiere
        {
            //se guarda la imagen en bytes ya codificada en el memory stream
            await Task.Run(() => codedImage.SaveTo(outputStream));
        }
        
        outputStream.Position = 0; // Se resetea la posición para que quien lo reciba pueda leerlo
        
        return outputStream;
    }
    
    public async Task SaveSpriteSheet(string fileName, string folderRoute, Stream spriteSheetStream)
    {
        string destinationRoute = Path.Combine(folderRoute, fileName + ".png");
        
        if (spriteSheetStream.CanSeek) spriteSheetStream.Position = 0;
        
        using (var fileStream = new FileStream(destinationRoute, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await spriteSheetStream.CopyToAsync(fileStream);
        }
        
        if (spriteSheetStream.CanSeek) spriteSheetStream.Position = 0;
    }

    public Result<bool, SpriteSheetError> DeleteSpriteSheetRange(string petName)
    {
        var root = Path.GetFullPath(Config.Config.SpriteSheetRoute);
        var destinated = Path.GetFullPath(Path.Combine(root, petName));

        if (!destinated.StartsWith(root + Path.DirectorySeparatorChar, StringComparison.OrdinalIgnoreCase))
            return Result.Failure<bool, SpriteSheetError>(new SpriteSheetError.SpriteSheetDeleteError($"Root '{destinated} invalid'"));

        if (Directory.Exists(destinated))
        {
            try
            {
                Directory.Delete(destinated, recursive: true);
            }
            catch (Exception ex)
            {
                return Result.Failure<bool, SpriteSheetError>(new SpriteSheetError.SpriteSheetDeleteError($"Delete method failed: {ex.Message}"));
            }
        }
        return Result.Success<bool, SpriteSheetError>(true);
    }
}