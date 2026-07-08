using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Factories;

public static class PetFactory
{
    private static string _defaultSpriteSheet = "DefaultRight";
    public static Pet CreateDefaultPet(SpriteSheetService service)
    {
        var spriteSheet = service.GetAll();
        return new Pet
        {
            Name = "Pingu",
            ActualAnimation = spriteSheet.FirstOrDefault(a => a.Name.Contains(_defaultSpriteSheet)),
        };
    }
}