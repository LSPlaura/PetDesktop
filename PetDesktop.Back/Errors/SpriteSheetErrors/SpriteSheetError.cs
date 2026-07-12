using PetDesktop.Back.Errors.Common;

namespace PetDesktop.Back.Errors.SpriteSheetErrors;

public abstract record SpriteSheetError(string Message) : DomainError(Message)
{
    public sealed record SpriteSheetGeneratorError(string Message) : SpriteSheetError(Message);
    public sealed record SpriteSheetDbError(string Message) : SpriteSheetError(Message);
}