using Microsoft.EntityFrameworkCore;
using PetDesktop.Back.Entities;

namespace PetDesktop.Back.Repositories.SpriteSheet;

public class SpriteSheetRepository (AppDbContext context) : ISpriteSheetRepository
{
    
    public IEnumerable<Models.SpriteSheet> GetAll(int page, int pageSize)
    {
        return context.SpriteSheet
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Models.SpriteSheet Create(Models.SpriteSheet value)
    {
        context.SpriteSheet.Add(value);
        context.SaveChanges();
        return value;
    }

    public Models.SpriteSheet Delete(Models.SpriteSheet value)
    {
        throw new NotImplementedException();
    }

    public Models.SpriteSheet Update(string key)
    {
        throw new NotImplementedException();
    }

    public Models.SpriteSheet GetById(string key)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string key)
    {
        throw new NotImplementedException();
    }
}