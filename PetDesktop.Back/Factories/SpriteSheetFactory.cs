using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Factories;

public static class SpriteSheetFactory
{
    private static readonly string _defaultSpritesRoute = Path.Combine(AppContext.BaseDirectory, "Assets", "Sprites", "DefaultPet");
    private static readonly string _spriteSheetsRoute =
        Path.Combine(Config.Config.SpriteSheetRoute, Config.Config.DefaultPetName);
    private static readonly ILogger _logger = Log.ForContext(typeof(SpriteSheetFactory));

    public static async Task<Result<bool, DefaultPetError>> CreateAnimationsDefaultPetAsync(SpriteSheetService service)
    {
        if (!Directory.Exists(_defaultSpritesRoute))
        {
            //Console.WriteLine($"[Factory] La ruta origen no existe: {_defaultSpritesRoute}");
            return Result.Failure<bool, DefaultPetError>(
                new DefaultPetError.DefaultSpriteSheetInicializationError.AssetsFolderNotFound("Assets file not found"));
        }

        var foldersRoute = Directory.GetDirectories(_defaultSpritesRoute);

        foreach (var folder in foldersRoute)
        {
            var routeFiles = Directory.GetFiles(folder, "*.png")
                .OrderBy(file => Path.GetFileName(file));

            if (!routeFiles.Any()) continue;

            string nameAnimation = Path.GetFileName(folder);
            List<Stream> imagesOpened = new List<Stream>();

            try
            {
                foreach (var fileRoute in routeFiles)
                {
                    FileStream image = File.OpenRead(fileRoute);
                    imagesOpened.Add(image);
                }
                
                //folder está mal
                var spriteSheet = new SpriteSheet(nameAnimation, Path.Combine(_spriteSheetsRoute, nameAnimation), Config.Config.DefaultPetFrameWidth,
                    Config.Config.DefaultPetFrameHeight, Config.Config.DefaultPetName);

                var result = await service.CreateAsync(_spriteSheetsRoute, spriteSheet, imagesOpened);
                if (result.IsFailure) return Result.Failure<bool, DefaultPetError>(new DefaultPetError.DefaultSpriteSheetInicializationError($"Error while creating the spriteSheets: {result.Error.Message}"));
                //Console.WriteLine($"[Factory] SpriteSheet '{spriteSheet.Name}' creado con éxito.");
            }
            catch (Exception ex)
            {
                return Result.Failure<bool, DefaultPetError>(
                    new DefaultPetError.DefaultSpriteSheetInicializationError($"Error: {ex.Message}"));
                //Console.WriteLine($"[Factory] Error al procesar la animación en {folder}: {ex.Message}");
            }
            finally
            {
                foreach (var stream in imagesOpened) await stream.DisposeAsync();
            }
        }
        return Result.Success<bool, DefaultPetError>(true);
    }
}
// while creating the spritesSheets of the defaultPet: 