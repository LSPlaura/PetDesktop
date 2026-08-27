using Microsoft.EntityFrameworkCore;
using PetDesktop.Back.Repositories.Common;

namespace PetDesktop.Back.Repositories.SpriteSheet;

public class SpriteSheetRepository (AppDbContext context) : ISpriteSheetRepository
{
    public async Task<IEnumerable<Models.SpriteSheet>> GetAllAsync(int page, int pageSize)
    {
        return await context.SpriteSheet
            .AsNoTracking()
            .OrderBy(p => p.AssociatedPet)
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

    public async Task<Models.SpriteSheet> DeleteAsync(Models.SpriteSheet value)
    {
        context.SpriteSheet.Remove(value);
        await context.SaveChangesAsync();
        return value;
    }
    
    // public async Task DeleteRangeAsync(string petName)
    // {
    //     var spriteSheet = await GetAllAssociatedPet(key);
    //     context.SpriteSheet.Remove(spriteSheet);
    //     await context.SaveChangesAsync();
    //     return spriteSheet;
    // }
    //
    // public async Task<IEnumerable<Models.SpriteSheet>> GetAllAssociatedPet(string petName)
    // {
    //     var spriteSheet = await GetByIdAsync(key);
    //     context.SpriteSheet.Remove(spriteSheet);
    //     await context.SaveChangesAsync();
    //     return spriteSheet;
    // }

    public Task<Models.SpriteSheet> UpdateAsync(string key)
    {
        throw new NotImplementedException();
    }

    public async Task<Models.SpriteSheet?> GetByIdAsync(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        
        return await context.SpriteSheet
            .FirstOrDefaultAsync(p => p.Name.ToLower() == key.ToLower());
    }

    public Task<bool> ExistsAsync(string key)
    {
        throw new NotImplementedException();
    }
}