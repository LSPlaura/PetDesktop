using PetDesktop.Back.Models;
using PetDesktop.Back.Repositories.Pet;
using PetDesktop.Back.Repositories.SpriteSheet;
using PetDesktop.Back.Services.SpriteSheet;

namespace PetDesktop.Back.Services.App;

public class PetDesktopService
{
    private readonly SpriteSheetService _spriteSheetManager;
    private readonly SpriteSheetGenerator _spriteSheetGenerator;
    private readonly ISpriteSheetRepository _spriteSheetRepository;
    private readonly IPetRepository _petRepository;
    
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
    
    public async Task InitAsync()
    {
        await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(_spriteSheetManager);
        
        var defaultPet = await Factories.PetFactory.CreateDefaultPet(_spriteSheetManager);
        
        await CreatePetAsync(defaultPet);
    }
    
    public async Task<IEnumerable<Models.Pet>> GetAllPetAsync(int page = 1, int pageSize = 20)
    {
        return await _petRepository.GetAllAsync(page, pageSize);
    }
    
    public async Task CreatePetAsync(Pet item)
    {
        await _petRepository.CreateAsync(item);
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