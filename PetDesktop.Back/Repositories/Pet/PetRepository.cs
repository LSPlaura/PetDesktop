using Microsoft.EntityFrameworkCore;
using PetDesktop.Back.Entities;

namespace PetDesktop.Back.Repositories.Pet;

public class PetRepository : IPetRepository
{
    private AppDbContext _context;
    public PetRepository(AppDbContext context)
    {
        _context = context;
    }

    public IEnumerable<Models.Pet> GetAll(int page, int pageSize)
    {
        return _context.Pet
            .AsNoTracking()
            .OrderBy(p => p.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();
    }

    public Models.Pet Create(Models.Pet value)
    {
        _context.Pet.Add(value);
        _context.SaveChanges();
        return value;
    }

    public Models.Pet Delete(Models.Pet value)
    {
        _context.Pet.Remove(value);
        _context.SaveChanges();
        return value;
    }

    public Models.Pet Update(string key)
    {
        throw new NotImplementedException();
    }

    public Models.Pet GetById(string key)
    {
        if (string.IsNullOrEmpty(key)) return null;
        return _context.Pet.FirstOrDefault(p => p.Name != null && p.Name.ToLower() == key.ToLower());
    }

    public bool Exists(string key)
    {
        throw new NotImplementedException();
    }
}