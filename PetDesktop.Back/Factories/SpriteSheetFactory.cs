using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Errors.SpriteSheetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Factories;

public static class SpriteSheetFactory
{
    private static readonly string DefaultSpritesRoute = Path.Combine(AppContext.BaseDirectory, "Assets", "Sprites", "DefaultPet");
    private static readonly ILogger _logger = Log.ForContext(typeof(SpriteSheetFactory));

    public static async Task<Result<bool, DefaultPetError>> CreateAnimationsDefaultPetAsync(SpriteSheetService service)
    {
        if (!Directory.Exists(DefaultSpritesRoute))
        {
            //Console.WriteLine($"[Factory] La ruta origen no existe: {DefaultSpritesRoute}");
            return Result.Failure<bool, DefaultPetError>(
                new DefaultPetError.DefaultSpriteSheetInicializationError.AssetsFolderNotFound("Assets file not found"));
        }

        var foldersRoute = Directory.GetDirectories(DefaultSpritesRoute);

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

                var spriteSheet = new SpriteSheet() 
                {
                    Name = $"{nameAnimation}_{Config.Config.DefaultPetName}",
                    FrameHeight = Config.Config.DefaultPetFrameHeight,
                    FrameWidth = Config.Config.DefaultPetFrameWidth,
                    Route = folder,
                    AssociatedPet = $"{Config.Config.DefaultPetName}"
                };

                var result = await service.CreateAsync(spriteSheet, imagesOpened);
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
                foreach (var stream in imagesOpened) stream?.Dispose();
            }
        }
        return Result.Success<bool, DefaultPetError>(true);
    }
}
// while creating the spritesSheets of the defaultPet: 