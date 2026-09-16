using CSharpFunctionalExtensions;
using PetDesktop.Back.Errors.PetError;
using PetDesktop.Back.Repositories.Pet;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Services.Pet;

public class PetService(IPetRepository petRepository)
{
    private static readonly ILogger Log = Serilog.Log.ForContext<PetService>();
    
    public async Task<IEnumerable<Models.Pet>> GetAllAsync(int page = 1, int pageSize = 20)
    {
        return await petRepository.GetAllAsync(page, pageSize);
    }
    
    public async Task<Result<bool, PetError>> CreateAsync(Models.Pet item)
    {
        try
        {
            await petRepository.CreateAsync(item);
            return Result.Success<bool, PetError>(true);
        }
        catch (Exception ex)
        {
            return Result.Failure<bool, PetError>(new PetError.PetDbError($"Error in the Database while saving a new Pet: {ex.Message}"));
        }
    }

    public async Task<Result<Models.Pet, PetError>> DeleteAsync(string key)
    {
        Log.Information("Request received to clear all pet records from the database.");
        try
        {
            var result = await GetById(key);
            if(result != null) return await petRepository.DeleteAsync(result);
            return Result.Failure<Models.Pet, PetError>(
                new PetError.MissingPetError($"Pet not found in the database:"));
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database infrastructure failed while attempting to delete all pets.");
            return Result.Failure<Models.Pet, PetError>(
                new PetError.PetDbError($"Error in the database: {ex.Message}"));
        }
    }

    public async Task<Models.Pet?> GetById(string key)
    {
        return await petRepository.GetByIdAsync(key);
    }
    
    public async Task<Result<bool, PetError>> DeleteAllAsync()
    { 
        Log.Information("Request received to clear all pet records from the database.");
        try
        {
            return await petRepository.DeleteAllAsync(); 
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database infrastructure failed while attempting to delete all pets.");
            return Result.Failure<bool, PetError>(new PetError.PetDbError($"Error in the database: {ex.Message}"));
        }
    }
    
    public async Task<Result<bool, PetError>> ExistsAsync(string name)
    { 
        Log.Information("Request received to clear all pet records from the database.");
        try
        {
            return await petRepository.ExistsAsync(name); 
        }
        catch (Exception ex)
        {
            Log.Error(ex, "Database infrastructure failed while attempting to delete all pets.");
            return Result.Failure<bool, PetError>(new PetError.PetDbError($"Error in the database: {ex.Message}"));
        }
    }
}