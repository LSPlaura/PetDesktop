using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Services.Pet;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;

namespace PetDesktop.Back.Services.Setup;

public class PetInitializationService(SpriteSheetService spriteSheetService, PetService petService)
{
    private static readonly ILogger Log = Serilog.Log.ForContext<PetInitializationService>();

    public async Task<Result<bool, DefaultPetError>> ProvisionDefaultPetAsync()
    { 
        Log.Information("Starting first-time setup to create the default pet...");
        
        var resultSpriteSheets = await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(spriteSheetService);
        if (resultSpriteSheets.IsFailure)
        {
            Log.Error("Failed to generate default pet spritesheets. Setup aborted. Error: {ErrorMessage}", resultSpriteSheets.Error.Message);
            await ExecuteRollbackAsync("spriteSheet failure");
            return Result.Failure<bool, DefaultPetError>(resultSpriteSheets.Error); 
        }
        
        var defaultPet = await Factories.PetFactory.CreateDefaultPet(spriteSheetService);
        if (defaultPet.IsFailure)
        { 
            Log.Error("Failed to build the default pet instance. Setup aborted. Error: {ErrorMessage}", defaultPet.Error.Message);
            await ExecuteRollbackAsync("Pet factory failure");
            return Result.Failure<bool, DefaultPetError>(defaultPet.Error);
        }
        
        var resultPet = await petService.CreateAsync(defaultPet.Value);
        if (resultPet.IsFailure)
        {
            Log.Error("Failed to save the default pet into the database. Setup aborted. Error: {ErrorMessage}", resultPet.Error.Message);
            await ExecuteRollbackAsync("persistence failure");
            return Result.Failure<bool, DefaultPetError>(new DefaultPetError.DefaultPetInicializationError(resultPet.Error.Message));
        }

        Log.Information("Default Pet successfully created and registered: {PetName}", defaultPet.Value.Name);
        return Result.Success<bool, DefaultPetError>(true);
    }

    /// <summary>
    /// Centralizes the best-effort rollback and handles its result to avoid duplicate logs.
    /// </summary>
    private async Task ExecuteRollbackAsync(string reason)
    {
        Log.Warning("Initiating installation rollback due to {Reason}...", reason);
        
        var rollbackResult = await petService.DeleteAllAsync();
        if (rollbackResult.IsFailure)
        {
            Log.Error("Critical: Rollback failed. Database may be corrupted. Details: {RollbackError}", rollbackResult.Error.Message);
        }
    }
}