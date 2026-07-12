using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Models;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Factories;

public static class PetFactory
{
    public static async Task<Result<Pet, DefaultPetError>> CreateDefaultPet(SpriteSheetService service)
    {
        var task = await service.GetAll();
        var spriteSheets = task.ToList();
        if (!spriteSheets.Any())
            return new DefaultPetError.DefaultPetInicializationError("There are not any spriteSheet for the default Pet");
        return new Pet
        {
            Name = Config.Config.DefaultPetName,
            ActualAnimation = spriteSheets.FirstOrDefault(a => a.Name.Contains(Config.Config.DefaultSpriteSheetName)),
        };
    }
}