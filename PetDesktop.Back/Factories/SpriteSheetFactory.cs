using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Factories;

public static class SpriteSheetFactory
{
    private static readonly string _spriteSheetsRoute =
        Path.Combine(Config.Config.SpriteSheetRoute, Config.Config.DefaultPetName);
    private static readonly ILogger Log = Serilog.Log.ForContext(typeof(SpriteSheetFactory));

    public static async Task<Result<bool, DefaultPetError>> CreateAnimationsDefaultPetAsync(SpriteSheetService service)
    {
        Log.Information("Iniciando la generación de animaciones para la mascota por defecto...");
        Log.Debug("Ruta origen de sprites: {DefaultPetSpritesRoute}", Config.Config.DefaultPetSpritesRoute);
        Log.Debug("Ruta destino de SpriteSheets: {SpriteSheetsRoute}", _spriteSheetsRoute);

        if (!Directory.Exists(Config.Config.DefaultPetSpritesRoute))
        {
            Log.Error("Error crítico: El directorio origen de sprites no existe en la ruta {Route}", Config.Config.DefaultPetSpritesRoute);
            return Result.Failure<bool, DefaultPetError>(
                new DefaultPetError.DefaultSpriteSheetInicializationError.AssetsFolderNotFound("Assets file not found"));
        }

        var foldersRoute = Directory.GetDirectories(Config.Config.DefaultPetSpritesRoute);
        Log.Information("Se encontraron {Count} carpetas de animación para procesar.", foldersRoute.Length);

        foreach (var folder in foldersRoute)
        {
            string nameAnimation = Path.GetFileName(folder);
            Log.Information("Procesando animación: {AnimationName} desde {FolderPath}", nameAnimation, folder);

            var routeFiles = Directory.GetFiles(folder, "*.png")
                .OrderBy(file => Path.GetFileName(file))
                .ToList();

            if (!routeFiles.Any())
            {
                Log.Warning("La carpeta de la animación {AnimationName} no contiene archivos .png. Omitiendo...", nameAnimation);
                continue;
            }

            Log.Debug("Se encontraron {FileCount} cuadros de imagen para la animación '{AnimationName}'", routeFiles.Count, nameAnimation);

            List<Stream> imagesOpened = new List<Stream>();

            try
            {
                foreach (var fileRoute in routeFiles)
                {
                    FileStream? image = null;
                    try
                    {
                        image = File.OpenRead(fileRoute);
                        imagesOpened.Add(image);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex, "Error al abrir el archivo de imagen {FileRoute} para la animación '{AnimationName}'", fileRoute, nameAnimation);
                        if (image != null) await image.DisposeAsync();
                        return Result.Failure<bool, DefaultPetError>(
                            new DefaultPetError.DefaultSpriteSheetInicializationError($"Error: {ex.Message}"));
                    }
                }

                var spriteSheet = new SpriteSheet(
                    nameAnimation, 
                    Path.Combine(_spriteSheetsRoute, nameAnimation), 
                    Config.Config.DefaultPetFrameWidth,
                    Config.Config.DefaultPetFrameHeight, 
                    Config.Config.DefaultPetName
                );

                Log.Debug("Solicitando la creación del SpriteSheet '{AnimationName}' al servicio...", nameAnimation);
                var result = await service.CreateAsync(_spriteSheetsRoute, spriteSheet, imagesOpened);

                if (result.IsFailure)
                {
                    Log.Error("Falló la creación del SpriteSheet '{AnimationName}'. Error: {ErrorMessage}", nameAnimation, result.Error.Message);
                    return Result.Failure<bool, DefaultPetError>(
                        new DefaultPetError.DefaultSpriteSheetInicializationError($"Error while creating the spriteSheets: {result.Error.Message}"));
                }

                Log.Information("SpriteSheet '{AnimationName}' creado y registrado con éxito.", nameAnimation);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Excepción inesperada al procesar la animación '{AnimationName}' en {Folder}", nameAnimation, folder);
                return Result.Failure<bool, DefaultPetError>(
                    new DefaultPetError.DefaultSpriteSheetInicializationError($"Error: {ex.Message}"));
            }
            finally
            {
                Log.Debug("Cerrando los streams de imagen para la animación '{AnimationName}'...", nameAnimation);
                foreach (var stream in imagesOpened) 
                    await stream.DisposeAsync();
            }
        }

        Log.Information("Generación de animaciones finalizada con éxito.");
        return Result.Success<bool, DefaultPetError>(true);
    }
}