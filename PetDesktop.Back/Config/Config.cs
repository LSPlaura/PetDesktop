using Avalonia.Media.Imaging;
using Microsoft.Extensions.Configuration;

namespace PetDesktop.Back.Config;

public static class Config
{
    public static readonly IConfigurationRoot Settings = new ConfigurationBuilder()
        .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
        .AddJsonFile("appsettings.json", false, true)
        .Build();

    public static string SpriteSheetRoute { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "PetDesktop", SpriteSheetFolder);

    public static string SpriteSheetFolder { get; } =
        Settings.GetValue<string>("SpriteSettings:SpriteSheetFolder") ?? "SpriteSheets";

    public static string Name { get; } = Settings.GetValue<string>("AnimationName") ?? "Default_Idle";

    public static string Route { get; }= Settings.GetValue<string>("Route") ??
                                         "avares://PetDesktop.UI/Assets/SpriteSheet/WalkRight.png";
    public static int FrameWidth { get; } = Settings.GetValue<int>("FrameWidth");
    public static int FrameHeight { get; } = Settings.GetValue<int>("FrameHeight");
    public static string AssociatedPet { get; } = Settings.GetValue<string>("Name") ?? "DefaultPet";
}