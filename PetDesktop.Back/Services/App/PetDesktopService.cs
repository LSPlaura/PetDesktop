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
        
        Init(); 
    }
    
    public async void Init()
    {
        await Factories.SpriteSheetFactory.CreateAnimationsDefaultPetAsync(_spriteSheetManager);
        var defaultPet = Factories.PetFactory.CreateDefaultPet(_spriteSheetManager);
        CreatePet(defaultPet);
    }
    public void CreatePet(Pet item)
    {
        _petRepository.Create(item);
    }

    public void DeletePet()
    {
        
    }

    public Models.Pet GetPet(string key)
    {
        return _petRepository.GetById(key);
    }

    public void GetAllPet()
    {
        
    }
}