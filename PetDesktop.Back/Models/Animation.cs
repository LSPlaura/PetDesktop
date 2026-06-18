namespace PetDesktop.Back.Models;

public class Animation
{
    public string Name { get; init; } = string.Empty;
    public List<SpriteInfo> SpritesSecuence { get; init; } = new List<SpriteInfo>();
    public string SpriteSheetPath { get; init; } = string.Empty;
    public bool IsLoop { get; init; } = true;
}