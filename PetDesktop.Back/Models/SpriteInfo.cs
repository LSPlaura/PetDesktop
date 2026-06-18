using Avalonia;

namespace PetDesktop.Back.Models;

/// <summary>
/// La clase que va a ser utilizada para crear las animaciones
/// </summary>
public class SpriteInfo
{
    public string Name { get; init; } = string.Empty;
    public double MoveX { get; init; }
    public double MoveY { get; init; }
    public int Intervalo { get; init; }
    public PixelRect SeccionRecorte { get; init; }
}