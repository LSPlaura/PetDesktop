namespace PetDesktop.Back.Models;

public class Pet
{
    public string Name { get; init; } = string.Empty;
    public SpriteSheet? ActualAnimation { get; set; } = null;
    public string? Message { get; set; } = "Drink Water!";
    public TimeSpan TimeMessageInterval { get; set; } = TimeSpan.FromMinutes(30);
}