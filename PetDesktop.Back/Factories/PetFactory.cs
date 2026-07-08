using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Factories;

public static class PetFactory
{
    public static Pet CreateDefaultPet(SpriteSheetService service)
    {
        var spriteSheet = service.GetAll();
        return new Pet
        {
            Name = "Pingu",
            ActualAnimation = spriteSheet.FirstOrDefault(a => a.Name.Contains(Config.Config.DefaultSpriteSheetName)),
        };
    }
}