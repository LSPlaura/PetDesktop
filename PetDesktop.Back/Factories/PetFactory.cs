using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Factories;

public static class PetFactory
{
    public static async Task<Pet> CreateDefaultPet(SpriteSheetService service)
    {
        var spriteSheet = await service.GetAll();
        return new Pet
        {
            Name = Config.Config.DefaultPetName,
            ActualAnimation = spriteSheet.FirstOrDefault(a => a.Name.Contains(Config.Config.DefaultSpriteSheetName)),
        };
    }
}