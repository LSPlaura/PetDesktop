using PetDesktop.Back.Repositories.Common;

namespace PetDesktop.Back.Repositories.Pet;

public interface IPetRepository : ICrud<string, Models.Pet>
{
    Task<bool> DeleteAllAsync();
    Task<bool> DeleteAllSpriteSheetsAsync();
}