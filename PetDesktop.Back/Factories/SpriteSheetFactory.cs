using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Factories;

public static class SpriteSheetFactory
{
    private static readonly string DefaultSpritesRoute = Path.Combine(AppContext.BaseDirectory, "Assets", "Sprites", "DefaultPet");
    
    public static async Task CreateAnimationsDefaultPetAsync(SpriteSheetService service)
    {
        if (!Directory.Exists(DefaultSpritesRoute))
        {
            //Console.WriteLine($"[Factory] La ruta origen no existe: {DefaultSpritesRoute}");
            return;
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
                
                await service.CreateAsync(spriteSheet, imagesOpened);
                
                //Console.WriteLine($"[Factory] SpriteSheet '{spriteSheet.Name}' creado con éxito.");
            }
            catch (Exception ex)
            {
                //Console.WriteLine($"[Factory] Error al procesar la animación en {folder}: {ex.Message}");
            }
            finally
            {
                foreach (var stream in imagesOpened) stream?.Dispose();
            }
        }
    }
}