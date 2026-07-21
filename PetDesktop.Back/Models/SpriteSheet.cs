namespace PetDesktop.Back.Models;

public class SpriteSheet
{
    public string Name { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public int FrameWidth { get; init; }
    public int FrameHeight { get; init; }
    public string AssociatedPet { get; set; } = string.Empty;

    public SpriteSheet(string name, string route, int frameWidth, int frameHeight, string associatedPet)
    {
        Name = name;
        Route = route;
        FrameHeight = frameHeight;
        FrameWidth = frameWidth;
        AssociatedPet = associatedPet;
    }
    private SpriteSheet() { }
} 