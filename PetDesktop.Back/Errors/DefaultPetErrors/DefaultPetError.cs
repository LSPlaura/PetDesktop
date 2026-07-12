using PetDesktop.Back.Errors.Common;
using PetDesktop.Back.Errors.SpriteSheetErrors;

namespace PetDesktop.Back.Errors.DefaultPetErrors;

public abstract record DefaultPetError(string Message) : DomainError(Message)
{
    public record DefaultPetInicializationError(string Message) : DefaultPetError(Message)
    {
        public sealed record SpriteSheetsNotAvailable(string Message) : DefaultPetInicializationError(Message);
    }

    public record DefaultSpriteSheetInicializationError(string Message) : DefaultPetError(Message)
    {
        public sealed record AssetsFolderNotFound(string Message) : DefaultSpriteSheetInicializationError(Message);
    }
}
