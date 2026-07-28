using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;

namespace PetDesktop.Back.Services.Pet;

public class PetCreationOrchestrator
{
    private static readonly ILogger Log = Serilog.Log.ForContext<PetCreationOrchestrator>();
    private readonly SpriteSheetService _spriteSheetService;
    private readonly SpriteSheetGenerator _serviceSpriteSheetGenerator;
    private readonly PetService _petService;
    
    public PetCreationOrchestrator(SpriteSheetService spriteSheetService, PetService petService, SpriteSheetGenerator serviceSpriteSheetGenerator)
    {
        _spriteSheetService = spriteSheetService;
        _petService = petService;
        _serviceSpriteSheetGenerator = serviceSpriteSheetGenerator;
        Init();
    }

    public void Init()
    {
        if (!Directory.Exists(Config.Config.SpriteSheetRoute)) 
            Directory.CreateDirectory(Config.Config.SpriteSheetRoute);
    }
    
    // public async Task<Result<bool, DefaultPetError>> PetCreatorInitAsync(){}

    public async Task<Result<bool, DefaultPetError>> PetCreatorInitAsync()
    { 
        Log.Information("Starting first-time setup to create the default pet...");
        
        var defaultPet = new Models.Pet
        {
            Name = Config.Config.DefaultPetName,
        };
        
        var resultSpriteSheets = await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(_spriteSheetService);
        if (resultSpriteSheets.IsFailure)
        {
            Log.Error("Failed to generate default pet spritesheets. Setup aborted. Error: {ErrorMessage}", resultSpriteSheets.Error.Message);
            await ExecuteLogicRollbackAsync(defaultPet.Name);
            ExecuteHardRollBackAsync(defaultPet.Name);
            return Result.Failure<bool, DefaultPetError>(resultSpriteSheets.Error); 
        }
        
        var resultPet = await _petService.CreateAsync(defaultPet);
        if (resultPet.IsFailure)
        {
            Log.Error("Failed to save the default pet into the database. Setup aborted. Error: {ErrorMessage}", resultPet.Error.Message);
            await ExecuteLogicRollbackAsync(defaultPet.Name);
            ExecuteHardRollBackAsync(defaultPet.Name);
            return Result.Failure<bool, DefaultPetError>(new DefaultPetError.DefaultPetInicializationError(resultPet.Error.Message));
        }
        Log.Information("Default Pet successfully created and registered: {PetName}", defaultPet.Name);
        return Result.Success<bool, DefaultPetError>(true);
    }
    
    /// <summary>
    /// Centralizes the best-effort rollback and handles its result to avoid duplicate logs. Deletes from database.
    /// </summary>
    private async Task ExecuteLogicRollbackAsync(string petName)
    {
        Log.Information("Initiating logical cleanup for {pet} (pet)", petName);
        var logicRollBackResult = await _petService.DeleteAsync(petName);
        if (logicRollBackResult.IsFailure)
            Log.Error("Critical: Logic Rollback failed. Database may be corrupted. Details: {RollbackError}", logicRollBackResult.Error.Message);
    }
    
    /// <summary>
    /// Centralizes the best-effort rollback and handles its result to avoid duplicate logs. Deletes from disk.
    /// </summary>
    private void ExecuteHardRollBackAsync(string petName)
    {
        Log.Information("Initiating physical folder cleanup for {pet} (pet)", petName);
        var hardRollBackResult = _serviceSpriteSheetGenerator.DeleteSpriteSheetRange(petName);
        if (hardRollBackResult.IsFailure)
            Log.Error("Critical: Hard Rollback failed. Details: {RollbackError}", hardRollBackResult.Error.Message);
    }
}