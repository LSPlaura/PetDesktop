using SkiaSharp;

namespace PetDesktop.Back.Services.SpriteSheet;

public class SpriteSheetGenerator
{
    public async Task<Stream> CreateSpriteSheetAsync(int frameWidth, int frameHeight, List<Stream> pngStreams)
    {
        if (pngStreams == null || pngStreams.Count == 0)
            throw new ArgumentException("La lista de imágenes no puede estar vacía.");

        int totalFrames = pngStreams.Count;
        int lienzoWidth = totalFrames * frameWidth;
        int lienzoHeight = frameHeight;

        // 1. Creamos la información del lienzo (Ancho total x Alto)
        var imageInfo = new SKImageInfo(lienzoWidth, lienzoHeight, SKColorType.Rgba8888, SKAlphaType.Premul);
        
        // 2. Inicializamos el Bitmap en memoria y el Canvas para "dibujar"
        var lienzoGiga = new SKBitmap(imageInfo);
        using (var canvas = new SKCanvas(lienzoGiga))
        {
            // Limpiamos el fondo para que sea transparente (esencial para Sprites)
            canvas.Clear(SKColors.Transparent);

            for (int i = 0; i < totalFrames; i++)
            {
                var stream = pngStreams[i];
                if (!stream.CanSeek) throw new InvalidDataException($"Stream no válido en el índice {i}");
                stream.Position = 0;

                // 3. Decodificamos el stream del usuario directamente a un objeto Skia
                using (var codec = SKCodec.Create(stream))
                using (var spriteUsuario = SKBitmap.Decode(codec))
                {
                    int pixelDestinoX = i * frameWidth;

                    // Definimos el rectángulo de destino en el lienzo giga
                    var rectDestino = SKRect.Create(pixelDestinoX, 0, frameWidth, frameHeight);

                    // 4. Dibujamos el sprite en su posición correspondiente
                    // Skia maneja el redimensionamiento automáticamente si el origen y destino difieren
                    canvas.DrawBitmap(spriteUsuario, rectDestino);
                }
            }
        }

        // 5. Convertimos el SKBitmap final en un Stream de PNG en memoria
        var outputStream = new MemoryStream();
        using (var image = SKImage.FromBitmap(lienzoGiga))
        using (var data = image.Encode(SKEncodedImageFormat.Png, 100))
        {
            // Guardamos los bytes en nuestro stream de salida de forma asíncrona
            await Task.Run(() => data.SaveTo(outputStream));
        }
        
        outputStream.Position = 0; // Reseteamos la posición para que quien lo reciba pueda leerlo

        // Liberamos la memoria del mapa de bits que creamos
        lienzoGiga.Dispose(); 

        return outputStream;
    }
    
    public async Task SaveSpriteSheet(string name, Stream spriteSheetStream)
    {
        // 2. La lógica de rutas se maneja aquí o en un FileSystemManager dedicado
        string nombreArchivo = $"{name}.png";

        string carpetaDestino = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PetDesktop", Config.Config.SpriteSheetFolder);

        if (!Directory.Exists(carpetaDestino)) 
            Directory.CreateDirectory(carpetaDestino);

        string rutaFisicaReal = Path.Combine(carpetaDestino, nombreArchivo);

        // 3. Guardado ASÍNCRONO para no congelar la app
        using (var fileStream = new FileStream(rutaFisicaReal, FileMode.Create, FileAccess.Write, FileShare.None, 4096, useAsync: true))
        {
            await spriteSheetStream.CopyToAsync(fileStream);
        }
    }
}