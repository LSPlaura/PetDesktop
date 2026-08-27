using PetDesktop.Back.Repositories.Common;

namespace PetDesktop.Back.Repositories.SpriteSheet;

public interface ISpriteSheetRepository : ICrud<(string PetName, string SpriteName), Models.SpriteSheet>
{
    
}