using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PetDesktop.Back.Models;

namespace PetDesktop.Back.Factories;

public static class PetFactory
{
    private const string DefaultSpriteRoute = "avares://PetDesktop.UI/Assets/Sprites/pet_default.png";

    public static Pet CreateDefaultPet()
    {
        return new Pet
        {
            ActualAnimation = new SpriteSheet
            {
                Name = "Default_Idle",
                Route = DefaultSpriteRoute,
                FrameWidth = 64,
                FrameHeight = 64,
                AssociatedPet = "Mascota por defecto"
            }
        };
    }
}