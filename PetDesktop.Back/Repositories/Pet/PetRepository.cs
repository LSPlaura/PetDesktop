using Microsoft.EntityFrameworkCore;
using PetDesktop.Back.Repositories.Common;
using ILogger = Serilog.ILogger;

namespace PetDesktop.Back.Repositories.Pet;

public class PetRepository(AppDbContext context) : IPetRepository
{
    private static readonly ILogger Log = Serilog.Log.ForContext<PetRepository>();
    public async Task<IEnumerable<Models.Pet>> GetAllAsync(int page, int pageSize)
    {
        return await context.Pet
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Models.Pet> CreateAsync(Models.Pet value)
    {
        context.Pet.Add(value);
        await context.SaveChangesAsync();
        return value;
    }

    public async Task<Models.Pet> DeleteAsync(Models.Pet value)
    {
        context.Pet.Remove(value);
        await context.SaveChangesAsync();
        return value;
    }

    public Task<Models.Pet> UpdateAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<Models.Pet?> GetByIdAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        
        return await context.Pet
            .FirstOrDefaultAsync(p => p.Name.ToLower() == key.ToLower());
    }

    public Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<bool> DeleteAllAsync()
    {
        Log.Information("Executing bulk delete for all pets in the database...");

        int rowsDeleted = await context.Pet.ExecuteDeleteAsync();

        if (rowsDeleted > 0)
        {
            Log.Information("Bulk delete completed successfully. Total pets removed: {RowsDeleted}", rowsDeleted);
        }
        else
        {
            Log.Information("Bulk delete executed, but no pet records were found to delete.");
        }
        return rowsDeleted > 0;
    }

    public Task<bool> DeleteAllSpriteSheetsAsync()
    {
        throw new NotImplementedException();
    }
}