using Microsoft.EntityFrameworkCore;
using PetDesktop.Back.Entities;

namespace PetDesktop.Back.Repositories.SpriteSheet;

public abstract class SpriteSheetRepository (AppDbContext context) : ISpriteSheetRepository
{
    public async Task<IEnumerable<Models.SpriteSheet>> GetAllAsync(int page, int pageSize)
    {
        return await context.SpriteSheet
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<Models.SpriteSheet> CreateAsync(Models.SpriteSheet value)
    {
        context.SpriteSheet.Add(value);
        await context.SaveChangesAsync();
        return value;
    }

    public Task<Models.SpriteSheet> DeleteAsync(Models.SpriteSheet value)
    {
        throw new NotImplementedException();
    }

    public Task<Models.SpriteSheet> UpdateAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<Models.SpriteSheet> GetByIdAsync(string key)
    {
        throw new NotImplementedException();
    }

    public Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }
}