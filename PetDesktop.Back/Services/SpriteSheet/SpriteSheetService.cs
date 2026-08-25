using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.SpriteSheetErrors;
using PetDesktop.Back.Repositories.SpriteSheet;
using Serilog;

namespace PetDesktop.Back.Services.SpriteSheet;

public class SpriteSheetService(SpriteSheetGenerator spriteSheetGenerator, ISpriteSheetRepository spriteSheetRepository)
{

   public async Task<Result<Models.SpriteSheet, SpriteSheetError>> CreateAsync(string folderRoute, Models.SpriteSheet item, List<Stream> pngImages)
   {
      try
      {
         if (pngImages.Count == 0)
         {
            Log.Warning("Intento de generación de SpriteSheet rechazado: Parámetros inválidos.");
            return Result.Failure<Models.SpriteSheet, SpriteSheetError>(new SpriteSheetError.SpriteSheetGeneratorError("Los datos del SpriteSheet o la lista de imágenes no pueden estar vacíos."));
         }
         using var spriteSheetCreated = await spriteSheetGenerator.CreateSpriteSheetAsync(item.FrameWidth, item.FrameHeight, pngImages);
         await spriteSheetGenerator.SaveSpriteSheet(item.Name, folderRoute, spriteSheetCreated);
         return await spriteSheetRepository.CreateAsync(item);
      }
      catch (Exception ex)
      {
         Log.Error(ex, "Error crítico al generar SpriteSheet: {Message}", ex.Message);
         
         return Result.Failure<Models.SpriteSheet, SpriteSheetError>(
            new SpriteSheetError.SpriteSheetGeneratorError($"Error en la generación del spriteSheet: {ex.Message}"));
      }
   }

   public async Task<IEnumerable<Models.SpriteSheet>> GetAll(int page = 1, int pageSize = 20)
   {
      return await spriteSheetRepository.GetAllAsync(page, pageSize);
   }
}