using PetDesktop.Back.Repositories;
using PetDesktop.Back.Repositories.SpriteSheet;

namespace PetDesktop.Back.Services.SpriteSheet;

public class SpriteSheetService(SpriteSheetGenerator spriteSheetGenerator, SpriteSheetRepository spriteSheetRepository)
{
   public async Task<Models.SpriteSheet> CreateAsync(Models.SpriteSheet item, List<Stream> pngImages)
   {
      try
      {
         using var spriteSheetCreated = await spriteSheetGenerator.CreateSpriteSheetAsync(item.FrameWidth, item.FrameHeight, pngImages);
         await spriteSheetGenerator.SaveSpriteSheet(item.Name, spriteSheetCreated);
         return await spriteSheetRepository.CreateAsync(item);
      }
      catch(Exception ex)
      {
         throw new Exception();
      }
   }

   public async Task<IEnumerable<Models.SpriteSheet>> GetAll(int page = 1, int pageSize = 20)
   {
      return await spriteSheetRepository.GetAllAsync(page, pageSize);
   }
}