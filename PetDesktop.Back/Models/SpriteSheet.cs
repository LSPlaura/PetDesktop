
using Avalonia.Media.Imaging;

namespace PetDesktop.Back.Models;

public class SpriteSheet : IDisposable
{
    public string Name { get; init; } = string.Empty;
    public string Route { get; init; } = string.Empty;
    public int FrameWidth { get; init; }
    public int FrameHeight { get; init; }
    public Bitmap? ImagenOrigen { get; private set; } 
    private bool _disposed = false;

    public SpriteSheet(string name, string route, int frameWidth, int frameHeight, Bitmap imagen)
    {
        Name = name;
        Route = route;
        FrameWidth = frameWidth;
        FrameHeight = frameHeight;
        ImagenOrigen = imagen;
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            ImagenOrigen = null; 
            _disposed = true;
        }
    }
}