namespace PetDesktop.Back.Models;

public class Pet
{
    public SpriteSheet? ActualAnimation { get; set; } = null;
    public List<SpriteSheet> Animations { get; set; } = new List<SpriteSheet>();
    public string Message { get; set; } = "Drink Water!";
    public TimeSpan TimeMessageInterval { get; set; } = TimeSpan.FromMinutes(30);
}