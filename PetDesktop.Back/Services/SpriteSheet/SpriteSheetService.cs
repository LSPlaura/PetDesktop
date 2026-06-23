using PetDesktop.Back.Repositories;

namespace PetDesktop.Back.Services.SpriteSheet;

public class SpriteSheetService(SpriteSheetGenerator spriteSheetGenerator, SpriteSheetRepository spriteSheetRepository)
{
   public async Task<Models.SpriteSheet> CreateAsync(Models.SpriteSheet item, List<Stream> pngImages)
   {
      try
      {
         using var spriteSheetCreated = await spriteSheetGenerator.CreateSpriteSheetAsync(item.FrameWidth, item.FrameHeight, pngImages);
         await spriteSheetGenerator.SaveSpriteSheet(item.Name, spriteSheetCreated);
         return spriteSheetRepository.Save(item);
      }
      catch(Exception ex)
      {
         throw new Exception();
      }
   }
}