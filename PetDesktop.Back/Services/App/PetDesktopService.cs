using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.DefaultPetErrors;
using PetDesktop.Back.Errors.PetError;
using PetDesktop.Back.Models;
using PetDesktop.Back.Repositories.Pet;
using PetDesktop.Back.Repositories.SpriteSheet;
using PetDesktop.Back.Services.SpriteSheet;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Services.App;

public class PetDesktopService
{
    private readonly SpriteSheetService _spriteSheetManager;
    private readonly SpriteSheetGenerator _spriteSheetGenerator;
    private readonly ISpriteSheetRepository _spriteSheetRepository;
    private readonly IPetRepository _petRepository;
    
    private static readonly ILogger Log = Serilog.Log.ForContext<PetDesktopService>();
    
    public PetDesktopService(
        SpriteSheetService spriteSheetManager, 
        SpriteSheetGenerator spriteSheetGenerator, 
        ISpriteSheetRepository spriteSheetRepository, 
        IPetRepository petRepository) 
    {
        _spriteSheetManager = spriteSheetManager;
        _spriteSheetGenerator = spriteSheetGenerator;
        _spriteSheetRepository = spriteSheetRepository;
        _petRepository = petRepository;
    }
    
    public async Task<Result<bool, DefaultPetError>> InitAsync()
    {
        Log.Debug("");
        var resultSpriteSheets = await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(_spriteSheetManager);
        if (resultSpriteSheets.IsFailure)
            return Result.Failure<bool, DefaultPetError>(resultSpriteSheets.Error);
        
        var defaultPet = await Factories.PetFactory.CreateDefaultPet(_spriteSheetManager);
        if(defaultPet.IsFailure)
            return Result.Failure<bool, DefaultPetError>(defaultPet.Error);
        
        var resultPet = await CreatePetAsync(defaultPet.Value);
        if (resultPet.IsFailure)
            return  Result.Failure<bool, DefaultPetError>(new DefaultPetError.DefaultPetInicializationError(resultPet.Error.Message));

        return Result.Success<bool, DefaultPetError>(true);
    }
    
    public async Task<IEnumerable<Models.Pet>> GetAllPetAsync(int page = 1, int pageSize = 20)
    {
        return await _petRepository.GetAllAsync(page, pageSize);
    }
    
    public async Task<Result<bool,PetError>> CreatePetAsync(Pet item)
    {
        try
        {
            await _petRepository.CreateAsync(item);
            return true;
        }
        catch (Exception ex)
        {
            return Result.Failure<bool, PetError>(new PetError.PetDbError($"Error in the Database while saving a new Pet: {ex.Message}"));
        }
    }

    public async Task DeletePetAsync(Models.Pet item)
    {
        await _petRepository.DeleteAsync(item);
    }
    
    public async Task<Pet> GetPet(string key)
    {
        return await _petRepository.GetByIdAsync(key);
    }
}