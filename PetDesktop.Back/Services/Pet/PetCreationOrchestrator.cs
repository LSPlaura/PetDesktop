using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Services.SpriteSheet;
using Serilog;

namespace PetDesktop.Back.Services.Pet;

public class PetCreationOrchestrator
{
    private static readonly ILogger Log = Serilog.Log.ForContext<PetCreationOrchestrator>();
    private readonly SpriteSheetService _spriteSheetService;
    private readonly PetService _petService;
    
    public PetCreationOrchestrator(SpriteSheetService spriteSheetService, PetService petService)
    {
        _spriteSheetService = spriteSheetService;
        _petService = petService;
        Init();
    }

    public void Init()
    {
        if (!Directory.Exists(Config.Config.SpriteSheetRoute)) 
            Directory.CreateDirectory(Config.Config.SpriteSheetRoute);
    }
    
    // public async Task<Result<bool, DefaultPetError>> PetCreatorAsync(){}

    public async Task<Result<bool, DefaultPetError>> PetCreatorAsync()
    { 
        Log.Information("Starting first-time setup to create the default pet...");
        
        var resultSpriteSheets = await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(_spriteSheetService);
        if (resultSpriteSheets.IsFailure)
        {
            Log.Error("Failed to generate default pet spritesheets. Setup aborted. Error: {ErrorMessage}", resultSpriteSheets.Error.Message);
            await ExecuteRollbackAsync("spriteSheet failure");
            return Result.Failure<bool, DefaultPetError>(resultSpriteSheets.Error); 
        }
        
        var defaultPet = await Factories.PetFactory.CreateDefaultPet(_spriteSheetService);
        if (defaultPet.IsFailure)
        { 
            Log.Error("Failed to build the default pet instance. Setup aborted. Error: {ErrorMessage}", defaultPet.Error.Message);
            await ExecuteRollbackAsync("Pet factory failure");
            return Result.Failure<bool, DefaultPetError>(defaultPet.Error);
        }
        
        var resultPet = await _petService.CreateAsync(defaultPet.Value);
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
    /// Centralizes the best-effort rollback and handles its result to avoid duplicate logs, deleting from database and disk.
    /// </summary>
    private async Task ExecuteRollbackAsync(string reason)
    {
        Log.Warning("Initiating installation rollback due to {Reason}...", reason);
        
        var rollbackResult = await _petService.DeleteAllAsync();
        if (rollbackResult.IsFailure)
            Log.Error("Critical: Rollback failed. Database may be corrupted. Details: {RollbackError}", rollbackResult.Error.Message);
        
        Log.Information("Initiating physical folder cleanup...");
        _spriteSheetService.DeleteDefaultPetFolder();
    }
}