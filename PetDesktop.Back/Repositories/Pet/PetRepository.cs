using PetDesktop.Back.Entities;

namespace PetDesktop.Back.Repositories.Pet;

public class PetRepository : IPetRepository
{
    private readonly AppDbContext _context;
    public Models.Pet Create(Models.Pet value)
    {
        throw new NotImplementedException();
    }

    public Models.Pet Delete(string key)
    {
        throw new NotImplementedException();
    }

    public Models.Pet Update(string key)
    {
        throw new NotImplementedException();
    }

    public Models.Pet GetById(string key)
    {
        throw new NotImplementedException();
    }

    public bool Exists(string key)
    {
        throw new NotImplementedException();
    }
}