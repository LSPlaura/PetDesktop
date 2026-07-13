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
    
    public async Task<Result<bool,PetError>> CreateAsync(Models.Pet item)
    {
        try
        {
            await petRepository.CreateAsync(item);
            return true;
        }
        catch (Exception ex)
        {
            return Result.Failure<bool, PetError>(new PetError.PetDbError($"Error in the Database while saving a new Pet: {ex.Message}"));
        }
    }
    
    public async Task DeleteAsync(Models.Pet item)
    {
        await petRepository.DeleteAsync(item);
    }
    
    public async Task<Models.Pet> GetById(string key)
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
}