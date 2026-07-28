using PetDesktop.Back.Errors.Common;
using PetDesktop.Back.Errors.SpriteSheetErrors;

namespace PetDesktop.Back.Errors.PetError;

public abstract record PetError(string Message) : DomainError(Message)
{
    public sealed record PetDbError(string Message) : PetError(Message);
    public sealed record MissingPetError(string Message) : PetError(Message);
}